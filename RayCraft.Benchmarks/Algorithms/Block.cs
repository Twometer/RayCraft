using RayCraft.Data;
using RayCraft.Renderer;
using System;
using System.Numerics;

namespace RayCraft.Algorithms
{
    // Intersection algorithms are available either with max ray length or max intersection count
    // The following table can be used for quality comparison in 1920x1080
    //
    // | Intersections | Min Length | Mean Length |
    // |---------------|------------|-------------|
    // |            64 |      37.37 |       43.03 |
    // |           128 |      74.73 |       85.39 |
    // |           192 |     112.04 |      127.76 |
    // |           256 |     149.39 |      170.13 |

    public static class Block
    {
        public static BlockHit IntersectMaxLength(IWorld world, Vector3 location, Vector3 direction, float maxRayLength)
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

            while (true)
            {
                if (nrx < nry && nrx < nrz)
                {
                    if (nrx > maxRayLength) return default;
                    x += orientationX;
                    byte block = world.GetBlock(x, unchecked((byte)y), z);
                    if (block != default)
                    {
                        return new BlockHit(x, unchecked((byte)y), z, block,
                            orientationX == 1 ? BlockFace.XPos : BlockFace.XNeg,
                            new Vector3(x, location.Y + nrx * direction.Y, location.Z + nrx * direction.Z));
                    }

                    nrx += srx;
                }
                else if (nry < nrx && nry < nrz)
                {
                    if (nry > maxRayLength) return default;
                    y += orientationY;
                    if (y < 0 || y > 255) return default;
                    byte block = world.GetBlock(x, unchecked((byte)y), z);
                    if (block != default)
                    {
                        return new BlockHit(x, unchecked((byte)y), z, block, 
                            orientationY == 1 ? BlockFace.YPos : BlockFace.YNeg,
                            new Vector3(location.X + nry * direction.X, y, location.Z + nry * direction.Z));
                    }

                    nry += sry;
                }
                else
                {
                    if (nrz > maxRayLength) return default;
                    z += orientationZ;
                    byte block = world.GetBlock(x, unchecked((byte)y), z);
                    if (block != default)
                    {
                        return new BlockHit(x, unchecked((byte)y), z, block,
                            orientationZ == 1 ? BlockFace.ZPos : BlockFace.ZNeg,
                            new Vector3(location.X + nrz * direction.X, location.Y + nrz * direction.Y, z));
                    }

                    nrz += srz;
                }
            }
        }

        public static BlockHit IntersectMaxCount(IWorld world, Vector3 location, Vector3 direction, int maxIntersections)
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

            for (int i = 0; i < maxIntersections; i++)
            {
                if (nrx < nry && nrx < nrz)
                {
                    x += orientationX;
                    byte block = world.GetBlock(x, unchecked((byte)y), z);
                    if (block != default)
                    {
                        return new BlockHit(x, unchecked((byte)y), z, block,
                            orientationX == 1 ? BlockFace.XPos : BlockFace.XNeg,
                            new Vector3(x, location.Y + nrx * direction.Y, location.Z + nrx * direction.Z));
                    }

                    nrx += srx;
                }
                else if (nry < nrx && nry < nrz)
                {
                    y += orientationY;
                    if (y < 0 || y > 255) return default;
                    byte block = world.GetBlock(x, unchecked((byte)y), z);
                    if (block != default)
                    {
                        return new BlockHit(x, unchecked((byte)y), z, block,
                            orientationY == 1 ? BlockFace.YPos : BlockFace.YNeg,
                            new Vector3(location.X + nry * direction.X, y, location.Z + nry * direction.Z));
                    }

                    nry += sry;
                }
                else
                {
                    z += orientationZ;
                    byte block = world.GetBlock(x, unchecked((byte)y), z);
                    if (block != default)
                    {
                        return new BlockHit(x, unchecked((byte)y), z, block,
                            orientationZ == 1 ? BlockFace.ZPos : BlockFace.ZNeg,
                            new Vector3(location.X + nrz * direction.X, location.Y + nrz * direction.Y, z));
                    }

                    nrz += srz;
                }
            }

            return default;
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
    }
}
