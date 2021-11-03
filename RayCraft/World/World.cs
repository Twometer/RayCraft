using Craft.Client.World.Entities;
using OpenTK;
using RayCraft.Utils;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Craft.Client.World
{
    public class World
    {
        //private Chunk[] chks = new Chunk[1024];
        private Chunk[,] chunks = new Chunk[64, 64];
        //private Dictionary<ChunkKey, Chunk> chunks = new Dictionary<ChunkKey, Chunk>();
        private ConcurrentDictionary<int, Entity> entities = new ConcurrentDictionary<int, Entity>();

        public IEnumerable<Entity> GetEntities()
        {
            return entities.Values;
        }

        public Entity GetEntity(int id)
        {
            if (!entities.ContainsKey(id))
                return null;
            return entities[id];
        }

        public void AddEntity(Entity entity)
        {
            entities.TryAdd(entity.EntityId, entity);
        }

        public void SetBlock(int x, int y, int z, int id)
        {
            if (id < 256)
                SetBlock(x, y, z, (byte)id);
        }


        public void SetBlock(int x, int y, int z, byte id)
        {
            Chunk chunk = GetChunkFromBlockCoords(x, z);
            if (chunk != null)
                chunk.SetBlock(x & 15, y, z & 15, id);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte GetBlock(int x, int y, int z)
        {
            uint cx = (uint)(x >> 4) + 32;
            uint cz = (uint)(z >> 4) + 32;

            if (cx > 63 || cz > 63)
                return 0;

            var chunk = chunks[cx, cz];
            if (chunk != null)
                return chunk.GetBlock(x, y, z);
            else return 0;
        }

        public void AddChunk(Chunk chunk)
        {
            chunks[chunk.x + 32, chunk.z + 32] = chunk;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Chunk GetChunkFromBlockCoords(int x, int z)
        {
            return GetChunk(x >> 4, z >> 4);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Chunk GetChunk(int x, int z)
        {
            return chunks[x + 32, z + 32];
        }

        public void DestroyChunk(int x, int z)
        {
            long h = Hash(x, z);
            /*if (chunks.ContainsKey(h))
            {
                chunks.TryRemove(h, out Chunk chunk);
                chunk.Destroy();
            }*/
        }



        public List<AxisAlignedBB> GetCubes(int xx, int yy, int zz, int r)
        {
            List<AxisAlignedBB> aabbs = new List<AxisAlignedBB>();
            for (int x = -r; x < r; x++)
            {
                for (int y = -r; y < r; y++)
                {
                    for (int z = -r; z < r; z++)
                    {
                        var bid = GetBlock(xx + x, yy + y, zz + z);
                        if (bid > 0 && bid != 8 && bid != 9 && bid != 31 && bid != 175)
                        {
                            aabbs.Add(new AxisAlignedBB(xx + x, yy + y, zz + z, xx + x, yy + y, zz + z).expand(1.0f, 1.0f, 1.0f));
                        }
                    }
                }
            }
            return aabbs;
        }

        public static AxisAlignedBB GetCubicBox(int x, int y, int z)
        {
            return new AxisAlignedBB(x, y, z, x, y, z).expand(1.0f, 1.0f, 1.0f);
        }

        public static long Hash(int x, int z)
        {
            return ((long)x << 32) + (long)z - -2147483648L;
        }

        public static long Hash(Chunk chk)
        {
            return ((long)chk.x << 32) + (long)chk.z - -2147483648L;
        }
    }
}