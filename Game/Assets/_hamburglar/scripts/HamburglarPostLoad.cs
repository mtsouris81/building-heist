using UnityEngine;
using System.Collections;

public class HamburglarPostLoad : MonoBehaviour {

	// Use this for initialization
	void Start () {

        MobileUIManager.Current.Manager.SwitchToScreen("login");
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
