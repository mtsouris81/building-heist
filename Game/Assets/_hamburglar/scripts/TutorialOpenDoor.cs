using UnityEngine;
using System.Collections;
using GiveUp.Core;

public class TutorialOpenDoor : MonoBehaviour {

    public TutorialOpenDoor()
    {
        timer = new ActionTimer(3, OpenRandomDoor);
    }
    ActionTimer timer;
	

    public void OpenRandomDoor()
    {
        int floor = HamburglarContext.Instance.Floor;
        var floors = HamburglarContext.Instance.BuildingView.GetComponentsInChildren<BuildingFloor>(true);
        var florParts = floors[floor].GetComponentsInChildren<FloorSegment>(true);
        foreach(var p in florParts)
        {
            if (p.IsRoom)
            {
                p.SetDoorState(true);
                return;
            }
        }
    }
    public void StepActivated()
    {
        timer.Reset();
        timer.Start();
    }
	void Update () {
        timer.Update();
	}
}
