using UnityEngine;
using Weenus;

public class HamburglarUiFriendRequests : MonoBehaviour
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
        View.SetClickHandler("add", delegate ()
        {
            MobileUIManager.Current.Manager.SwitchToScreen("searchplayers");
        });
    }
    public void GetFriendsList()
    {
        HamburglarContext.Instance.Service.FriendGet((response) =>
        {
            if (response != null)
            {
                if (response.s && response.i != null)
                {
                    scrollList.BindableList.BindList(response.i,
                                    x => x.t,
                                    x => x.i);

                    EmptyDataLine.gameObject.SetActive(response.i.Count < 1);
                }
            }
        });
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