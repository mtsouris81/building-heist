using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class BlacklightButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {

    public bool IsBlacklightActive { get; private set; }
    public HamburglarSelectorSurface Surface = null;
    public float DistanceFromOrigin = 3;
    public float LightSize = 3;
    public RectTransform BlacklightCursor = null;
    public RectTransform ButtonImage = null;
    void Start ()
    {
    }

    void Update()
    {
        bool showDisplay = (MobileUIManager.Current.Manager.Mode == Weenus.UIScreenManager.UiMode.GamePlay);
        ButtonImage.SetActiveState(showDisplay);
        if (!showDisplay)
        {
            if (BlacklightCursor.gameObject.activeSelf)
            {
                BlacklightCursor.SetActiveState(false);
            }
        }

        if (IsBlacklightActive)
        {
            ButtonImage.TurnOff();
            var pos = Surface.GetPointerPosition();
            var t = Surface.RayCastFromScreen<Transform>(pos);
            if (t != null)
            {
                BuildingRoom room = HamburglarContext.Instance.GetCurrentRoom();
                if ( room != null)
                {
                    var camDirection = Vector3.Normalize(Surface.CurrentCamera.transform.position - t.Position);
                    room.Blacklight.transform.position = t.Position + (camDirection * DistanceFromOrigin);
                    room.Blacklight.range = LightSize;
                    BlacklightCursor.SetScreenPosition(pos);
                }
            }
        }
    }


    protected void TurnBlacklightOn()
    {
        var room = HamburglarContext.Instance.GetCurrentRoom();
        if (room != null)
        {
            ButtonImage.TurnOff();
            room.TurnBlacklightOn();
            BlacklightCursor.TurnOn();
        }
    }
    protected void TurnBlacklightOff()
    {
        var room = HamburglarContext.Instance.GetCurrentRoom();
        if (room != null)
        {
            ButtonImage.TurnOn();
            room.TurnBlacklightOff();
            BlacklightCursor.TurnOff();
        }
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        IsBlacklightActive = true;
        TurnBlacklightOn();
        Debug.Log("on");
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        IsBlacklightActive = false;
        TurnBlacklightOff();
        Debug.Log("off");
    }
}
