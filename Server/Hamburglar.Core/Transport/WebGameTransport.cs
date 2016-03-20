
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hamburglar.Core
{
    public class WebGameTransport : GameTransport
    {
        public bool success { get; set; }
        public string error { get; set; }
        public bool isFullGame { get; set; }
        public PlayerMetaData roomOccupant { get; set; }
        public static WebGameTransport CreateSuccess()
        {
            return new WebGameTransport() { success = true };
        }
        public static WebGameTransport CreateError(string message)
        {
            return new WebGameTransport() { success = false, error = message };
        }
        public static WebGameTransport CheckGameVersion(Game sourceData, WebGameTransport game, int? floor, string playerId, int gameVersion, int floorVersion, int playerVersion)
        {
            game.id = sourceData.Id;
            if (sourceData.GameMetaVersion > gameVersion)
            {
                game.game = new VersionedGameData<GameMetaData>()
                {
                    v = sourceData.GameMetaVersion,
                    d = new GameMetaData()
                    {
                        floors = sourceData.Floors,
                        roomsPerFloor = sourceData.RoomsPerFloor,
                        players = GetPlayerModels(sourceData.Players),
                        buildingScore = sourceData.Building.TotalValue,
                        start= sourceData.StartUTC,
                        state = sourceData.RunningState
                    }
                };
                if (sourceData.Winner != null)
                {
                    game.game.d.winner = PlayerMetaData.FromPlayer(sourceData.Winner);
                }
            }
            if (floor.HasValue)
            {
                if (sourceData.FloorVersions[floor.Value] > floorVersion)
                {
                    // only get floor with mismatched version
                    game.floors = new List<VersionedGameData<FloorData>>()
                        {
                            new VersionedGameData<FloorData>()
                            {
                                v = sourceData.FloorVersions[floor.Value],
                                d = new FloorData()
                                {
                                    i = floor.Value,
                                    occ = sourceData.Building.Floors[floor.Value].OccupiedRooms,
                                    r = sourceData.Building.Floors[floor.Value].Rooms,
                                    t = sourceData.Building.Floors[floor.Value].Traps
                                }
                            }
                        };
                }
            }
            else
            {
                // get all floors for game (for initial requests)
                game.floors = new List<VersionedGameData<FloorData>>();
                for (int fl = 0; fl < sourceData.Floors; fl++)
                {
                    game.floors.Add(new VersionedGameData<FloorData>()
                    {
                        v = sourceData.FloorVersions[fl],
                        d = new FloorData()
                        {
                            i = fl,
                            occ = sourceData.Building.Floors[fl].OccupiedRooms,
                            r = sourceData.Building.Floors[fl].Rooms,
                            t = sourceData.Building.Floors[fl].Traps
                        }
                    });
                }
            }
            if (sourceData.PlayerVersions[playerId] > playerVersion)
            {
                var player = sourceData.GetLocalPlayer(playerId);
                int playerScore = 0;
                sourceData.PlayerScore.TryGetValue(playerId, out playerScore);
                game.me = new VersionedGameData<PlayerMetaData>()
                {
                    v = sourceData.PlayerVersions[playerId],
                    d = new PlayerMetaData()
                    {
                        id = player.Id,
                        username = player.Username,
                        score = playerScore,
                        floor = player.Floor,
                        room = player.Room
                    }
                };
            }
            return game;
        }
        private static List<PlayerMetaData> GetPlayerModels(List<Player> list)
        {
            return list.Select(x => new PlayerMetaData()
            {
                id = x.Id,
                username = x.Username
            }).ToList();
        }
    }

}
