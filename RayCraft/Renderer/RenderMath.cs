using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayCraft.Renderer
{
    public static class RenderMath
    {
        private const float PI = 3.1415926535897931f;

        public static Vector CreateRay(Location src, float xOff, float yOff)
        {
            Vector ray = new Vector();

            float rotX = src.Yaw + xOff;
            float rotY = src.Pitch + yOff;
            ray.Y = (float)-Math.Sin(ToRadians(rotY));
            float xz = (float)Math.Cos(ToRadians(rotY));
            ray.X = (float)(-xz * Math.Sin(ToRadians(rotX)));
            ray.Z = (float)(xz * Math.Cos(ToRadians(rotX)));

            return ray;
        }

        public static float ToRadians(float degrees) => PI * degrees / 180.0f;
    }
}
