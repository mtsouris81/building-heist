using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class RoomDebugManager : MonoBehaviour {

    public RoomDebugView DebugRoomPrefab = null;

    public int CurrentFloor = 0;

    Color defaultBackColor;
    List<RoomDebugView> RoomViewList = new List<RoomDebugView>();
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
    void FixedUpdate()
    {
        if (HamburglarContext.Instance.BuildingData != null)
        {
            UpdateData(HamburglarContext.Instance.Floor);
            UpdateSelectedRoom();
        }
    }

    public void UpdateData(int floor)
    {
        if (RoomViewList.Count == 0 || floor != CurrentFloor)
        {
            CurrentFloor = floor;
            int rooms =     HamburglarContext.Instance.BuildingData.RoomsPerFloor;
            var floorData = HamburglarContext.Instance.BuildingData.Building.Floors;

            foreach(var d in RoomViewList)
            {
                GameObject.Destroy(d.gameObject);
            }
            RoomViewList.Clear();
            for (int i = 0; i < rooms; i++)
            {
                var item = GameObject.Instantiate(DebugRoomPrefab) as RoomDebugView;
                item.transform.SetParent(this.transform);
                item.SetRoom(floor, i);
                RoomViewList.Add(item);
            }
        }
    }
    public void UpdateSelectedRoom()
    {
        foreach(var r in RoomViewList)
        {
            if (r.RoomNumber == HamburglarContext.Instance.Room)
            {
                r.SetBackgroundColor(Color.blue);
            }
            else
            {
                r.ResetBackgroundColor();
            }
        }
    }
}
