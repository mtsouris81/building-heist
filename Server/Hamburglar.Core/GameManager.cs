using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hamburglar.Core
{
    public class GameDataManager
    {
        public static int MAX_ITEMS_PER_ROOM = 5;
        public static int MAX_GAME_FLOORS = 25;
        public static int MAX_GAME_ROOMS_PER_FLOOR = 9;

        public static GameDataManager Current { get; set; }


        public GameDataManager()
        {

        }

        public Game Create(Player owner, string title, int floors, int roomsPerFloor, List<Player> players)
        {
            if (floors > MAX_GAME_FLOORS)
                floors = MAX_GAME_FLOORS;

            if (roomsPerFloor > MAX_GAME_ROOMS_PER_FLOOR)
                roomsPerFloor = MAX_GAME_ROOMS_PER_FLOOR;
            
            players.Add(owner);
            var playerIds = players.Select(x => x.Id).Distinct().ToList();
            Game result = new Game()
            {
                Title = title,
                Creator = owner,
                PlayerIds= playerIds,
                Players = players.Select(x => x.Clone()).ToList(),
                Id = Guid.NewGuid().ToString("N"),
                Floors = floors,
                RoomsPerFloor = roomsPerFloor,
                Building = Building.Generate(floors, roomsPerFloor, MAX_ITEMS_PER_ROOM, 10),
                FloorVersions = new int[floors],
                PlayerVersions = new Dictionary<string,int>(StringComparer.OrdinalIgnoreCase),
                GameMetaVersion = 1
            };
            for (int i = 0; i < floors; i++)
            {
                result.FloorVersions[i] = 1;
            }
            foreach(var id in playerIds)
            {
                result.PlayerVersions.Add(id, 1);
                result.PlayerScore.Add(id, 0);
            }
            return result;
        }
    }
}
