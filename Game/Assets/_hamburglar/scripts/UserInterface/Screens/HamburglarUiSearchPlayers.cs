using Hamburglar.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Weenus;

public class HamburglarUiSearchPlayers : MonoBehaviour
{
    public RectTransform EmptyDataLine = null;
    public ScrollableItemList scrollList = null;
    WeenusUI View;

    public void Start()
    {
        View = new WeenusUI(this);
        scrollList.BindableList.OnItemClicked = (BindableListItem item) =>
        {
            Debug.Log(item.ItemText);
        };
        View.SetOnChangedHandler("search", PerformSearch);
    }

    private void Rebind()
    {
        PerformSearch(View.GetInputText("search"));
    }

    private void PerformSearch(string searchPattern)
    {
        if (string.IsNullOrEmpty(searchPattern) || searchPattern.Length < 2)
        {
            ClearSearch();
        }
        else
        {
            HamburglarContext.Instance.Service.SearchPlayers(searchPattern, (response) =>
            {
                if (response != null)
                {
                    if (response.s && response.i != null)
                    {
                        scrollList.BindableList.BindList<GameListItem, FriendListItem>(response.i,
                                        x => x.t,
                                        x => x.i,
                                        PostProcessBinding);

                        EmptyDataLine.gameObject.SetActive(response.i.Count < 1);
                    }
                }
            });
        }
    }




    private void ClearSearch()
    {
        scrollList.BindableList.ClearBindings();
        EmptyDataLine.gameObject.SetActive(false);
    }


    public void PostProcessBinding(FriendListItem item, string display, object value)
    {
        string id = value.ToString();
        var friendType = HamburglarUiFriends.GetFriendshipType(this.View, id);
        item.SetFriendData(friendType, id, Rebind);
    }

    public void UiActivated()
    {
        if (string.IsNullOrEmpty(HamburglarContext.Instance.PlayerId))
        {
            MobileUIManager.Current.Manager.SwitchToScreen("login");
            return;
        }
        View.SetInputText("search", "");
    }
}