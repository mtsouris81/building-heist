using UnityEngine;
using System.Collections;
using System;

public class GameButtonBase<T> : MonoBehaviour where T: Component
{
    public event EventHandler Clicked;
    public event EventHandler MouseEnterd;
    public event EventHandler MouseExited;

    public T Element { get; private set; }

	public virtual void Start () {
        Element = GetComponent<T>();
	}

    public virtual void OnMouseDown()
    {
        if (Clicked != null)
            Clicked(this, EventArgs.Empty);
    }
    public virtual void OnMouseEnter()
    {
        if (MouseEnterd != null)
            MouseEnterd(this, EventArgs.Empty);
    }
    public virtual void OnMouseExit()
    {
        if (MouseExited != null)
            MouseExited(this, EventArgs.Empty);
    }
	
	public virtual void Update () {
	
	}
}
