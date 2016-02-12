using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class ModalDisplay : MonoBehaviour, IPointerDownHandler
{

    public Transform DisplayContainer = null;
    public Text TextItem = null;

	void Start () {
	
	}
	
	void Update () {
	
	}
    public Action OnClosing { get; set; }

    public void ShowMessage(string text)
    {
        if (!string.IsNullOrEmpty(text))
        {
            TextItem.text = text;
            this.DisplayContainer.gameObject.SetActive(true);
        }
    }

    public void OnPointerDown(UnityEngine.EventSystems.PointerEventData eventData)
    {
        this.DisplayContainer.gameObject.SetActive(false);
        if (OnClosing != null)
        {
            OnClosing();
        }
    }
}
