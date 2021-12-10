using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace RayCraft.Data
{
    public static class WorldSerializer
    {
        public static async Task<ChunkSectionWorld> LoadChunkSectionWorld(string filename)
        {
            using FileStream file = new(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
            Chunk[]? chunks = await JsonSerializer.DeserializeAsync<Chunk[]>(file);
            if (chunks is null) throw new IOException("Deserialization failed. The chunk array is null.");

            return new ChunkSectionWorld(chunks);
        }

        public class Chunk
        {
            [JsonConstructor]
            public Chunk(int x, int z, Section[] sections)
            {
                X = x;
                Z = z;
                Sections = sections;
            }

            public int X { get; }
            public int Z { get; }
            public Section[] Sections { get; }
        }

        public class Section
        {
            [JsonConstructor]
            public Section(int y, byte[] data)
            {
                Y = y;
                Data = data;
            }

            public int Y { get; }
            public byte[] Data { get; }
        }
    }
}
