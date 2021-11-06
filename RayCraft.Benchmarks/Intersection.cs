using BenchmarkDotNet.Attributes;
using System;
using System.Numerics;

namespace RayCraft.Benchmarks
{
    public class Intersection
    {
        private readonly (int, int)[] hits2d = new (int, int)[64];
        private readonly (int, int, int)[] hits3d = new (int, int, int)[64];

        [Benchmark]
        public void BenchIntersect()
        {
            for (int x = 0; x < 1920; x++)
            {
                for (int y = 0; y < 1080; y++)
                {
                    Intersect(new Vector2(0.5f + x * 0.0025f, 0.5f + y * 0.0025f), new Vector2(1.0f, 0.5f), hits2d);
                }
            }
        }

        [Benchmark]
        public void BenchIntersect_3d()
        {
            for (int x = 0; x < 1920; x++)
            {
                for (int y = 0; y < 1080; y++)
                {
                    Intersect(new Vector3(0.5f + x * 0.0025f, 82f, 0.5f + y * 0.0025f), new Vector3(1.0f, 0.5f, 0.5f), hits3d);
                }
            }
        }

        [Benchmark]
        public void BenchIntersect2()
        {
            for (int x = 0; x < 1920; x++)
            {
                for (int y = 0; y < 1080; y++)
                {
                    Intersect2(new Vector2(0.5f + x * 0.0025f, 0.5f + y * 0.0025f), new Vector2(1.0f, 0.5f), hits2d);
                }
            }
        }

        [Benchmark]
        public void BenchIntersect2_3d()
        {
            for (int x = 0; x < 1920; x++)
            {
                for (int y = 0; y < 1080; y++)
                {
                    Intersect2(new Vector3(0.5f + x * 0.0025f, 82f, 0.5f + y * 0.0025f), new Vector3(1.0f, 0.5f, 0.5f), hits3d);
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
                    Bresenham3d(x, y, 0, x + 1000, y + 1000, 1000, hits3d);
                }
            }
        }

        private void Bresenham3d(int x1, int y1, int z1, int x2, int y2, int z2, Span<(int, int, int)> hits)
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

        private void Intersect(Vector2 location, Vector2 direction, Span<(int, int)> hits)
        {
            int x = (int)MathF.Floor(location.X);
            int y = (int)MathF.Floor(location.Y);

            int orientationX = MathF.Sign(direction.X);
            int orientationY = MathF.Sign(direction.Y);

            for (int i = 0; i < hits.Length; i++)
            {
                float rx = (x + orientationX - location.X) / direction.X;
                float ry = (y + orientationY - location.Y) / direction.Y;

                if (rx < ry)
                {
                    hits[i] = (x += orientationX, y);
                    location = new Vector2(x, location.Y + rx * direction.Y);
                }
                else
                {
                    hits[i] = (x, y += orientationY);
                    location = new Vector2(location.X + ry * direction.X, y);
                }
            }
        }

        private void Intersect(Vector3 location, Vector3 direction, Span<(int, int, int)> hits)
        {
            int x = (int)MathF.Floor(location.X);
            int y = (int)MathF.Floor(location.Y);
            int z = (int)MathF.Floor(location.Z);

            int orientationX = MathF.Sign(direction.X);
            int orientationY = MathF.Sign(direction.Y);
            int orientationZ = MathF.Sign(direction.Z);

            for (int i = 0; i < hits.Length; i++)
            {
                float rx = (x + orientationX - location.X) / direction.X;
                float ry = (y + orientationY - location.Y) / direction.Y;
                float rz = (z + orientationZ - location.Z) / direction.Z;

                if (rx < ry && rx < rz)
                {
                    hits[i] = (x += orientationX, y, z);
                    location = new Vector3(x, location.Y + rx * direction.Y, location.Z + rx * direction.Z);
                }
                else if (ry < rx && ry < rz)
                {
                    hits[i] = (x, y += orientationY, z);
                    location = new Vector3(location.X + ry * direction.X, y, location.Z + ry * direction.Z);
                }
                else
                {
                    hits[i] = (x, y, z += orientationZ);
                    location = new Vector3(location.X + rz * direction.X, location.Y + rz * direction.Y, z);
                }
            }
        }

        private void Intersect2(Vector2 location, Vector2 direction, Span<(int, int)> hits)
        {
            int x = (int)MathF.Floor(location.X);
            int y = (int)MathF.Floor(location.Y);

            int orientationX = MathF.Sign(direction.X);
            int orientationY = MathF.Sign(direction.Y);

            float srx = 1 / direction.X;
            float sry = 1 / direction.Y;

            float nrx = (x + orientationX - location.X) * srx;
            float nry = (y + orientationY - location.Y) * sry;

            for (int i = 0; i < hits.Length; i++)
            {
                if (nrx < nry)
                {
                    hits[i] = (x += orientationX, y);
                    nrx += srx;
                }
                else
                {
                    hits[i] = (x, y += orientationY);
                    nry += sry;
                }
            }
        }

        private void Intersect2(Vector3 location, Vector3 direction, Span<(int, int, int)> hits)
        {
            int x = (int)MathF.Floor(location.X);
            int y = (int)MathF.Floor(location.Y);
            int z = (int)MathF.Floor(location.Z);

            int orientationX = MathF.Sign(direction.X);
            int orientationY = MathF.Sign(direction.Y);
            int orientationZ = MathF.Sign(direction.Z);

            float srx = 1 / direction.X;
            float sry = 1 / direction.Y;
            float srz = 1 / direction.Z;

            float nrx = (x + orientationX - location.X) * srx;
            float nry = (y + orientationY - location.Y) * sry;
            float nrz = (z + orientationZ - location.Z) * srz;

            for (int i = 0; i < hits.Length; i++)
            {
                if (nrx < nry && nrx < nrz)
                {
                    hits[i] = (x += orientationX, y, z);
                    nrx += srx;
                }
                else if (nry < nrx && nry < nrz)
                {
                    hits[i] = (x, y += orientationY, z);
                    nry += sry;
                }
                else
                {
                    hits[i] = (x, y, z += orientationZ);
                    nrz += srz;
                }
            }
        }
    }
}
