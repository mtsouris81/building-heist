using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using UnityEngine.Events;

public class WeenusInputField : MonoBehaviour
{

    public InputField Input = null;
    public void SetValue(string val)
    {
        Input.text = val;
    }

    public string GetValue()
    {
        return Input.text;
    }

    public int GetValueInt()
    {
        int result = 0;
        int.TryParse(this.Input.text, out result);
        return result;
    }

    public void OnChanged(UnityAction<string> changeCallback)
    {
        this.Input.onValueChanged.AddListener(changeCallback);
    }


	void Start () {
	}
	

	void Update () {
	
	}
}
