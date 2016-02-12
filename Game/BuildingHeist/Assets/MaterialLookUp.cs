using UnityEngine;
using System.Collections;

public class MaterialLookUp : MonoBehaviour {

    public MaterialAtlas Hallway = null;
    public MaterialAtlas HallwayFloors = null;
    public MaterialAtlas Room = null;
    public MaterialAtlas RoomFloors = null;

    public Color GetRandomColor()
    {
        return PossibleColors[Random.Range(0, PossibleColors.Length - 1)];
    }

    public Color[] PossibleColors = null;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
