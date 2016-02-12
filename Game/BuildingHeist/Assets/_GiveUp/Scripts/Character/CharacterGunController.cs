using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GiveUp.Character;
using System;
using GiveUp.Core;

public class CharacterGunController : MonoBehaviour {




    string MouseScrollWheelAxisName = "Mouse ScrollWheel";
    int initWait = 0;
    int initMax = 5;
    Dictionary<string, CharacterGun> GunLookup = new Dictionary<string,CharacterGun>();
    List<CharacterGun> Guns;
    Camera cam;
    CharacterGun _nextGun;

    public bool AutoReload = false;
    public AudioSource GunSwitchSound = null;
    public CharacterGun CurrentGun { get; private set; }

    public bool AllowUserGunSwitching = true;
    object _mouseLook = null;
    public int CurrentAmmo
    {
        get
        {
            if (CurrentGun == null)
                return 0;

            return CurrentGun.Ammo;
        }
    }
    public int CurrentClip
    {
        get
        {
            if (CurrentGun == null)
                return 0;

            return CurrentGun.Clip;
        }
    }
    public int CurrentGunIndex
    {
        get
        {
            if (CurrentGun == null)
                return 0;

            return Guns.IndexOf(CurrentGun);
        }
    }

    private void OnGunLoadedOut()
    {
        if (_nextGun != null)
        {
            CurrentGun = _nextGun;
            _nextGun = null;
            CurrentGun.LoadInGun();
        }
    }

	void Start () {
        autoFireTimer = new ActionTimer(10, Shoot);
        initWait = 0;
        cam = this.GetComponentInChildren<Camera>();
        Guns = new List<CharacterGun>( cam.gameObject.GetComponentsInChildren<CharacterGun>(true));
        for (int i = 0; i < Guns.Count; i++)
        {
            Guns[i].GunDisabledCallback = OnGunLoadedOut;
            GunLookup.Add(Guns[i].GunName, Guns[i]);
        }
      //  _mouseLook = this.gameObject.GetComponent<WeenusSoft.MouseLook>();
	}



    public CharacterGun Find(NpcHurtType type)
    {
        foreach (var g in Guns)
        {
            if (g.HurtType == type)
                return g;
        }
        throw new Exception("Could not find gun! " + type.ToString());
    }
    public void SetGun(NpcHurtType type)
    {
        CharacterGun gun = Find(type);

        if (CurrentGun == gun)
            return;

        if (GunSwitchSound != null)
        {
            GunSwitchSound.Play();
        }

        if (CurrentGun == null)
        {
            CurrentGun = gun;
            CurrentGun.LoadInGun();
        }
        else
        {
            CurrentGun.LoadOutGun();
            _nextGun = gun;
        }
    }

    public void EnableGun(int index)
    {
        if (CurrentGun == null)
        {
            CurrentGun = Guns[index];
            CurrentGun.LoadInGun();
        }
        else
        {
            CurrentGun.LoadOutGun();
            _nextGun = Guns[index];
        }
    }

    public void NextGun()
    {
        int newGunVal = CurrentGunIndex + 1;

        if (newGunVal < 0)
        {
            newGunVal = Guns.Count - 1;
        }

        if (newGunVal >= Guns.Count)
        {
            newGunVal = 0;
        }

        EnableGun(newGunVal);
    }
    public void PreviousGun()
    {
        int newGunVal = CurrentGunIndex - 1;

        if (newGunVal < 0)
        {
            newGunVal = Guns.Count - 1;
        }

        if (newGunVal >= Guns.Count)
        {
            newGunVal = 0;
        }

        EnableGun(newGunVal);
    }


    bool nextGunTransitioning;
    bool gunTransitioning;
    bool gunReady;


	void Update () 
    {
        CheckForInitialInit();

        if (LevelContext.Current.IsPaused)
        {
            return;
        }

        if (AllowUserGunSwitching)
        {
            float scrollWheelDelta = Input.GetAxis(MouseScrollWheelAxisName);
            if (scrollWheelDelta != 0)
            {
                if (scrollWheelDelta > 0)
                {
                    NextGun();
                }
                else
                {
                    PreviousGun();
                }
            }
        }

        //if (_mouseLook != null && _mouseLook.SuspendMouse)
        //{
        //    return;
        //}


        if (AllowUserGunSwitching)
        {
            if (Input.GetButton("NextGun"))
            {
                NextGun();
            }
            if (Input.GetButton("PreviousGun"))
            {
                NextGun();
            }
        }

        autoFireTimer.Update();
        if (IsGunReady())
        {
            if (this.CurrentGun.SupportsAutoFire)
            {
                UpdateAutoFire(Input.GetButton("Fire1"));
            }
            else
            {
                autoFireTimer.Stop();
                if (Input.GetButtonDown("Fire1"))
                {
                    Shoot();
                }
            }
        }
        else
        {
            if (autoFireTimer.Enabled) // gun is not ready but autofire still going
            {
                autoFireTimer.Stop();
            }

        }

        if (!AutoReload)
        {
            if (Input.GetButtonDown("Reload"))
            {
                if (CurrentGun != null)
                {
                    this.CurrentGun.ReloadClip();
                }
            }
        }
	}

    public bool IsGunReady()
    {
        if (CurrentGun == null)
            return false;

        if (this.CurrentGun.IsTransitioning)
            return false;
        
        if (_nextGun != null && _nextGun.IsTransitioning)
            return false;

        return true;
    }
    ActionTimer autoFireTimer;
   
    public void UpdateAutoFire(bool shouldBeShooting)
    {
        autoFireTimer.TimeLimit = this.CurrentGun.AutoFireInterval;

        if (shouldBeShooting && !autoFireTimer.Enabled)
        {
            autoFireTimer.Reset();
            autoFireTimer.Loop = true;
            autoFireTimer.Start();
            Shoot(); // first shot
        }

        if (!shouldBeShooting && autoFireTimer.Enabled)
        {
            autoFireTimer.Stop();
        }
    }

    public void Shoot()
    {
        if (IsGunReady())
        {
            this.CurrentGun.Shoot();

            if (AutoReload)
            {
                ReloadAllGunAmmo();
            }
        }
    }
    public void ShootFromPoint(Vector3 screenPosition)
    {
        if (IsGunReady())
        {
            Ray ray = Camera.main.ScreenPointToRay(screenPosition);
            this.CurrentGun.Shoot(ray);
            if (AutoReload)
            {
                ReloadAllGunAmmo();
            }
        }
    }

    public void ReloadAllGunAmmo()
    {
        foreach (var g in Guns)
        {
            g.ReloadAmmo();
        }
    }

    private void CheckForInitialInit()
    {
        if (initWait < initMax)
        {
            initWait++;
            if (initWait >= initMax)
            {
                initWait += initMax;
                EnableGun(0);
            }
        }
    }
}
