namespace RayCraft.Renderer
{
    public readonly struct BlockHit
    {
        public BlockHit(int x, byte y, int z, byte block, BlockFace face)
        {
            X = x;
            Y = y;
            Z = z;
            Block = block;
            Face = face;
        }

        public int X { get; }
        public byte Y { get; }
        public int Z { get; }
        public byte Block { get; }
        public BlockFace Face { get; }
    }
}
