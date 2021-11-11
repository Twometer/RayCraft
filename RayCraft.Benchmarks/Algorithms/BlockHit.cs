namespace RayCraft.Algorithms
{
    public readonly struct BlockHit
    {
        public BlockHit(int x, byte y, int z, byte block)
        {
            X = x;
            Y = y;
            Z = z;
            Block = block;
        }

        public int X { get; }
        public byte Y { get; }
        public int Z { get; }
        public byte Block { get; }
    }
}
