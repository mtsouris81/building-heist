using Hamburglar.Core;
using System.Collections.Generic;
using UnityEngine;
using Weenus;
using System.Linq;
using System;

public class HamburglarUiFriends : MonoBehaviour
{
    public RectTransform EmptyDataLine = null;
    public RectTransform FriendRequestsLine = null;
    public ScrollableItemList scrollList = null;
    WeenusUI View;

    public void Start()
    {
        View = new WeenusUI(this);
        scrollList.BindableList.OnItemClicked = (BindableListItem item) =>
        {
            
        };
        View.SetClickHandler("add", delegate()
        {
            MobileUIManager.Current.Manager.SwitchToScreen("searchplayers");
        });
    }
    public void GetFriendsList()
    {
        // clear view data
        View.UI.SetViewData("friend.friends", new List<GameListItem>());
        View.UI.SetViewData("friend.requests", new List<GameListItem>());
        View.UI.SetViewData("friend.pending", new List<GameListItem>());

        HamburglarContext.Instance.Service.FriendGet((response) =>
        {
            if (response != null && response.s)
            {
                List<GameListItem> list = new List<GameListItem>();
                if (response.i != null) // bind actual friends
                {
                    View.UI.SetViewData("friend.friends", response.i);
                    list.AddRange(response.i);
                }
                if (response.fr != null && response.fr.Count > 0) // bind friend requests
                {
                    View.UI.SetViewData("friend.requests", response.fr);
                    list.AddRange(response.fr);
                }
                if (response.pf != null && response.pf.Count > 0) // bind friend pending
                {
                    View.UI.SetViewData("friend.pending", response.pf);
                    list.AddRange(response.pf);
                }

                EmptyDataLine.gameObject.SetActive(list.Count < 1);
                list = list.OrderBy(i => i.t).ToList();
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
        var friendType = HamburglarUiFriends.GetFriendshipType(this.View, id);
        item.SetFriendData(friendType, id, GetFriendsList);
    }
    public static FriendListItem.FriendType GetFriendshipType(WeenusUI View, string playerId)
    {
        if (playerId.Equals(HamburglarContext.Instance.PlayerId, StringComparison.OrdinalIgnoreCase))
            return FriendListItem.FriendType.You;

        var _pendingFriends = View.UI.GetViewData<List<GameListItem>>("friend.pending");
        var _friendRequests = View.UI.GetViewData<List<GameListItem>>("friend.requests");
        var _friends = View.UI.GetViewData<List<GameListItem>>("friend.friends");

        if (_friendRequests != null && _friendRequests.Any(x => x.i.Equals(playerId, StringComparison.InvariantCultureIgnoreCase)))
            return FriendListItem.FriendType.FriendRequest;

        if (_friends != null && _friends.Any(x => x.i.Equals(playerId, StringComparison.InvariantCultureIgnoreCase)))
            return FriendListItem.FriendType.Friend;

        if (_pendingFriends != null && _pendingFriends.Any(x => x.i.Equals(playerId, StringComparison.InvariantCultureIgnoreCase)))
            return FriendListItem.FriendType.Pending;

        return FriendListItem.FriendType.Nothing;
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