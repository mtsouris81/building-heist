using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using GiveUp.Core;


public class SwipeHelper
{
    public SwipeHelper()
    {
        SwiperTimer = new ActionTimer(0.25f, DetermineSwipe);
        SwiperTimer.AccurateMode = true;
    }


    public bool DominantDeltaOnly { get; set; }

    public float MaxDirectionDeviationScreenPercentage = 0.2f;

    public float MaxDirectionDeviation
    {
        get
        {
            return Screen.width * MaxDirectionDeviationScreenPercentage;
        }
    }
    public float MinDistanceScreenPercentage = 0.3f;
    public float MinDistance
    {
        get
        {
            return Screen.width * MinDistanceScreenPercentage;
        }
    }

    public Action<SwipeDirection> OnSwipe;

    public Action<Vector2> OnDelta;

    public int FingerID = 0;
    public bool IsActive = false;

    Vector2 start;
    Vector2 end;
    int currentPointerID;
    GameObject obj;

    ActionTimer SwiperTimer;

    Vector2 lastPos;
    Vector2 currPos;

    public Vector2 Delta { get; private set; }

    public void Down(PointerEventData eventData)
    {
        IsActive = true;
        SwiperTimer.Reset();
        SwiperTimer.Start();
        currentPointerID = eventData.pointerId;

        currPos =
        lastPos =
        start = GetCurrentPointerPosition();
    }
    public void Update()
    {
        if (OnSwipe != null)
        {
            SwiperTimer.Update();
        }

        if (IsActive && OnDelta != null)
        {
            DetermineDelta();
        }
    }

    private void DetermineDelta()
    {
        currPos = GetCurrentPointerPosition();
        Delta = currPos - lastPos;
        if (OnDelta != null && Delta != Vector2.zero)
        {
            if (DominantDeltaOnly)
            {
                if (Mathf.Abs(Delta.x) >= Mathf.Abs(Delta.y))
                {
                    Delta = new Vector2(Delta.x, 0);
                }
                else
                {
                    Delta = new Vector2(0, Delta.y);
                }
            }
            OnDelta(Delta);
        }
        lastPos = currPos;
    }

    private void DetermineSwipe()
    {
        end = GetCurrentPointerPosition();
        if (end == Vector2.zero)
        {
            end = start;
        }
        float distance = Vector2.Distance(start, end);
        if (distance < MinDistance)
        {
            return;
        }

        Vector2 diff = end - start;
        SwipeDirection? dir = null;
        bool longerHorizontal = Mathf.Abs(diff.x) > Mathf.Abs(diff.y);
        if (longerHorizontal)
        {
            if (Mathf.Abs(diff.y) > MaxDirectionDeviation)
            {
                return; // too diagonal to say it's a directional swipe
            }
        }
        else
        {
            if (Mathf.Abs(diff.x) > MaxDirectionDeviation)
            {
                return; // too diagonal to say it's a directional swipe
            }
        }
        if (longerHorizontal)
        {
            dir = (diff.x < 0) ? SwipeDirection.Left : SwipeDirection.Right;
        }
        else
        {
            dir = (diff.y < 0) ? SwipeDirection.Down : SwipeDirection.Up;
        }

        if (dir.HasValue && this.OnSwipe != null)
        {
            this.OnSwipe(dir.Value);
        }

    }
    private Vector2 GetCurrentPointerPosition()
    {
        if (currentPointerID < 0)
        {
            return new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        }
        foreach (var t in Input.touches)
        {
            if (t.fingerId == currentPointerID)
            {
                return t.position;
            }
        }
        return Vector2.zero;
    }



    public void Exit()
    {
        IsActive = false;
    }
}
public enum SwipeDirection
{
    Up,
    Down,
    Left,
    Right
}
