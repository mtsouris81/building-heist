using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Weenus;
using System;
using Tsouris.StoryBuilder.StoryParts;
using UnityEngine.EventSystems;
using Tsouris.StoryBuilder;

public class CustomPropertyPageDisplayItem : MonoBehaviour //, IPointerDownHandler
{

    public Action<CustomPropertyPageDisplayItem> Clicked { get; set; }

    public Text KeyText = null;
    public Text ValueText = null;

    WeenusUI View;

    public KeyVal Data { get; private set; }

    public void Bind(KeyVal data)
    {
        Data = data;
        KeyText.text = data.Key;
        ValueText.text = data.Value.ToString();
    }

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

        View = new WeenusUI(this);

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

    }


}
