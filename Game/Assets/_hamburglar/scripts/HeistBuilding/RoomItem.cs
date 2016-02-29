using UnityEngine;
using System.Collections;
using System;

public class RoomItem : MonoBehaviour {

    public FurnitureType Type = FurnitureType.Bed;

    public int Floor { get; set; }
    public int Room { get; set; }
    public int LootIndex { get; set; }

    public RoomItemParticles Particles { get; set; }
    public Animator ItemAnimation { get; set; }
    public Action<RoomItem> AnimationFinishedCallback { get; set; }

    int particleBurstAmount = 11;

    void Start () {
        //ItemAnimation = GetComponentInChildren<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    public void ActivateMoneyParticles()
    {
        if (Particles != null)
            Particles.Money.Emit(particleBurstAmount);
    }
    public void ActivateTrapParticles()
    {
        if (Particles != null)
            Particles.Trap.Emit(particleBurstAmount);
    }
    public void OnShakeAnimationFinished()
    {
        if (AnimationFinishedCallback != null)
        {
            AnimationFinishedCallback(this);
            AnimationFinishedCallback = null;
        }
    }

    public void PlayAnimation(string name)
    {
        if (ItemAnimation == null)
        {
            ItemAnimation = this.GetComponent<Animator>();
            if (ItemAnimation == null)
            {
                return;
            }
        }
        ItemAnimation.Play(name);
    }



    private void SetDoor(bool animated, string animation, string immediateAnimation)
    {
        if (this.Type != FurnitureType.Door || ItemAnimation == null)
            return;

        ItemAnimation.Play(animated ? animation: immediateAnimation);
    }
    public void OpenDoor(bool animated)
    {
        SetDoor(animated, "Open", "OpenImmediate");
    }
    public void CloseDoor(bool animated)
    {
        SetDoor(animated, "Close", "CloseImmediate");
    }



    public int CheckForLoot()
    {
        if (HamburglarContext.Instance.BuildingData != null)
        {
            return HamburglarContext.Instance.BuildingData.Building.Floors[Floor].Rooms[Room, LootIndex];
        }
        return 0;
    }
}
