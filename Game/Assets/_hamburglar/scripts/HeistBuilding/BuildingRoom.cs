using UnityEngine;
using System.Collections;

public class BuildingRoom : MonoBehaviour {

    public int Room { get; set; }
    public int Floor { get; set; }
    public Vector3 Dimensions { get; set; }
	
	void Start () {
	
	}
	
	void Update () {
	
	}



    public void AssignIndexesToItems()
    {
        RoomItem[] items = this.GetComponentsInChildren<RoomItem>(true);
        int index = 0;

        foreach (var i in items)
        {
            i.Room = Room;
            i.Floor = Floor;

            if (i.Type == FurnitureType.Door)
                continue;

            i.LootIndex = index;
            index++;
        }

    }
}
