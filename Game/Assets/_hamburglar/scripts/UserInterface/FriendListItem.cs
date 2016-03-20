using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using System.Linq;
using System.Collections.Generic;
using Hamburglar.Core;

public class FriendListItem : MonoBehaviour
{
    public Button Accept = null;
    public Button Reject = null;
    public Button Request = null;
    public Button Delete = null;
    public Toggle Select = null;
    public Image Background = null;
    public Color YouColor = Color.clear;
    public Color FriendRequestColor = Color.gray;
    public Color FriendPendingColor = Color.blue;

    public string PlayerId { get; private set; }
    public string OriginalName { get; private set; }
    public Action RebindCallback { get; private set; }
    public FriendType Type { get; private set; }
    public bool IsSelected { get; private set; }
    public void Start()
    {
        Accept.onClick.AddListener(AcceptFriend);
        Reject.onClick.AddListener(RejectFriend);
        Request.onClick.AddListener(RequestFriend);
        Delete.onClick.AddListener(DeleteFriend);
        Select.onValueChanged.AddListener((isChecked) =>
        {
            Debug.Log(OriginalName);
            IsSelected = isChecked;
        });
    }
    public void SetFriendData(FriendType t, string playerId, Action rebindCallback)
    {
        RebindCallback = rebindCallback;
        PlayerId = playerId;
        Type = t;
        HideAll();
        Background.color = Color.clear;
        switch (this.Type)
        {
            case FriendType.Friend:
                Show(Delete);
                break;
            case FriendType.Removed:
                Background.color = Color.Lerp(Color.red, Color.black, 0.7f);
                break;
            case FriendType.FriendRequest:
                Background.color = FriendRequestColor;
                Show(Accept);
                Show(Reject);
                break;
            case FriendType.Pending:
                Background.color = FriendPendingColor;
                Show(Delete);
                break;
            case FriendType.You:
                Background.color = YouColor;
                break;
            case FriendType.Nothing:
                Show(Request);
                break;
            default:
                break;
        }

        var bindableItem = this.GetComponent<BindableListItem>();

        if(bindableItem != null)
        {
            if (string.IsNullOrEmpty(OriginalName))
                OriginalName = bindableItem.ItemText;

            bindableItem.SetData(GetTypedPlayerName(Type, OriginalName), playerId);
        }
    }

    public static List<FriendListItem> GetSelected(Transform container)
    {
        var allFriends = container.GetComponentsInChildren<FriendListItem>();
        var result = new List<FriendListItem>();
        foreach(var i in allFriends)
        {
            Debug.Log(i.OriginalName);
            if (i.IsSelected)
            {
                result.Add(i);
            }
        }
        Debug.Log(string.Join(" ", result.Select(x => x.OriginalName).ToArray()));
        return result;
    }
    public void SetSelectable(bool _isSelected)
    {
        Select.isOn = _isSelected;
        this.IsSelected = _isSelected;
        HideAll();
        Select.gameObject.SetActive(true);
    }
    private void AcceptFriend()
    {
        HamburglarContext.Instance.Service.FriendAccept(PlayerId, (response) =>
        {
            if (response != null && response.s)
            {
                HamburglarContext.Instance.SetFloatingMessage(response.m, Color.white);
                var _pendingFriends = MobileUIManager.Current.Manager.GetViewData<List<GameListItem>>("friend.requests");
                var found = _pendingFriends.Where(x => x.i == PlayerId).FirstOrDefault();
                if (found != null)
                {
                    _pendingFriends.Remove(found);
                }
                var _friends = MobileUIManager.Current.Manager.GetViewData<List<GameListItem>>("friend.friends");
                _friends.Add(found);
                RebindCallback();
            }
        });
    }
    private void RejectFriend()
    {
        HamburglarContext.Instance.Service.FriendReject(PlayerId, (response) =>
        {
            if (response != null && response.s)
            {
                var _pendingFriends = MobileUIManager.Current.Manager.GetViewData<List<GameListItem>>("friend.requests");
                var found = _pendingFriends.Where(x => x.i == PlayerId).FirstOrDefault();
                if (found != null)
                {
                    _pendingFriends.Remove(found);
                }
                RebindCallback();
            }
        });
    }
    private void RequestFriend()
    {
        HamburglarContext.Instance.Service.FriendRequest(PlayerId, (response) =>
        {
            if (response != null && response.s)
            {
                HamburglarContext.Instance.SetFloatingMessage("Friend request sent!", Color.white);
                var _pendingFriends = MobileUIManager.Current.Manager.GetViewData<List<GameListItem>>("friend.pending");
                _pendingFriends.Add(new GameListItem()
                {
                    i = PlayerId,
                    t = OriginalName
                });
                RebindCallback();
            }
        });
    }
    private void DeleteFriend()
    {
        if (this.Type == FriendType.Friend)
        {
            HamburglarContext.Instance.Service.FriendDelete(PlayerId, (response) =>
            {
                if (response != null && response.s)
                {
                    HamburglarContext.Instance.SetFloatingMessage("Friend deleted", Color.red);
                    SetFriendData(FriendType.Removed, PlayerId, RebindCallback);
                    RebindCallback();
                }
            });
        }
        else if (this.Type == FriendType.Pending)
        {
            HamburglarContext.Instance.Service.FriendReject(HamburglarContext.Instance.PlayerId, PlayerId, (response) =>
            {
                if (response != null && response.s)
                {
                    HamburglarContext.Instance.SetFloatingMessage("Friend Request Canceled", Color.red);
                    var _pendingFriends = MobileUIManager.Current.Manager.GetViewData<List<GameListItem>>("friend.pending");
                    var found = _pendingFriends.Where(x => x.i == PlayerId).FirstOrDefault();
                    if (found != null)
                    {
                        _pendingFriends.Remove(found);
                    }
                    SetFriendData(FriendType.Nothing, PlayerId, RebindCallback);
                    RebindCallback();
                }
            });
        }
    }
    private void HideAll()
    {
        Accept .gameObject.SetActive(false);
        Reject .gameObject.SetActive(false);
        Request.gameObject.SetActive(false);
        Delete .gameObject.SetActive(false);
        Select .gameObject.SetActive(false);
    }
    private void Show(Button b)
    {
        b.gameObject.SetActive(true);
    }

    public enum FriendType
    {
        Nothing = 0,
        Friend,
        FriendRequest,
        Pending,
        You,
        Removed
    }

    public static string GetTypedPlayerName(FriendType t, string name)
    {
        string suffix = string.Empty;

        switch (t)
        {
            case FriendListItem.FriendType.Friend:
                suffix = "(FRIEND)";
                break;
            case FriendListItem.FriendType.FriendRequest:
                suffix = "(waiting your approval)";
                break;
            case FriendListItem.FriendType.Pending:
                suffix = "(request sent)";
                break;
            case FriendListItem.FriendType.You:
                suffix = "(YOU)";
                break;
            default:
                break;
        }
        return string.Format("{0}   {1}", name, suffix);
    }
}
 