using UnityEngine;
using System.Collections;

public class BuildingRoom : MonoBehaviour {

    public int Room { get; set; }
    public int Floor { get; set; }
    public Vector3 Dimensions { get; set; }
    public Light RoomLight { get; set; }
    public Light Blacklight { get; set; }
    RoomItem[] RoomItems = null;


    void Start () {
	
	}
	void Update () {
	
	}

    public void StartLightFlicker()
    {
        var flicker = RoomLight.GetComponent<LightFlicker>();
        if (flicker != null)
        {
            flicker.StartFlicker();
        }
    }

    public void TurnBlacklightOn()
    {
        RoomLight.gameObject.SetActive(false);
        Blacklight.gameObject.SetActive(true);
        ShowHands();
    }
    public void TurnBlacklightOff()
    {
        RoomLight.gameObject.SetActive(true);
        Blacklight.gameObject.SetActive(false);
        HideHands();
    }
    public void ShowHands()
    {
        int floor = HamburglarContext.Instance.Floor;
        int room = HamburglarContext.Instance.Room;
        if (RoomItems != null)
        {
            foreach (var i in RoomItems)
            {
                if (i.Type == FurnitureType.Door)
                    continue;

                var value = HamburglarContext.Instance.BuildingData.Building.Floors[floor].Rooms[room, i.LootIndex];
                if (value < 0)
                {
                    i.ShowHands();
                }
            }
        }
    }
    public void HideHands()
    {
        if (RoomItems != null)
        {
            foreach (var i in RoomItems)
            {
                i.HideHands();
            }
        }
    }

    public void AssignIndexesToItems()
    {
        RoomItems = this.GetComponentsInChildren<RoomItem>(true);
        int index = 0;

        foreach (var i in RoomItems)
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
