using Craft.Net.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Craft.Net.Packets.Play
{
    public class S23BlockUpdate : IPacket
    {
        public BlockPos blockPos;
        public int newId;

        public int GetId()
        {
            return 0x23;
        }

        public void Handle(NetHandler netHandler)
        {
            netHandler.HandleS23BlockUpdate(this);
        }

        public void Receive(PacketBuffer buffer)
        {
            blockPos = BlockPos.FromLong(buffer.ReadULong());
            newId = buffer.ReadVarInt() >> 4;
        }

        public void Send(PacketBuffer buffer)
        {
            throw new NotImplementedException();
        }
    }
}
