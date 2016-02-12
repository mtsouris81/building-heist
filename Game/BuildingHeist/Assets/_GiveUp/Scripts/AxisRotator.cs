using UnityEngine;
using System.Collections;
using GiveUp.Core;

public class AxisRotator : MonoBehaviour {

    public enum Axis
    {
        X,
        Y,
        Z
    }

    public Axis RotationAxis = Axis.X;

    public float AnglePerSecond = 360;
    Quaternion newIncrement;
    float amount;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        amount = Time.smoothDeltaTime * AnglePerSecond;

        switch (this.RotationAxis)
        {
            case Axis.X: newIncrement = Quaternion.Euler(amount, 0, 0);
                break;
            case Axis.Y: newIncrement = Quaternion.Euler(0, amount, 0);
                break;
            case Axis.Z: newIncrement = Quaternion.Euler(0, 0, amount);
                break;
            default:
                break;
        }
        this.transform.localRotation *= newIncrement;
	}
}
