using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Craft.Net.Packets.Login
{
    public class S02LoginSuccessful : IPacket
    {
        public int GetId()
        {
            return 0x02;
        }

        public void Handle(NetHandler netHandler)
        {
            netHandler.HandleS02LoginSuccesful(this);
        }

        public void Receive(PacketBuffer buffer)
        {
            
        }

        public void Send(PacketBuffer buffer)
        {
            
        }
    }
}
