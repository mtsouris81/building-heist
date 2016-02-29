using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using GiveUp.Core;

public class BindableListItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler , IPointerDownHandler, IPointerUpHandler
{

    SwipeHelper swipe;

    public Text UiTextItem = null;
    public Image UiBackground = null;
    public Image UiImage = null;
    public Color TextColor = Color.white;
    public Color BackgroundColor = Color.black;
    public Color HoverTextColor = Color.black;
    public Color HoverBackgroundColor = Color.white;
    public Color SelectedBackgroundColor = Color.yellow;
    public Color SelectedTextColor = Color.black;

    public void SetTextColor(Color c, Color selectedColor)
    {
        TextColor = c;
        SelectedTextColor = selectedColor;
        if (UiTextItem != null)
        {
            UiTextItem.color = c;
        }
        _initialColorsSet = true;
    }

    public string ItemText { get; private set; }
    public object Value { get; private set; }
    public bool Selected { get; private set; }
    public Action<BindableListItem> OnClicked { get; set; }

    public bool TrackSelection { get; set; }
    public bool TrackHover { get; set; }

    MobileTouchManager tap = new MobileTouchManager();

    bool _initialColorsSet = false;

    public void SetData(string _text, object value)
    {
        ItemText = _text;
        Value = value;
        if (UiTextItem != null)
        {
            UiTextItem.text = ItemText;
        }
    }
    public void SetData(string _text, object value, Texture2D image)
    {
        ItemText = _text;
        Value = value;
        if (UiTextItem != null)
        {
            UiTextItem.text = ItemText;
        }
        if (UiImage != null)
        {
            var sprite = Sprite.Create(image, new Rect(0, 0, image.width, image.height), Vector2.zero);
            UiImage.sprite = sprite;
        }
    }

	void Start () {

        swipe = new SwipeHelper();

        Button button = this.GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(OnButtonClicked);
        }


        if (_initialColorsSet)
            return;

        _initialColorsSet = true;
        if (UiTextItem != null)
        {
            UiTextItem.color = TextColor;
        }
	}

    private void OnButtonClicked()
    {
        SetSelected(!this.Selected);
        if (OnClicked != null)
        {
            OnClicked(this);
        }

    }
	
	// Update is called once per frame
	void Update () {
        //tap.Update();
        swipe.Update();
	}

    public void SetSelected(bool isSelected)
    {
        if (TrackSelection)
        {
            this.Selected = isSelected;

            if (UiBackground != null)
            {
                //UiBackground.color = this.Selected ? SelectedBackgroundColor : BackgroundColor;
                UiBackground.gameObject.SetActive(isSelected);
            }
        }
    }

    public void OnPointerDown(UnityEngine.EventSystems.PointerEventData eventData)
    {
        swipe.Down(eventData);
        //tap.Down(eventData);
    }
    public void OnPointerUp(UnityEngine.EventSystems.PointerEventData eventData)
    {

        //tap.Up(eventData);
        //if (tap.IsTap())
        //{
        //    SetSelected(!this.Selected);
        //    if (OnClicked != null)
        //    {
        //        OnClicked(this);
        //    }
        //}
    }
    public void OnPointerEnter(UnityEngine.EventSystems.PointerEventData eventData)
    {
        if (!TrackHover)
            return;

        if (UiTextItem != null)
        {
            UiTextItem.color = HoverTextColor;
        }
        if (UiBackground != null)
        {
            UiBackground.color = HoverBackgroundColor;
        }
    }
    public void OnPointerExit(UnityEngine.EventSystems.PointerEventData eventData)
    {
        swipe.Exit();

        if (!TrackHover)
            return;

        if (UiTextItem != null)
        {
            UiTextItem.color = TextColor;
        }
        if (UiBackground != null)
        {
            UiBackground.color = BackgroundColor;
        }
    }







}
