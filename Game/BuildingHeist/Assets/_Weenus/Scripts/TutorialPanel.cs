using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Weenus;
using System;
using UnityEngine.EventSystems;

public class TutorialPanel : MonoBehaviour, IPointerDownHandler
{
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

    public void OnPointerDown(PointerEventData eventData)
    {
        SendMessageUpwards("TutorialPanelClicked");
    }
}
