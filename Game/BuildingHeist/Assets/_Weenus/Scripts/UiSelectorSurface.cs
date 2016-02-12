using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

public class UiSelectorSurface : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {

    Button surfaceButton;
    private float MaxRayDistance = 50;
    private float singlePointerDownTime = 0;
    private float clickThreshold = 0.2f;
    private float _rotationScale = 0.1f;
    private bool isPointerDown;
    
    private float minDragThreshold = 20;
    private float totalDistanceTraveled;
    private int pointerId;
    private Vector2 lastPointerPos;
    private Vector3 pointerDirection;

    protected float minCameraX = 0;
    protected float maxCameraX = 0;
    protected float? minCameraY;
    protected float? maxCameraY;
    float? lastZoomDistance = null;
    float lastZoomScroll = 0;
    
    List<Vector3> twoToucheIndicies = new List<Vector3>();

    public UiMode Mode = UiMode.Desktop;

    public Transform HighestVisibleElement = null;
    public Transform LowestVisibleElement = null;
    public Transform RotatorRing = null;
    public Transform MainFocusObject = null;
    public Camera CurrentCamera = null;
    public float ConstrainRangeX = 11;

    public float MinZoomZ = -4.2f;
    public float MaxZoomZ = 8;

    public UiSelectable SelectedItem { get; set; }
    public bool IsDragging { get; set; }
    public bool IsRotating { get; private set; }

	public virtual void Start () {

        maxCameraX = (CurrentCamera.transform.position + (CurrentCamera.transform.right * ConstrainRangeX)).x;
        minCameraX = (CurrentCamera.transform.position - (CurrentCamera.transform.right * ConstrainRangeX)).x;

	}

    public virtual void Update()
    {

        if (isPointerDown)
        {
            if (Mode == UiMode.TouchDevice)
            {
                Touch t = Input.GetTouch(pointerId);
                totalDistanceTraveled += Vector2.Distance(t.position, lastPointerPos);
                pointerDirection = t.position - lastPointerPos;
                lastPointerPos = t.position;
            }
            else
            {
                totalDistanceTraveled += Vector2.Distance(Input.mousePosition, lastPointerPos);
                pointerDirection =  new Vector2(Input.mousePosition.x, Input.mousePosition.y) - lastPointerPos;
                lastPointerPos = Input.mousePosition;
            }
            if (IsZooming())
            {
                ApplyZoom();
            }
            else if (IsRotating)
            {
                ApplyRotationToObject(pointerDirection.x);
            }
            else if (totalDistanceTraveled > minDragThreshold)
            {
                ApplyDragToObject(pointerDirection);
            }
        }
        else
        {
            if (IsZooming())
            {
                ApplyZoom();
            }
        }
	}

    private void ApplyRotationToObject(float rotation)
    {
        if (this.MainFocusObject != null)
        {
            this.MainFocusObject.rotation *= Quaternion.Euler(Vector3.up * -rotation * _rotationScale);
            this.RotatorRing.rotation *= Quaternion.Euler(Vector3.up * -rotation * _rotationScale);
        }
    }
    private void ApplyDragToObject(Vector3 pointerDirection)
    {
        float x, y, z;
        var originalCamPosition = CurrentCamera.transform.position;
        Vector3 newPos;
        Vector3 transformedDirection = CurrentCamera.transform.rotation * pointerDirection;
        newPos = CurrentCamera.transform.position - (transformedDirection * 0.02f);
        x = newPos.x;
        y = newPos.y;
        z = originalCamPosition.z;// newPos.z;
        CurrentCamera.transform.position = newPos;


        if (HighestVisibleElement != null && LowestVisibleElement != null)
        {
            // figure min/max Y by checking screen coords of HighestVisibleElement and LowestVisibleElement
            
            var highScreenPoint = CurrentCamera.WorldToScreenPoint(HighestVisibleElement.position);
            var lowScreenPoint = CurrentCamera.WorldToScreenPoint(LowestVisibleElement.position);
            if (highScreenPoint.y < 0)
            {
                if (!minCameraY.HasValue)
                {
                    minCameraY = originalCamPosition.y;
                }
                y = minCameraY.Value;
            }
            if (lowScreenPoint.y > Screen.height)
            {
                if (!maxCameraY.HasValue)
                {
                    maxCameraY = originalCamPosition.y;
                }
                y = maxCameraY.Value;
            }
        }
        else if (minCameraY.HasValue && maxCameraY.HasValue)
        {
            // use pre-determined min/max Y to clamp
            if (y < minCameraY.Value)
                y = minCameraY.Value;

            if (y > maxCameraY.Value)
                y = maxCameraY.Value;
        }
        if (x < minCameraX)
        {
            x = minCameraX;
        }
        if (x > maxCameraX)
        {
            x = maxCameraX;
        }
        CurrentCamera.transform.position = new Vector3(x, y, z);
    }

    private void SetCamZoomPosition(Vector3 newPos)
    {
        float x, y, z; 
        x = newPos.x;
        y = newPos.y;
        z = newPos.z;

        if (z > MaxZoomZ)
            z = MaxZoomZ;

        if (z < MinZoomZ)
            z = MinZoomZ;

        CurrentCamera.transform.position = new Vector3(x, y, z);
    }
    public void ApplyZoom()
    {

        Vector3 newPos;

        if (Mode == UiMode.TouchDevice)
        {
            if (twoToucheIndicies.Count > 1)
            {
                float dist = Vector3.Distance(twoToucheIndicies[0], twoToucheIndicies[1]);
                if (lastZoomDistance.HasValue)
                {
                    newPos = CurrentCamera.transform.position + Vector3.forward * ((dist - lastZoomDistance.Value) * 0.03f);
                    SetCamZoomPosition(newPos);
                    lastZoomDistance = dist;
                }
                else
                {
                    lastZoomDistance = dist;
                }
            }
        }
        else
        {
            if (lastZoomScroll != 0)
            {
                newPos = CurrentCamera.transform.position + (Vector3.forward * lastZoomScroll);
                SetCamZoomPosition(newPos);
            }
        }
    }


    public void OnPointerDown(PointerEventData eventData)
    {
        IsRotating = false;
        lastPointerPos = eventData.position;
        pointerId = eventData.pointerId;
        singlePointerDownTime = Time.timeSinceLevelLoad;
        isPointerDown = true;
        var hitObject = RayCastFromScreen<Transform>(eventData.position);
        if (hitObject != null && hitObject.Value == this.RotatorRing && MainFocusObject != null)
        {
            IsRotating = true;
        }
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        lastZoomDistance = null;
        totalDistanceTraveled = 0;
        isPointerDown = false;
        if (IsRotating)
        {
            return;
        }
        if (Time.timeSinceLevelLoad - singlePointerDownTime < clickThreshold)
        {
            AttemptRayCastForTargetObject(eventData.position);
        }
    }


    protected void RaycastForType<T>(Vector3 pointerPosition, Action<EasyRayCastResult<T>> hitCallback) 
        where T: Component
    {
        var hitObject = RayCastFromScreen<T>(pointerPosition);
        if (hitObject != null)
        {
            hitCallback(hitObject);
        }
    }


    public virtual void AttemptRayCastForTargetObject(Vector3 pointerPosition)
    {
        RaycastForType<UiSelectableCollisionBroadcaster>(pointerPosition, (x) => { x.Value.OnSelected(x.Position); });
    }


    public bool IsZooming()
    {
        if (Mode == UiMode.TouchDevice)
        {
            twoToucheIndicies.Clear();
            foreach (Touch t in Input.touches)
            {
                if (t.phase != TouchPhase.Began && t.phase != TouchPhase.Canceled && t.phase != TouchPhase.Ended)
                {
                    twoToucheIndicies.Add(t.position);
                }
                if (twoToucheIndicies.Count > 1)
                {
                    break;
                }
            }
            if (twoToucheIndicies.Count > 1)
            {
                return true;
            }
            else
            {
                lastZoomDistance = null;
                return false;
            }
        }
        else
        {
            float scrollSpeed = 4;
            float wheelMove = Input.GetAxis("Mouse ScrollWheel");
            wheelMove *= scrollSpeed;
            if (wheelMove != 0)
            {
                lastZoomScroll = wheelMove;
                Debug.Log(lastZoomScroll);
                twoToucheIndicies.Clear();
                twoToucheIndicies.Add(GetScreenCenter());
                twoToucheIndicies.Add(GetScreenCenter() + (Vector2.up * lastZoomScroll));
                return true;
            }
            else
            {
                lastZoomScroll = 0;
            }
            lastZoomDistance = null;
            return false;
        }
    }
    public Vector2 GetScreenCenter()
    {
        return new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
    }
    public EasyRayCastResult<T> RayCastFromScreen<T>(Vector3 screenPos) where T : Component
    {
        RaycastHit possibleHit;
        Ray tapRayCast = CurrentCamera.ScreenPointToRay(screenPos);
        if (Physics.Raycast(tapRayCast, out possibleHit))
        {
            EasyRayCastResult<T> result = new EasyRayCastResult<T>();
            result.Position = possibleHit.point;
            result.Value = possibleHit.collider.GetComponent<T>();
            if (result.Value != null)
                return result;
        }
        return null;
    }
    public class EasyRayCastResult<T> where T : Component
    {
        public T Value { get; set; }
        public Vector3 Position { get; set; }
    }

}
