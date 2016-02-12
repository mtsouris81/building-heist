using UnityEngine;
using System.Collections;

public class UiSelectableCollisionBroadcaster : MonoBehaviour {


    public UiSelectable UiCoordinator { get; set; }
	void Start () {
	
	}
	
	void Update () {
	
	}

    public void OnSelected(Vector3 pointerPosition)
    {
        if (this.UiCoordinator != null)
        {
            UiSelectorSurface surface = GameObject.FindObjectOfType<UiSelectorSurface>();
            if (surface != null)
            {
                surface.SelectedItem = this.UiCoordinator;
                this.UiCoordinator.SelectionHandler(pointerPosition);
            }
        }
    }

}
