using Craft.Client.World;
using Craft.Net.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Craft.Net.Packets.Play
{
    public class S26MapChunkBulk : IPacket
    {
        public List<Chunk> chunks = new List<Chunk>();


        public int GetId()
        {
            return 0x26;
        }

        public void Handle(NetHandler netHandler)
        {
            netHandler.HandleS26MapChunkBulk(this);
        }

        public void Receive(PacketBuffer buffer)
        {
            bool skylight = buffer.ReadBool();
            int colCount = buffer.ReadVarInt();

            int[] xCoords = new int[colCount];
            int[] zCoords = new int[colCount];
            ChunkExtracted[] ce = new ChunkExtracted[colCount];

            for (int i = 0; i < colCount; i++)
            {
                int xC = buffer.ReadInt();
                int zC = buffer.ReadInt();
                int bitmask = buffer.ReadShort();
                xCoords[i] = xC;
                zCoords[i] = zC;
                ce[i] = new ChunkExtracted()
                {
                    bitmask = bitmask
                };
                int bitcount = GetBitcount(ce[i].bitmask);
                int len = GetChunkPacketSize(bitcount, skylight, true);
                ce[i].storage = new byte[len];
            }

            for (int i = 0; i < colCount; i++)
            {
                buffer.GetStream().Read(ce[i].storage, 0, ce[i].storage.Length);
            }

            int chunkECount = 0;
            lock (chunks)
            {
                foreach (ChunkExtracted e in ce)
                {
                    int chunkX = xCoords[chunkECount];
                    int chunkZ = zCoords[chunkECount];
                    Chunk theChunk = new Chunk(chunkX, chunkZ);
                    theChunk.Initialize(e, chunkECount, (ushort)e.bitmask);
                    chunks.Add(theChunk);
                    chunkECount++;
                }
            }
        }

        public void Send(PacketBuffer buffer)
        {
            throw new NotImplementedException();
        }

        private int GetBitcount(int n)
        {
            int count = 0;
            while (n != 0)
            {
                count++;
                n &= (n - 1);
            }
            return count;
        }

        private int GetChunkPacketSize(int length, bool skyLight, bool continuous)
        {
            int var3 = length * 2 * 16 * 16 * 16;
            int var4 = length * 16 * 16 * 16 / 2;
            int var5 = skyLight ? length * 16 * 16 * 16 / 2 : 0;
            int var6 = continuous ? 256 : 0;
            return var3 + var4 + var5 + var6;
        }
    }
}
