using Craft.Net.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Craft.Net.Packets.Play
{
    public class S22MultiBlockUpdate : IPacket
    {
        public int chunkX;
        public int chunkZ;
        public BlockUpdate[] updates;

        public int GetId()
        {
            return 0x22;
        }

        public void Handle(NetHandler netHandler)
        {
            netHandler.HandleS22MultiBlockUpdate(this);
        }

        public void Receive(PacketBuffer buffer)
        {
            chunkX = buffer.ReadInt();
            chunkZ = buffer.ReadInt();

            int arrLen = buffer.ReadVarInt();
            updates = new BlockUpdate[arrLen];
            for(int i = 0; i < arrLen; i++)
            {
                short temp = buffer.ReadShort();
                int x = temp >> 12 & 15;
                int y = temp & 255;
                int z = temp >> 8 & 15;
                int newId = buffer.ReadVarInt() >> 4;
                updates[i] = new BlockUpdate(x, y, z, newId);
            }
        }

        public void Send(PacketBuffer buffer)
        {
            throw new NotImplementedException();
        }
    }
}
