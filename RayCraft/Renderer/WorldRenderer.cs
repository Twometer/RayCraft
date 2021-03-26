using Craft.Client.World;
using Craft.Client.World.Entities;
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
        private const int MaxRayLength = 30;
        private const float Precision = 0.2f;
        private const float EyeHeight = 1.8f;
        private const int LightDistance = 150;

        private DisplayBuffer displayBuffer;
        private int width;
        private int height;

        private ConcurrentQueue<WorkItem> workItems = new ConcurrentQueue<WorkItem>();
        const int tileDiv = 4;
        private SemaphoreSlim sem = new SemaphoreSlim(0);
        private CountdownEvent e = new CountdownEvent(tileDiv * tileDiv);

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
            BootThreads();
        }

        private void BootThreads()
        {
            for (int i = 0; i < 16; i++)
            {
                Thread thread = new Thread(() =>
                {
                    while (true)
                    {
                        sem.Wait();
                        if (workItems.TryDequeue(out WorkItem item))
                        {

                            if (item != null)
                            {
                                EntityPlayer player = RayCraftGame.Instance.Player;
                                Location origin = new Location((float)player.PosX, (float)player.PosY, (float)player.PosZ, player.Yaw, player.Pitch);
                                int hWidth = width / 2 / 10;
                                int hHeight = height / 2 / 10;
                                float d = (float)width / height * 4;
                                for (int x = item.x; x < item.x + item.w; x++)
                                {
                                    for (int y = item.y; y < item.y + item.h; y++)
                                    {
                                        Vector ray = RenderMath.CreateRay(origin, (x - hWidth) / d, (y - hHeight) / d);
                                        displayBuffer.SetPixel(x, y, GetBlockColor(GetHitResult(origin, ray)));
                                    }
                                }
                                e.Signal();
                            }
                        }
                    }
                });
                thread.Start();
            }
        }

      

        public Bitmap RenderWorld()
        {
            e.Reset();
            displayBuffer.Begin();

            float w = width / tileDiv;
            float h = height / tileDiv;

            for (int i = 0; i < tileDiv; i++)
            {
                for (int j = 0; j < tileDiv; j++)
                {
                    workItems.Enqueue(new WorkItem((int)(w * i), (int)(h * j), (int)w, (int)h));
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
            if (!BlockRegistry.BlockColors.ContainsKey(result.BlockType)) return 0;
            ColorCollection color = BlockRegistry.BlockColors[result.BlockType];
            if (result.Face == EnumFace.NULL) return color.Lvl0;
            if (result.Face == EnumFace.ZPos || result.Face == EnumFace.ZNeg) return color.Lvl1;
            if (result.Face == EnumFace.XPos || result.Face == EnumFace.XNeg) return color.Lvl2;
            if (result.Face == EnumFace.YNeg) return color.Lvl3;
            return color.Lvl0;
        }

        private HitResult GetHitResult(Location origin, Vector ray)
        {
            Vector scaledRay = ray.Multiply(Precision);
            Vector targetPos = new Vector(0, 0, 0);
            World wld = RayCraftGame.Instance.World;
            int iterations = (int)((double)MaxRayLength / scaledRay.Length);
            for (int i = 1; i < iterations; i++)
            {
                targetPos.Add(scaledRay);
                float x = origin.X + targetPos.X;
                float y = origin.Y + 1.8f + targetPos.Y;
                float z = origin.Z + targetPos.Z;
                int xf = (int)(x);
                int yf = (int)(y);
                int zf = (int)(z);
                byte blockType = wld.GetBlock(xf, yf, zf);
                if (i % 300 == 0)
                {
                    scaledRay = scaledRay.Multiply(4);
                    iterations = (int)((double)MaxRayLength / scaledRay.Length);
                }
                if (blockType != 0)
                {
                    EnumFace face = EnumFace.NULL;
                    if (i < LightDistance)
                    {
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
                    }
                    return new HitResult(blockType, face, false);
                }
            }
            return null;
        }

        private bool IsError(double x, int y)
        {
            return Math.Abs(y - x) <= 0.1;
        }
    }
}
