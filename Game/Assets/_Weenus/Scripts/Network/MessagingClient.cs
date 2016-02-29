using System;
using System.Collections.Generic;
using SignalR.Client._20.Hubs;

namespace Weenus.Network
{
    public class MessagingClient
    {
        public Action OnConnectedCallback { get; set; }
        public object PendingUpdateData { get; protected set; }
        public Queue<ICaughtMessagePackage> CaughtMessages = new Queue<ICaughtMessagePackage>();

        protected HubConnection Connection;
        protected IHubProxy Proxy;

        public void Connect(string url, string hubName)
        {
            this.Disconnect();
            var query = new Dictionary<string, object>();
            RegisterMessageQueryParams(query);
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
                CaughtMessages.Enqueue(new DefaultCaughtMessagePackage()
                {
                    IsImmediateResponse = true,
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
                CaughtMessages.Enqueue(new DefaultCaughtMessagePackage()
                {
                    Callback = () => { finished(e.Result); },
                    IsImmediateResponse = true
                });
            };
        }
        protected virtual void RegisterMessageQueryParams(Dictionary<string, object> queryParams) { }
        protected virtual void SubscribeToMessages(IHubProxy proxy) { }
        public virtual void SetRequestParams()
        {
            Connection.UpdateQueryString();
        }
    }
    public interface ICaughtMessagePackage
    {
        bool IsImmediateResponse { get; set; }
        object Response { get; set; }
        Action Callback { get; set; }
    }
    public class DefaultCaughtMessagePackage : ICaughtMessagePackage
    {
        public bool IsImmediateResponse { get; set; }
        public object Response { get; set; }
        public Action Callback { get; set; }
    }
}