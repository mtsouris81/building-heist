using UnityEngine;
using System.Collections;

public class RoomItemCollider : MonoBehaviour {

    public RoomItem Item { get; private set; }

	// Use this for initialization
	void Start () {
        Item = this.GetComponentInParent<RoomItem>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
