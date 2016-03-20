using UnityEngine;
using System.Collections;
using System;

public class HamburglarSelectorSurface : UiSelectorSurface
{
    public HamburglarViewMode ViewMode;
    public Vector2 debugX;
    public Vector2 debugY;

    public void ResetRangeX(Vector3 startPos, float totalItems, float itemWidth)
    {
        minCameraX = startPos.x - (itemWidth / 2f);
        maxCameraX = startPos.x + (totalItems * itemWidth) - (itemWidth / 2f);
        debugX = new Vector2(minCameraX, maxCameraX);
    }
    public void ResetRangeX(Vector3 startPos, float itemWidth)
    {
        minCameraX = startPos.x - (itemWidth / 2f);
        maxCameraX = startPos.x + (itemWidth / 2f);
        debugX = new Vector2(minCameraX, maxCameraX);
    }
    public void ResetRangeY(Vector3 startPos, float itemWidth, bool heightFromCenter)
    {
        if (heightFromCenter)
        {
            minCameraY = startPos.y - (itemWidth / 2);
            maxCameraY = startPos.y + (itemWidth / 2);
        }
        else
        {
            minCameraY = startPos.y;
            maxCameraY = startPos.y + itemWidth;
        }
        debugY = new Vector2(minCameraY.Value, maxCameraY.Value);
    }
    public void ResetRangeY(Vector3 startPos, float itemWidth)
    {
        ResetRangeY(startPos, itemWidth, false);
        debugY = new Vector2(minCameraY.Value, maxCameraY.Value);
    }
    public void CenterCameraOnRange()
    {
        float x = this.CurrentCamera.transform.position.x;
        float y = this.CurrentCamera.transform.position.y;
        float z = this.CurrentCamera.transform.position.z;
        x = Mathf.Lerp(minCameraX, maxCameraX, 0.5f);
        if (minCameraY.HasValue && maxCameraY.HasValue)
        {
            y = Mathf.Lerp(minCameraY.Value, maxCameraY.Value, 0.5f);
        }
        this.CurrentCamera.transform.position = new Vector3(x, y, z);
    }
    public override void AttemptRayCastForTargetObject(Vector3 pointerPosition)
    {
        switch (ViewMode)
        {
            case HamburglarViewMode.Room:
                HandleRayCastForRoom(pointerPosition);
                break;
            case HamburglarViewMode.Building:
                HandleRayCastForFloor(pointerPosition);
                break;
            default:
                break;
        }
    }

    private void HandleRayCastForRoom(Vector3 pointerPosition)
    {
        RaycastForType<RoomItemCollider>(pointerPosition, (x) =>
        {

            if (x.Value.Item.Type == FurnitureType.Door)
            {
                if (RoomRayCastInterceptor != null)
                {
                    RoomRayCastInterceptor(pointerPosition, x.Value);
                    return;
                }
                HamburglarContext.Instance.Service.OnExitingRoom(HamburglarContext.Instance.Floor, HamburglarContext.Instance.Room);
            }
            else
            {
                x.Value.Item.PlayAnimation("shake");
                x.Value.Item.AnimationFinishedCallback = (RoomItem item) =>
                {
                    if (RoomRayCastInterceptor != null)
                    {
                        RoomRayCastInterceptor(pointerPosition, x.Value);
                        return;
                    }
                    int value = x.Value.Item.CheckForLoot();
                    if (value < 0)
                    {
                        item.ActivateTrapEffect();
                        HamburglarContext.Instance.RoomCamera.GetComponent<Shaker>().StartShaking();
                        HamburglarContext.Instance.GetCurrentRoom().StartLightFlicker();
                        HamburglarContext.Instance.Service.LootServiceCall(item.LootIndex, 0);
                    }
                    else
                    {
                        if (value > 0)
                        {
                            item.ActivateMoneyParticles();
                        }
                        HamburglarContext.Instance.LootPrompt.ShowDisplay(pointerPosition, item, value, true, x.Value.Item.LootIndex);
                    }
                };
            }
        });
    }
    private void HandleRayCastForFloor(Vector3 pointerPosition)
    {
        RaycastForType<FloorItemCollider>(pointerPosition, (x) =>
        {
            if (FloorRayCastInterceptor != null)
            {
                FloorRayCastInterceptor(pointerPosition, x.Value);
                return;
            }
            if (!x.Value.FloorSegment.IsElevator)
            {
                HamburglarContext.Instance.Floor = x.Value.FloorSegment.Floor;
                HamburglarContext.Instance.Room = x.Value.FloorSegment.Room;
                HamburglarContext.Instance.Service.OnEnteringRoom(x.Value.FloorSegment.Floor, x.Value.FloorSegment.Room);
            }
            else
            {
                HamburglarContext.Instance.ElevatorPrompt.ShowDisplay(pointerPosition, HamburglarContext.Instance.Floor);
            }
        });
    }

    // for tutorial use
    public Action<Vector3, FloorItemCollider> FloorRayCastInterceptor { get; set; }
    public Action<Vector3, RoomItemCollider> RoomRayCastInterceptor { get; set; }
}
