using UnityEngine;
using System.Collections;
using GiveUp.Core;
using System;

public class LightFlicker : MonoBehaviour
{

    public int Steps = 20;

    ValueTimerSequence<float> lightFlickerer = null;
    Light Light;
    float originalIntensity = 0;

    public void Start ()
    {
        Light = this.GetComponent<Light>();
        originalIntensity = Light.intensity;
        var shakeRandoms = this.GetRandoms(Steps, 0.5f, 0.91f);
        for (int i = 0; i < shakeRandoms.Length; i++)
        {
            if (i % 2 == 0)
            {
                shakeRandoms[i] = 0;
            }
        }
        lightFlickerer = new ValueTimerSequence<float>(this.GetRandoms(Steps, 0.05f, 0.15f), shakeRandoms);
        lightFlickerer.DetermineValue = s => s;
        lightFlickerer.Lerp = (a, b, t) => { return b; };
        lightFlickerer.FinishedCallback = () => { Light.intensity = originalIntensity; };
    }
	public void Update ()
    {
        lightFlickerer.Update();
        if (lightFlickerer.Timer.Enabled)
        {
            Light.intensity = lightFlickerer.GetCurrentValue();
        }
    }
    public void StartFlicker()
    {
        lightFlickerer.StartSequence();
    }
}
