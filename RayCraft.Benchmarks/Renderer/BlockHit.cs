using System.Numerics;

namespace RayCraft.Renderer
{
    public readonly struct BlockHit
    {
        public BlockHit(int x, byte y, int z, byte block, BlockFace face, Vector3 intersection)
        {
            X = x;
            Y = y;
            Z = z;
            Block = block;
            Face = face;
            Intersection = intersection;
        }

        public int X { get; }
        public byte Y { get; }
        public int Z { get; }
        public byte Block { get; }
        public BlockFace Face { get; }
        public Vector3 Intersection { get; }
    }
}
