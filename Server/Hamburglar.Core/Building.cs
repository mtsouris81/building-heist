using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hamburglar.Core
{
    [Serializable]
    public class Building
    {
        public static Random BuildingRandom = new Random(DateTime.Now.Millisecond);
        public Building() { }
        public int TotalValue { get; set; }
        public Floor[] Floors { get; set; }
        public static Building Generate(int floors, int rooms, int maxItemsPerRoom, int lootBias)
        {
            Building result = new Building();
            result.Floors = new Floor[floors];
            Random random = new Random(DateTime.Now.Millisecond);
            for (int f = 0; f < floors; f++)
            {
                Floor floor = new Floor(rooms, maxItemsPerRoom);
                floor.Populate(BuildingRandom, result, lootBias);
                result.Floors[f] = floor;
            }
            return result;
        }
    }
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
