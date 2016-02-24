
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hamburglar.Core
{
    public class GameTransport
    {
        public string id { get; set; }
        public List<VersionedGameData<FloorData>> floors { get; set; }
        public VersionedGameData<PlayerMetaData> me { get; set; }
        public VersionedGameData<GameMetaData> game { get; set; }
        public static Game ToGame(GameTransport game)
        {
            Game result = new Game();
            result.Building = new Building();
            result.Building.Floors = game.floors.Select(x => FloorData.ToFloor(x.d)).ToArray();
            result.Building.TotalValue = game.game.d.buildingScore;
            result.Floors = game.game.d.floors;
            result.RoomsPerFloor = game.game.d.roomsPerFloor;
            result.PlayerVersions = new Dictionary<string, int>();
            result.Players = game.game.d.players.Select(x => PlayerMetaData.ToPlayer(x)).ToList();
            result.PlayerIds = game.game.d.players.Select(x => x.id).ToList();
            result.Id = game.id;
            if (game.me != null && game.me.d != null)
            {
                result.PlayerScore.Add(game.me.d.id, game.me.d.score);
            }
            // set versions
            result.FloorVersions = game.floors.Select(x => x.v).ToArray();
            result.GameMetaVersion = game.game.v;
            result.PlayerVersions.Add(game.me.d.id, game.me.v);
            return result;
        }
    }

}
