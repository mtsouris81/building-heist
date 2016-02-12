using UnityEngine;
using System.Collections;

public class RoomWall : MonoBehaviour {
    public Transform WallCube = null;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void SetSize(float width, float height)
    {
        WallCube.localScale = new Vector3(width, height, 0.5f);
    }
}
