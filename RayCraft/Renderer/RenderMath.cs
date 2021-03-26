using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayCraft.Renderer
{
    public class RenderMath
    {
        private const float PI = 3.1415926535897931f;

        private int imageWidth;
        private int imageHeight;
        private float aspectRatio = 1.0f;
        private float fov = 70.0f;

        public void Initialize(int width, int height)
        {
            this.imageWidth = width;
            this.imageHeight = height;
            aspectRatio = (float)width / height;
        }

        public Vector3 CreateRay(Location src, float x, float y)
        {
            float scale = (float)Math.Tan(MathHelper.DegreesToRadians(fov * 0.5));
            float Px = (float)(2 * (x + 0.5) / (float)imageWidth - 1) * aspectRatio * scale;
            float Py = (float)(1 - 2 * (y + 0.5) / (float)imageHeight) * scale;

            Matrix4 cam = Matrix4.CreateFromQuaternion(Quaternion.FromEulerAngles(MathHelper.DegreesToRadians(src.Pitch), MathHelper.DegreesToRadians(src.Yaw), 0.0f));
            var rayPWorld = cam * new Vector4(Px, Py, -1, 1);
            var rayDirection = rayPWorld.Xyz;
            rayDirection.NormalizeFast();
            return rayDirection;
        }

        public static float ToRadians(float f) => MathHelper.DegreesToRadians(f);

    }
}
