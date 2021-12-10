using System;

namespace RayCraft.Data
{
    public class ChunkSectionWorld : IWorld
    {
        //               byte[]        Section.Blocks
        //                     ?       Section
        //                      []     Chunk.Sections
        //                        ?    Chunk
        //                         [,] chunks
        private readonly byte[]?[]?[,] chunks;

        public ChunkSectionWorld()
        {
            chunks = new byte[]?[]?[64, 64];
        }

        public ChunkSectionWorld(WorldSerializer.Chunk[] chunks)
        {
            this.chunks = new byte[]?[]?[64, 64];
            foreach (var chunk in chunks)
            {
                byte[]?[] sections = new byte[]?[16];
                foreach (var section in chunk.Sections)
                {
                    sections[section.Y] = section.Data;
                }
                this.chunks[chunk.X + 32, chunk.Z + 32] = sections;
            }
        }

        public byte GetBlock(int x, byte y, int z)
        {
            uint cx = (uint)(x >> 4) + 32;
            uint cz = (uint)(z >> 4) + 32;

            if (cx > 63 || cz > 63)
                return 0;

            byte[]?[]? chunk = chunks[cx, cz];
            if (chunk is null)
                return 0;

            byte[]? section = chunk[y >> 4];
            if (section is null)
                return 0;

            return section[((((y & 0x0F) << 4) + (z & 0x0F)) << 4) + (x & 0x0F)];
        }

        public void SetBlock(int x, byte y, int z, byte blockId)
        {
            uint cx = (uint)(x >> 4) + 32;
            uint cz = (uint)(z >> 4) + 32;

            if (cx > 63)
                throw new ArgumentOutOfRangeException(nameof(x));
            if (cz > 63)
                throw new ArgumentOutOfRangeException(nameof(z));

            if (chunks[cx, cz] is null)
                chunks[cx, cz] = new byte[]?[16];
            byte[]?[] chunk = chunks[cx, cz]!;

            if (chunk[y >> 4] is null) chunk[y >> 4] = new byte[4096];
            byte[] section = chunk[y >> 4]!;

            section[((((y & 0x0F) << 4) + (z & 0x0F)) << 4) + (x & 0x0F)] = blockId;
        }
    }
}
