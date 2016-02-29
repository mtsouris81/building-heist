using UnityEngine;
using System.Collections;

public class RotatingUiElement : MonoBehaviour {

    RectTransform RectTransform;
    public enum RotationDirection
    {
        Clockwise,
        CounterClockwise
    }

    public RotationDirection Direction = RotationDirection.Clockwise;
    public float rotationSpeed = 2;
    
    
	// Use this for initialization
	void Start () {
        RectTransform = this.GetComponent<RectTransform>();
	}
	
	// Update is called once per frame
	void Update () {
        float turnSpeed = Direction == RotationDirection.Clockwise ? -rotationSpeed : rotationSpeed;
        Vector3 turnVec = new Vector3(0, 0, turnSpeed);
        Quaternion turn = Quaternion.Euler(turnVec);
        RectTransform.localRotation *= turn;
	}
}
