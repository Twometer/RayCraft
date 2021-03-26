using Craft.Client.World;
using Craft.Client.World.Entities;
using OpenTK;
using RayCraft.Game;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RayCraft.Renderer
{
    public class WorldRenderer
    {
        private const int MaxRayLength = 60;
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
                            for (int x = item.x; x < item.x + item.w; x++)
                            {
                                for (int y = item.y; y < item.y + item.h; y++)
                                {
                                    Vector3 ray = math.CreateRay(x, y);
                                    displayBuffer.SetPixel(x, y, GetBlockColor(GetHitResult(currentLocation, ray)));
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

        private HitResult GetHitResult(Location origin, Vector3 ray)
        {
            Vector3 scaledRay = ray * (Precision);
            Vector3 targetPos = new Vector3(0, 0, 0);
            World wld = RayCraftGame.Instance.World;
            int iterations = (int)((double)MaxRayLength / scaledRay.Length);
            int scaler = 4;
            for (int i = 1; i < iterations; i++)
            {
                targetPos += (scaledRay);
                float x = origin.X + targetPos.X;
                float y = origin.Y + EyeHeight + targetPos.Y;
                float z = origin.Z + targetPos.Z;
                int xf = (int)(x);
                int yf = (int)(y);
                int zf = (int)(z);
                byte blockType = wld.GetBlock(xf, yf, zf);
                if (i % PrecisionStep == 0)
                {
                    scaledRay = scaledRay * (scaler > 1 ? scaler-- : 1);
                    iterations = (int)((double)MaxRayLength / scaledRay.Length);
                }
                if (blockType != 0)
                {
                    EnumFace face = EnumFace.XPos;
                    double xd = Math.Round(x - xf, 2);
                    double yd = Math.Round(y - yf, 2);
                    double zd = Math.Round(z - zf, 2);
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
