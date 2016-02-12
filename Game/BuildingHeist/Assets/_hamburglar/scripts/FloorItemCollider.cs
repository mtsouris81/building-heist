using UnityEngine;
using System.Collections;

public class FloorItemCollider : MonoBehaviour {

    public FloorSegment FloorSegment { get; private set; }
	// Use this for initialization
	void Start () {
        FloorSegment = this.GetComponentInParent<FloorSegment>();
	}
}
