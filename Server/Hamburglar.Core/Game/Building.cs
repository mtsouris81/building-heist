using System;

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
}
