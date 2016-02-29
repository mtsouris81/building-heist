using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Weenus;
using System;

public class ModalCloseButton : MonoBehaviour
{

    public string ModalName = null;
	
	
	private bool hasInitialized = false;
    WeenusUI View;
	void Start () 
    {
        Init();
	}
	
	void Update () {
	
	}

    private void CloseModal()
    {
        this.View.UI.CloseModalScreen(ModalName);
    }

    public void Init()
    {
        if (hasInitialized)
            return;

        hasInitialized = true;
        this.View = new WeenusUI(this);
        Button b = this.GetComponent<Button>();
        b.onClick.AddListener(CloseModal);
    }
}
