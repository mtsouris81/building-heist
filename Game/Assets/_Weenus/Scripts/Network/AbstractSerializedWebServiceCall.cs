using UnityEngine;
using System.Text;
using System.Collections.Generic;
using System;

namespace Weenus.Network
{
    public abstract class AbstractSerializedWebServiceCall<T> : IServiceResolver
    {
        public AbstractSerializedWebServiceCall(string name)
        {
            Name = name;
            UseLoadingPrompt = true;
        }
        public bool UseLoadingPrompt { get; set; }
        public string Name { get; private set; }
        public bool IsActive { get; private set; }
        public static string LoadingDisplayGameObjectName = "_webLoading";
        public T Result { get; private set; }
        public object Response { get { return this.Result; } }
        WWW www;
        bool hasResolved = false;
        bool errorState = false;
        string _lastError = string.Empty;
        WWWForm form;
        public Dictionary<string, string> Headers = new Dictionary<string, string>();
        public Dictionary<string, string> Params = new Dictionary<string, string>();
        public byte[] ParamsSer = null;
        public bool DeserializeResponse = true;
        public bool InjectSecurityHeaders = true;
        public string DownloadedString = string.Empty;
        private object FormPostObject = null;
        public GameObject WebLoadingDisplay { get; set; }
        public Action<string, string> OnErrorCallback { get; set; }
        public void SetHeader(string name, string value)
        {
            if (Headers.ContainsKey(name))
            {
                Headers[name] = value;
            }
            else
            {
                Headers.Add(name, value);
            }
        }
        public void SetFormParam(string name, string value)
        {
            Params.Add(name, value);
        }
        public void SetForm<TInput>(TInput value)
        {
            SetHeader("Content-Type", "application/json");
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(value);
            ParamsSer = Encoding.UTF8.GetBytes(json);
            FormPostObject = value;
        }
        public void StartRawRequest(string url)
        {
            this.DeserializeResponse = false;
            this.InjectSecurityHeaders = false;
            StartRequest(url);
        }
        public void StartRequest(string url)
        {
            if (UseLoadingPrompt && this.WebLoadingDisplay != null)
            {
                this.WebLoadingDisplay.SetActive(true);
            }
            IsActive = true;
            Result = default(T);
            hasResolved = false;
            errorState = false;
            _lastError = string.Empty;
            if (ParamsSer != null)
            {
                www = new WWW(url, ParamsSer, Headers);
            }
            else if (Params.Count > 0)
            {
                form = new WWWForm();
                foreach (var kv in Params)
                {
                    form.AddField(kv.Key, kv.Value);
                }
                www = new WWW(url, form.data, Headers);
            }
            else
            {
                www = new WWW(url, null, Headers);
            }
            Params.Clear();
        }
        public bool AttemptResolveResponse()
        {
            if (hasResolved || errorState)
            {
                return false;
            }
            if ((www == null || !www.isDone))
            {
                return false;
            }
            if (!string.IsNullOrEmpty(www.error))
            {
                errorState = true;
                _lastError = www.error;
                if (OnErrorCallback != null)
                {
                    OnErrorCallback(www.error, www.text);
                }
                if (WebServiceGlobals.GlobalErrorCallback != null)
                {
                    WebServiceGlobals.GlobalErrorCallback(www.error, www.text);
                }
                if (UseLoadingPrompt && this.WebLoadingDisplay != null)
                {
                    this.WebLoadingDisplay.SetActive(false);
                }
                hasResolved = true;
                IsActive = false;
                return false;
            }
            if (DeserializeResponse)
            {
                Result = Deserialize<T>(www.text);
            }
            else
            {
                this.DownloadedString = www.text;
            }
            hasResolved = true;
            IsActive = false;
            if (UseLoadingPrompt && this.WebLoadingDisplay != null)
            {
                this.WebLoadingDisplay.SetActive(false);
            }
            return true;
        }
        public T Deserialize(string xml)
        {
            return Deserialize<T>(xml);
        }
        public abstract Tobj Deserialize<Tobj>(string xml);
        public abstract string Serialize<Tobj>(Tobj data);
    }
}
