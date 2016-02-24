
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hamburglar.Core
{
    public class FloorData
    {
        public int i;
        public int[,] r;
        public LootTrap[,] t;
        public string[] occ;
        public static Floor ToFloor(FloorData floor)
        {
            Floor result = new Floor(floor.r.Length, 5);
            result.OccupiedRooms = floor.occ;
            result.Rooms = floor.r;
            result.Traps = floor.t;
            return result;
        }
    }

}
