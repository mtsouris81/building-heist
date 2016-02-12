using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class InputReporter : MonoBehaviour
{
    public Text TouchCount = null;
    public Text MouseButton = null;
    public Text TouchTime = null;

    public void ReportTouchTime(float time)
    {
        TouchTime.text = string.Format("time : {0}", time);
    }


    string touchesInfo;

    public void Update()
    {
        touchesInfo = string.Empty;
        if (Input.touches.Length > 0)
        {
            touchesInfo = string.Join(", ",
                                    Input.touches.Select(x => x.phase.ToString()).ToArray());
        }
        TouchCount.text = string.Format("touches : {0} \nwhatever {1}", Input.touchCount, touchesInfo);
        MouseButton.text = string.Format("mouse : {0}", Input.GetMouseButton(0));
    }
}