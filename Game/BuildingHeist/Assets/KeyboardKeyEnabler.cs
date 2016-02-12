using UnityEngine;
using System.Collections;

public class KeyboardKeyEnabler : MonoBehaviour {


    public Transform ActiveObject = null;
    public KeyCode Key = KeyCode.C;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(Key))
        {
            ActiveObject.gameObject.SetActive(!ActiveObject.gameObject.activeSelf);
        }
	}
}
