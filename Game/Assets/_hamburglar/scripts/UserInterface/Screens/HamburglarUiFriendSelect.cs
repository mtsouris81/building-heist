using Hamburglar.Core;
using System.Collections.Generic;
using UnityEngine;
using Weenus;
using System.Linq;
using System;


public class HamburglarUiFriendSelect : MonoBehaviour
{
    public RectTransform EmptyDataLine = null;
    public ScrollableItemList scrollList = null;

    WeenusUI View;
    List<GameListItem> SelectedPlayers = new List<GameListItem>();
    List<GameListItem> Friends = new List<GameListItem>();
    public Action<IEnumerable<GameListItem>> OnSelectionsFinished { get; set; }

    
    
    // pull normal friends, save locally

    // on search, 
    // 1. filter out any players that are already friends or in selected players
    // 2. bind search results first
    // 3. bind sorted friends + selected players


    public void CaptureCurrentSelectedPlayers()
    {
        SelectedPlayers = FriendListItem.GetSelected(scrollList.BindableList.transform).Select(x => 
            new GameListItem()
            {
                i = x.PlayerId,
                t = x.OriginalName
            }).ToList();
    }
    public bool IsFriendOrSelected(string id)
    {
        return Friends.Any(x => x.i.Equals(id, StringComparison.InvariantCultureIgnoreCase)) || SelectedPlayers.Any(x => x.i.Equals(id, StringComparison.InvariantCultureIgnoreCase));
    }
    public List<GameListItem> RemoveExistingFriendsOrSelected(List<GameListItem> players)
    {
        List<GameListItem> result = new List<GameListItem>();
        for (int i = 0; i < players.Count; i++)
        {
            if (!IsFriendOrSelected(players[i].i))
            {
                result.Add(players[i]);
            }
        }
        return result;
    }
    public bool WasSelected(string id)
    {
        return SelectedPlayers.Any(x => x.i.Equals(id, StringComparison.InvariantCultureIgnoreCase));
    }
    public IEnumerable<GameListItem> MergeSelectedAndFriends()
    {
        List<GameListItem> result = new List<GameListItem>();
        result.AddRange(Friends);
        foreach(var s in SelectedPlayers)
        {
            if (result.Any(x => x.i == s.i))
                continue;

            result.Add(s);
        }
        return result.OrderBy(x => x.t);
    }

    public void Start()
    {
        View = new WeenusUI(this);
        View.SetOnChangedHandler("search", PerformSearch);
        View.SetClickHandler("accept", () =>
        {
            if (OnSelectionsFinished != null)
            {
                CaptureCurrentSelectedPlayers();
                OnSelectionsFinished(SelectedPlayers);
            }
        });
    }


    private void PerformSearch(string searchPattern)
    {
        CaptureCurrentSelectedPlayers();

        if (string.IsNullOrEmpty(searchPattern) || searchPattern.Length < 2)
        {
            scrollList.BindableList.BindList<GameListItem, FriendListItem>(MergeSelectedAndFriends(),
                x => x.t,
                x => x.i,
                PostProcessBinding);
        }
        else
        {
            HamburglarContext.Instance.Service.SearchPlayers(searchPattern, (response) =>
            {
                if (response != null)
                {
                    if (response.s && response.i != null)
                    {
                        var searchList = RemoveExistingFriendsOrSelected(response.i);
                        if (searchList.Count > 0)
                        {

                            scrollList.BindableList.BindList<GameListItem, FriendListItem>(searchList,
                                            x => x.t,
                                            x => x.i,
                                            PostProcessBinding);

                            scrollList.BindableList.BindList<GameListItem, FriendListItem>(MergeSelectedAndFriends(),
                                            x => x.t,
                                            x => x.i,
                                            PostProcessBinding,
                                            true);

                            EmptyDataLine.gameObject.SetActive(scrollList.BindableList.boundItems.Count < 1);
                        }
                    }
                }
            });
        }
    }

    public void GetFriendsList()
    {
        Friends = new List<GameListItem>();
        HamburglarContext.Instance.Service.FriendGet((response) =>
        {
            if (response != null && response.s)
            {
                List<GameListItem> list = new List<GameListItem>();
                if (response.i != null) // bind actual friends
                {
                    list.AddRange(response.i);
                }
                if (response.fr != null && response.fr.Count > 0) // bind friend requests
                {
                    list.AddRange(response.fr);
                }
                if (response.pf != null && response.pf.Count > 0) // bind friend pending
                {
                    list.AddRange(response.pf);
                }
                EmptyDataLine.gameObject.SetActive(list.Count < 1);
                list = list.OrderBy(i => i.t).ToList();
                Friends = list;
                scrollList.BindableList.BindList<GameListItem, FriendListItem>(list,
                    x => x.t,
                    x => x.i,
                    PostProcessBinding,
                    false);
            }
        });
    }
    public void PostProcessBinding(FriendListItem item, string display, object value)
    {
        string id = value.ToString();
        item.SetFriendData(FriendListItem.FriendType.Nothing, id, null);
        item.SetSelectable(WasSelected(id));
    }
    public void UiActivated()
    {
        if (string.IsNullOrEmpty(HamburglarContext.Instance.PlayerId))
        {
            MobileUIManager.Current.Manager.SwitchToScreen("login");
            return;
        }
        GetFriendsList();
    }
}