namespace RayCraft.Data
{
    public interface IWorld
    {
        byte GetBlock(int x, byte y, int z);
        void SetBlock(int x, byte y, int z, byte blockId);
    }
}
