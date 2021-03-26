using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Craft.Net.Packets.Login
{
    public class S03Compression : IPacket
    {
        public int threshold;

        public int GetId()
        {
            return 0x03;
        }

        public void Handle(NetHandler netHandler)
        {
            netHandler.HandleS03Compression(this);
        }

        public void Receive(PacketBuffer buffer)
        {
            threshold = buffer.ReadVarInt();
        }

        public void Send(PacketBuffer buffer)
        {
            throw new NotImplementedException();
        }
    }
}
