using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class RoomBuilder : MonoBehaviour 
{

    // designer props
    public Vector3 MinDimensions = new Vector3(10, 8, 5);
    public Vector3 MaxDimensions = new Vector3(10, 8, 5);
    public RoomWall WallPartPrefab = null;
    public Light LightPrefab = null;
    public RoomItemParticles ParticlesPrefab = null;
    public Transform HandsPrefab = null;
    public UnityEngine.Object RoomItemAnimationController = null;

    // private
    List<GameObject> BuiltRooms = new List<GameObject>();
    Vector3 Dimensions = new Vector3(10, 8, 5);
    List<FurnitureDefinition> FurnitureSizes = new List<FurnitureDefinition>();
    RoomSide BedSide;
    RoomWall wallLeft;
    RoomWall wallRight;
    RoomWall wallBack;
    RoomWall floor;
    RoomWall ceiling;


    List<RoomSide> SideAssignmentOrder = new List<RoomSide>();
    List<FurnitureType> assignedFurniture = new List<FurnitureType>();


    // start up
    public RoomBuilder()
    {
        FurnitureSizesAdd(FurnitureType.Bed, new Vector2(3, 1));
        FurnitureSizesAdd(FurnitureType.BookShelf, new Vector2(1, 2));
        FurnitureSizesAdd(FurnitureType.Desk, new Vector2(2, 1.3f));
        FurnitureSizesAdd(FurnitureType.Door, new Vector2(1.6f, 3));
        FurnitureSizesAdd(FurnitureType.Dresser, new Vector2(2, 2.3f));
        FurnitureSizesAdd(FurnitureType.SmallDrawer, new Vector2(1, 1));
    }
    void Start()
    {
    }



    // public
    public void SetWallMaterial(Material mat)
    {
        wallLeft.WallCube.GetComponentInChildren<Renderer>().material = mat;
        wallRight.WallCube.GetComponentInChildren<Renderer>().material = mat;
        wallBack.WallCube.GetComponentInChildren<Renderer>().material = mat;
    }
    public void SetFloorMaterial(Material mat)
    {
        floor.WallCube.GetComponentInChildren<Renderer>().material = mat;
    }
    public int GenerateRoom(int floorIndex, int roomIndex, Material wallMaterial, Material floorMaterial, Transform parentContainer)
    {
        string newName = string.Format("room{0}-{1}", floorIndex, roomIndex);
        assignedFurniture.Clear();
        PopulateSideAssignmentOrder();

        Dimensions = new Vector3(
            UnityEngine.Random.Range(MinDimensions.x, MaxDimensions.x),
            UnityEngine.Random.Range(MinDimensions.y, MaxDimensions.y),
            UnityEngine.Random.Range(MinDimensions.z, MaxDimensions.z));

        var _light = GameObject.Instantiate(LightPrefab, Vector3.zero, Quaternion.identity) as Light;
        var _blacklight = GameObject.Instantiate(LightPrefab, Vector3.zero, Quaternion.identity) as Light;
        wallLeft = GameObject.Instantiate(WallPartPrefab, Vector3.zero, Quaternion.identity) as RoomWall;
        wallRight = GameObject.Instantiate(WallPartPrefab, Vector3.zero, Quaternion.identity) as RoomWall;
        wallBack = GameObject.Instantiate(WallPartPrefab, Vector3.zero, Quaternion.identity) as RoomWall;
        floor = GameObject.Instantiate(WallPartPrefab, Vector3.zero, Quaternion.identity) as RoomWall;
        ceiling = GameObject.Instantiate(WallPartPrefab, Vector3.zero, Quaternion.identity) as RoomWall;

        GameObject room = new GameObject(newName, typeof(BuildingRoom));
        room.transform.SetToParentZero(parentContainer);

        room.transform.position = Vector3.left * 100;
        var roomComponent = room.GetComponent<BuildingRoom>();
        roomComponent.Room = roomIndex;
        roomComponent.Floor = floorIndex;
        roomComponent.Dimensions = Dimensions;
        BuiltRooms.Add(room);

        wallLeft.transform.SetParent(room.transform);
        wallRight.transform.SetParent(room.transform);
        wallBack.transform.SetParent(room.transform);
        floor.transform.SetParent(room.transform);
        ceiling.transform.SetParent(room.transform);
        _light.transform.SetParent(room.transform);
        _blacklight.transform.SetParent(room.transform);

        floor.transform.localScale = new Vector3(Dimensions.x, 0.1f, Dimensions.z);
        ceiling.transform.localScale = new Vector3(Dimensions.x, 0.1f, Dimensions.z);
        wallRight.SetSize(Dimensions.z, Dimensions.y);
        wallLeft.SetSize(Dimensions.z, Dimensions.y);
        wallBack.SetSize(Dimensions.x, Dimensions.y);

        _light.transform.localPosition = Vector3.zero;
        _blacklight.transform.localPosition = Vector3.zero;
        floor.transform.localPosition = Vector3.zero;
        ceiling.transform.localPosition = Vector3.zero;
        _light.transform.localPosition += Vector3.up * Dimensions.y;
        _blacklight.transform.localPosition += Vector3.up * Dimensions.y;
        wallLeft.transform.localPosition = new Vector3(Dimensions.x * 0.5f, 0, 0);
        wallRight.transform.localPosition = new Vector3(-(Dimensions.x * 0.5f), 0, 0);
        wallBack.transform.localPosition = new Vector3(0, 0, (Dimensions.z * 0.5f));
        ceiling.transform.localPosition += Vector3.up * Dimensions.y;
        wallLeft.transform.localRotation = Quaternion.Euler(Vector3.up * 90);
        wallRight.transform.localRotation = Quaternion.Euler(Vector3.up * -90);

        _light.range = (Dimensions.x + Dimensions.z) * 2;
        _blacklight.range = 5; // (Dimensions.x + Dimensions.z) * 2;

        BedSide = (RoomSide)UnityEngine.Random.Range(0, 2);
        SideAssignmentOrder.Remove(BedSide);
        SideAssignmentOrder.Insert(0, BedSide);
        FillWalls();

        roomComponent.AssignIndexesToItems();

        SetWallMaterial(wallMaterial);
        SetFloorMaterial(floorMaterial);

        _blacklight.color = Color.Lerp(Color.red, Color.blue, .5f);
        _blacklight.intensity = 4;
        _blacklight.gameObject.SetActive(false);
        roomComponent.Blacklight = _blacklight;
        roomComponent.RoomLight = _light;

        return BuiltRooms.Count - 1;
    }
    public void ClearAllRooms()
    {
        while (BuiltRooms.Count > 0)
        {
            GameObject.Destroy(BuiltRooms[0]);
            BuiltRooms.RemoveAt(0);
        }
    }
    public GameObject GetRoom(int index)
    {
        if (index >= BuiltRooms.Count)
            throw new ArgumentException("room index is out of bounds");

        return BuiltRooms[index];
    }
    public void ActivateRoom(GameObject obj)
    {
        foreach (var o in BuiltRooms)
        {
            o.SetActive(o == obj);
        }
    }
    public void OffsetAllRooms()
    {
        Vector3 move = Vector3.up * 9;
        for (int i = 0; i < BuiltRooms.Count; i++)
        {
            BuiltRooms[i].transform.position += (move * (float)i);
        }
    }

    // private
    private void FurnitureSizesAdd(FurnitureType t, Vector2 size)
    {
        FurnitureSizes.Add(new FurnitureDefinition()
        {
            Type = t,
            Size = size
        });
    }
    private RoomItem GetFurniture(FurnitureType t)
    {
        var item = RoomItemRegistry.Instance.GetItem(t);
        var result = GameObject.Instantiate(item) as RoomItem;
        
        var particles = GameObject.Instantiate(this.ParticlesPrefab) as RoomItemParticles;
        particles.transform.SetParent(result.transform);
        particles.transform.localPosition = Vector3.zero;
        particles.transform.localRotation = Quaternion.identity;
        result.Particles = particles;

        var innerContainer = new GameObject("InnerContainer");
       
        List<Transform> moveChildren = new List<Transform>();

        for (int i = 0; i < result.transform.childCount; i++)
        {
            moveChildren.Add(result.transform.GetChild(i));
        }
        innerContainer.transform.SetParent(result.transform);
        innerContainer.transform.localPosition = Vector3.zero;

        foreach(var c in moveChildren)
        {
            c.SetParent(innerContainer.transform, true);
        }
        var itemAnimator = innerContainer.AddComponent<Animator>();
        var notifier = innerContainer.AddComponent<AnimationParentNotifier>();
        itemAnimator.runtimeAnimatorController = this.RoomItemAnimationController as RuntimeAnimatorController;

        result.ItemAnimation = itemAnimator;
        result.gameObject.SetActive(true);

        // ensure RoomItemColliders are attached
        if (result.GetComponentInChildren<RoomItemCollider>() == null)
        {
            Collider colliders = result.GetComponentInChildren<Collider>();
            if (colliders != null)
            {
                var cc = colliders.GetComponent<RoomItemCollider>(); // should exist, but check anyway
                if (cc == null)
                {
                    colliders.gameObject.AddComponent<RoomItemCollider>();
                }
            }
        }

        return result;
    }
    private Vector2 GetFurnitureSize(FurnitureType t)
    {
        return FurnitureSizes.Where(x => x.Type == t).First().Size;
    }
    private FurnitureType? GetNextFurniture(float maxSize, List<FurnitureType> assigned)
    {
        var def = FurnitureSizes
            .Where(x => x.Size.x < maxSize && !assigned.Contains(x.Type))
            .FirstOrDefault();

        if (def == null)
            return null;

        return def.Type;
    }
    private float AssignFurnitureToSide(FurnitureType furnitureType, RoomSide side, float workingX)
    {
        RoomItem item = GetFurniture(furnitureType);
        Vector2 size = GetFurnitureSize(furnitureType);
        Transform wall = GetWall(side);
        float sideLength = GetWallWidth(side);
        item.transform.SetParent(wall, true);
        item.transform.localPosition = Vector3.zero;
        item.transform.localRotation = Quaternion.identity;
        item.transform.localPosition += Vector3.right * ((size.x / 2) + workingX);
        item.transform.localPosition += Vector3.left * (sideLength / 2);
        item.transform.localPosition += Vector3.back * 0.5f;
        item.transform.localScale = new Vector3(size.x, size.y, 1);
        assignedFurniture.Add(furnitureType);

        if (furnitureType != FurnitureType.Door)
        {
            var hands = GameObject.Instantiate<Transform>(HandsPrefab);
            hands.SetParent(item.transform, true);
            hands.localPosition = Vector3.zero + (Vector3.back * 0.505f);
            hands.localRotation = Quaternion.identity;
            item.Hands = hands;
        }

        item.HideHands();


        return size.x;
    }
    private Transform GetWall(RoomSide side)
    {
        switch (side)
        {
            case RoomSide.Back: return wallBack.transform;
            case RoomSide.Left: return wallLeft.transform;
            case RoomSide.Right: return wallRight.transform;
            default: return null;
        }
    }
    private float GetWallWidth(RoomSide side)
    {
        switch (side)
        {
            case RoomSide.Back: 
                return Dimensions.x;
            case RoomSide.Left:
            case RoomSide.Right: 
                return Dimensions.z;
            default:
                return Dimensions.x;
        }
    }
    private void FillWalls()
    {
        while (SideAssignmentOrder.Count > 0)
        {
            RoomSide side = SideAssignmentOrder[0];
            FurnitureType? nextFurniture = null;
            float sideSize = GetWallWidth(side);
            float sidePadding = sideSize * 0.12f;
            SideAssignmentOrder.RemoveAt(0);
            float workingX = 0;

            if (side == RoomSide.Back)
            {
                workingX += 1.1f;
                sideSize -= 1.1f;
            }
            if (side == RoomSide.Back && !assignedFurniture.Contains(FurnitureType.Door))
            {
                workingX += AssignFurnitureToSide(FurnitureType.Door, side, workingX) + sidePadding;
            }
            if (side == BedSide && !assignedFurniture.Contains(FurnitureType.Bed))
            {
                workingX += AssignFurnitureToSide(FurnitureType.Bed, side, workingX) + sidePadding;
            }

            float remainingSpace = sideSize - workingX;
            nextFurniture = GetNextFurniture(remainingSpace, assignedFurniture);
            while (nextFurniture.HasValue)
            {
                workingX += AssignFurnitureToSide(nextFurniture.Value, side, workingX) + sidePadding;
                remainingSpace = sideSize - workingX;
                nextFurniture = GetNextFurniture(remainingSpace, assignedFurniture);
            }
        }
    }
    private Vector3 ForcePositive(Vector3 v)
    {
        return new Vector3(
            v.x < 0 ? -v.x : v.x,
            v.y < 0 ? -v.y : v.y,
            v.z < 0 ? -v.z : v.z);
    }
    private void PopulateSideAssignmentOrder()
    {
        SideAssignmentOrder.Clear();
        SideAssignmentOrder.AddRange(new[] { RoomSide.Back, RoomSide.Right, RoomSide.Left });
    }





}
public enum RoomSide
{
    Back,
    Left,
    Right
}
public class FurnitureDefinition
{
    public FurnitureType Type { get; set; }
    public Vector2 Size { get; set; }
}
public enum FurnitureType
{
    Bed,
    Desk,
    Door,
    BookShelf,
    SmallDrawer,
    Dresser
}
