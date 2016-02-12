using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;
using Tsouris.StoryBuilder.Data.StoryParts;

public class StoryPartItem : MonoBehaviour , IPointerDownHandler, IPointerExitHandler
{

    SwipeHelper swipe;
    UnityXmlService<bool> DeleteRequest = new UnityXmlService<bool>();
    UnityXmlService<bool> MoveRequest = new UnityXmlService<bool>();

    MobileTouchManager tap = new MobileTouchManager();

    public Text DisplayText = null;
    public RectTransform Menu = null;
    public Button MenuButton = null;
    public Button MenuBackButton = null;

    public Button DeleteButton = null;
    public Button MoveUpButton = null;
    public Button MoveDownButton = null;

    public long StoryPartID { get; set; }
    public string PartTypeSystemName { get; set; }
    public Action<StoryPartItem> Clicked { get; set; }
    public object Value { get; set; }
    public StoryExportPart Data { get; private set; }

    bool isMenuOpen = false;
    public void SetItem(StoryExportPart data, bool isFirst, bool isLast)
    {
        Data = data;
        StoryPartID = data.StoryPartID;
        PartTypeSystemName = data.Type;

        string prefix = "then";
        
        if (isFirst)
        {
            prefix = "First";
        }
        if (isLast)
        {
            prefix = "and finally,";
        }

        DisplayText.text = string.Format("{0} {1} ... ", prefix, data.PartData.GetDisplayString());
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        swipe.Down(eventData);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        swipe.Exit();
    }

    //public void OnPointerUp(PointerEventData eventData)
    //{
    //    tap.Up(eventData);
    //    if (tap.IsTap())
    //    {
    //        if (Clicked != null)
    //        {
    //            Clicked(this);
    //        }
    //    }
    //}

    private void DeletePart()
    {
        DeleteRequest.StartRequest(ServiceUrl.Get(Urls.Builder.DeletePart, this.Data.StoryPartID));
    }
    private void MoveUp()
    {
        MoveRequest.StartRequest(ServiceUrl.Get(Urls.Builder.MovePart, this.Data.StoryPartID, "up"));
    }
    private void MoveDown()
    {
        MoveRequest.StartRequest(ServiceUrl.Get(Urls.Builder.MovePart, this.Data.StoryPartID, "down"));
    }

	void Start () {

        swipe = new SwipeHelper();

        swipe.OnSwipe = delegate(SwipeDirection direction)
        {
            switch (direction)
            {
                case SwipeDirection.Left:
                    SetContextMenu(false);
                    break;
                case SwipeDirection.Right:
                    SetContextMenu(true);
                    break;
                default:
                    break;
            }
        };

        Button button = this.GetComponent<Button>();

        if (button != null)
        {
            button.onClick.AddListener(
                delegate()
                {
                    if (Clicked != null)
                    {
                        Clicked(this);
                    }
                });
        }

        MenuButton.onClick.AddListener(delegate()
        {
            SetContextMenu(!isMenuOpen);
        });
        MenuBackButton.onClick.AddListener(delegate()
        {
            SetContextMenu(false);
        });

        DeleteButton.onClick.AddListener(DeletePart);
        MoveUpButton.onClick.AddListener(MoveUp);
        MoveDownButton.onClick.AddListener(MoveDown);

	}

    public void Update()
    {
        swipe.Update();
        if (DeleteRequest.AttemptResolveReesponse() || MoveRequest.AttemptResolveReesponse())
        {
            MobileUIManager.Current.Manager.SwitchToScreen("StoryDetail");
        }

    }

    public void SetContextMenu(bool active)
    {
        isMenuOpen = active;
        //this.DisplayText.gameObject.SetActive(!active);
        this.Menu.gameObject.SetActive(active);
    }

    //void Update () {
    //    tap.Update();
    //}
}
