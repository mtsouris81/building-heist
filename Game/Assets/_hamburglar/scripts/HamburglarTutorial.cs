using UnityEngine;
using System.Collections;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using GiveUp.Core;

public class HamburglarTutorial : MonoBehaviour {

    public HamburglarSelectorSurface BuildingSurface = null;
    public HamburglarSelectorSurface RoomSurface = null;
    public MobileUIManager UiManager = null;

    public TransformLookupItem[] TransformLookup = new TransformLookupItem[]{
        new TransformLookupItem() { Name="Intro" },
        new TransformLookupItem() { Name="Zoom" },
        new TransformLookupItem() { Name="Door" },
        new TransformLookupItem() { Name="Loot" },
        new TransformLookupItem() { Name="LeaveTrap" },
        new TransformLookupItem() { Name="LeaveRoom" },
        new TransformLookupItem() { Name="SeeOtherPlayers" },
        new TransformLookupItem() { Name="KickOutRules" },
        new TransformLookupItem() { Name="Recap" },
    };

    public Transform GetItem(string name)
    {
        foreach(var t in TransformLookup)
        {
            if (t.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
            {
                return t.Item;
            }
        }
        return null;
    }

    public SelectionList<TutorialStep> Steps { get; set; }
    bool isDone = false;
	void Start () {

        Steps = new SelectionList<TutorialStep>()
        {
            new TutorialStep(){
                ViewMode = HamburglarViewMode.Building,
                ActivationItem = GetItem("Intro")
                // page causes next
            },
            new TutorialStep(){
                ViewMode = HamburglarViewMode.Building,
                ActivationItem = GetItem("Zoom")
                // page causes next
            },
            new TutorialStep(){
                ViewMode = HamburglarViewMode.Building,
                ActivationItem = GetItem("Door"),
                FloorRayCastInterceptor = (Vector3 p, FloorItemCollider x) => {
                    if (!x.FloorSegment.IsElevator)
                    {
                        // it's a door!
                        NextStep();
                    }
                }
            },
            new TutorialStep(){
                ViewMode = HamburglarViewMode.Room,
                ActivationItem = GetItem("Loot"),
                RoomRayCastInterceptor = (Vector3 p, RoomItemCollider x) => {
                    if (x.Item.Type != FurnitureType.Door)
                    {
                        HamburglarContext.Instance.LootPrompt.ShowDisplay(p,x.Item, 1, true, 0);
                    }
                }
            },
            new TutorialStep(){
                ViewMode = HamburglarViewMode.Room,
                ActivationItem = GetItem("LeaveTrap")
            },
            new TutorialStep(){
                ViewMode = HamburglarViewMode.Room,
                ActivationItem = GetItem("LeaveRoom"),
                RoomRayCastInterceptor = (Vector3 p, RoomItemCollider x) => {
                    if (x.Item.Type == FurnitureType.Door)
                    {
                        // it's an item
                        NextStep();
                    }
                }
            },
            new TutorialStep(){
                ViewMode = HamburglarViewMode.Building,
                ActivationItem = GetItem("SeeOtherPlayers"),
                FloorRayCastInterceptor = (Vector3 p, FloorItemCollider x) => {
                    if (x.FloorSegment.IsDoorOpen)
                    {
                        // it's a door!
                        NextStep();
                    }
                }
            },
            new TutorialStep(){
                ViewMode = HamburglarViewMode.Room,
                ActivationItem = GetItem("KickOutRules")
            },
            new TutorialStep(){
                ViewMode = HamburglarViewMode.Building,
                ActivationItem = GetItem("Recap")
            }
        };
        Steps.EndOfList += Steps_EndOfList;
	}

    void Steps_EndOfList(object sender, EventArgs e)
    {
        FinishTutorial();
    }
	
    public static void StartTutorial()
    {
        HamburglarTutorial tutorial = GameObject.FindObjectOfType<HamburglarTutorial>();
        tutorial.isDone = false;
        tutorial.Steps.ResetIndex();
        tutorial.SetCurrentStep(tutorial.Steps.CurrentValue);
        tutorial.UiManager.MenuButton.gameObject.SetActive(false);
        LootPrompt.InterceptAction = () =>
        {
            tutorial.NextStep();
            HamburglarContext.Instance.LootPrompt.CloseDisplay();
        };
        TrapButton.InterceptAction = () =>
        {
            // do nothing
        };
    }
    public void FinishTutorial()
    {
        isDone = true;

        UiManager.MenuButton.gameObject.SetActive(true);
        LootPrompt.InterceptAction = null;
        TrapButton.InterceptAction = null;
        HamburglarSelectorSurface surface = GetSelectorSurface(HamburglarViewMode.Building);
        surface.FloorRayCastInterceptor = null;
        surface.RoomRayCastInterceptor = null;
        surface = GetSelectorSurface(HamburglarViewMode.Room);
        surface.FloorRayCastInterceptor = null;
        surface.RoomRayCastInterceptor = null;
        HamburglarContext.Instance.Building.DestroyBuilding();
        MobileUIManager.Current.Manager.SwitchToScreen("Games");
    }
    public void NextStep()
    {
        Steps.Next();
        SetCurrentStep(Steps.CurrentValue);
    }
    private void SetCurrentStep(TutorialStep tutorialStep)
    {
        foreach(var s in Steps)
        {
            if (s.ActivationItem != null)
                s.ActivationItem.gameObject.SetActive(false);
        }

        if (isDone)
            return;
        
        if(tutorialStep.ViewMode != HamburglarContext.Instance.Mode)
            HamburglarContext.Instance.SetView(tutorialStep.ViewMode);
        
        HamburglarSelectorSurface surface = GetSelectorSurface(tutorialStep.ViewMode);
        surface.FloorRayCastInterceptor = tutorialStep.FloorRayCastInterceptor == null
                                            ? (p, x) => { } // do nothing method
                                            : tutorialStep.FloorRayCastInterceptor;
        surface.RoomRayCastInterceptor = tutorialStep.RoomRayCastInterceptor == null
                                            ? (p, x) => { } // do nothing method
                                            : tutorialStep.RoomRayCastInterceptor;
        if (tutorialStep.ActivationItem != null)
        {
            tutorialStep.ActivationItem.gameObject.SetActive(true);
            tutorialStep.ActivationItem.SendMessage("StepActivated", SendMessageOptions.DontRequireReceiver);
        }
    }

    private HamburglarSelectorSurface GetSelectorSurface(HamburglarViewMode viewMode)
    {
        switch (viewMode)
        {
            case HamburglarViewMode.Room:
                return this.RoomSurface;
            case HamburglarViewMode.Building:
                return this.BuildingSurface;
            default:
                return null;
        }
    }



	void Update () {
	
	}

    [Serializable]
    public class TransformLookupItem
    {
        public string Name = "";
        public Transform Item = null;
    }

    public class TutorialStep
    {
        public HamburglarViewMode ViewMode { get; set; }
        public System.Action<Vector3, FloorItemCollider> FloorRayCastInterceptor { get; set; }
        public System.Action<Vector3, RoomItemCollider> RoomRayCastInterceptor { get; set; }
        public Transform ActivationItem = null;
    }
}
