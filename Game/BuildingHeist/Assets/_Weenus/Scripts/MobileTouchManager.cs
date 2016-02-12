using UnityEngine;
using UnityEngine.EventSystems;

public class MobileTouchManager
{
    public const float TapDuration = 0.35f;

    //public float TouchTime = 0;
    //bool isDown = false;

    float startTime = 0;
    float endTime = 0;
    bool canceled = false;

    public void Update()
    {
        //if (isDown)
        //{
        //    TouchTime += Time.unscaledDeltaTime;
        //}
    }

    public void Down(PointerEventData pointerData)
    {
        startTime = Time.realtimeSinceStartup;
        endTime = startTime;
        canceled = false;
        //TouchTime = 0;
        //isDown = true;
    }

    public void Up(PointerEventData pointerData)
    {
        if (pointerData.dragging)
        {
            canceled = true;
            return;
        }
        //isDown = false;
        endTime = Time.realtimeSinceStartup;
    }

    public bool IsTap()
    {
        if (canceled)
        {
            return false;
        }
       // bool result = false;
        
        //if (Input.touches.Length > 0)
        //{
        //    foreach (var t in Input.touches)
        //    {
        //        TouchPhase phase = t.phase;
        //        if (phase != TouchPhase.Ended && phase != TouchPhase.Canceled)
        //        {
        //            return false;
        //        }
        //    }
        //}



        float dur = (endTime - startTime);
        return dur < TapDuration;
    }

}