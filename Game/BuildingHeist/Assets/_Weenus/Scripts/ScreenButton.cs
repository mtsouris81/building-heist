using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ScreenButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public Vector3 ButtonCenter;

    private void OnUp()
    {
        if (MouseUpCallback != null)
        {
            MouseUpCallback();
        }
    }

    private void OnDown()
    {
        if (MouseDownCallback != null)
        {
            MouseDownCallback();
        }
    }

    public Action MouseDownCallback { get; set; }
    public Action MouseUpCallback { get; set; }

    public void OnPointerDown(UnityEngine.EventSystems.PointerEventData eventData)
    {
        OnDown();
    }
    public void OnPointerUp(UnityEngine.EventSystems.PointerEventData eventData)
    {
        OnUp();
    }
    public void OnMouseDown()
    {
        OnDown();
    }
    public void OnMouseUp()
    {
        OnUp();
    }

	// Use this for initialization
	void Start () {

        RectTransform b = this.GetComponent<RectTransform>();
        ButtonCenter = new Vector3(b.rect.position.x, b.rect.position.y, 0);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
