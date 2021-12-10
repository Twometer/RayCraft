using Craft.Client.World;
using Craft.Client.World.Entities;
using RayCraft.Game;
using System;
using System.Collections.Concurrent;
using System.Numerics;
using System.Threading;

namespace RayCraft.Renderer
{
    public class WorldRenderer
    {
        private const int MaxRayLength = 128;
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
        private Camera camera;
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
            camera = new Camera(width, height, 70);
            displayBuffer = new DisplayBuffer(width, height);
            this.width = width;
            this.height = height;
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
                                    var ray = camera.CreateRay(x, y);
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
            camera.Update(currentLocation.Yaw, currentLocation.Pitch);

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

        private HitResult BlockLookup(int x, int y, int z)
        {
            var blockId = RayCraftGame.Instance.World.GetBlock(x, y, z);
            if (blockId != 0)
            {
                return new HitResult(blockId, EnumFace.YPos, false);
            }
            return null;
        }

        private HitResult GetHitResult(in Location origin, Vector3 direction)
        {
            Vector3 location = new(origin.X, origin.Y + EyeHeight, origin.Z);

            int orientationX = MathF.Sign(direction.X);
            int orientationY = MathF.Sign(direction.Y);
            int orientationZ = MathF.Sign(direction.Z);

            float srx = MathF.Abs(1 / direction.X);
            float sry = MathF.Abs(1 / direction.Y);
            float srz = MathF.Abs(1 / direction.Z);

            float fx = MathF.Floor(location.X);
            float fy = MathF.Floor(location.Y);
            float fz = MathF.Floor(location.Z);

            int x = GetCurrent(location.X, fx, orientationX);
            int y = GetCurrent(location.Y, fy, orientationY);
            int z = GetCurrent(location.Z, fz, orientationZ);

            float nrx = GetNextR(location.X, fx, orientationX, srx);
            float nry = GetNextR(location.Y, fy, orientationY, sry);
            float nrz = GetNextR(location.Z, fz, orientationZ, srz);

            for (int i = 0; i < MaxRayLength; i++)
            {
                if (nrx < nry && nrx < nrz)
                {
                    HitResult hit = BlockLookup(x += orientationX, y, z);
                    if (hit is not null) return hit;
                    nrx += srx;
                }
                else if (nry < nrx && nry < nrz)
                {
                    HitResult hit = BlockLookup(x, y += orientationY, z);
                    if (hit is not null) return hit;
                    nry += sry;
                }
                else
                {
                    HitResult hit = BlockLookup(x, y, z += orientationZ);
                    if (hit is not null) return hit;
                    nrz += srz;
                }
            }

            return null;
        }

        private static int GetCurrent(float location, float floor, int orientation)
        {
            if (location == floor && orientation == -1)
            {
                return (int)floor - 1;
            }
            else
            {
                return (int)floor;
            }
        }

        private static float GetNextR(float location, float floor, int orientation, float sr)
        {
            if (location == floor)
            {
                // This returns infinity when sr is infinity.
                return 1 * sr;
            }
            else
            {
                // The first term will always be greater than zero and therefore returns inifnity when sr is infinity.
                return MathF.Abs(floor + (orientation == 1 ? 1 : 0) - location) * sr;
            }
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
