using System;
using System.Numerics;

namespace RayCraft.Algorithms
{
    public static class Line
    {
        // A block at x, y is a symmetric interval [x, x+1] and [y, y+1]. From point x in positive direction the next block is x+1.
        public static void Intersect(Vector2 location, Vector2 direction, Span<(int, int)> hits)
        {
            int orientationX = MathF.Sign(direction.X);
            int orientationY = MathF.Sign(direction.Y);

            float srx = MathF.Abs(1 / direction.X);
            float sry = MathF.Abs(1 / direction.Y);

            float fx = MathF.Floor(location.X);
            float fy = MathF.Floor(location.Y);

            int x = GetCurrent(location.X, fx, orientationX);
            int y = GetCurrent(location.Y, fy, orientationY);

            float nrx = GetNextR(location.X, fx, orientationX, srx);
            float nry = GetNextR(location.Y, fy, orientationY, sry);

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

        public static void Intersect(Vector3 location, Vector3 direction, Span<(int, int, int)> hits)
        {
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

        private static int GetCurrent(float location, float floor, int orientation)
        {
            if (location == floor && orientation == -1)
            {
                return (int)floor -1;
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
    }
}
