using UnityEngine;
using System.Collections;
using Weenus;
using Hamburglar;
using Hamburglar.Core;

public class HamburglarUiCreateUser : MonoBehaviour {

    bool hasInitialized = false;
    WeenusUI View = null;

    void Start()
    {
        Init();
    }

    public void Init()
    {
        if (hasInitialized)
            return;

        hasInitialized = true;
        View = new WeenusUI(this);

        View.SetClickHandler("save", delegate()
        {
            string url = UrlResolver.CreateUser(
                                    View.GetInputText("username"),
                                    View.GetInputText("password"),
                                    View.GetInputInt("imageurl"));

            HamburglarContext.Instance.Service.Call("signup", url, OnPlayerCreated);
        });
    }

    private void OnPlayerCreated(object data)
    {
        var game = data as Player;
        if (game != null)
        {
            HamburglarContext.Instance.PlayerId = game.Id;
            MobileUIManager.Current.Manager.SwitchToScreen("games");
        }
    }

    public void UiActivated()
    {
        View.SetInputText("username", string.Empty);
        View.SetInputText("password", string.Empty);
        View.SetInputText("imageurl", string.Empty);
        Init();
    }

    public void UiDeactivated()
    {

    }
}
