using UnityEngine;
using System.Collections;

public class UiSelectable : MonoBehaviour {

    public Light SelectionLight;
    UiSelectableCollisionBroadcaster[] broadcasters;

	// Use this for initialization
	void Start () {
        broadcasters = this.GetComponentsInChildren<UiSelectableCollisionBroadcaster>(true);
        foreach (var b in broadcasters)
        {
            b.UiCoordinator = this;
        }
	}

    public void SelectionHandler(Vector3 pointerPosition)
    {
        if (SelectionLight != null)
        {
            SelectionLight.transform.position = pointerPosition;
            SelectionLight.intensity = 8;
        }
    }

	// Update is called once per frame
	void Update () {
	
	}
}
