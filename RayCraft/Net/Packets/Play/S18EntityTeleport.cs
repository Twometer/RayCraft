using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Craft.Net.Packets.Play
{
    public class S18EntityTeleport : IPacket
    {
        public int entityId;

        public double x;
        public double y;
        public double z;

        public float yaw;
        public float pitch;

        public bool onGround;

        public int GetId()
        {
            return 0x18;
        }

        public void Handle(NetHandler netHandler)
        {
            netHandler.HandleS18EntityTeleport(this);
        }

        public void Receive(PacketBuffer buffer)
        {
            entityId = buffer.ReadVarInt();

            x = buffer.ReadFractInt();
            y = buffer.ReadFractInt();
            z = buffer.ReadFractInt();

            yaw = buffer.ReadByteEncRot();
            pitch = buffer.ReadByteEncRot();

            onGround = buffer.ReadBool();
        }

        public void Send(PacketBuffer buffer)
        {
            throw new NotImplementedException();
        }
    }
}
