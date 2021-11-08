using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Numerics;

using static RayCraft.Algorithms;

namespace RayCraft.Tests
{
    [TestClass]
    public class LineAlgorithmTests
    {
        [TestMethod]
        public void TestIntersect_2D_X()
        {
            Span<(int, int)> hits = stackalloc (int, int)[3];
            Intersect(new Vector2(0.5f, 0.5f), new Vector2(1.0f, 0.0f), hits);
            Assert.AreEqual((1, 0), hits[0]);
            Assert.AreEqual((2, 0), hits[1]);
            Assert.AreEqual((3, 0), hits[2]);

            Intersect(new Vector2(-0.125f, -0.5f), new Vector2(0.1f, 0.0f), hits);
            Assert.AreEqual((0, -1), hits[0]);
            Assert.AreEqual((1, -1), hits[1]);
            Assert.AreEqual((2, -1), hits[2]);

            Intersect(new Vector2(0.0f, 0.0f), new Vector2(-1.0f, 0.0f), hits);
            Assert.AreEqual((-1, 0), hits[0]);
            Assert.AreEqual((-2, 0), hits[1]);
            Assert.AreEqual((-3, 0), hits[2]);
        }

        [TestMethod]
        public void TestIntersect_2D_XY()
        {
            Span<(int, int)> hits = stackalloc (int, int)[5];
            Intersect(new Vector2(0.5f, 0.5f), new Vector2(1.0f, 0.5f), hits);
            Assert.AreEqual((1, 0), hits[0]);
            Assert.AreEqual((1, 1), hits[1]);
            Assert.AreEqual((2, 1), hits[2]);
            Assert.AreEqual((3, 1), hits[3]);
            Assert.AreEqual((3, 2), hits[4]);

            Intersect(new Vector2(0.5f, 0.5f), new Vector2(1.0f, -0.5f), hits);
            Assert.AreEqual((1, -0), hits[0]);
            Assert.AreEqual((1, -1), hits[1]);
            Assert.AreEqual((2, -1), hits[2]);
            Assert.AreEqual((3, -1), hits[3]);
            Assert.AreEqual((3, -2), hits[4]);
        }

        [TestMethod]
        public void TestIntersect_3D_XY()
        {
            Span<(int, int, int)> hits = stackalloc (int, int, int)[5];
            Intersect(new Vector3(0.5f, 0.5f, 0.0f), new Vector3(1.0f, 0.5f, 0.0f), hits);
            Assert.AreEqual((1, 0, 0), hits[0]);
            Assert.AreEqual((1, 1, 0), hits[1]);
            Assert.AreEqual((2, 1, 0), hits[2]);
            Assert.AreEqual((3, 1, 0), hits[3]);
            Assert.AreEqual((3, 2, 0), hits[4]);
        }

        [TestMethod]
        public void TestIntersect_3D_XZ()
        {
            Span<(int, int, int)> hits = stackalloc (int, int, int)[5];
            Intersect(new Vector3(0.5f, 0.0f, 0.5f), new Vector3(1.0f, 0.0f, 0.5f), hits);
            Assert.AreEqual((1, 0, 0), hits[0]);
            Assert.AreEqual((1, 0, 1), hits[1]);
            Assert.AreEqual((2, 0, 1), hits[2]);
            Assert.AreEqual((3, 0, 1), hits[3]);
            Assert.AreEqual((3, 0, 2), hits[4]);
        }

        [TestMethod]
        public void TestIntersect_3D_XYZ()
        {
            Span<(int, int, int)> hits = stackalloc (int, int, int)[5];
            Intersect(new Vector3(0.0f, 0.0f, -0.5f), new Vector3(1.2f, 1.1f, 1.0f), hits);
            Assert.AreEqual((0, 0, 0), hits[0]);
            Assert.AreEqual((1, 0, 0), hits[1]);
            Assert.AreEqual((1, 1, 0), hits[2]);
            Assert.AreEqual((1, 1, 1), hits[3]);
            Assert.AreEqual((2, 1, 1), hits[4]);

            Intersect(new Vector3(0.0f, 0.0f, 0.0f), new Vector3(-0.9f, 1.1f, 1.0f), hits);
            Assert.AreEqual((-1, 1, 0), hits[0]);
            Assert.AreEqual((-1, 1, 1), hits[1]);
            Assert.AreEqual((-2, 1, 1), hits[2]);
            Assert.AreEqual((-2, 2, 1), hits[3]);
            Assert.AreEqual((-2, 2, 2), hits[4]);
        }
    }
}
