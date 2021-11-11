using BenchmarkDotNet.Running;

namespace RayCraft.Benchmarks
{
    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<Intersection>();
        }
    }
}
