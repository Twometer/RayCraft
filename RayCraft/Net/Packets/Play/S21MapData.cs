using Craft.Client.World;
using Craft.Net.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Craft.Net.Packets.Play
{
    public class S21MapData : IPacket
    {
        public bool Delete;
        public int DeletedX;
        public int DeletedZ;

        public Chunk Chunk;


        public int GetId()
        {
            return 0x21;
        }

        public void Handle(NetHandler netHandler)
        {
            netHandler.HandleS21MapData(this);
        }

        public void Receive(PacketBuffer buffer)
        {
            int chunkX = buffer.ReadInt();
            int chunkZ = buffer.ReadInt();
            bool groundUpContinuous = buffer.ReadBool();
            short bitmask = buffer.ReadShort();
            int size = buffer.ReadVarInt();

            if (groundUpContinuous && bitmask == 0)
            {
                Delete = true;
                DeletedX = chunkX;
                DeletedZ = chunkZ;
            }
            else
            {
                Delete = false;
                Chunk chunk = new Chunk(chunkX, chunkZ);
                ChunkExtracted extracted = new ChunkExtracted();
                extracted.bitmask = bitmask;
                extracted.storage = new byte[size];
                buffer.GetStream().Read(extracted.storage, 0, extracted.storage.Length);
                chunk.Initialize(extracted, 0, (ushort)bitmask);
                Chunk = chunk;
            }
        }

        public void Send(PacketBuffer buffer)
        {
            throw new NotImplementedException();
        }

        private static int getChunkPacketSize(int length, bool skyLight, bool continuous)
        {
            int var3 = length * 2 * 16 * 16 * 16;
            int var4 = length * 16 * 16 * 16 / 2;
            int var5 = skyLight ? length * 16 * 16 * 16 / 2 : 0;
            int var6 = continuous ? 256 : 0;
            return var3 + var4 + var5 + var6;
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
    }
}
