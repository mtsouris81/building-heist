using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hamburglar.Core
{
    [Serializable]
    public class Game
    {
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
        public MessageManager Messages { get; set; }
        public int GameMetaVersion { get; set; }
        public int[] FloorVersions { get; set; }
        public Dictionary<string, int> PlayerVersions { get; set; }
        public Dictionary<string, int> PlayerScore { get; set; }

        public Game()
        {
            Players = new List<Player>();
            PlayerIds = new List<string>();
            PlayerScore = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            Messages = new MessageManager();
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
                CreateMessageForAllPlayers(MessageType.GameUpdate, playerId);
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
        public void CreateMessageForAllPlayers(MessageType type, string exceptPlayerId)
        {
            CreateMessageForAllPlayers(type, null, exceptPlayerId);
        }
        public void CreateMessageForAllPlayers(MessageType type, string tag, string exceptPlayerId)
        {
            foreach(var id in PlayerIds)
            {
                if (exceptPlayerId != null && exceptPlayerId.Equals(id))
                    continue;

                if (tag == null)
                {
                    Messages.Create(id, type);
                }
                else
                {
                    Messages.Create(id, type, tag);
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
    }

    public enum OccupyRoomValidationResult
    {
        Valid,
        AlreadyInRoom,
        NotInRoom
    }
    public enum LootValidationResult
    {
        Valid,
        NotInRoom
    }
    public class ClearLootResult
    {
        public int? LootEarned { get; set; }
        public string TrapOpponentId { get; set; }
        public bool WasTrapped
        {
            get
            {
                return !string.IsNullOrEmpty(TrapOpponentId);
            }
        }
        public bool IsNothing
        {
            get
            {
                return (!LootEarned.HasValue || LootEarned.Value == 0) && !WasTrapped;
            }
        }
    }
    public class RoomOccupiedResult
    {
        public bool OpponentCaught { get; set; }
        public string OpponentId { get; set; }
    }

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
                        buildingScore = sourceData.Building.TotalValue
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

    public class GameListResult
    {
        public bool s { get; set; }
        public List<GameListItem> i { get; private set; }

        public GameListResult()
        {
            i = new List<GameListItem>();
            s = false;
        }
        public GameListResult(IEnumerable<Game> games)
        {
            s = true;
            i = games.Select(x => new GameListItem()
            {
                i = x.Id,
                t = x.Title,
               // o = x.Creator.Username
            }).ToList();
        }
    }
    public class GameListItem
    {
        public string i { get; set; }
        public string o { get; set; }
        public string t { get; set; }
    }

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
    public class GameMetaData
    {
        public int floors { get; set; }
        public int roomsPerFloor { get; set; }
        public int buildingScore { get; set; }
        public List<PlayerMetaData> players { get; set; }
        public PlayerMetaData winner { get; set; }
    }
    public class VersionedGameData<T>
    {
        public T d { get; set; }
        public int v { get; set; }
    }
}
