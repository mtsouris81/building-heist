using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using GiveUp.Core;

public class TextGrower : MonoBehaviour
{
    public Vector2 Padding = new Vector2();

    LayoutElement layoutElement;
    RectTransform rt;
    public Text txt;

    void Start()
    {
        rt = gameObject.GetComponent<RectTransform>();
        layoutElement = gameObject.GetComponent<LayoutElement>();
    }

    void Update()
    {
        rt.sizeDelta = new Vector2(rt.rect.width + Padding.x, txt.preferredHeight + Padding.y);
        if (layoutElement != null)
        {
            layoutElement.preferredHeight = rt.sizeDelta.y;
        }
    }
}

