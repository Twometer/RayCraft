using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Craft.Net.Packets.Play
{
    public class S02ChatMessage : IPacket
    {
        public string jsonMessage;

        public int GetId()
        {
            return 0x02;
        }

        public void Handle(NetHandler netHandler)
        {
            netHandler.HandleS02ChatMessage(this);
        }

        public void Receive(PacketBuffer buffer)
        {
            this.jsonMessage = buffer.ReadString();
        }

        public void Send(PacketBuffer buffer)
        {
            throw new NotImplementedException();
        }
    }
}
