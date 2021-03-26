using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Craft.Client.World.Entities
{
    public class Entity
    {
        public int EntityId;

        public double PosX;
        public double PosY;
        public double PosZ;

        public double LastTickPosX;
        public double LastTickPosY;
        public double LastTickPosZ;

        public float Yaw;
        public float Pitch;
        public double MotionX;
        public double MotionY;
        public double MotionZ;

        public float Health;

        public void SetPosition(double x, double y, double z)
        {
            PosX = x;
            PosY = y;
            PosZ = z;
        }
    }
}