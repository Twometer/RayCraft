using RayCraft.Renderer;
using System.Numerics;
using Xunit;

namespace RayCraft.Tests
{
    public class CameraTests
    {
        private readonly Camera camera;

        public CameraTests()
        {
            camera = new Camera(1920, 1080, 70);
            camera.Update(0.0f, 0.0f);
        }

        [Fact]
        public void TestNormalized()
        {
            for (int x = 0; x < 1920; x++)
            {
                for (int y = 0; y < 1080; y++)
                {
                    Vector3 ray = camera.CreateRay(x, y);
                    Assert.Equal(1.0f, ray.LengthSquared(), 6);
                }
            }
        }
    }
}
