using UnityEngine;
using System.Collections;
using System;

public class Game3DTextButton : GameButtonBase<TextMesh>
{

    public float HoverRotation = 0;
    public Color TextColor = Color.white;
    public Color HoverColor = Color.yellow;


    public override void Start()
    {
        base.Start();
    }
    public override void OnMouseEnter()
    {
        Element.color = HoverColor;
        base.OnMouseEnter();
    }

    public override void OnMouseExit()
    {
        Element.color = TextColor;
        base.OnMouseExit();
    }
}
