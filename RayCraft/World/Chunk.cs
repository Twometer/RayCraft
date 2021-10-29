using Craft.Net.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Craft.Client.World
{
    public class Chunk
    {
        public int x;
        public int z;

        public Section[] sections;
        public byte[] lightValues;

        public bool lightBaked;

        public bool isDestroyed;


        public void Initialize(ChunkExtracted extracted, int c, ushort bitmask)
        {
            int idx = 0;
            for (int ct = 0; ct < 16; ct++)
            {
                if ((bitmask & (1 << ct)) != 0)
                {
                    Section section = GetSection(ct);

                    for (int y = 0; y < 16; y++)
                    {
                        for (int z = 0; z < 16; z++)
                        {
                            for (int x = 0; x < 16; x++)
                            {
                                byte b = (byte)(((extracted.storage[idx + 1] & 255) << 8 | extracted.storage[idx] & 255) >> 4);
                                if (b > 0)
                                {
                                    section.Blocks[GetIdx(x, y, z)] = b;
                                }
                                idx += 2;
                            }
                        }
                    }

                }
            }
            lightValues = new byte[16 * 16];
        }

        public int GetWorldX()
        {
            return x << 4;
        }

        public int GetWorldZ()
        {
            return z << 4;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte GetBlock(int x, int y, int z)
        {
            y &= 0xFF;
            Section section = sections[y >> 4];
            if (section == null)
                return 0;
            else
                return section.Blocks[((((y & 0x0F) << 4) + (z & 0x0F)) << 4) + (x & 0x0F)];
        }

        public void SetBlock(int x, int y, int z, byte block)
        {
            if (x >= 0 && y >= 0 && z >= 0 && x < 16 && y < 256 && z < 16)
            {
                int secIdx = y >> 4;
                Section section = GetSection(secIdx);
                section.SetBlock(x, y - (secIdx << 4), z, block);
            }
        }

        public void Destroy()
        {
            if (isDestroyed)
                return;
        }

        public void SetBlock(int x, int y, int z, int block)
        {
            if (block < 256)
                SetBlock(x, y, z, (byte)block);
        }

        private int GetIdx(int x, int y, int z)
        {
            return ((y * 16 + z) * 16 + x);
        }

        private Section GetSection(int idx)
        {
            if (sections[idx] == null)
                sections[idx] = new Section(this, x, idx, z);
            return sections[idx];
        }

        public Chunk(int x, int z)
        {
            this.x = x;
            this.z = z;
            this.sections = new Section[16];
        }
    }
}
