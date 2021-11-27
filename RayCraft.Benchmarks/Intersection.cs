using BenchmarkDotNet.Attributes;
using RayCraft.Algorithms;
using RayCraft.Renderer;
using System;
using System.Numerics;

namespace RayCraft.Benchmarks
{
    public class Intersection
    {
        private readonly Camera camera;
        private readonly (int, int)[] hits2d;
        private readonly (int, int, int)[] hits3d;

        public Intersection()
        {
            camera = new Camera(1920, 1080, 70);
            camera.Update(0.0f, 0.0f);
            hits2d = new (int, int)[64];
            hits3d = new (int, int, int)[64];
        }

        [Benchmark]
        public void BenchIntersect2d()
        {
            for (int x = 0; x < 1920; x++)
            {
                for (int y = 0; y < 1080; y++)
                {
                    Vector3 vec = camera.CreateRay(x, y);
                    Line.Intersect(new Vector2(0.5f, 0.5f), new Vector2(vec.X, vec.Z), hits2d);
                }
            }
        }

        [Benchmark]
        public void BenchIntersect3d()
        {
            for (int x = 0; x < 1920; x++)
            {
                for (int y = 0; y < 1080; y++)
                {
                    Line.Intersect(new Vector3(0.5f, 82f, 0.5f), camera.CreateRay(x, y), hits3d);
                }
            }
        }

        [Benchmark]
        public void BenchBresenham3d()
        {
            for (int x = 0; x < 1920; x++)
            {
                for (int y = 0; y < 1080; y++)
                {
                    Vector3 vec = camera.CreateRay(x, y);
                    Bresenham3d(0, 82, 0, (int)(1000.0f * vec.X), 82, (int)(1000.0f * vec.Z), hits3d);
                }
            }
        }

        private static void Bresenham3d(int x1, int y1, int z1, int x2, int y2, int z2, Span<(int, int, int)> hits)
        {
            int hitCount = 0;

            var dx = Math.Abs(x2 - x1);
            var dy = Math.Abs(y2 - y1);
            var dz = Math.Abs(z2 - z1);

            int xs;
            int ys;
            int zs;
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

                    hits[hitCount++] = (x1, y1, z1);
                    if (hitCount == hits.Length) return;
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

                    hits[hitCount++] = (x1, y1, z1);
                    if (hitCount == hits.Length) return;
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

                    hits[hitCount++] = (x1, y1, z1);
                    if (hitCount == hits.Length) return;
                }
            }
        }
    }
}
