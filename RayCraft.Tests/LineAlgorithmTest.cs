using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Numerics;

namespace RayCraft.Tests
{
    [TestClass]
    public class LineAlgorithmTest
    {
        [TestMethod]
        public void TestGetFirstIntersection()
        {
            Assert.AreEqual((1, 0), GetFirstIntersection(new Vector2(0.0f, 0.0f), new Vector2(2.0f, 1.9f)));
            Assert.AreEqual((1, -2), GetFirstIntersection(new Vector2(1.5f, -0.7f), new Vector2(0.5f, -1.5f)));
        }

        [TestMethod]
        public void TestIntersect()
        {
            Span<(int, int)> hits = stackalloc (int, int)[5];
            Intersect(new Vector2(0.5f, 0.5f), new Vector2(1.0f, 0.5f), hits);
            Assert.AreEqual((1, 0), hits[0]);
            Assert.AreEqual((1, 1), hits[1]);
            Assert.AreEqual((2, 1), hits[2]);
            Assert.AreEqual((3, 1), hits[3]);
            Assert.AreEqual((3, 2), hits[4]);
        }

        private (int x, int y) GetFirstIntersection(Vector2 location, Vector2 direction)
        {
            int x = (int)MathF.Floor(location.X);
            int y = (int)Math.Floor(location.Y);

            int orientationX = MathF.Sign(direction.X);
            int orientationY = MathF.Sign(direction.Y);

            float rx = (x + orientationX - location.X) / direction.X;
            float ry = (y + orientationY - location.Y) / direction.Y;

            if (rx < ry)
                return (x + orientationX, y);
            else
                return (x, y + orientationY);
        }

        private void Intersect(Vector2 location, Vector2 direction, Span<(int, int)> hits)
        {
            int x = (int)MathF.Floor(location.X);
            int y = (int)Math.Floor(location.Y);

            int orientationX = MathF.Sign(direction.X);
            int orientationY = MathF.Sign(direction.Y);

            for (int i = 0; i < hits.Length; i++)
            {
                float rx = (x + orientationX - location.X) / direction.X;
                float ry = (y + orientationY - location.Y) / direction.Y;

                if (rx < ry)
                {
                    hits[i] = (x += orientationX, y);
                    location = new Vector2(x, location.Y + rx * direction.Y);
                }
                else
                {
                    hits[i] = (x, y += orientationY);
                    location = new Vector2(location.X + ry * direction.X, y);
                }
            }
        }

        private void Intersect3d(Vector3 location, Vector3 direction, Span<(int, int, int)> hits)
        {
            int x = (int)MathF.Floor(location.X);
            int y = (int)MathF.Floor(location.Y);
            int z = (int)MathF.Floor(location.Z);

            int orientationX = MathF.Sign(direction.X);
            int orientationY = MathF.Sign(direction.Y);
            int orientationZ = MathF.Sign(direction.Z);

            for (int i = 0; i < hits.Length; i++)
            {
                float rx = (x + orientationX - location.X) / direction.X;
                float ry = (y + orientationY - location.Y) / direction.Y;
                float rz = (z + orientationZ - location.Z) / direction.Z;

                if (rx < ry && rx < rz)
                {
                    hits[i] = (x += orientationX, y, z);
                    location = new Vector3(x, location.Y + rx * direction.Y, location.Z + rx * direction.Z);
                }
                else if (ry < rx && ry < rz)
                {
                    hits[i] = (x, y += orientationY, z);
                    location = new Vector3(location.X + ry * direction.X, y, location.Z + ry * direction.Z);
                }
                else
                {
                    hits[i] = (x, y, z += orientationZ);
                    location = new Vector3(location.X + rz * direction.X, location.Y + rz * direction.Y, z);
                }
            }
        }
    }
}
