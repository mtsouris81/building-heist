
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hamburglar.Core
{
    public class GameMetaData
    {
        public int floors { get; set; }
        public int roomsPerFloor { get; set; }
        public int buildingScore { get; set; }
        public List<PlayerMetaData> players { get; set; }
        public PlayerMetaData winner { get; set; }
    }

}
