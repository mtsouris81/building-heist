using UnityEngine;
using System.Collections;

public class FloorBuilder : MonoBehaviour {

    
    public Transform Room = null;
    public Transform Elevator = null;
    public Transform RightEnd = null;
    public Transform LeftEnd = null;
    public float SegmentWidth = 3;
    public float SegmentHeight = 3;

	void Start () {
	
	}

    public BuildingFloor GenerateFloor(int floor, int rooms, int elevatorIndex, Material wallMaterial, Material floorMaterial, Transform parentContainer)
    {
        int totalRoomSlots = rooms + 1;
        GameObject newFloor = new GameObject("floor " +  (floor + 1).ToString(), typeof(BuildingFloor));
        var fl = newFloor.GetComponent<BuildingFloor>();

        fl.FloorNumber = floor;
        int roomNumber = 0;
        for (int i = 0; i < totalRoomSlots; i++)
        {
            var prefab = i == elevatorIndex ? Elevator : Room;
            var r = GameObject.Instantiate(prefab) as Transform;
            FloorSegment segment = r.GetComponent<FloorSegment>();
            r.transform.SetParent(newFloor.transform);
            r.transform.localPosition = Vector3.right * (i * SegmentWidth);
            if (i == elevatorIndex)
            {
                segment.IsElevator = true;
                segment.SetFloorNumber(floor);
            }
            else
            {
                segment.IsElevator = false;
                segment.SetRoomNumber(floor, roomNumber++);
            }
            segment.SetWallMaterial(wallMaterial);
            segment.SetFloorMaterial(floorMaterial);
        }


        var l_end = GameObject.Instantiate(LeftEnd) as Transform;
        l_end.transform.SetParent(newFloor.transform);
        l_end.transform.localPosition = Vector3.left * (0.5f * SegmentWidth + (SegmentWidth / 2));

        var r_end = GameObject.Instantiate(RightEnd) as Transform;
        r_end.transform.SetParent(newFloor.transform);
        r_end.transform.localPosition = Vector3.right * (totalRoomSlots * SegmentWidth );

        FloorSegment left_floorSeg = l_end.GetComponent<FloorSegment>();
        FloorSegment right_floorSeg = r_end.GetComponent<FloorSegment>();
        left_floorSeg.SetFloorNumber(floor);
        right_floorSeg.SetFloorNumber(floor);
        left_floorSeg.SetWallMaterial(wallMaterial);
        right_floorSeg.SetWallMaterial(wallMaterial);
        left_floorSeg.SetFloorMaterial(floorMaterial);
        right_floorSeg.SetFloorMaterial(floorMaterial);

        newFloor.transform.SetToParentZero(parentContainer);
        newFloor.transform.position += Vector3.up * (floor * SegmentHeight);
        
        return fl;
    }
	
}
