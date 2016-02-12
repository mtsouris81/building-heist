using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Weenus;
using System;
using UnityEngine.EventSystems;

public class ZoomButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{


    public Action OnDown { get; set; }
    public Action OnUp { get; set; }
	
	private bool hasInitialized = false;

	void Start () 
    {
        Init();
	}
	
	void Update () {
	
	}

    public void Init()
    {
        if (hasInitialized)
            return;

        hasInitialized = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (OnUp != null)
            OnUp();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (OnDown != null)
            OnDown();
    }
}
