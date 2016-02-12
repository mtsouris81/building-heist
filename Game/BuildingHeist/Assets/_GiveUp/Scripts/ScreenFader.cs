using UnityEngine;
using System.Collections;
using System;
using GiveUp.Core;
using UnityEngine.UI;

public class ScreenFader : MonoBehaviour {

    public enum FadeMode
    {
        FadeIn,
        FadeOut,
        FadeOutThenIn,
        FadeOutHoldThenIn
    }

    public FadeMode Mode { get; set; }

    public Action OnFadeStart { get; set; }
    public Action OnFadeApex { get; set; }
    public Action OnFadeHoldStart { get; set; }
    public Action OnFadeHoldEnd { get; set; }
    public Action OnFadeEnd { get; set; }

    public ActionTimer timer { get; set; }

    public Image _image;
    public RectTransform ExtraMessage = null;

    bool isFadingIn;
    bool isHolding;


    float fadeInTime;
    float fadeOutTime;
    float fadeHoldTime;

    Color FadeInColor;
    Color FadeOutColor;



    public void FadeIn(Color color, float duration)
    {
        fadeInTime = duration;
        this.Mode = FadeMode.FadeIn;
        isFadingIn = true;
        FadeInColor = color;
        timer.TimeLimit = duration;
        timer.Reset();
        timer.Start();
    }
    public void FadeOut(Color color, float duration)
    {
        fadeOutTime = duration;
        this.Mode = FadeMode.FadeOut;
        isFadingIn = false;
        FadeOutColor = color;
        timer.TimeLimit = duration;
        timer.Reset();
        timer.Start();
    }
    public void FadeOutThenIn(Color outColor, Color inColor, float outDur, float inDur)
    {
        fadeInTime = inDur;
        fadeOutTime = outDur;
        this.Mode = FadeMode.FadeOutThenIn;
        isFadingIn = false;
        FadeOutColor = outColor;
        FadeInColor = inColor;
        timer.TimeLimit = outDur;
        timer.Reset();
        timer.Start();
    }
    public void FadeOutThenHoldThenFadeIn(Color outColor, Color inColor, float outDur, float inDur, float holdDuration)
    {
        fadeHoldTime = holdDuration;
        fadeInTime = inDur;
        fadeOutTime = outDur;
        this.Mode = FadeMode.FadeOutHoldThenIn;
        isFadingIn = false;
        FadeOutColor = outColor;
        FadeInColor = inColor;
        timer.TimeLimit = outDur;
        timer.Reset();
        timer.Start();
    }

	
    void Start () {
        //_image = this.GetComponent<Image>();
        timer = new ActionTimer(10, OnFadeTimerComplete);
        timer.Stop();
	}

	
    void Update () {

        if (timer.Enabled && !_image.gameObject.activeSelf)
        {
            _image.gameObject.SetActive(true);
        }
        if (!timer.Enabled && _image.gameObject.activeSelf)
        {
            _image.gameObject.SetActive(false);
        }

        
        if (timer.Enabled)
        {
            timer.Update();

            if (isFadingIn && !isHolding)
            {
                _image.color = Color.Lerp(FadeInColor, Color.clear, timer.Ratio);
            }
            if (!isFadingIn && !isHolding)
            {
                _image.color = Color.Lerp(Color.clear, FadeOutColor, timer.Ratio);
            }
            if (isHolding)
            {
                _image.color = FadeOutColor;
            }
        }
	}

    public void ShowExtraItem()
    {
        if (this.ExtraMessage != null)
        {
            this.ExtraMessage.gameObject.SetActive(true);
        }
    }
    public void HideExtraItem()
    {
        if (this.ExtraMessage != null)
        {
            this.ExtraMessage.gameObject.SetActive(false);
        }
    }

    void OnFadeTimerComplete()
    {
        switch (this.Mode)
        {
            case FadeMode.FadeIn:
                _image.color = Color.clear;
                break;
            case FadeMode.FadeOut:
                _image.color = FadeOutColor;
                if (OnFadeEnd != null)
                {
                    OnFadeEnd();
                }
                break;
            case FadeMode.FadeOutThenIn:
                if (OnFadeApex != null)
                    OnFadeApex();

                timer.TimeLimit = fadeInTime;
                timer.Reset();
                timer.Start();
                isFadingIn = true;
                break;
            case FadeMode.FadeOutHoldThenIn:
                if (!isHolding && !isFadingIn) // start holding
                {
                    if (OnFadeHoldStart != null)
                        OnFadeHoldStart();

                    timer.TimeLimit = fadeHoldTime;
                    isHolding = true;
                    timer.Reset();
                    timer.Start();
                    break;
                }
                else if (isHolding && !isFadingIn) // same as above, already held, so fade back in
                {
                    if (OnFadeHoldEnd != null)
                        OnFadeHoldEnd();

                    timer.TimeLimit = fadeInTime;
                    isFadingIn = true;
                    isHolding = false;
                    timer.Reset();
                    timer.Start();
                }
                else if (isFadingIn && !isHolding) // finished the 
                {
                    if (OnFadeEnd != null)
                        OnFadeEnd();
                }

                break;
            default:
                break;
        }

    }

}
