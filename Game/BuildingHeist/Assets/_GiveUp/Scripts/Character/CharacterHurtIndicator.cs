using UnityEngine;
using System.Collections;
using GiveUp.Core;

public class CharacterHurtIndicator : MonoBehaviour {

    // TEXTURES
    public Texture2D TexturesGeneral;
    public Texture2D TexturesLeft;
    public Texture2D TexturesRight;
    public Texture2D TexturesBehind;

    public float TimesGeneralFade = 2;
    public float TimesIndicator = 1;


    TimedInterpolation TimerGeneralFade;
    TimedInterpolation TimerIndicator;
    Rect FullScreenRectangle;
    Color ColorFullyOpaque = new Color(1, 1, 1, 1);
    Color ColorFullyTransparent = new Color(1, 1, 1, 0);
    Texture2D currentIndicator = null;

	void Start () {
        FullScreenRectangle = new Rect(0, 0, Screen.width, Screen.height);
        TimerGeneralFade = new TimedInterpolation(TimesGeneralFade);
        TimerIndicator = new TimedInterpolation(TimesIndicator);
	}
	

	void Update () {

        TimerGeneralFade.Update();
        TimerIndicator.Update();
	
	}

    private Texture2D DetermineHurtIndicator(HurtInfo hurtInfo)
    {
        float[] distances = new float[4];
        distances[0] = Vector3.Distance((this.transform.position + -this.GetComponent<Camera>().transform.forward), hurtInfo.Position);
        distances[1] = Vector3.Distance((this.transform.position + this.GetComponent<Camera>().transform.right), hurtInfo.Position);
        distances[2] = Vector3.Distance((this.transform.position + -this.GetComponent<Camera>().transform.right), hurtInfo.Position);
        distances[3] = Vector3.Distance((this.transform.position + this.GetComponent<Camera>().transform.forward), hurtInfo.Position);


        float lowest = float.MaxValue;
        int lowestIndex = 0;

        for (int i = 0; i < distances.Length; i++)
        {
            if (distances[i] < lowest)
            {
                lowest = distances[i];
                lowestIndex = i;
            }
        }

        switch (lowestIndex)
        {
            case 0: return TexturesBehind;
            case 1: return TexturesLeft;
            case 2: return TexturesRight;
            default: return null;
        }
    }

    public void OnHurt(HurtInfo hurtInfo)
    {
        TimerGeneralFade.Reset();
        TimerGeneralFade.Start();

        currentIndicator = DetermineHurtIndicator(hurtInfo);
        if (currentIndicator != null)
        {
            TimerIndicator.Reset();
            TimerIndicator.Start();
        }
    }

    void OnGUI()
    {
        UpdateTransparentTexture(TimerGeneralFade, TexturesGeneral);

        if (currentIndicator != null)
        {
            UpdateTransparentTexture(TimerIndicator, currentIndicator);
        }
    }

    public void UpdateTransparentTexture(TimedInterpolation timer, Texture2D texture)
    {
        if (timer.Enabled)
        {
            GUI.color = Color.Lerp(ColorFullyOpaque, ColorFullyTransparent, timer.Ratio);
            GUI.DrawTexture(FullScreenRectangle, texture, ScaleMode.StretchToFill, true, 1);
        }
    }

}
