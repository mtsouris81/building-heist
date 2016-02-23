using UnityEngine;
using System.Collections;
using System.IO;
using System.Xml.Serialization;
using System.Text;
using System.Collections.Generic;
using Weenus.OfflineData;
using System;


public class UnityXmlService<T>
{

    public static string[] OfflineUrlPrefix = new string[]{
        "api/story",
        "api/character"
    };

    public T Result { get; private set; }
    WWW www;
    bool hasResolved = false;
    bool errorState = false;
    string _lastError = string.Empty;
    WWWForm form;


    public Dictionary<string, string> Headers = new Dictionary<string, string>();
    public Dictionary<string, string> Params = new Dictionary<string, string>();
    public byte[] ParamsSer = null;

    public bool DeserializeFromXml = true;
    public bool InjectSecurityHeaders = true;
    public string DownloadedString = string.Empty;

    private object FormPostObject = null;
    private bool IsOfflineMode = false;
    private T OfflineResponseValue;

    private void SetSecurityHeaders()
    {

    }

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

    public void StartRequest(string url)
    {
        StartRequest(url, true);
    }

    public void StartRawRequest(string url)
    {
        this.DeserializeFromXml = false;
        this.InjectSecurityHeaders = false;
        StartRequest(url, false);
    }

    public bool IsOfflineRequest(string url, out string checkUrl)
    {
        checkUrl = url;
        string baseUrl = ServiceUrl.GetBaseUrl();

        if (checkUrl.StartsWith(baseUrl))
        {
            checkUrl = checkUrl.Replace(baseUrl, "").TrimStart('/');
        }

        foreach (var pre in OfflineUrlPrefix)
        {
            if (checkUrl.StartsWith(pre, System.StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }

    string offlineUrl = string.Empty;

    public void StartRequest(string url, bool setSecurityToken)
    {

        Result = default(T);
        hasResolved = false;
        errorState = false;
        _lastError = string.Empty;


        if (IsOfflineRequest(url, out offlineUrl))
        {
            IsOfflineMode = true;
            object o = RouteManager.ExecuteRouteMethod(offlineUrl, FormPostObject);
            OfflineResponseValue = (T)((ActionResult)o).Data;
            return;
        }
        else
        {
            if (setSecurityToken)
            {
                SetSecurityHeaders();
            }
            form = new WWWForm();
            foreach (var kv in Params)
            {
                form.AddField(kv.Key, kv.Value);
            }
            if (ParamsSer != null)
            {
                www = new WWW(url, ParamsSer, Headers);
            }
            else if (Params.Count > 0)
            {
                www = new WWW(url, form.data, Headers);
            }
            else
            {
                www = new WWW(url, null, Headers);
            }
        }



        Params.Clear();
    }

    public bool AttemptResolveReesponse()
    {
        if (hasResolved || errorState)
        {
            return false;
        }

        if (!IsOfflineMode && (www == null || !www.isDone))
        {
            return false;
        }



        if (!IsOfflineMode && !string.IsNullOrEmpty(www.error))
        {
            errorState = true;
            _lastError = www.error;

            return false;
        }

        hasResolved = true;


        if (IsOfflineMode)
        {
            Result = OfflineResponseValue;
            return true;
        }

        if (DeserializeFromXml)
        {
            using (StringReader reader = new StringReader(www.text))
            {
                XmlSerializer ser = new XmlSerializer(typeof(T));
                Result = (T)ser.Deserialize(reader);
            }
        }
        else
        {
            this.DownloadedString = www.text;
        }

        return true;
    }

    public static T Deserialize(string xml)
    {
        return Deserialize<T>(xml);
    }


    public static Tobj Deserialize<Tobj>(string xml)
    {
        using (StringReader reader = new StringReader(xml))
        {
            XmlSerializer ser = new XmlSerializer(typeof(Tobj));
            return (Tobj)ser.Deserialize(reader);
        }
    }
    public static string Serialize<Tobj>(Tobj data)
    {
        StringBuilder sb = new StringBuilder();
        using (StringWriter writer = new StringWriter(sb))
        {
            XmlSerializer ser = new XmlSerializer(typeof(Tobj));
            ser.Serialize(writer, data);
        }
        return sb.ToString();
    }
}

public interface IServiceResolver
{
    void StartRequest(string url);
    bool AttemptResolveResponse();
    string Name { get; }
    object Response { get; }
    bool IsActive { get; }
    GameObject WebLoadingDisplay { get; set; }
}

public static class WebServiceGlobals
{

    public static Action<string, string> GlobalErrorCallback { get; set; }
}
public class JsonWebServiceCall<T> : IServiceResolver
{
    public JsonWebServiceCall(string name)
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

    public bool DeserializeFromJson = true;
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
        this.DeserializeFromJson = false;
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

        if (DeserializeFromJson)
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

    public static T Deserialize(string xml)
    {
        return Deserialize<T>(xml);
    }
    public static Tobj Deserialize<Tobj>(string xml)
    {
        return Newtonsoft.Json.JsonConvert.DeserializeObject<Tobj>(xml);
    }
    public static string Serialize<Tobj>(Tobj data)
    {
        return Newtonsoft.Json.JsonConvert.SerializeObject(data);
    }
}

