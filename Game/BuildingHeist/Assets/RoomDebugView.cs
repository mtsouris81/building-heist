using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class RoomDebugView : MonoBehaviour {

    public int RoomNumber { get; set; }
    public Text Text = null;
    public Image Image1 = null;
    public Image Image2 = null;
    public Image Image3 = null;
    public Image Image4 = null;
    public Image Image5 = null;


    public Color DefaultBackgroundColor { get; private set; }
    Image background = null;
	// Use this for initialization
	void Start () {
        background = GetComponent<Image>();
        DefaultBackgroundColor = background.color;
	}


    int Floor;
    public void SetBackgroundColor(Color c)
    {
        if (background != null)
        {
            background.color = c;
        }
    }
    public void ResetBackgroundColor()
    {
        if (background != null)
        {
            background.color = DefaultBackgroundColor;
        }
    }
    public void SetRoom(int floor, int room)
    {

        Floor = floor;
        RoomNumber = room;
        this.Text.text = FloorSegment.SetDoorNumber(floor, room);
        UpdateItems();
    }

    private void UpdateItems()
    {
        var floors = HamburglarContext.Instance.BuildingData.Building.Floors;
        Image1.color = GetColorForItem(floors[Floor].Rooms[RoomNumber, 0]);
        Image2.color = GetColorForItem(floors[Floor].Rooms[RoomNumber, 1]);
        Image3.color = GetColorForItem(floors[Floor].Rooms[RoomNumber, 2]);
        Image4.color = GetColorForItem(floors[Floor].Rooms[RoomNumber, 3]);
        Image5.color = GetColorForItem(floors[Floor].Rooms[RoomNumber, 4]);
    }

    private Color GetColorForItem(int p)
    {
        if (p < 0)
        {
            return Color.red;
        }
        else if (p == 0)
        {
            return Color.grey;
        }
        else
        {
            return Color.green;
        }
    }
	// Update is called once per frame
	void FixedUpdate () {
        UpdateItems();
	}
}
