using UnityEngine;
using System.Collections;

public class RandomRotator : MonoBehaviour {
    public float rotationIncrement = 4;

	// Use this for initialization
	void Start () {

        if (Random.Range(0, 50) % 2 == 0)
        {
            rotationIncrement = -rotationIncrement;
        }
	
	}
	
	// Update is called once per frame
	void Update () {

        this.transform.localRotation *= Quaternion.Euler(0, rotationIncrement, 0);
	}
}
