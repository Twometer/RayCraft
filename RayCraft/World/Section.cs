
namespace Craft.Client.World
{
    public class Section
    {
        public Chunk chunk;
        public byte[] Blocks;

        public int X;
        public int YIdx;
        public int Z;

        public Section(Chunk chunk, int x, int yIdx, int z)
        {
            this.chunk = chunk;
            X = x;
            YIdx = yIdx;
            Z = z;
            Blocks = new byte[4096];
        }

        public byte GetBlock(int x, int y, int z)
        {
            if (Blocks == null)
                return 0;
            return Blocks[GetIdx(x, y, z)];
        }

        public void SetBlock(int x, int y, int z, byte id)
        {
            Blocks[GetIdx(x, y, z)] = id;
        }

        private int GetIdx(int x, int y, int z)
        {
            return ((y * 16 + z) * 16 + x);
        }

    }
}
