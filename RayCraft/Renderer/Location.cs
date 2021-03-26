using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayCraft.Renderer
{
    public struct Location
    {
        public float X;
        public float Y;
        public float Z;

        public float Yaw;
        public float Pitch;

        public Location(float x, float y, float z, float yaw, float pitch)
        {
            X = x;
            Y = y;
            Z = z;
            Yaw = yaw;
            Pitch = pitch;
        }
    }
}
