using System;
using System.Numerics;

namespace RayCraft
{
    public static class Algorithms
    {
        // A block at x, y is a symmetric interval [x, x+1] and [y, y+1].
        public static void Intersect(Vector2 location, Vector2 direction, Span<(int, int)> hits)
        {
            int x = (int)MathF.Floor(location.X);
            int y = (int)MathF.Floor(location.Y);

            int orientationX = MathF.Sign(direction.X);
            int orientationY = MathF.Sign(direction.Y);

            float srx = MathF.Abs(1 / direction.X);
            float sry = MathF.Abs(1 / direction.Y);

            float nrx = GetNextR(location.X, direction.X, x, orientationX, srx);
            float nry = GetNextR(location.Y, direction.Y, y, orientationY, sry);

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
            int x = (int)MathF.Floor(location.X);
            int y = (int)MathF.Floor(location.Y);
            int z = (int)MathF.Floor(location.Z);

            int orientationX = MathF.Sign(direction.X);
            int orientationY = MathF.Sign(direction.Y);
            int orientationZ = MathF.Sign(direction.Z);

            float srx = MathF.Abs(1 / direction.X);
            float sry = MathF.Abs(1 / direction.Y);
            float srz = MathF.Abs(1 / direction.Z);

            float nrx = GetNextR(location.X, direction.X, x, orientationX, srx);
            float nry = GetNextR(location.Y, direction.Y, y, orientationY, sry);
            float nrz = GetNextR(location.Z, direction.Z, z, orientationZ, srz);

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

        private static float GetNextR(float location, float direction, int current, int orientation, float sr)
        {
            if (orientation == 0)
            {
                return float.PositiveInfinity;
            }
            else if (location == 0.0f)
            {
                return MathF.Abs(current + orientation) * sr;
            }
            else
            {
                return MathF.Abs(current + (direction > 0 ? 1 : 0) - location) * sr;
            }
        }
    }
}
