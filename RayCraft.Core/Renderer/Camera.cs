using System;
using System.Numerics;

namespace RayCraft.Renderer
{
    public class Camera
    {
        private Matrix4x4 matrix;
        private float inverseWidth;
        private float inverseHeight;
        private float aspectRatio;
        private float scale;

        public Camera(int width, int height, float fov)
        {
            inverseWidth = 1.0f / width;
            inverseHeight = 1.0f / height;
            aspectRatio = width * inverseHeight;
            scale = MathF.Tan(ToRadians(fov * 0.5f));
        }

        public void Update(float yaw, float pitch)
        {
            matrix = Matrix4x4.CreateFromYawPitchRoll(ToRadians(-yaw), ToRadians(-pitch), 0.0f);
        }

        public Vector3 CreateRay(int x, int y)
        {
            float px = (2 * (x + 0.5f) * inverseWidth - 1) * aspectRatio * scale;
            float py = (1 - 2 * (y + 0.5f) * inverseHeight) * scale;

            return Vector3.Normalize(Vector3.Transform(new Vector3(px, py, -1), matrix));
        }

        public static float ToRadians(float f)
        {
            return f * MathF.PI / 180.0f;
        }
    }
}
