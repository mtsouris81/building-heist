using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using GiveUp.Core;
using System;

public class TrapButton : MonoBehaviour {

    public int LootIndex { get; private set; }
    public RoomItem FollowObject { get; private set; }
    public Vector3 FollowOffset { get; private set; }

    public RectTransform LeftButton = null;
    public RectTransform RightButton = null;

    public Graphic StickLeft = null;
    public Graphic StickRight = null;

    public float AnimationTime = 0.6f;

    RectTransform rect;

    Vector2 leftButtonInitialPosition = Vector2.one;
    Vector2 rightButtonInitialPosition = Vector2.one;
    ActionTimer timer;

    public static Action InterceptAction { get; set; }

    bool hasStarted = false;

    public void SetData(RoomItem item)
    {
        LootIndex = item.LootIndex;
        FollowObject = item;
    }

    void Start () 
    {
        timer = new ActionTimer(AnimationTime, AnimationFinished);
        rect = GetComponent<RectTransform>();
        LeftButton.GetComponentInChildren<Button>().onClick.AddListener(() => { HandleTrapButtonClick(false); });
        RightButton.GetComponentInChildren<Button>().onClick.AddListener(() => { HandleTrapButtonClick(true); });
        leftButtonInitialPosition = LeftButton.anchoredPosition;
        rightButtonInitialPosition = RightButton.anchoredPosition;
        hasStarted = true;
	}

    private void AnimationFinished()
    {
        LeftButton.anchoredPosition = Vector2.zero;
        RightButton.anchoredPosition = Vector2.zero;
        if (FollowObject != null)
        {
            FollowObject.ActivateTrapParticles();
        }
        this.gameObject.SetActive(false);

        HamburglarContext.Instance.SetFloatingMessage("Trap Set!");
    }

    private void HandleTrapButtonClick(bool isRightButton)
    {
        if (InterceptAction == null)
        {
            var playerId = HamburglarContext.Instance.PlayerId;
            var score = HamburglarContext.Instance.BuildingData.GetPlayerScore(playerId);
            var trapCost = Hamburglar.Core.Game.CostOfTrap;
            HamburglarContext.Instance.LootPrompt.ClearLoot(true);
            if (score < trapCost)
            {
                HamburglarContext.Instance.SetFloatingMessage(string.Format("You don't have enough money to set a trap. Traps cost ${0}.", trapCost));
                return;
            }
        }

        timer.Reset();
        timer.Start();

        StickLeft.gameObject.SetActive(false);
        StickRight.gameObject.SetActive(false);
    }

    public void ActivateButton()
    {
        this.gameObject.SetActive(true);
        if (hasStarted)
        {
            LeftButton.anchoredPosition = leftButtonInitialPosition;
            RightButton.anchoredPosition = rightButtonInitialPosition;
        }
        StickLeft.gameObject.SetActive(true);
        StickRight.gameObject.SetActive(true);
    }

	void Update () 
    {
        if (HamburglarContext.Instance.Mode != HamburglarViewMode.Room || FollowObject == null)
        {
            this.gameObject.SetActive(false);
        }
        else
        {
            timer.Update();

            var screenPos = HamburglarContext.Instance.RoomCamera.WorldToScreenPoint(FollowObject.transform.position);
            rect.position = screenPos;
            bool isRight = screenPos.x < Screen.width / 2;
            LeftButton.gameObject.SetActive(!isRight);
            RightButton.gameObject.SetActive(isRight);
            if (timer.Enabled)
            {
                LeftButton.anchoredPosition = Vector2.Lerp(leftButtonInitialPosition, Vector2.zero, timer.Ratio);
                RightButton.anchoredPosition = Vector2.Lerp(rightButtonInitialPosition, Vector2.zero, timer.Ratio);
            }

        }
	}

}
