using UnityEngine;
using System.Collections;
using Weenus;
using Hamburglar;

public class HamburglarUiLogin : MonoBehaviour {

    public System.Action<string> LogInCallback { get; set; }
    WeenusUI View;

    void ClearForm()
    {
        View.SetInputText("username", string.Empty);
        View.SetInputText("password", string.Empty);
    }
    void Start()
    {
        View = new WeenusUI(this);
        View.SetClickHandler("login", delegate()
        {
            string username = View.GetInputText("username").Trim();
            string password = View.GetInputText("password").Trim();
            if (string.IsNullOrEmpty(username))
            {
                return; // no username!
            }
            string url = UrlResolver.Login(username, password);
            HamburglarContext.Instance.Service.Call("login", url, OnLoggedIn);
        });
        View.SetClickHandler("create", delegate()
        {
            MobileUIManager.Current.Manager.SwitchToScreen("SignUp");
        });
        if (HamburglarContext.Instance.DebugUser)
        {
            OnLoggedIn(HamburglarContext.Instance.DegugUserID);
        }
    }
    private void OnLoggedIn(object value)
    {
        string playerId = value.ToString();
        if (LogInCallback != null)
        {
            LogInCallback(playerId);
        }
        HamburglarContext.Instance.PlayerId = playerId;
        MobileUIManager.Current.Manager.SwitchToScreen("Games");
    }
}
