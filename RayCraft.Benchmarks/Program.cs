using BenchmarkDotNet.Running;

namespace RayCraft.Benchmarks
{
    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkSwitcher
                .FromTypes(new[] { typeof(LineIntersection), typeof(WorldIntersection) })
                .Run(args);
        }
    }
}
