using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class BuildingManager : MonoBehaviour {



    const string MATERIAL_COLOR_PROPERTY_NAME = "_Color";




    public int TotalFloors = 10;
    public int RoomsPerFloor = 6;
    public RoomBuilder RoomBuilder = null;
    public FloorBuilder FloorBuilder = null;
    public MaterialLookUp MaterialLookUp = null;


    BuildingFloor[] floors;
    int[,] roomIndicies;

    void Start () {
	}

    public void UpdateDoorState(int floor, int room, bool isOpen)
    {
        var floorObject = floors[floor].transform;
        int roomIndex = 0;
        for (int i = 0; i < floorObject.childCount; i++)
		{
            var o = floorObject.GetChild(i);
            var c = o.GetComponent<FloorSegment>();
            if (c != null && c.IsRoom)
            {
                if (roomIndex == room)
                {
                    c.SetDoorState(isOpen);
                }
                roomIndex++;
            }
        }
    }

    public void DestroyBuilding()
    {

        RoomBuilder.ClearAllRooms();
        ClearAllFloors();
    }
    public void Build()
    {
        DestroyBuilding();
        // enforce max to avoid memory errors or tampering with http responses
        if (TotalFloors > Hamburglar.Core.GameDataManager.MAX_GAME_FLOORS)
        {
            TotalFloors = Hamburglar.Core.GameDataManager.MAX_GAME_FLOORS;
        }
        if (RoomsPerFloor > Hamburglar.Core.GameDataManager.MAX_GAME_ROOMS_PER_FLOOR)
        {
            RoomsPerFloor = Hamburglar.Core.GameDataManager.MAX_GAME_ROOMS_PER_FLOOR;
        }

        floors = new BuildingFloor[TotalFloors];
        roomIndicies = new int[TotalFloors, RoomsPerFloor];
        int elevatorIndex = UnityEngine.Random.Range(0, RoomsPerFloor);
        for (int floorIndex = 0; floorIndex < TotalFloors; floorIndex++)
        {
            var floor = FloorBuilder.GenerateFloor(
                                        floorIndex, 
                                        RoomsPerFloor, 
                                        elevatorIndex, 
                                        GetMaterial(BuildingMaterialType.HallwayWall, true),
                                        GetMaterial(BuildingMaterialType.HallwayFloor, false), 
                                        HamburglarContext.Instance.BuildingView);
            
            floors[floorIndex] = floor;
            for (int roomIndex = 0; roomIndex < RoomsPerFloor; roomIndex++)
            {
                int newRoomIndex = RoomBuilder.GenerateRoom(
                                        floorIndex,
                                        roomIndex,
                                        GetMaterial(BuildingMaterialType.RoomWall, true),
                                        GetMaterial(BuildingMaterialType.RoomFloor, false),
                                        HamburglarContext.Instance.RoomView);

                roomIndicies[floorIndex, roomIndex] = newRoomIndex;
            }
        }
        RoomBuilder.OffsetAllRooms();
        HamburglarContext.Instance.OnBuildingReady();
    }

    private Material GetMaterial(BuildingMaterialType type, bool randomColor)
    {
        MaterialAtlas atlas = null;
        switch (type)
        {
            case BuildingMaterialType.HallwayWall:
                atlas = MaterialLookUp.Hallway;
                break;
            case BuildingMaterialType.HallwayFloor:
                atlas = MaterialLookUp.HallwayFloors;
                break;
            case BuildingMaterialType.RoomWall:
                atlas = MaterialLookUp.Room;
                break;
            case BuildingMaterialType.RoomFloor:
                atlas = MaterialLookUp.RoomFloors;
                break;
            default:
                break;
        }
        if (atlas == null)
            return null;

        Material result = atlas.GetRandom<Material>();
        if (result == null)
        {
            throw new Exception("no material found");
        }
     
        result = new Material(result); // make copy

        if (randomColor)
        {
            result.SetColor(MATERIAL_COLOR_PROPERTY_NAME, MaterialLookUp.GetRandomColor());
        }

        return result;
    }


    public enum BuildingMaterialType
    {
        HallwayWall,
        HallwayFloor,
        RoomWall,
        RoomFloor
    }


    private void ClearAllFloors()
    {
        if (floors != null)
        {
            for (int i = 0; i < this.floors.Length; i++)
            {
                if (floors[i] == null)
                    continue;

                GameObject.Destroy(floors[i].gameObject);
            }
        }
    }
    public GameObject GetRoom(int floor, int room)
    {
        return RoomBuilder.GetRoom(roomIndicies[floor, room]);
    }
    public BuildingFloor GetFloor(int floor)
    {
        return floors[floor];
    }

    public void ActivateFloor(int floorNumber)
    {
        for (int i = 0; i < this.floors.Length; i++)
        {
            this.floors[i].gameObject.SetActive(i == floorNumber);
        }
    }
}
