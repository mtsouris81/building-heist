using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

public class SimpleEventButton : MonoBehaviour {


    public Action<SimpleEventButton> ClickHandler;

	// Use this for initialization
	void Start () {

        this.GetComponent<Button>().onClick.AddListener(delegate()
        {
            if (this.ClickHandler != null)
                this.ClickHandler(this);
        });
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
