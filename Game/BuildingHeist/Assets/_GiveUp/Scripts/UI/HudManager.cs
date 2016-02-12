using UnityEngine;
using System.Collections;
using GiveUp.Core;

public class HudManager : MonoBehaviour {


    public Color FadeColor;
    public float FadeTime = 6;
    public bool FadeEnabled = true;

    Color _fadeColor;
    float _fadeTime;
    bool isFadingIn = true;
    bool twoWayFade = false;
    public TimedInterpolation FadeTimer { get; private set; }
    ActionTimer.TimerExpireDelegate twoWayOutDelegate;
    ActionTimer.TimerExpireDelegate twoWayFinalDelegate;
    Color _twoWayFadeColor;



    float _fadeBackTime = 0;
   
    void OnTwoWayFadeOutFinished()
    {
        if (twoWayOutDelegate != null)
        {
            twoWayOutDelegate();
        }
        StartFade(true, _twoWayFadeColor, _fadeBackTime, twoWayFinalDelegate);
    }
    public void StartFade(bool fadingIn)
    {
        StartFade(fadingIn, this.FadeColor, this.FadeTime, null);
    }
    public void StartFade(bool fadingIn, GiveUp.Core.ActionTimer.TimerExpireDelegate callback)
    {
        StartFade(fadingIn, FadeColor, FadeTime, callback);
    }
    public void StartFade(bool fadingIn, Color color, float time, GiveUp.Core.ActionTimer.TimerExpireDelegate callback)
    {
        if (!FadeEnabled)
        {
            return;
        }
        twoWayFade = false;
        isFadingIn = fadingIn;
        _fadeColor = color;
        FadeTimer.TimeLimit = time;
        FadeTimer.TimerExpireCallback = callback;
        FadeTimer.Reset();
        FadeTimer.Start();
        Blackout.color = fadingIn ? _fadeColor : Color.clear;
    }
    public void StartFadeOutThenFadeIn(Color outColor, Color inColor, float time, float fadeBackTime, GiveUp.Core.ActionTimer.TimerExpireDelegate inFinishedcallback, GiveUp.Core.ActionTimer.TimerExpireDelegate outFinishedcallback)
    {
        if (!FadeEnabled)
        {
            return;
        }

        _fadeBackTime = fadeBackTime;
        twoWayFade = true;
        twoWayOutDelegate = outFinishedcallback;
        twoWayFinalDelegate = inFinishedcallback;
        isFadingIn = false;
        _twoWayFadeColor = inColor;
        _fadeColor = outColor;
        FadeTimer.TimeLimit = time;
        FadeTimer.TimerExpireCallback = OnTwoWayFadeOutFinished;
        FadeTimer.Reset();
        FadeTimer.Start();
        Blackout.color = Color.clear;
    }

    private Color GetFadeColor()
    {
        if (isFadingIn)
        {
            return Color.Lerp(_fadeColor, Color.clear, FadeTimer.Ratio);
        }
        else
        {
            return Color.Lerp(Color.clear, _fadeColor, FadeTimer.Ratio);
        }
    }


    public void SetDisplaysEnabled(bool enabled)
    {
        foreach (var i in TextItems)
        {
            i.enabled = enabled;
        }
        foreach (var i in ImageItems)
        {
            i.enabled = enabled;
        }
    }

    Hero _hero = null;

    GUIText HealthDisplay;
    GUIText AmmoDisplay;

    GUIText ReloadPrompt;

    GUITexture Gun1Display;
    GUITexture Gun2Display;
    GUITexture Gun3Display;
    GUITexture Gun4Display;

    GUITexture Blackout;

    GUITexture[] GunDisplays = new GUITexture[4];

    ActionTimer Flash;
    bool isFlashOn = false;
	// Use this for initialization

    public GUIText[] TextItems { get; private set; }
    public GUITexture[] ImageItems { get; private set; }
	void Start () {

        TextItems  = this.GetComponentsInChildren<GUIText>(true);
        ImageItems = this.GetComponentsInChildren<GUITexture>(true);

        foreach (var i in TextItems)
        {
            if (i.name.Equals("PlayerHealthDisplay", System.StringComparison.CurrentCultureIgnoreCase))
            {
                HealthDisplay = i;
            }
            if (i.name.Equals("AmmoDisplay", System.StringComparison.CurrentCultureIgnoreCase))
            {
                AmmoDisplay = i;
            }
            if (i.name.Equals("ReloadPrompt", System.StringComparison.CurrentCultureIgnoreCase))
            {
                ReloadPrompt = i;
            }
        }

        foreach (var i in ImageItems)
        {
            if (i.name.Equals("Gun_1", System.StringComparison.CurrentCultureIgnoreCase))
            {
                Gun1Display = i;
            }
            if (i.name.Equals("Gun_2", System.StringComparison.CurrentCultureIgnoreCase))
            {
                Gun2Display = i;
            }
            if (i.name.Equals("Gun_3", System.StringComparison.CurrentCultureIgnoreCase))
            {
                Gun3Display = i;
            }
            if (i.name.Equals("Gun_4", System.StringComparison.CurrentCultureIgnoreCase))
            {
                Gun4Display = i;
            }
            if (i.name.Equals("ScreenBlackout", System.StringComparison.CurrentCultureIgnoreCase))
            {
                Blackout = i;
                Blackout.gameObject.SetActive(true);
            }
        }

        GunDisplays[0] = Gun1Display;
        GunDisplays[1] = Gun2Display;
        GunDisplays[2] = Gun3Display;
        GunDisplays[3] = Gun4Display;


        ReloadPrompt.text = "RE-LOAD! \n[ r ] key ";


        FadeTimer = new TimedInterpolation(FadeTime);
        StartFade(true);


        Flash = new ActionTimer(0.5f, delegate()
        {
            isFlashOn = !isFlashOn;
        });
        Flash.Loop = true;
        Flash.Start();
	}
	
	// Update is called once per frame
	void Update () {

        FadeTimer.Update();
        Flash.Update();

        Blackout.enabled = FadeTimer.Enabled;
        if (FadeTimer.Enabled)
        {
            Blackout.color = GetFadeColor();
        }

        if (_hero == null)
        {
            _hero = PlayerUtility.Hero;
        }

        if (HealthDisplay != null)
        {
            HealthDisplay.text = string.Format("health    {0:0}", _hero.Health);
        }

        if (AmmoDisplay != null && _hero.GunController != null)
        {
            ReloadPrompt.enabled = _hero.GunController.CurrentClip == 0;
            AmmoDisplay.text = string.Format("{0} / {1}", _hero.GunController.CurrentClip, _hero.GunController.CurrentAmmo);

            int currentGun = _hero.GunController.CurrentGunIndex;

            for (int i = 0; i < GunDisplays.Length; i++)
            {
                GunDisplays[i].gameObject.SetActive((currentGun == i));
            }
        }

        if (Input.GetKey(KeyCode.B))
        {
            StartFadeOutThenFadeIn(Color.red, Color.black, 1, 1, null, delegate() { PlayerUtility.Hero.KillPlayer(); });
        }

        ReloadPrompt.color = isFlashOn ? Color.red : Color.white;
	}
}
