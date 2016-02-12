using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using SignalR.Client._20.Hubs;
using Newtonsoft.Json.Linq;
using Hamburglar.Core;


namespace Hamburglar
{
    public class MessagingClient
    {
        protected HubConnection Connection;
        protected IHubProxy Proxy;

        public void Connect(string url, string hubName)
        {
            this.Disconnect();
            var query = new Dictionary<string, object>();
            SetMessageQueryParams(query);
            Connection = new HubConnection(url, query);
            Proxy = Connection.CreateProxy(hubName);
            Connection.Start();
            Connection.ConnectionReady += OnConnectionReady;
        }
        public void Disconnect()
        {
            if (this.Connection != null && this.Connection.IsActive)
            {
                this.Connection.Stop();
                this.Connection = null;
                this.Proxy = null;
            }
        }
        protected virtual void OnConnectionReady(object sender, EventArgs e)
        {
            UnityEngine.Debug.Log("Connected!");
            SubscribeToMessages(Proxy);

            if (OnConnectedCallback != null)
            {
                CaughtMessages.Enqueue(new CaughtMessage()
                {
                    Type = IncomingMessageType.ImmediateResponse,
                    Callback = OnConnectedCallback
                });
            }
        }
        public void SendMessage(string name, params object[] args)
        {
            SetRequestParams();
            Proxy.Invoke(name, args);
        }
        public void SendMessage<T>(string name, Action<T> finished, params object[] args)
        {
            SetRequestParams();
            var signal = Proxy.Invoke<T>(name, args);
            signal.Finished += (s, e) =>
            {
                CaughtMessages.Enqueue(new CaughtMessage()
                {
                    Callback = () => { finished(e.Result); },
                    Type = IncomingMessageType.ImmediateResponse
                });
            };
        }
        protected virtual void SetMessageQueryParams(Dictionary<string, object> queryParams) { }
        protected virtual void SubscribeToMessages(IHubProxy proxy) { }
        public virtual void SetRequestParams()
        {
            Connection.UpdateQueryString();
        }




        public Action OnConnectedCallback { get; set; }



        public Queue<CaughtMessage> CaughtMessages = new Queue<CaughtMessage>();
        public object PendingUpdateData { get; protected set; }
        public class CaughtMessage
        {
            public IncomingMessageType Type { get; set; }
            public string PlayerId { get; set; }
            public string OpponentId { get; set; }
            public int? Room { get; set; }
            public int? Floor { get; set; }
            public object Response { get; set; }
            public Action Callback { get; set; }
        }
        public enum IncomingMessageType
        {
            RoomEntered,
            RoomExited,
            GameUpdate,
            LootResult,
            OpponentTrapped ,
            OpponentCaught,
            ImmediateResponse
        }
    }
 
   
    public class HamburglarMessagingClient : MessagingClient
    {
        public Action<object> GameUpdateCallback { get; private set; }
        public string PlayerId { get; private set; }
        public void Connect(string playerId,  Action<object> gameUpdateCallback) 
        {
            PlayerId = playerId;
            GameUpdateCallback = gameUpdateCallback;
            base.Connect(UrlResolver.Host, UrlResolver.RealTimeHub);
        }
        public override void SetRequestParams()
        {
            Connection.QueryStringParams["p"] = HamburglarContext.Instance.BuildingData.PlayerVersions[PlayerId];
            Connection.QueryStringParams["f"] = HamburglarContext.Instance.BuildingData.FloorVersions[HamburglarContext.Instance.Floor];
            Connection.QueryStringParams["g"] = HamburglarContext.Instance.BuildingData.GameMetaVersion;
            base.SetRequestParams();
        }
        protected override void SetMessageQueryParams(Dictionary<string, object> queryParams)
        {
            queryParams.Add("pp", PlayerId);
            queryParams.Add("p", 0);
            queryParams.Add("g", 0);
            queryParams.Add("f", 0);
        }
        protected override void SubscribeToMessages(IHubProxy proxy)
        {
            proxy.Subscribe("RoomEntered").Data += RoomChange;
            proxy.Subscribe("RoomExited").Data += RoomChange;
            proxy.Subscribe("OnPlayerTrapped").Data += (object[] args) =>
            {
                CaughtMessages.Enqueue(new CaughtMessage()
                {
                    Type = IncomingMessageType.OpponentTrapped,
                    PlayerId = args[0] as string,
                    OpponentId = args[1] as string
                });
            };
            proxy.Subscribe("OpponentCaught").Data += (object[] args) =>
            {
                CaughtMessages.Enqueue(new CaughtMessage()
                {
                    Type = IncomingMessageType.OpponentCaught,
                    PlayerId = args[0] as string,
                    OpponentId = args[1] as string,
                });
            };
            proxy.Subscribe("NeedUpdate").Data += (object[] args) =>
            {
                UnityEngine.Debug.Log("update!");
                NeedsUpdate();
            };

            proxy.Subscribe("GameFinished").Data += (object[] args) =>
            {
                UnityEngine.Debug.Log("GAME FINISHED!");
                CaughtMessages.Enqueue(new CaughtMessage()
                {
                    Type = IncomingMessageType.OpponentCaught,
                    PlayerId = args[0] as string
                });
            };
            
        }
        protected override void OnConnectionReady(object sender, EventArgs e)
        {
            base.OnConnectionReady(sender, e);
            JoinGame();
        }
        public void JoinGame()
        {
            SendMessage("JoinGame", HamburglarContext.Instance.BuildingData.Id);
        }



        void RoomChange(object[] args)
        {
            HandleRoomChange((int)args[0], (int?)args[1], (string)args[2]);
        }
        void NeedsUpdate()
        {
            UnityEngine.Debug.Log("NeedsUpdate");
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
                CaughtMessages.Enqueue(new CaughtMessage()
                {
                    Type = IncomingMessageType.GameUpdate,
                    Response = e.Result
                });
            };
        }
        void HandleRoomChange(int floor, int? room, string playerId)
        {
            UnityEngine.Debug.Log("local floor room change!");
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
                    CaughtMessages.Enqueue(new CaughtMessage()
                    {
                        Type = IncomingMessageType.GameUpdate
                    });
                };
            }
        }

    }
}