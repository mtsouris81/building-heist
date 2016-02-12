using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ElevatorPrompt : HamburglarDisplayPrompt
{
    public Button OkButton = null;
    public Button UpButton = null;
    public Button DownButton = null;
    public Text Text = null;
    public Color Color = Color.black;

    int FloorSelection = 0;

    public override void Start()
    {
        base.Start();
        OkButton.onClick.AddListener(delegate()
        {
            HamburglarContext.Instance.Floor = FloorSelection;
            HamburglarContext.Instance.SetView(HamburglarViewMode.Building);
            CloseDisplay();
        });
        UpButton.onClick.AddListener(delegate()
        {
            FloorSelection++;
            EnforceFloorBounds();
            UpdateDisplay();
        });
        DownButton.onClick.AddListener(delegate()
        {
            FloorSelection--;
            EnforceFloorBounds();
            UpdateDisplay();
        });
    }
    private void UpdateDisplay()
    {
        this.Text.text = (FloorSelection + 1).ToString();
    }
    private void EnforceFloorBounds()
    {
        if (FloorSelection >= HamburglarContext.Instance.Building.TotalFloors)
            FloorSelection = HamburglarContext.Instance.Building.TotalFloors - 1;

        if (FloorSelection < 0)
            FloorSelection = 0;
    }
    public void ShowDisplay(Vector3 screenPos, int floor)
    {
        this.ShowDisplay(screenPos);
        FloorSelection = floor;
        UpdateDisplay();
        SetColor(Color);
    }

}