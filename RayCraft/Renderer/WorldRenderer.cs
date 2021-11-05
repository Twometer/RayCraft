using Craft.Client.World;
using Craft.Client.World.Entities;
using RayCraft.Game;
using System;
using System.Collections.Concurrent;
using System.Numerics;
using System.Threading;

namespace RayCraft.Renderer
{
    using line_t = Int32;

    public class WorldRenderer
    {
        private const int MaxRayLength = 40;
        private const float Precision = 0.1f;
        private const int PrecisionStep = 100;
        private const float EyeHeight = 1.8f;

        private DisplayBuffer displayBuffer;
        private int width;
        private int height;

        private ConcurrentQueue<WorkItem> workItems = new ConcurrentQueue<WorkItem>();
        const int tileDiv = 4;
        private SemaphoreSlim sem = new SemaphoreSlim(0);
        private CountdownEvent e = new CountdownEvent(tileDiv * tileDiv);
        private RenderMath math = new RenderMath();
        private Location currentLocation;

        private class WorkItem
        {
            public int x;
            public int y;
            public int w;
            public int h;

            public WorkItem(int x, int y, int w, int h)
            {
                this.x = x;
                this.y = y;
                this.w = w;
                this.h = h;
            }
        }

        public WorldRenderer(int width, int height)
        {
            displayBuffer = new DisplayBuffer(width, height);
            this.width = width;
            this.height = height;
            math.Initialize(width, height);
            BootThreads();
        }

        private void BootThreads()
        {
            for (int i = 0; i < (tileDiv * tileDiv); i++)
            {
                Thread thread = new Thread(() =>
                {
                    while (true)
                    {
                        sem.Wait();
                        if (workItems.TryDequeue(out WorkItem item))
                        {
                            for (int y = item.y; y < item.y + item.h; y++)
                            {
                                for (int x = item.x; x < item.x + item.w; x++)
                                {
                                    var ray = math.CreateRay(x, y);
                                    var hitResult = GetHitResult(currentLocation, ray);
                                    var blockColor = GetBlockColor(hitResult);
                                    displayBuffer.SetPixel(x, y, blockColor);
                                }
                            }
                            e.Signal();
                        }
                    }
                });
                thread.Start();
            }
        }



        public System.Drawing.Bitmap RenderWorld()
        {
            e.Reset();
            displayBuffer.Begin();

            EntityPlayer player = RayCraftGame.Instance.Player;
            currentLocation = new Location((float)player.PosX, (float)player.PosY, (float)player.PosZ, player.Yaw, player.Pitch);
            math.UpdateMatrix(currentLocation);

            float tileWidth = width / tileDiv;
            float tileHeight = height / tileDiv;

            for (int i = 0; i < tileDiv; i++)
            {
                for (int j = 0; j < tileDiv; j++)
                {
                    workItems.Enqueue(new WorkItem((int)(tileWidth * i), (int)(tileHeight * j), (int)tileWidth, (int)tileHeight));
                }
            }
            sem.Release(tileDiv * tileDiv);
            e.Wait();
            displayBuffer.Finish();
            return displayBuffer.GetBitmap();
        }

        private int GetBlockColor(HitResult result)
        {
            if (result == null) return BlockRegistry.BlockColors[0].Lvl0;
            if (result.HitEntity) return 0xff0000;
            ColorCollection color = BlockRegistry.BlockColors[result.BlockType];
            if (result.Face == EnumFace.NULL) return color.Lvl0;
            if (result.Face == EnumFace.ZPos || result.Face == EnumFace.ZNeg) return color.Lvl1;
            if (result.Face == EnumFace.XPos || result.Face == EnumFace.XNeg) return color.Lvl2;
            if (result.Face == EnumFace.YNeg) return color.Lvl3;
            return color.Lvl0;
        }

        private bool TestRayAabbIntersection(float originX, float originY, float originZ, float rayX, float rayY, float rayZ, line_t minX, line_t minY, line_t minZ)
        {
            var maxX = minX + 1;
            var maxY = minY + 1;
            var maxZ = minZ + 1;
            var tMin = -1E9f;
            var tMax = 1E9f;
            if (rayX != 0f)
            {
                var tx1 = (minX - originX) / rayX;
                var tx2 = (maxX - originX) / rayX;
                tMin = Math.Max(tMin, Math.Min(tx1, tx2));
                tMax = Math.Min(tMax, Math.Max(tx1, tx2));
            }
            if (rayY != 0f)
            {
                var ty1 = (minY - originY) / rayY;
                var ty2 = (maxY - originY) / rayY;
                tMin = Math.Max(tMin, Math.Min(ty1, ty2));
                tMax = Math.Min(tMax, Math.Max(ty1, ty2));
            }
            if (rayZ != 0f)
            {
                var tz1 = (minZ - originZ) / rayZ;
                var tz2 = (maxZ - originZ) / rayZ;
                tMin = Math.Max(tMin, Math.Min(tz1, tz2));
                tMax = Math.Min(tMax, Math.Max(tz1, tz2));
            }

            return tMax >= tMin;
        }


        private HitResult BlockLookup(Vector3 origin, Vector3 ray, line_t x, line_t y, line_t z)
        {
            var blockId = RayCraftGame.Instance.World.GetBlock((int)x, (int)y, (int)z);
            if (blockId != 0 && TestRayAabbIntersection(origin.X, origin.Y, origin.Z, ray.X, ray.Y, ray.Z, x, y, z))
            {
                return new HitResult(blockId, EnumFace.YPos, false);
            }
            return null;
        }

        private HitResult Bresenham3d(Vector3 origin, Vector3 ray, line_t x1, line_t y1, line_t z1, line_t x2, line_t y2, line_t z2)
        {
            var dx = Math.Abs(x2 - x1);
            var dy = Math.Abs(y2 - y1);
            var dz = Math.Abs(z2 - z1);

            line_t xs;
            line_t ys;
            line_t zs;
            if (x2 > x1)
                xs = 1;
            else
                xs = -1;
            if (y2 > y1)
                ys = 1;
            else
                ys = -1;
            if (z2 > z1)
                zs = 1;
            else
                zs = -1;

            if (dx >= dy && dx >= dz)
            {
                var p1 = 2 * dy - dx;
                var p2 = 2 * dz - dx;
                while (x1 != x2)
                {
                    x1 += xs;
                    if (p1 >= 0)
                    {
                        y1 += ys;
                        p1 -= 2 * dx;
                    }
                    if (p2 >= 0)
                    {
                        z1 += zs;
                        p2 -= 2 * dx;
                    }
                    p1 += 2 * dy;
                    p2 += 2 * dz;

                    var result = BlockLookup(origin, ray, x1, y1, z1);
                    if (result != null)
                        return result;

                }
            }
            else if (dy >= dx && dy >= dz)
            {
                var p1 = 2 * dx - dy;
                var p2 = 2 * dz - dy;
                while (y1 != y2)
                {
                    y1 += ys;
                    if (p1 >= 0)
                    {
                        x1 += xs;
                        p1 -= 2 * dy;
                    }
                    if (p2 >= 0)
                    {
                        z1 += zs;
                        p2 -= 2 * dy;
                    }
                    p1 += 2 * dx;
                    p2 += 2 * dz;

                    var result = BlockLookup(origin, ray, x1, y1, z1);
                    if (result != null)
                        return result;
                }
            }
            else
            {
                var p1 = 2 * dy - dz;
                var p2 = 2 * dx - dz;
                while (z1 != z2)
                {
                    z1 += zs;
                    if (p1 >= 0)
                    {
                        y1 += ys;
                        p1 -= 2 * dz;
                    }
                    if (p2 >= 0)
                    {
                        x1 += xs;
                        p2 -= 2 * dz;
                    }
                    p1 += 2 * dy;
                    p2 += 2 * dx;

                    var result = BlockLookup(origin, ray, x1, y1, z1);
                    if (result != null)
                        return result;
                }
            }

            return null;
        }

        private HitResult GetHitResult(Location origin, Vector3 ray)
        {
            float x1 = origin.X;
            float y1 = origin.Y + EyeHeight;
            float z1 = origin.Z;

            float x2 = x1 + ray.X * MaxRayLength;
            float y2 = y1 + ray.Y * MaxRayLength;
            float z2 = z1 + ray.Z * MaxRayLength;

            return Bresenham3d(new Vector3(x1, y1, z1), ray, (line_t)x1, (line_t)y1, (line_t)z1, (line_t)x2, (line_t)y2, (line_t)z2);
        }

        private HitResult GetHitResult0(Location origin, Vector3 ray)
        {
            World wld = RayCraftGame.Instance.World;
            Vector3 scaledRay = ray * Precision;
            int precisionIterCounter = 0;
            int iterations = (int)(MaxRayLength / scaledRay.Length());
            int scalar = 4;

            float rx = origin.X;
            float ry = origin.Y + EyeHeight;
            float rz = origin.Z;

            float dx = scaledRay.X;
            float dy = scaledRay.Y;
            float dz = scaledRay.Z;

            for (int i = 1; i < iterations; i++)
            {
                rx += dx;
                ry += dy;
                rz += dz;

                int xf = (int)rx;
                int yf = (int)ry;
                int zf = (int)rz;

                byte blockType = wld.GetBlock(xf, yf, zf);
                if (precisionIterCounter++ >= PrecisionStep && scalar > 1)
                {
                    precisionIterCounter = 0;
                    dx *= scalar;
                    dy *= scalar;
                    dz *= scalar;
                    scaledRay.X = dx;
                    scaledRay.Y = dy;
                    scaledRay.Z = dz;
                    iterations = (int)(MaxRayLength / scaledRay.Length());
                }
                if (blockType != 0)
                {
                    EnumFace face = EnumFace.XPos;
                    float xd = rx - xf;
                    float yd = ry - yf;
                    float zd = rz - zf;
                    if (IsError(xd, 1))
                        face = EnumFace.XPos;
                    else if (IsError(xd, 0))
                        face = EnumFace.XNeg;
                    else if (IsError(yd, 1))
                        face = EnumFace.YPos;
                    else if (IsError(yd, 0))
                        face = EnumFace.YNeg;
                    else if (IsError(zd, 1))
                        face = EnumFace.ZPos;
                    else if (IsError(zd, 0))
                        face = EnumFace.ZNeg;
                    return new HitResult(blockType, face, false);
                }
            }
            return null;
        }

        private bool IsError(double x, int y)
        {
            return Math.Abs(y - x) <= Precision;
        }
    }
}
