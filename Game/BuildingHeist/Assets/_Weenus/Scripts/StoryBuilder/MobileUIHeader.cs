using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Weenus;

public class MobileUIHeader : MonoBehaviour {


    public Button BackButton = null;
    public Text Title = null;
    public RectTransform MenuButton = null;

    WeenusUI View;

	// Use this for initialization
	void Start () {
        View = new WeenusUI(this);
	}

    void FixedUpdate()
    {
    }

	// Update is called once per frame
	void Update () {

       // BackButton.gameObject.SetActive(View.UI.History.Count > 1);

	}
}
