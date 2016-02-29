using UnityEngine;
using System.Collections;
using Weenus;
using Hamburglar;
using Hamburglar.Core;

public class HamburglarUiCreateGame : MonoBehaviour {

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
            int floors = View.GetInputInt("floors");
            int rooms = View.GetInputInt("rooms");

            if (floors > GameDataManager.MAX_GAME_FLOORS)
                floors = GameDataManager.MAX_GAME_FLOORS;

            if (rooms > GameDataManager.MAX_GAME_ROOMS_PER_FLOOR)
                rooms = GameDataManager.MAX_GAME_ROOMS_PER_FLOOR;

            string url = UrlResolver.CreateGame(
                                    View.GetInputText("title"),
                                    View.GetInputText("players"),
                                    floors,
                                    rooms);

            HamburglarContext.Instance.Service.Call("create", url, OnGameCreated);
        });

    }

    private void OnGameCreated(object data)
    {
        var game = data as WebGameTransport;
        if (game != null)
        {
            MobileUIManager.Current.Manager.SwitchToScreen("games");
        }
    }

    public void UiActivated()
    {
        View.SetInputText("title", "my new game");
        View.SetInputText("players", string.Empty);
        View.SetInputText("floors", "12");
        View.SetInputText("rooms", "5");
        Init();
    }

    public void UiDeactivated()
    {

    }
}
