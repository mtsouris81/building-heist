
namespace Hamburglar.Core
{
    public class PlayerMetaData
    {
        public string id { get; set; }
        public string username { get; set; }
        public int score { get; set; }
        public int floor { get; set; }
        public int? room { get; set; }
        public int? utcoffset { get; set; }

        public static Player ToPlayer(PlayerMetaData player)
        {
            if (player == null)
                return null;

            Player result = new Player();
            result.Id = player.id;
            result.Username = player.username;
            result.Floor = player.floor;
            result.Room = player.room;
            result.UTCOffset = player.utcoffset;
            return result;
        }
        public static PlayerMetaData FromPlayer(Player player)
        {
            PlayerMetaData result = new PlayerMetaData();
            result.id = player.Id;
            result.room = player.Room;
            result.floor = player.Floor;
            result.username = player.Username;
            result.utcoffset = player.UTCOffset;
            return result;
        }
    }

}
