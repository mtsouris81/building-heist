using UnityEngine;
using System.Collections;
using System;

public class GameTextButton : GameButtonBase<GUIText>
{

    public Color TextColor = Color.white;
    public Color HoverColor = Color.yellow;

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
