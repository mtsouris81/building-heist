using UnityEngine;
using System.Collections;
using Hamburglar;
using Hamburglar.Core;
using System;
using System.Collections.Generic;
using GiveUp.Core;

public class HamburglarContext : MonoBehaviour {

    public static HamburglarContext Instance { get; private set; }

    public BuildingManager Building = null;
    public Transform BuildingView = null;
    public Transform RoomView = null;
    public LootPrompt LootPrompt = null;
    public TrappedPrompt TrappedPrompt = null;
    public ElevatorPrompt ElevatorPrompt = null;
    public HamburglarUiPopUp Popup = null;
    public HamburglarScoreDisplay ScoreDisplay = null;
    public GameStartCountdown Countdown = null;
    public RectTransform WaitingForPlayersDisplay = null;
    public Camera BuildingCamera = null;
    public Camera RoomCamera = null;
    public HamburglarSelectorSurface BuildingSurface = null;
    public HamburglarSelectorSurface RoomSurface = null;
    public HamburglarDataService Service = null;

    public RectTransform UISpawnContainer = null;
    public FloatingMessage FloatingMessagePrefab = null;

    public bool DebugUser = false;
    public string DegugUserID = "";
    public string PlayerId { get; set; }
    public int Floor { get; set; }
    public int Room { get; set; }
    public int? PlayerStartRoom { get; set; }
    public Hamburglar.Core.Game BuildingData { get; set; }
    public int HighestMessageReceived { get; private set; }
    public HamburglarMessagingClient Messaging { get; private set; }
    public HamburglarViewMode Mode { get; private set; }
    bool isBuildingReady;
    Camera[] AllCameras;
    bool isFirstFrame = true;
    int? lastPreparedFloor = null;
    float lastMessageSpawned = 0;
    Queue<QueuedFloatingMessage> floatingMessageQueue = new Queue<QueuedFloatingMessage>();
    public float minTimeBetweenMessages = 0.85f;
    ActionTimer floatingMessageTimer;

    public class QueuedFloatingMessage
    {
        public string Message { get; set; }
        public Color Color { get; set; }
    }
    void Start()
    {
        Instance = this;
        Messaging = new HamburglarMessagingClient();
        Game.ActionsAffectScore = false;
	}
    void Update()
    {
        if (floatingMessageTimer != null)
        {
            floatingMessageTimer.Update();
        }
    }
    public void SetView(HamburglarViewMode mode)
    {
        SetView(mode, false);
    }
    public void SetView(HamburglarViewMode mode, bool isReturning)
    {

        Mode = mode;
        switch (mode)
        {
            case HamburglarViewMode.Room:
                RoomView.gameObject.SetActive(true);
                BuildingView.gameObject.SetActive(false);
                TurnOnCamera(RoomCamera);
                if (isBuildingReady)
                {
                    var roomItem = Building.GetRoom(this.Floor, this.Room).GetComponent<BuildingRoom>();
                    if (!isReturning)
                    {
                        this.RoomSurface.ResetRangeX(roomItem.transform.position, roomItem.Dimensions.x);
                        this.RoomSurface.ResetRangeY(roomItem.transform.position, roomItem.Dimensions.y, false);
                        this.RoomSurface.CenterCameraOnRange();
                    }
                    Building.RoomBuilder.ActivateRoom(roomItem.gameObject);
                }
                break;
            case HamburglarViewMode.Building:
                BuildingView.gameObject.SetActive(true);
                RoomView.gameObject.SetActive(false);
                TurnOnCamera(BuildingCamera);
                if (isBuildingReady)
                {
                    bool floorIsPrepared = lastPreparedFloor.HasValue && lastPreparedFloor.Value == this.Floor;
                    if (!isReturning || !floorIsPrepared)
                    {
                        lastPreparedFloor = this.Floor;
                        var floorItem = Building.GetFloor(this.Floor);
                        this.BuildingSurface.ResetRangeX(floorItem.transform.position, floorItem.transform.childCount, Building.FloorBuilder.SegmentWidth);
                        this.BuildingSurface.ResetRangeY(floorItem.transform.position, Building.FloorBuilder.SegmentHeight, true);
                        this.BuildingSurface.CenterCameraOnRange();
                    }
                    Building.ActivateFloor(this.Floor);
                }
                break;
            default:
                break;
        }
    }
    public void TurnOnCamera(Camera cam)
    {
        //foreach (var c in AllCameras)
        //{
        //    c.gameObject.SetActive(c == cam); 
        //}
    }


    public void SetFloatingMessage(string message)
    {
        SetFloatingMessage(message, Color.yellow);
    }

    public void SetFloatingMessage(string message, Color color)
    {
        if (floatingMessageTimer == null)
        {
            floatingMessageTimer = new ActionTimer(minTimeBetweenMessages, () =>
            {
                if (floatingMessageQueue.Count > 0)
                    CheckFloatingMessageQueue();
            });
            floatingMessageTimer.Loop = true;
            floatingMessageTimer.Reset();
            floatingMessageTimer.Start();
        }

        floatingMessageQueue.Enqueue(new QueuedFloatingMessage()
        {
            Color = color,
            Message = message
        });
        if ((Time.realtimeSinceStartup - lastMessageSpawned) > minTimeBetweenMessages)
        {
            // clear immediately if no other active messages
            floatingMessageTimer.Reset();
            CheckFloatingMessageQueue();
        }

    }
    public void CheckFloatingMessageQueue()
    {
        lastMessageSpawned = Time.realtimeSinceStartup;
        var m = floatingMessageQueue.Dequeue();
        var floatingMessage = GameObject.Instantiate(this.FloatingMessagePrefab) as FloatingMessage;
        floatingMessage.SetText(m.Message);
        floatingMessage.SetColor(m.Color);
        floatingMessage.transform.SetParent(HamburglarContext.Instance.UISpawnContainer.transform);
        floatingMessage.StartDisplay();
    }
    public void ClearGamePrompts()
    {
        for (int i = 0; i < this.UISpawnContainer.childCount; i++)
        {
            var t = UISpawnContainer.GetChild(i);
            if (t.gameObject.name.IndexOf("clone",0, StringComparison.OrdinalIgnoreCase) >= 0)
            {
                GameObject.Destroy(t.gameObject);
            }
            else
            {
                t.gameObject.SetActive(false);
            }
        }
    }

    public void OnGameDataUpdatedAndLoaded()
    {
        int score;
        BuildingData.PlayerScore.TryGetValue(PlayerId, out score);
        ScoreDisplay.SetScores(BuildingData.Building.TotalValue, score);
        UpdateFloorDoors(Floor);
    }
    public void OnBuildingReady()
    {
        isBuildingReady = true;
        lastPreparedFloor = null;
        if (!PlayerStartRoom.HasValue)
        {
            Room = 0;
            SetView(HamburglarViewMode.Building);
        }
        else
        {
            Room = PlayerStartRoom.Value;
            SetView(HamburglarViewMode.Room);
        }
        UpdateFloorDoors();
    }
    private void UpdateFloorDoors()
    {
        if (this.BuildingData == null)
        {
            return;
        }
        for (int floor = 0; floor < this.BuildingData.Floors; floor++)
		{
            UpdateFloorDoors(floor);
        }
    }
    private void UpdateFloorDoors(int floor)
    {
        for (int room = 0; room < this.BuildingData.RoomsPerFloor; room++)
        {
            bool isOccupied = this.BuildingData.Building.Floors[floor].OccupiedRooms[room] != null;
            this.Building.UpdateDoorState(floor, room, isOccupied);
        }
    }
    public Player GetPlayerData(string id)
    {
        if (BuildingData != null)
        {
            foreach (var p in BuildingData.Players)
            {
                if (p.Id.Equals(id, StringComparison.OrdinalIgnoreCase))
                    return p;
            }
        }
        return null;
    }

    public BuildingRoom GetCurrentRoom()
    {
        return Instance
                                    .Building
                                    .GetRoom(
                                        Instance.Floor,
                                        Instance.Room)
                                        .GetComponent<BuildingRoom>();
    }

}
