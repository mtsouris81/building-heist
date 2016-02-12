using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

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



	void Start () {
	
	}
	

	void Update () {
	
	}
}
