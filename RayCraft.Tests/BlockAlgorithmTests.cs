using RayCraft.Algorithms;
using RayCraft.Data;
using RayCraft.Renderer;
using System.Numerics;
using Xunit;

namespace RayCraft.Tests
{
    public class BlockAlgorithmTests
    {
        [Fact]
        public void TestIntersect_None()
        {
            IWorld world = new ChunkSectionWorld();
            BlockHit hit = Block.IntersectMaxLength(world, new Vector3(0.0f, 0.0f, -0.5f), new Vector3(1.2f, 1.1f, 1.0f), 10.0f);
            Assert.Equal(default, hit);
        }
        
        [Fact]
        public void TestIntersect_Xpos_Ypos_Zpos()
        {
            IWorld world = new ChunkSectionWorld();
            BlockHit hit;

            world.SetBlock(1, 1, 0, 1);
            hit = Block.IntersectMaxLength(world, new Vector3(0.0f, 0.0f, -0.5f), new Vector3(1.2f, 1.1f, 1.0f), 10.0f);
            Assert.Equal(1, hit.X);
            Assert.Equal(1, hit.Y);
            Assert.Equal(0, hit.Z);
            Assert.Equal(1, hit.Block);
            Assert.Equal(BlockFace.YPos, hit.Face);
            Assert.Equal(1.0909091f, hit.Intersection.X, 6);
            Assert.Equal(1.0f, hit.Intersection.Y);
            Assert.Equal(0.4090909f, hit.Intersection.Z, 6);

            world.SetBlock(1, 0, 0, 1);
            hit = Block.IntersectMaxLength(world, new Vector3(0.0f, 0.0f, -0.5f), new Vector3(1.2f, 1.1f, 1.0f), 10.0f);
            Assert.Equal(1, hit.X);
            Assert.Equal(0, hit.Y);
            Assert.Equal(0, hit.Z);
            Assert.Equal(1, hit.Block);
            Assert.Equal(BlockFace.XPos, hit.Face);
            Assert.Equal(1.0f, hit.Intersection.X);
            Assert.Equal(0.9166667f, hit.Intersection.Y, 6);
            Assert.Equal(0.3333333f, hit.Intersection.Z, 6);

            world.SetBlock(0, 0, 0, 1);
            hit = Block.IntersectMaxLength(world, new Vector3(0.0f, 0.0f, -0.5f), new Vector3(1.2f, 1.1f, 1.0f), 10.0f);
            Assert.Equal(0, hit.X);
            Assert.Equal(0, hit.Y);
            Assert.Equal(0, hit.Z);
            Assert.Equal(1, hit.Block);
            Assert.Equal(BlockFace.ZPos, hit.Face);
            Assert.Equal(0.6f, hit.Intersection.X, 6);
            Assert.Equal(0.55f, hit.Intersection.Y, 6);
            Assert.Equal(0.0f, hit.Intersection.Z);
        }

        [Fact]
        public void TestIntersect_Xneg_Yneg_Zneg()
        {
            IWorld world = new ChunkSectionWorld();
            BlockHit hit;

            world.SetBlock(2, 49, -4, 1);
            hit = Block.IntersectMaxLength(world, new Vector3(3.1f, 50.2f, -2.5f), new Vector3(-1.3f, -1.0f, -0.8f), 10.0f);
            Assert.Equal(2, hit.X);
            Assert.Equal(49, hit.Y);
            Assert.Equal(-4, hit.Z);
            Assert.Equal(1, hit.Block);
            Assert.Equal(BlockFace.ZNeg, hit.Face);
            Assert.Equal(2.29f, hit.Intersection.X, 2);
            Assert.Equal(49.58f, hit.Intersection.Y, 2);
            Assert.Equal(-3.0f, hit.Intersection.Z);

            world.SetBlock(2, 49, -3, 1);
            hit = Block.IntersectMaxLength(world, new Vector3(3.1f, 50.2f, -2.5f), new Vector3(-1.3f, -1.0f, -0.8f), 10.0f);
            Assert.Equal(2, hit.X);
            Assert.Equal(49, hit.Y);
            Assert.Equal(-3, hit.Z);
            Assert.Equal(1, hit.Block);
            Assert.Equal(BlockFace.YNeg, hit.Face);
            Assert.Equal(2.84f, hit.Intersection.X, 2);
            Assert.Equal(50.0f, hit.Intersection.Y);
            Assert.Equal(-2.66f, hit.Intersection.Z, 2);

            world.SetBlock(2, 50, -3, 1);
            hit = Block.IntersectMaxLength(world, new Vector3(3.1f, 50.2f, -2.5f), new Vector3(-1.3f, -1.0f, -0.8f), 10.0f);
            Assert.Equal(2, hit.X);
            Assert.Equal(50, hit.Y);
            Assert.Equal(-3, hit.Z);
            Assert.Equal(1, hit.Block);
            Assert.Equal(BlockFace.XNeg, hit.Face);
            Assert.Equal(3.0f, hit.Intersection.X);
            Assert.Equal(50.12f, hit.Intersection.Y, 2);
            Assert.Equal(-2.56f, hit.Intersection.Z, 2);
        }

        [Fact]
        public void TestIntersect_BelowWorld()
        {
            IWorld world = new ChunkSectionWorld();
            world.SetBlock(-15, 0, -8, 1);
            BlockHit hit = Block.IntersectMaxLength(world, new Vector3(0.5f, -150.0f, 0.0f), new Vector3(-0.1f, 1.0f, -0.05f), 256);
            Assert.Equal(-15, hit.X);
            Assert.Equal(0, hit.Y);
            Assert.Equal(-8, hit.Z);
            Assert.Equal(1, hit.Block);
            Assert.Equal(BlockFace.YPos, hit.Face);
            Assert.Equal(-7.5f, hit.Intersection.X, 6);
            Assert.Equal(0.0f, hit.Intersection.Y);
            Assert.Equal(-14.5f, hit.Intersection.Z, 6);
        }

        [Fact]
        public void TestIntersect_AboveWorld()
        {
            IWorld world = new ChunkSectionWorld();
            world.SetBlock(22, 79, 37, 23);
            BlockHit hit = Block.IntersectMaxLength(world, new Vector3(0.3f, 300.0f, 0.0f), new Vector3(0.1f, -1.0f, 0.17f), 256);
            Assert.Equal(22, hit.X);
            Assert.Equal(79, hit.Y);
            Assert.Equal(37, hit.Z);
            Assert.Equal(23, hit.Block);
            Assert.Equal(BlockFace.YNeg, hit.Face);
            Assert.Equal(22.3f, hit.Intersection.X, 6);
            Assert.Equal(80.0f, hit.Intersection.Y);
            Assert.Equal(37.4f, hit.Intersection.Z, 6);
        }
    }
}
