using UnityEngine;
using System.Collections;
using GiveUp.Core;

namespace GiveUp.Character
{
    public class CharacterGun : MonoBehaviour
    {
        bool hasInitialized = false;
        public enum State
        {
            Idle,
            Shooting, 
            LoadIn,
            LoadOut
        }

        public NpcHurtType HurtType = NpcHurtType.None;

        CharacterProjectileShooter ProjectileShooter;

        public State CurrentState { get; private set; }

        ActionTimer GunShotReadyTimer;
        ActionTimer RecoilTimer;
        ActionTimer LoadInTimer;
        bool isGunReadyForNextShot = true;

        public string GunName;
        public float RecoilAmount = 0.1f;
        public float RecoilRotation = 2;
        public float RecoilTime = 0.2f;

        public float LoadTime = 0.05f;
        public float LoadInDistance = 0.3f;
        public float LoadOutDistance = 0.3f;

        public float ReloadTime = 2;
        public float ReloadDistance = 0.2f;

        public int MaxAmmo = 44;
        public int ClipSize = 8;

        public float GunOutOffset = 2;
        public float TimeBetweenShots = 0;

        public int Clip { get; private set; }
        public int Ammo { get; private set; }


        Vector3 gunOutPosition;
        Vector3 nativeRotation;
        Vector3 LoadInPosition;
        Vector3 LoadOutPosition;
        Vector3 RecoilPosition;
        Vector3 NaturalPosition;

        public ActionTimer.TimerExpireDelegate GunDisabledCallback;

        public void ReloadClip()
        {
            int clipRequest = ClipSize - Clip;

            if (clipRequest == 0)
                return;

            if (Ammo >= clipRequest)
            {
                Clip = ClipSize;
                Ammo -= clipRequest;
            }
            else
            {
                Clip += Ammo;
                Ammo = 0;
            }
        }

        public void ReloadAmmo()
        {
            this.Ammo = MaxAmmo;

            ReloadClip();

            this.Ammo = MaxAmmo;
        }

        public void DisableGun()
        {
            this.transform.localPosition = LoadInPosition;
            this.gameObject.SetActive(false);
        }
        public void EnableGun()
        {
            this.gameObject.SetActive(true);
        }
        public void LoadOutGun()
        {
            CurrentState = State.LoadOut;
            LoadInTimer.Reset();
            LoadInTimer.Start();
        }
        public void LoadInGun()
        {
            this.gameObject.SetActive(true);

            Initialize();
            CurrentState = State.LoadIn;
            LoadInTimer.Reset();
            LoadInTimer.Start();
        }
        // Use this for initialization
        void Start()
        {
            Initialize();

            Ammo = MaxAmmo;
            Clip = ClipSize;
        }

        private void Initialize()
        {
            if (hasInitialized)
                return;

            if (LoadInTimer == null)
            {
                nativeRotation = this.transform.localRotation.eulerAngles;

                ProjectileShooter = this.GetComponentInChildren<CharacterProjectileShooter>();

                LoadInTimer = new ActionTimer(LoadTime, delegate()
                    {
                        this.transform.localPosition = CurrentState == State.LoadOut
                                                       ? LoadOutPosition
                                                       : NaturalPosition;
                    });
                LoadInTimer.TimerExpireCallback = LoadInFinished;

                NaturalPosition = this.transform.localPosition;
                LoadInPosition = new Vector3(this.transform.localPosition.x, this.transform.localPosition.y - LoadInDistance, this.transform.localPosition.z);
                LoadOutPosition = new Vector3(this.transform.localPosition.x + LoadInDistance, this.transform.localPosition.y - LoadInDistance, this.transform.localPosition.z);

                RecoilTimer = new ActionTimer(RecoilTime, RecoilEnded);
                RecoilPosition = new Vector3(this.transform.localPosition.x, this.transform.localPosition.y, this.transform.localPosition.z - RecoilAmount);


                GunShotReadyTimer = new ActionTimer(TimeBetweenShots, MakeGunReady);

                CurrentState = State.Idle;


                DisableGun();
                hasInitialized = true;

            }
        }

        public void MakeGunReady()
        {
            isGunReadyForNextShot = true;
        }

        public void RecoilEnded()
        {
            if (TimeBetweenShots == 0)
            {
                isGunReadyForNextShot = true;
                return;
            }

            GunShotReadyTimer.Reset();
            GunShotReadyTimer.Start();
        }

        public bool IsTransitioning
        {
            get
            {
                return this.LoadInTimer.Enabled;
            }
        }

        // Update is called once per frame
        void Update()
        {
            RecoilTimer.Update();
            LoadInTimer.Update();
            GunShotReadyTimer.Update();

            if (GunShotReadyTimer.Enabled)
            {
                gunOutPosition = new Vector3(NaturalPosition.x, NaturalPosition.y - GunOutOffset, NaturalPosition.z);
                this.transform.localPosition = Vector3.Lerp(gunOutPosition, NaturalPosition, GunShotReadyTimer.Ratio);
            }

            if (RecoilTimer.Enabled)
            {
                RecoilPosition = new Vector3(this.transform.localPosition.x, this.transform.localPosition.y, this.transform.localPosition.z - RecoilAmount);
                gunOutPosition = TimeBetweenShots > 0 ? new Vector3(NaturalPosition.x, NaturalPosition.y - GunOutOffset, NaturalPosition.z) : NaturalPosition;
                this.transform.localPosition = Vector3.Lerp(RecoilPosition, gunOutPosition, RecoilTimer.Ratio);
                this.transform.localRotation = Quaternion.Euler(new Vector3(Mathf.Lerp(RecoilRotation + nativeRotation.x, nativeRotation.x, RecoilTimer.Ratio), nativeRotation.y, nativeRotation.z));
            }

            if (LoadInTimer.Enabled)
            {
                if (CurrentState == State.LoadOut)
                {
                    this.transform.localPosition = Vector3.Lerp(NaturalPosition, LoadOutPosition, LoadInTimer.Ratio);
                }
                else
                {
                    this.transform.localPosition = Vector3.Lerp(LoadInPosition, NaturalPosition, LoadInTimer.Ratio);
                }
            }
        }



        public void LoadInFinished()
        {
            if (this.CurrentState == State.LoadOut)
            {
                if (GunDisabledCallback != null)
                    GunDisabledCallback();
            }
        }

        public void Shoot()
        {
            Shoot(null);
        }
        public void Shoot(Ray? ray)
        {
            if (Clip < 1 || !isGunReadyForNextShot)
                return;

            isGunReadyForNextShot = SupportsAutoFire;

            Clip--;
            ProjectileShooter.Shoot(ray);

            RecoilTimer.Reset();
            RecoilTimer.Start();
        }

        public bool SupportsAutoFire = false;

        public float AutoFireInterval = 0.1f;
    }
}