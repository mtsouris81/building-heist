using UnityEngine;
using System.Collections.Generic;
using Hamburglar.Core;
using System;
using Hamburglar;
using System.Reflection;
using System.ComponentModel;
using Weenus.Network;
using System.IO;

namespace Hamburglar
{
    public class HamburglarDataService : MonoBehaviour
    {
        public RectTransform WebLoadingDisplay = null;
        public Hosts Host = Hosts.LocalDebug;
        public Action<object> ServiceResolved { get; set; }

        List<IServiceResolver> services = new List<IServiceResolver>();
        Queue<QueuedRequest> RequestQueue = new Queue<QueuedRequest>();
        HamburglarContext context { get { return HamburglarContext.Instance; } }

        public static float GameCountdownSeconds = 5;

        public string GameId
        {
            get { return HamburglarContext.Instance.BuildingData.Id; }
        }
        public string PlayerId
        {
            get { return HamburglarContext.Instance.PlayerId; }
        }

        void Start()
        {
            UrlResolver.Host = GetConfiguredHost();
            Debug.Log(string.Format("Service Environment : {0} - url : {1}", this.Host, UrlResolver.Host));

            services.Add(new JsonWebServiceCall<string>("login"));
            services.Add(new JsonWebServiceCall<WebGameTransport>("create"));
            services.Add(new JsonWebServiceCall<GameListResult>("gamelist"));
            services.Add(new JsonWebServiceCall<WebGameTransport>("game"));
            services.Add(new JsonWebServiceCall<Player>("signup"));

            services.Add(new JsonWebServiceCall<PlayerListResult>("search") { UseLoadingPrompt = false });

            services.Add(new JsonWebServiceCall<BaseTransport>("friend.request"));
            services.Add(new JsonWebServiceCall<BaseTransport>("friend.reject"));
            services.Add(new JsonWebServiceCall<BaseTransport>("friend.delete"));
            services.Add(new JsonWebServiceCall<BaseTransport>("friend.accept"));

            services.Add(new JsonWebServiceCall<FriendListResult>("friend.get"));
            services.Add(new JsonWebServiceCall<GameListResult>("friend.games"));

            foreach (var s in services)
            {
                s.WebLoadingDisplay = WebLoadingDisplay.gameObject;
            }
            WebServiceGlobals.GlobalErrorCallback = (errorResonse, responseBody) =>
            {
                string errorBody = string.Empty;
                if (responseBody != null && responseBody.Length < 220)// only allow short error messages to display
            {
                    errorBody = responseBody;
                }
                HamburglarContext.Instance.SetFloatingMessage(
                    string.Format("{0}\n{1}", errorResonse, errorBody),
                    Color.red);
            };
        }

        private string GetConfiguredHost()
        {
            try
            {
                var hosts = Resources.Load<TextAsset>("hosts");
                Dictionary<string, string> HostsUrlLookup = ParseUrlLookUp(hosts.text);
                string key = this.Host.ToString().ToLower();
                return HostsUrlLookup[key];
            }
            catch
            {
                HamburglarContext.Instance.SetFloatingMessage("Unable to load hosts file.", Color.red);
                throw;
            }
        }

        private Dictionary<string, string> ParseUrlLookUp(string text)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            string[] lines = text.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            foreach(var line in lines)
            {
                if (string.IsNullOrEmpty(line.Trim()))
                    continue;

                var firstSpaceIndex = line.IndexOf(' ');
                if (firstSpaceIndex < 0)
                    continue;

                var name = line.Substring(0, firstSpaceIndex).Trim();
                var url = line.Substring(firstSpaceIndex).Trim();
                if (result.ContainsKey(name))
                {
                    result[name] = url;
                }
                else
                {
                    result.Add(name, url);
                }
            }
            return result;
        }

        public void OnApplicationQuit()
        {
            if (context != null && context.Messaging != null)
            {
                context.Messaging.Disconnect();
            }
        }
        public void Call(string name, string url, Action<object> finishedCallback)
        {
            if (AreAnyServicesActive()) // one call at a time!
            {
                QueueRequest(name, url, finishedCallback);
                return;
            }
            foreach (var s in services)
            {
                if (s.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                {
                    ServiceResolved = finishedCallback;
                    s.StartRequest(url);
                    Debug.Log(url);
                    return;
                }
            }
        }

        public bool AreAnyServicesActive()
        {
            foreach (var s in services)
            {
                if (s.IsActive)
                {
                    return true;
                }
            }
            return false;
        }
        void QueueRequest(string name, string url, Action<object> callback)
        {
            RequestQueue.Enqueue(new QueuedRequest()
            {
                name = name,
                url = url,
                callback = callback
            });
        }
        void FixedUpdate()
        {
            if (!AreAnyServicesActive() && RequestQueue.Count > 0)
            {
                var request = RequestQueue.Dequeue();
                Call(request.name, request.url, request.callback);
            }
            foreach (var s in services)
            {
                if (s.AttemptResolveResponse())
                {
                    ServiceResolved(s.Response);
                }
            }
        }
        void Update()
        {
            if (context.Messaging != null && context.Messaging.CaughtMessages.Count > 0)
            {
                while (context.Messaging.CaughtMessages.Count > 0)
                {
                    var m = context.Messaging.CaughtMessages.Dequeue();
                    if (m.IsImmediateResponse && m.Callback != null)
                    {
                        // handle simple immediate response
                        m.Callback();
                        continue;
                    }

                    HeistCaughtMessage q = m as HeistCaughtMessage;
                    if (q == null)
                    {
                        continue;
                    }

                    switch (q.Tag)
                    {
                        case IncomingMessageType.GameReady:
                            StartGameCountDown(q.StartTime);
                            break;
                        case IncomingMessageType.GameUpdate:
                            context.Messaging.GameUpdateCallback(q.Response);
                            break;
                        case IncomingMessageType.LootResult:
                            break;
                        case IncomingMessageType.ImmediateResponse:
                            q.Callback();
                            break;
                        case IncomingMessageType.OpponentTrapped:

                            if (q.PlayerId.Equals(this.PlayerId) && q.OpponentId.Equals(this.PlayerId))
                            {
                                HamburglarContext.Instance.SetFloatingMessage("You fell for one of your own traps!", Color.red);
                            }
                            else if (q.OpponentId.Equals(this.PlayerId))
                            {
                                var p1 = HamburglarContext.Instance.GetPlayerData(q.PlayerId);
                                string username1 = string.Empty;
                                if (p1 != null)
                                {
                                    username1 = p1.Username;
                                }
                                HamburglarContext.Instance.SetFloatingMessage(string.Format("{0} fell for one of your traps!", username1));
                            }
                            else if (q.PlayerId.Equals(this.PlayerId))
                            {
                                HamburglarContext.Instance.TrappedPrompt.ShowDisplay(q.OpponentId);
                            }

                            break;
                        case IncomingMessageType.OpponentCaught:

                            UnityEngine.Debug.Log("someone got caught!");

                            if (q.OpponentId == PlayerId)
                            {
                                var player = HamburglarContext.Instance.GetPlayerData(q.PlayerId);
                                var opponent = HamburglarContext.Instance.GetPlayerData(q.OpponentId);

                                if (opponent.Id == PlayerId)
                                {
                                    HamburglarContext.Instance.Popup.Show(string.Format("You got kicked out of the room by {0}.\nYou lost ${1}", player.Username, Game.PointsForKickOut), Color.red);
                                    HamburglarContext.Instance.SetView(HamburglarViewMode.Building, true);
                                }

                            }
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        private void StartGameCountDown(DateTime? startTime)
        {
            try
            {
                HamburglarContext.Instance.WaitingForPlayersDisplay.gameObject.SetActive(false);
                float totalSeconds = 0;
                if (startTime.HasValue)
                {
                    DateTime hardStartTime = startTime.Value.AddSeconds(Game.COUNTDOWN_TIME_SECONDS);
                    int? TimeOffsetMilliseconds = HamburglarContext.Instance.BuildingData.GetLocalPlayer(HamburglarContext.Instance.PlayerId).UTCOffset;
                    DateTime adjustedNow = DateTime.UtcNow.AddMilliseconds(TimeOffsetMilliseconds.GetValueOrDefault());
                    totalSeconds = (float)(hardStartTime - adjustedNow).TotalSeconds;
                }
                if (totalSeconds > 0)
                {
                    HamburglarContext.Instance.Countdown.StartCountdown(totalSeconds, null);
                }
                else
                {
                    HamburglarContext.Instance.Countdown.gameObject.SetActive(false);
                }
            }
            catch (Exception ex)
            {
                Debug.Log(ex.Message);
            }
        }

        public class QueuedRequest
        {
            public string name;
            public string url;
            public Action<object> callback;
        }

        public void OnFullGameLoaded(object data)
        {
            var gameData = data as WebGameTransport;
            if (gameData != null)
            {
                Debug.Log(string.Format("floors:{0} rooms:{1} start room: {2} floor {3}", gameData.game.d.floors, gameData.game.d.roomsPerFloor, gameData.me.d.room, gameData.me.d.floor));
                context.PlayerStartRoom = gameData.me.d.room;
                context.Floor = gameData.me.d.floor;
                context.BuildingData = GameTransport.ToGame(gameData);
                context.Building.TotalFloors = context.BuildingData.Floors;
                context.Building.RoomsPerFloor = context.BuildingData.RoomsPerFloor;

                EnforceBuildingSizeConstraints();

                context.Building.Build();
                context.OnGameDataUpdatedAndLoaded();
                MobileUIManager.Current.Manager.StartPlayMode();
                context.ClearGamePrompts();
                this.WebLoadingDisplay.gameObject.SetActive(true);
                context.Messaging.ConnectToHeistGame(context.PlayerId, UpdateGameDataFromServiceResponse);

                if (context.BuildingData.RunningState == GameState.WaitingForAllToJoin)
                {
                    context.WaitingForPlayersDisplay.gameObject.SetActive(true);
                }
                else if (context.BuildingData.StartUTC.HasValue)
                {
                    context.WaitingForPlayersDisplay.gameObject.SetActive(false);
                    StartGameCountDown(context.BuildingData.StartUTC.Value);
                }
            }
        }
        private void EnforceBuildingSizeConstraints()
        {
            if (context.Building.TotalFloors > Hamburglar.Core.Game.MAX_GAME_FLOORS)
            {
                context.Building.TotalFloors = Hamburglar.Core.Game.MAX_GAME_FLOORS;
            }
            if (context.Building.RoomsPerFloor > Hamburglar.Core.Game.MAX_GAME_ROOMS_PER_FLOOR)
            {
                context.Building.RoomsPerFloor = Hamburglar.Core.Game.MAX_GAME_ROOMS_PER_FLOOR;
            }
            if (context.Floor >= Hamburglar.Core.Game.MAX_GAME_FLOORS)
            {
                context.Floor = Hamburglar.Core.Game.MAX_GAME_FLOORS - 1;
            }
            if (context.PlayerStartRoom.GetValueOrDefault() >= Hamburglar.Core.Game.MAX_GAME_ROOMS_PER_FLOOR)
            {
                context.PlayerStartRoom = Hamburglar.Core.Game.MAX_GAME_ROOMS_PER_FLOOR - 1;
            }
        }
        public void StartGame(string gameId)
        {
            if (gameId.Equals("tutorial", StringComparison.OrdinalIgnoreCase))
            {
                // game with ID "tutorial" is special. instead of loading from web
                // the tutorial game is loaded from internal resources
                StartTutorial();
                return;
            }
            // all other games are loaded from a web service call
            context.Service.Call("game", UrlResolver.Game(gameId), OnFullGameLoaded);
        }
        public void StartTutorial()
        {
            WebGameTransport gameData = CreateGameForTutorial();
            context.PlayerStartRoom = null;
            context.Floor = 0;
            context.BuildingData = GameTransport.ToGame(gameData);
            context.Building.TotalFloors = context.BuildingData.Floors;
            context.Building.RoomsPerFloor = context.BuildingData.RoomsPerFloor;
            context.Building.Build();
            context.OnGameDataUpdatedAndLoaded();
            MobileUIManager.Current.Manager.StartPlayMode(); // the App Menu UI to go away for game play mode
            HamburglarTutorial.StartTutorial();
        }
        private WebGameTransport CreateGameForTutorial()
        {
            int roomsPerFloor = 4;
            int floors = 3;
            int itemsPerRoom = 5;
            return new WebGameTransport()
            {
                floors = new List<VersionedGameData<FloorData>>()
            {
                MakeFloor(0, roomsPerFloor, itemsPerRoom),
                MakeFloor(1, roomsPerFloor, itemsPerRoom),
                MakeFloor(2, roomsPerFloor, itemsPerRoom),
            },
                id = "tutorial",
                success = true,
                isFullGame = true,
                game = new VersionedGameData<GameMetaData>()
                {
                    d = new GameMetaData()
                    {
                        buildingScore = 100,
                        floors = floors,
                        players = new List<PlayerMetaData>(),
                        roomsPerFloor = roomsPerFloor,
                    }
                },
                me = new VersionedGameData<PlayerMetaData>()
                {
                    d = new PlayerMetaData()
                    {
                        floor = 0,
                        id = HamburglarContext.Instance.PlayerId
                    }
                }
            };
        }
        private VersionedGameData<FloorData> MakeFloor(int index, int roomsPerFloor, int itemsPerRoom)
        {
            return new VersionedGameData<FloorData>()
            {
                d = new FloorData()
                {
                    occ = new string[roomsPerFloor],
                    t = new LootTrap[roomsPerFloor, itemsPerRoom],
                    i = index,
                    r = new int[roomsPerFloor, itemsPerRoom]
                },
                v = 1
            };
        }

        public void LootServiceCall(int index, int trapId)
        {
            if (context.Messaging != null)
            {
                context.Messaging.SendMessage("Loot", GameId, PlayerId, context.Floor, context.Room, index, trapId);
            }
        }
        public void UpdateGameDataFromServiceResponse(object data)
        {
            WebGameTransport game = data as WebGameTransport;
            if (game != null)
            {
                context.BuildingData.UpdateFrom(game, false);
                context.OnGameDataUpdatedAndLoaded();

                if (game.game.d.winner != null)
                {
                    context.Building.DestroyBuilding();
                    context.ClearGamePrompts();
                    HamburglarContext.Instance.Popup.OnOK = () => // when player clicks ok, send back to games screen
                    {
                        MobileUIManager.Current.Manager.SwitchToScreen("Games");
                    };
                    HamburglarContext.Instance.Popup.Show(string.Format("GAME OVER!\n{0} won the game", game.game.d.winner.username), Color.white);
                }
            }
        }
        public void OnEnteringRoom(int floor, int room)
        {
            if (context.Messaging != null)
            {
                context.Messaging.SendMessage<WebGameTransport>("EnterRoom", EnterRoomReady, GameId, PlayerId, floor, room);
            }
        }
        public void OnExitingRoom(int floor, int room)
        {
            if (context.Messaging != null)
            {
                context.Messaging.SendMessage<WebGameTransport>("ExitRoom", ExitRoomReady, GameId, PlayerId, floor, room);
            }
        }
        public void EnterRoomReady(object data)
        {
            WebGameTransport game = data as WebGameTransport;
            if (game != null)
            {
                if (game.roomOccupant != null)
                {
                    HamburglarContext.Instance.Popup.Show(
                        string.Format("You kicked {0} out of the room and earned ${1}.", game.roomOccupant.username, Game.PointsForKickOut),
                        Color.green);
                }
                context.BuildingData.UpdateFrom(game);
                context.OnGameDataUpdatedAndLoaded();

                context.SetView(HamburglarViewMode.Room);
            }
        }
        public void ExitRoomReady(object data)
        {
            WebGameTransport game = data as WebGameTransport;
            if (game != null)
            {
                context.BuildingData.UpdateFrom(game);
                context.OnGameDataUpdatedAndLoaded();
                context.SetView(HamburglarViewMode.Building, true);
            }
        }

        public void SearchPlayers(string search, Action<PlayerListResult> callback)
        {
            Call("search", UrlResolver.SearchPlayers(search), (x) => { callback(x as PlayerListResult); });
        }
        public void FriendRequest(string friendId, Action<BaseTransport> callback)
        {
            Call("friend.request", UrlResolver.RequestFriend(friendId), (x) => { callback(x as BaseTransport); });
        }
        public void FriendReject(string friendId, Action<BaseTransport> callback)
        {
            Call("friend.reject", UrlResolver.RejectFriend(friendId), (x) => { callback(x as BaseTransport); });
        }
        public void FriendReject(string requestingFriend, string playerId, Action<BaseTransport> callback)
        {
            Call("friend.reject", UrlResolver.RejectFriend(requestingFriend, playerId), (x) => { callback(x as BaseTransport); });
        }
        public void FriendDelete(string friendId, Action<BaseTransport> callback)
        {
            Call("friend.delete", UrlResolver.DeleteFriend(friendId), (x) => { callback(x as BaseTransport); });
        }
        public void FriendAccept(string friendId, Action<BaseTransport> callback)
        {
            Call("friend.accept", UrlResolver.AcceptFriend(friendId), (x) => { callback(x as BaseTransport); });
        }
        public void FriendGet(Action<FriendListResult> callback)
        {
            Call("friend.get", UrlResolver.GetFriends(), (x) => { callback(x as FriendListResult); });
        }
        public void FriendGetGamesInCommon(string friendId, Action<GameListResult> callback)
        {
            Call("friend.games", UrlResolver.GetGamesInCommonWithFriend(friendId), (x) => { callback(x as GameListResult); });
        }
    }
}