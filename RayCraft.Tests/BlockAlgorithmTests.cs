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
            BlockHit hit = Block.Intersect(world, new Vector3(0.0f, 0.0f, -0.5f), new Vector3(1.2f, 1.1f, 1.0f), 10.0f);
            Assert.Equal(default, hit);
        }
        
        [Fact]
        public void TestIntersect_Xpos_Ypos_Zpos()
        {
            IWorld world = new ChunkSectionWorld();
            BlockHit hit;

            world.SetBlock(1, 1, 0, 1);
            hit = Block.Intersect(world, new Vector3(0.0f, 0.0f, -0.5f), new Vector3(1.2f, 1.1f, 1.0f), 10.0f);
            Assert.Equal(1, hit.X);
            Assert.Equal(1, hit.Y);
            Assert.Equal(0, hit.Z);
            Assert.Equal(1, hit.Block);
            Assert.Equal(BlockFace.YPos, hit.Face);
            Assert.Equal(1.0909091f, hit.Intersection.X, 6);
            Assert.Equal(1.0f, hit.Intersection.Y);
            Assert.Equal(0.4090909f, hit.Intersection.Z, 6);

            world.SetBlock(1, 0, 0, 1);
            hit = Block.Intersect(world, new Vector3(0.0f, 0.0f, -0.5f), new Vector3(1.2f, 1.1f, 1.0f), 10.0f);
            Assert.Equal(1, hit.X);
            Assert.Equal(0, hit.Y);
            Assert.Equal(0, hit.Z);
            Assert.Equal(1, hit.Block);
            Assert.Equal(BlockFace.XPos, hit.Face);
            Assert.Equal(1.0f, hit.Intersection.X);
            Assert.Equal(0.9166667f, hit.Intersection.Y, 6);
            Assert.Equal(0.3333333f, hit.Intersection.Z, 6);

            world.SetBlock(0, 0, 0, 1);
            hit = Block.Intersect(world, new Vector3(0.0f, 0.0f, -0.5f), new Vector3(1.2f, 1.1f, 1.0f), 10.0f);
            Assert.Equal(0, hit.X);
            Assert.Equal(0, hit.Y);
            Assert.Equal(0, hit.Z);
            Assert.Equal(1, hit.Block);
            Assert.Equal(BlockFace.ZPos, hit.Face);
            Assert.Equal(0.6f, hit.Intersection.X, 6);
            Assert.Equal(0.55f, hit.Intersection.Y, 6);
            Assert.Equal(0.0f, hit.Intersection.Z);
        }
    }
}
