using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Craft.Net.Packets.Play
{
    public class S12Velocity : IPacket
    {
        public double xVelocity;
        public double yVelocity;
        public double zVelocity;

        public int GetId()
        {
            return 0x12;
        }

        public void Handle(NetHandler netHandler)
        {
            netHandler.HandleS12Velocity(this);
        }

        public void Receive(PacketBuffer buffer)
        {
            xVelocity = buffer.ReadShort() / 8000;
            yVelocity = buffer.ReadShort() / 8000;
            zVelocity = buffer.ReadShort() / 8000;
        }

        public void Send(PacketBuffer buffer)
        {
            throw new NotImplementedException();
        }
    }
}
