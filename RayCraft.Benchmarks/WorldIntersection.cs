using BenchmarkDotNet.Attributes;
using RayCraft.Algorithms;
using RayCraft.Data;
using RayCraft.Renderer;
using System.Numerics;
using System.Threading.Tasks;

namespace RayCraft.Benchmarks
{
    public class WorldIntersection
    {
        private const int resolutionX = 1920;
        private const int resolutionY = 1080;

        private Camera camera;
        private ChunkSectionWorld chunkSectionWorld;

        public WorldIntersection()
        {
            camera = new Camera(resolutionX, resolutionY, 70);
            chunkSectionWorld = WorldSerializer.LoadChunkSectionWorld("world.json").GetAwaiter().GetResult();
        }

        [Benchmark]
        [Arguments(128f)]
        [Arguments(256f)]
        public void BenchSingleThread(float maxRayLength)
        {
            for (int x = 0; x < resolutionX; x++)
            {
                for (int y = 0; y < resolutionY; y++)
                {
                    Block.Intersect(chunkSectionWorld, new Vector3(0.5f, 82.0f, 0.5f), camera.CreateRay(x, y), maxRayLength);
                }
            }
        }


        [Benchmark]
        [Arguments(128f)]
        [Arguments(256f)]
        public void BenchLinqParallel(float maxRayLength)
        {
            Parallel.For(0, resolutionX, x =>
            {
                for (int y = 0; y < resolutionY; y++)
                {
                    Block.Intersect(chunkSectionWorld, new Vector3(0.5f, 82.0f, 0.5f), camera.CreateRay(x, y), maxRayLength);
                };
            });
        }
    }
}
