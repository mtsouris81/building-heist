using UnityEngine;
using System.Collections;
using Weenus;
using Hamburglar;
using Hamburglar.Core;
using System.Collections.Generic;

public class HamburglarUiGamesList : MonoBehaviour {

    public RectTransform EmptyDataLine = null;
    public ScrollableItemList scrollList = null;
    WeenusUI View;

    public void Start()
    {
        View = new WeenusUI(this);
        scrollList.BindableList.OnItemClicked = (BindableListItem item) =>
        {
            // first check if game request is for game that's currently loaded
            if (HamburglarContext.Instance.BuildingData != null && HamburglarContext.Instance.BuildingData.Id.Equals(item.Value.ToString(), System.StringComparison.OrdinalIgnoreCase))
            {
                // send to play mode directly
                MobileUIManager.Current.Manager.StartPlayMode();
                return;
            }
            // start game loading workflow
            HamburglarContext.Instance.Service.StartGame(item.Value.ToString());
        };
        View.SetClickHandler("add", delegate()
        {
            MobileUIManager.Current.Manager.SwitchToScreen("create");
        });
    }

    public void GetGamesList()
    {
        string url = UrlResolver.GameList();
        HamburglarContext.Instance.Service.Call("gamelist", url, OnGamesLoaded);
    }

    public void OnGamesLoaded(object value)
    {
        GameListResult list = value as GameListResult;
        List<GameListItem> items = new List<GameListItem>();
        items.Add(new GameListItem()
        {
            i = "tutorial",
            t = "How To Play"
        });
        if (list != null)
        {
            items.AddRange(list.i);
        }

        scrollList.BindableList.BindList(items,
                                        x => x.t,
                                        x => x.i);

        EmptyDataLine.gameObject.SetActive(items.Count < 2);
        if (scrollList.BindableList.boundItems.Count > 0)
        {
            scrollList.BindableList.boundItems[0].SendMessage("IgnoreStyles", true, SendMessageOptions.RequireReceiver);
            scrollList.BindableList.boundItems[0].SetTextColor(Color.yellow, Color.yellow);
            Debug.Log("set color");
        }
    }

    public void UiActivated()
    {
        if (string.IsNullOrEmpty(HamburglarContext.Instance.PlayerId))
        {
            MobileUIManager.Current.Manager.SwitchToScreen("login");
            Debug.LogError("not signed in!");
            return;
        }
        GetGamesList();
    }

}
