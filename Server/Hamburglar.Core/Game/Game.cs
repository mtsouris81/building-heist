using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hamburglar.Core
{
    [Serializable]
    public class Game
    {
        public const int MAX_ITEMS_PER_ROOM = 5;
        public const int MAX_GAME_FLOORS = 25;
        public const int MAX_GAME_ROOMS_PER_FLOOR = 9;

        public static bool ActionsAffectScore = true;
        public static int PointsForKickOut = 5;
        public static int CostOfTrap = 2;

        public string Id { get; set; }
        public string Title { get; set; }
        public int Floors { get; set; }
        public int RoomsPerFloor { get; set; }
        public Player Creator { get; set; }
        public Player Winner { get; set; }
        public List<Player> Players { get; set; }
        public List<string> PlayerIds { get; set; }
        public Building Building { get; set; }
        public int GameMetaVersion { get; set; }
        public int[] FloorVersions { get; set; }
        public Dictionary<string, int> PlayerVersions { get; set; }
        public Dictionary<string, int> PlayerScore { get; set; }

        public Game()
        {
            Players = new List<Player>();
            PlayerIds = new List<string>();
            PlayerScore = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        }
        
        private void SetPlayerRoom(string playerId, int floor, int? room)
        {
            var player = GetLocalPlayer(playerId);
            player.Floor = floor;
            player.Room = room;
            //UPDATE VERSION
            if (!PlayerVersions.ContainsKey(playerId))
            {
                PlayerVersions.Add(playerId, 0);
            }
            PlayerVersions[playerId]++;
        }

        public Player GetLocalPlayer(string id)
        {
            return Players.Where(x => x.Id.Equals(id, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
        }
        public string GetRoomOccupant(int floor, int room)
        {
            return Building.Floors[floor].OccupiedRooms[room];
        }
        public RoomOccupiedResult SetRoomOccupant(string playerId, int floor, int room)
        {
            RoomOccupiedResult result = new RoomOccupiedResult();
            if (IsValidForRoomEnter(playerId, floor, room) != OccupyRoomValidationResult.Valid)
            {
                return result;
            }
            string currentOccupant = Building.Floors[floor].OccupiedRooms[room];
            if (currentOccupant != null && !currentOccupant.Equals(playerId, StringComparison.OrdinalIgnoreCase))
            { // room is occupied by someone else
                AddToPlayerScore(currentOccupant, -PointsForKickOut);
                AddToPlayerScore(playerId, PointsForKickOut);
                result.OpponentCaught = true;
                result.OpponentId = currentOccupant;
                var opp = GetLocalPlayer(currentOccupant);
                opp.Room = null;
            }
            SetPlayerRoom(playerId, floor, room);
            Building.Floors[floor].OccupiedRooms[room] = playerId;
            FloorVersions[floor]++;
            return result;
        }
        public int GetPlayerScore(string playerId)
        {
            if (PlayerScore.ContainsKey(playerId))
                return PlayerScore[playerId];

            return 0;
        }
        public void AddToPlayerScore(string playerId, int increment)
        {
            if (!ActionsAffectScore || increment == 0)
            {
                return;
            }
            // SET SCORE AND ENFORCE LOWER BOUNDS
            if (!PlayerScore.ContainsKey(playerId))
                PlayerScore.Add(playerId, 0);

            PlayerScore[playerId] += increment;
            if (PlayerScore[playerId] < 0)
            {
                PlayerScore[playerId] = 0;
            }
            //UPDATE VERSION
            if (!PlayerVersions.ContainsKey(playerId))
            {
                PlayerVersions.Add(playerId, 0);
            }
            PlayerVersions[playerId]++;
        }
        public void AddToBuildingScore(int increment)
        {
            if (!ActionsAffectScore)
            {
                return;
            }
            Building.TotalValue += increment;
            if (Building.TotalValue < 0)
            {
                Building.TotalValue = 0;
            }
            IncrementGameMetaVersion();
        }
        public void IncrementGameMetaVersion()
        {
            if (ActionsAffectScore)
            {
                this.GameMetaVersion++;
            }
        }
        public void ExitRoom(string playerId, int floor, int room)
        {
            if (IsValidForRoomExit(playerId, floor, room) == OccupyRoomValidationResult.Valid)
            {
                SetPlayerRoom(playerId, floor, null);
                Building.Floors[floor].OccupiedRooms[room] = null;
                FloorVersions[floor]++;
            }
        }
        public ClearLootResult ClearRoomLoot(string playerId, int floor, int room, int lootIndex)
        {
            ClearLootResult result = new ClearLootResult();
            if (IsValidForLoot(playerId, floor, room, lootIndex) != LootValidationResult.Valid)
            {
                return result; // invalid, do nothing
            }
            int lootValue = Building.Floors[floor].Rooms[room, lootIndex];
            if (lootValue != 0)
            {
                bool isTrap = lootValue < 0;
                AddToPlayerScore(playerId, lootValue);
                if (isTrap)
                {
                    // get trap
                    var trap = Building.Floors[floor].Traps[room, lootIndex];
                    if (trap != null)
                    {
                        AddToPlayerScore(trap.PlayerId, CostOfTrap);
                        result.TrapOpponentId = trap.PlayerId;
                    }
                }
                else
                {
                    result.LootEarned = lootValue;
                    Building.TotalValue -= lootValue;
                }
                // finally clear placement
                Building.Floors[floor].Rooms[room, lootIndex] = 0;
                FloorVersions[floor]++;
                IncrementGameMetaVersion();
            }
            return result;
        }
        public void SetRoomLootTrap(string playerId, int floor, int room, int lootIndex, int trapId)
        {
            if (IsValidForLoot(playerId, floor, room, lootIndex) != LootValidationResult.Valid)
            {
                return;
            }
            if (GetPlayerScore(playerId) < CostOfTrap)
                return;

            Building.Floors[floor].Traps[room, lootIndex] = new LootTrap()
            {
                TrapId = trapId,
                PlayerId = playerId,
            };
            Building.Floors[floor].Rooms[room, lootIndex] = -1;
            FloorVersions[floor]++;
            AddToPlayerScore(playerId, -CostOfTrap);
        }
        public void UpdateFrom(GameTransport data)
        {
            UpdateFrom(data, true);
        }
        public void CheckForWinner()
        {
            if (Building.TotalValue > 0)
            {
                return;
            }
            string winnerId = this.PlayerScore.OrderByDescending(x => x.Value).Select(x => x.Key).FirstOrDefault();
            if (!string.IsNullOrEmpty(winnerId))
            {
                Winner = GetLocalPlayer(winnerId);
            }
        }
        public void UpdateFrom(GameTransport data, bool checkVersions)
        {
            if (this.Building == null)
                return;

            if (data.floors != null)
            {
                foreach(var f in data.floors)
                {
                    if ((checkVersions && ( f.d.i < this.FloorVersions.Length && this.FloorVersions[f.d.i] < f.v)) || !checkVersions) // if data has higher version
                    {
                        this.FloorVersions[f.d.i] = f.v;
                        this.Building.Floors[f.d.i] = FloorData.ToFloor(f.d);
                    }
                }
            }
            if (data.game != null)
            {
                if ((checkVersions && GameMetaVersion < data.game.v) || !checkVersions) // if data has higher version
                {
                    GameMetaVersion = data.game.v;
                    Winner = PlayerMetaData.ToPlayer(data.game.d.winner);
                    Building.TotalValue = data.game.d.buildingScore;
                }
            }
            if (data.me != null)
            {
                var player = data.me.d;
                if (player != null)
                {
                    if (!PlayerVersions.ContainsKey(player.id))
                    {
                        PlayerVersions.Add(player.id, 0);
                    }
                    if ((checkVersions && PlayerVersions[player.id] < data.me.v) || !checkVersions) // if data has higher version
                    {
                        PlayerVersions[player.id] = data.me.v;
                        if (!PlayerScore.ContainsKey(player.id))
                        {
                            PlayerScore.Add(player.id, 0);
                        }
                        PlayerScore[player.id] = player.score;
                    }
                }
            }
        }
        public LootValidationResult IsValidForLoot(string playerId, int floor, int room, int index)
        {
            var currentRoomOccupant = GetRoomOccupant(floor, room);
            if (currentRoomOccupant != null && currentRoomOccupant.Equals(playerId, StringComparison.OrdinalIgnoreCase))
            {
                return LootValidationResult.Valid;
            }
            return LootValidationResult.NotInRoom;
        }
        public OccupyRoomValidationResult IsValidForRoomEnter(string playerId, int floor, int room)
        {
            var currentRoomOccupant = GetRoomOccupant(floor, room);
            if (currentRoomOccupant != null && currentRoomOccupant.Equals(playerId, StringComparison.OrdinalIgnoreCase))
            {
                return OccupyRoomValidationResult.AlreadyInRoom;
            }
            return OccupyRoomValidationResult.Valid;
        }
        public OccupyRoomValidationResult IsValidForRoomExit(string playerId, int floor, int room)
        {
            var currentRoomOccupant = GetRoomOccupant(floor, room);
            if (currentRoomOccupant != null && currentRoomOccupant.Equals(playerId, StringComparison.OrdinalIgnoreCase))
            {
                return OccupyRoomValidationResult.Valid;
            }
            return OccupyRoomValidationResult.NotInRoom;
        }

        public static Game Create(Player owner, string title, int floors, int roomsPerFloor, List<Player> players)
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
                PlayerIds = playerIds,
                Players = players.Select(x => x.Clone()).ToList(),
                Id = Guid.NewGuid().ToString("N"),
                Floors = floors,
                RoomsPerFloor = roomsPerFloor,
                Building = Building.Generate(floors, roomsPerFloor, MAX_ITEMS_PER_ROOM, 10),
                FloorVersions = new int[floors],
                PlayerVersions = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase),
                GameMetaVersion = 1
            };
            for (int i = 0; i < floors; i++)
            {
                result.FloorVersions[i] = 1;
            }
            foreach (var id in playerIds)
            {
                result.PlayerVersions.Add(id, 1);
                result.PlayerScore.Add(id, 0);
            }
            return result;
        }
    }
}
