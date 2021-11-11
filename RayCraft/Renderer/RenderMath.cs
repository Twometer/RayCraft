using System;
using System.Numerics;

namespace RayCraft.Renderer
{
    public class RenderMath
    {
        private int imageWidth;
        private int imageHeight;
        private float aspectRatio = 1.0f;
        private static float fov = 70.0f;
        private float scale = MathF.Tan(ToRadians(fov * 0.5f));
        private Matrix4x4 cam = new Matrix4x4();

        public void Initialize(int width, int height)
        {
            this.imageWidth = width;
            this.imageHeight = height;
            aspectRatio = (float)width / height;
        }

        public void UpdateMatrix(Location src)
        {
            cam = Matrix4x4.CreateFromYawPitchRoll(ToRadians(-src.Yaw), ToRadians(-src.Pitch), 0.0f);
        }

        public Vector3 CreateRay(float x, float y)
        {
            float px = (2 * (x + 0.5f) / imageWidth - 1) * aspectRatio * scale;
            float py = (1 - 2 * (y + 0.5f) / imageHeight) * scale;
            
            return Vector3.Normalize(Vector3.Transform(new Vector3(px, py, -1), cam));
        }

        public static float ToRadians(float f)
        {
            return f * (float)Math.PI / 180.0f;
        }

    }
}
