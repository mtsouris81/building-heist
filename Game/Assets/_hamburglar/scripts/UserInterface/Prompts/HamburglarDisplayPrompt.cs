using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using GiveUp.Core;

public class HamburglarDisplayPrompt : MonoBehaviour {

    public RectTransform Parent = null;
    public Graphic[] ColoredGraphics = null;

    protected ActionTimer showTimer;
    protected Vector3 originPosition;
    protected Vector3 screenCenter;

	public virtual void Start ()
    {
        EnsureLootTimer();
	}

    public void EnsureLootTimer()
    {
        if (showTimer != null)
            return;

        showTimer = new ActionTimer(0.15f, delegate()
        {
            this.Parent.rect.Set(0, 0, Screen.width, Screen.height);
        });
    }
	void Update ()
    {
        showTimer.Update();
        if (showTimer.Enabled)
        {
            this.Parent.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, showTimer.Ratio);
            this.Parent.anchoredPosition = Vector3.Lerp(originPosition, screenCenter, showTimer.Ratio);
        }
	}
    public void ShowDisplay(Vector3 screenPos)
    {
        EnsureLootTimer();
        this.gameObject.SetActive(true);
        this.Parent.gameObject.SetActive(true);
        float x = screenPos.x - Screen.width / 2;
        float y = screenPos.y - Screen.height / 2;
        originPosition = new Vector3(x, y, 0);
        this.Parent.anchoredPosition = originPosition;
        this.Parent.localScale = Vector3.zero;
        showTimer.Reset();
        showTimer.Start();
    }
    public void SetColor(Color col)
    {
        if (ColoredGraphics != null && ColoredGraphics.Length > 0)
        {
            foreach (var g in ColoredGraphics)
            {
                g.color = col;
            }
        }
    }
    public void CloseDisplay()
    {
        this.gameObject.SetActive(false);
        this.Parent.gameObject.SetActive(false);
    }

}

