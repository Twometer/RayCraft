using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Craft.Net.Packets.Login
{
    public class S00LoginRejected : IPacket
    {

        public string reason;

        public int GetId()
        {
            return 0x00;
        }

        public void Handle(NetHandler netHandler)
        {
            netHandler.HandleS00LoginRejected(this);
        }

        public void Receive(PacketBuffer buffer)
        {
            reason = buffer.ReadString();
        }

        public void Send(PacketBuffer buffer)
        {
            
        }
    }
}
