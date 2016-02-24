
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hamburglar.Core
{
    public class PlayerMetaData
    {
        public string id { get; set; }
        public string username { get; set; }
        public int score { get; set; }
        public int floor { get; set; }
        public int? room { get; set; }
        public static Player ToPlayer(PlayerMetaData player)
        {
            if (player == null)
                return null;
            Player result = new Player();
            result.Id = player.id;
            result.Username = player.username;
            result.Floor = player.floor;
            result.Room = player.room;
            return result;
        }
        public static PlayerMetaData FromPlayer(Player player)
        {
            PlayerMetaData result = new PlayerMetaData();
            result.id = player.Id;
            result.room = player.Room;
            result.floor = player.Floor;
            result.username = player.Username;
            return result;
        }
    }

}
