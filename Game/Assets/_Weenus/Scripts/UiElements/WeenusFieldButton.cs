using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

public class WeenusFieldButton : MonoBehaviour
{

    public Action OnButtonClick { get; set; }

    public Button Input = null;


    public void SetButtonText(string button_text)
    {
        Text[] list = Input.GetComponentsInChildren<Text>(true);

        if (list == null || list.Length == 0)
            return;

        list[0].text = button_text;
    }

	void Start () {
        this.Input.onClick.AddListener(_clicked);
	}

    void _clicked()
    {
        if (this.OnButtonClick != null)
        {
            this.OnButtonClick();
        }
    }
	void Update () {
	
	}

    public void SetButtonColor(Color color)
    {
        this.Input.GetComponent<Image>().color = color;
    }
}
