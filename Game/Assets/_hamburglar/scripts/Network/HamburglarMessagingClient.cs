using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using SignalR.Client._20.Hubs;
using Newtonsoft.Json.Linq;
using Hamburglar.Core;
using Weenus.Network;

namespace Hamburglar
{
    public class HamburglarMessagingClient : MessagingClient
    {
        public Action<object> GameUpdateCallback { get; private set; }
        public string PlayerId { get; private set; }

        // game specific
        public void ConnectToHeistGame(string playerId,  Action<object> gameUpdateCallback) 
        {
            PlayerId = playerId;
            GameUpdateCallback = gameUpdateCallback;
            this.OnConnectedCallback = () =>
            {
                HamburglarContext.Instance.Service.WebLoadingDisplay.gameObject.SetActive(false);
                HamburglarContext.Instance.SetFloatingMessage("Connected to game server");
            };
            base.Connect(UrlResolver.RealTimeURL, UrlResolver.RealTimeHub);
        }
        public void JoinGame()
        {
            SendMessage("JoinGame", HamburglarContext.Instance.BuildingData.Id, HamburglarContext.Instance.PlayerId, DateTime.UtcNow);
        }
        public void RoomChange(object[] args)
        {
            HandleRoomChange((int)args[0], (int?)args[1], (string)args[2]);
        }
        public void NeedsUpdate()
        {
            var versions = UrlResolver.GetVersions();
            var signal = Proxy.Invoke<WebGameTransport>(
                                        "GameUpdate",
                                        versions.Player,
                                        versions.Floor,
                                        versions.Game,
                                        HamburglarContext.Instance.BuildingData.Id,
                                        HamburglarContext.Instance.PlayerId,
                                        HamburglarContext.Instance.Floor);
            signal.Finished += (s, e) =>
            {
                PendingUpdateData = e.Result;
                CaughtMessages.Enqueue(new HeistCaughtMessage()
                {
                    Tag = IncomingMessageType.GameUpdate,
                    Response = e.Result
                });
            };
        }
        public void HandleRoomChange(int floor, int? room, string playerId)
        {
            if (floor == HamburglarContext.Instance.Floor)
            {
                var versions = UrlResolver.GetVersions();
                var signal = Proxy.Invoke<WebGameTransport>(
                                            "GameUpdate",
                                            versions.Player,
                                            versions.Floor,
                                            versions.Game,
                                            HamburglarContext.Instance.BuildingData.Id,
                                            HamburglarContext.Instance.PlayerId,
                                            HamburglarContext.Instance.Floor);
                signal.Finished += (s, e) =>
                {
                    PendingUpdateData = e.Result;
                    CaughtMessages.Enqueue(new HeistCaughtMessage()
                    {
                        Tag = IncomingMessageType.GameUpdate
                    });
                };
            }
        }

        // base overrides
        public override void SetRequestParams()
        {
            Connection.QueryStringParams["p"] = HamburglarContext.Instance.BuildingData.PlayerVersions[PlayerId];
            Connection.QueryStringParams["f"] = HamburglarContext.Instance.BuildingData.FloorVersions[HamburglarContext.Instance.Floor];
            Connection.QueryStringParams["g"] = HamburglarContext.Instance.BuildingData.GameMetaVersion;
            base.SetRequestParams();
        }
        protected override void RegisterMessageQueryParams(Dictionary<string, object> queryParams)
        {
            // player id
            queryParams.Add("pp", PlayerId);
            // game player meta version
            queryParams.Add("p", 0);
            // game meta version
            queryParams.Add("g", 0);
            // game floor version
            queryParams.Add("f", 0);
        }
        protected override void SubscribeToMessages(IHubProxy proxy)
        {
            proxy.Subscribe("RoomEntered").Data += RoomChange;
            proxy.Subscribe("RoomExited").Data += RoomChange;
            proxy.Subscribe("OnPlayerTrapped").Data += (object[] args) =>
            {
                CaughtMessages.Enqueue(new HeistCaughtMessage()
                {
                    Tag = IncomingMessageType.OpponentTrapped,
                    PlayerId = args[0] as string,
                    OpponentId = args[1] as string
                });
            };
            proxy.Subscribe("OpponentCaught").Data += (object[] args) =>
            {
                CaughtMessages.Enqueue(new HeistCaughtMessage()
                {
                    Tag = IncomingMessageType.OpponentCaught,
                    PlayerId = args[0] as string,
                    OpponentId = args[1] as string,
                });
            };
            proxy.Subscribe("NeedUpdate").Data += (object[] args) =>
            {
                NeedsUpdate();
            };
            proxy.Subscribe("GameFinished").Data += (object[] args) =>
            {
                UnityEngine.Debug.Log("GAME FINISHED!");
                CaughtMessages.Enqueue(new HeistCaughtMessage()
                {
                    Tag = IncomingMessageType.OpponentCaught,
                    PlayerId = args[0] as string
                });
            };
            proxy.Subscribe("GameReady").Data += (object[] args) =>
            {
                UnityEngine.Debug.Log("EVERYONE JOINED!");
                CaughtMessages.Enqueue(new HeistCaughtMessage()
                {
                    Tag = IncomingMessageType.GameReady,
                    StartTime = ((DateTime)args[0])
                });
            };
        }
        protected override void OnConnectionReady(object sender, EventArgs e)
        {
            base.OnConnectionReady(sender, e);
            JoinGame();
        }
    }
    public class HeistCaughtMessage : DefaultCaughtMessagePackage
    {
        public IncomingMessageType Tag { get; set; }
        public string PlayerId { get; set; }
        public string OpponentId { get; set; }
        public int? Room { get; set; }
        public int? Floor { get; set; }
        public DateTime? StartTime { get; set; }
    }
    public enum IncomingMessageType
    {
        RoomEntered,
        RoomExited,
        GameUpdate,
        LootResult,
        OpponentTrapped,
        OpponentCaught,
        ImmediateResponse,
        GameReady
    }
}