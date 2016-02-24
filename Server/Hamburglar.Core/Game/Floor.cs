using System;

namespace Hamburglar.Core
{
    [Serializable]
    public class Floor
    {
        public int RoomCount { get; private set; }
        public int MaxItems { get; private set; }
        public Floor(int roomCount, int maxItems)
        {
            RoomCount = roomCount;
            MaxItems = maxItems;
            Rooms = new int[roomCount, maxItems];
            Traps = new LootTrap[roomCount, maxItems];
            OccupiedRooms = new string[roomCount];
        }
        public Floor()
        {

        }
        public int[,] Rooms { get; set; }
        public LootTrap[,] Traps { get; set; }
        public string[] OccupiedRooms { get; set; }
        public void Populate(Random random, Building building, int bias)
        {
            int rooms = Rooms.GetLength(0);
            int items = Rooms.GetLength(1);
            for (int r = 0; r < rooms; r++)
            {
                for (int i = 0; i < items; i++)
                {
                    int val = random.Next(0, 100) + bias;
                    val = (val >= 50) ? 1 : 0;
                    building.TotalValue += val;
                    Rooms[r, i] = val;
                }
            }
        }
    }
}
