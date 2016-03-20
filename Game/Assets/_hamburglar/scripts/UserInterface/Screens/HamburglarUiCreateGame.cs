using UnityEngine;
using System.Collections;
using Weenus;
using Hamburglar;
using Hamburglar.Core;
using System.Collections.Generic;
using System.Linq;

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
            string friendSelectScreenName = "FriendsSelect";
            var selectScreen = MobileUIManager.Current.Manager.GetComponentFromScreen<HamburglarUiFriendSelect>(friendSelectScreenName);
            selectScreen.OnSelectionsFinished = OnPlayersSelected;
            MobileUIManager.Current.Manager.SwitchToScreen(friendSelectScreenName);
        });
    }
    private void OnPlayersSelected(IEnumerable<GameListItem> players)
    {
        string playersString = string.Join(" ", players.Select(x => x.t).ToArray());
        Debug.Log(playersString);

        int floors = View.GetInputInt("floors");
        int rooms = View.GetInputInt("rooms");

        if (floors > Game.MAX_GAME_FLOORS)
            floors = Game.MAX_GAME_FLOORS;

        if (rooms > Game.MAX_GAME_ROOMS_PER_FLOOR)
            rooms = Game.MAX_GAME_ROOMS_PER_FLOOR;

        string url = UrlResolver.CreateGame(
                                View.GetInputText("title"),
                                playersString,
                                floors,
                                rooms);

        HamburglarContext.Instance.Service.Call("create", url, OnGameCreated);
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
