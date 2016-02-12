using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class FloorSegment : MonoBehaviour {

    public Text DisplayText = null;
    public Transform CloseDoorTransform = null;
    public Animator DoorAnimator = null;
    public Renderer[] Renderers = null;
    Vector3 originalPos;
    Quaternion originalRot;
    Vector3 originalScale;


    public int Room { get; private set; }
    public int Floor { get; private set; }
    public bool IsElevator { get; set; }

    public bool IsRoom { get; set; }

    public void Start()
    {
    }

   
    public static string SetDoorNumber(int floor, int room)
    {
        int tempRoom = room + 1;
        return string.Format("{0}{1}{2}",
                                    floor + 1,
                                    tempRoom.ToString().Length > 1 ? "" : "0",
                                    tempRoom);
    }
    public void SetRoomNumber(int floor, int room)
    {
        IsRoom = true;
        Room = room;
        Floor = floor;
        if (DisplayText != null)
        {
            DisplayText.text = SetDoorNumber(floor, room);
        }
        if (CloseDoorTransform != null)
        {
            originalPos = CloseDoorTransform.localPosition;
            originalRot = CloseDoorTransform.localRotation;
            originalScale = CloseDoorTransform.localScale;
        }
    }
    public void SetFloorNumber(int floor)
    {
        Floor = floor;
        if (DisplayText != null)
        {
            DisplayText.text = (floor + 1).ToString();
        }
    }

    public bool IsDoorOpen { get; private set; }
    public void SetDoorState(bool isOpen)
    {
        if (IsRoom)
        {
            if (CloseDoorTransform == null || DoorAnimator == null)
                return;

            if (IsDoorOpen == isOpen)
            {
                return;
            }
            IsDoorOpen = isOpen;
            DoorAnimator.Play(isOpen ? "Open" : "Close");
            //CloseDoorTransform.localRotation = isOpen ? OpenDoorTransform.localRotation : originalRot;
            //CloseDoorTransform.localPosition = isOpen ? OpenDoorTransform.localPosition : originalPos;
            //CloseDoorTransform.localScale = isOpen ? OpenDoorTransform.localScale : originalScale;
        }
    }




    List<Renderer> WallRenderers = new List<Renderer>();
    List<Renderer> FloorRenderers = new List<Renderer>();

    public void OrganizeRenderables()
    {
        if (FloorRenderers.Count > 0 || WallRenderers.Count > 0) // already been populated
            return;

        foreach(var r in this.Renderers)
        {
            if (r.name.StartsWith("floor", System.StringComparison.OrdinalIgnoreCase))
            {
                FloorRenderers.Add(r);
            }
            else
            {
                WallRenderers.Add(r);
            }
        }
    }

    private void SetRendererListMaterial(IEnumerable<Renderer> list, Material mat)
    {
        if (list == null)
            return;

        foreach(var r in list)
        {
            r.material = new Material(mat);
        }
    }

    public void SetWallMaterial(Material wallMaterial)
    {
        OrganizeRenderables();
        SetRendererListMaterial(WallRenderers, wallMaterial);
    }

    public void SetFloorMaterial(Material floorMaterial)
    {
        OrganizeRenderables();
        SetRendererListMaterial(FloorRenderers, floorMaterial);
    }
}
