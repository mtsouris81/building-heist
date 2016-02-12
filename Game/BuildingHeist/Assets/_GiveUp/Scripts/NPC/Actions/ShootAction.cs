using System;
using UnityEngine;
namespace GiveUp.Core
{
    public class ShootAction : ActionBase
    {
        public bool DisabledAction { get; set; }
        int currentShot = 0;
        int TotalShots;
        NpcCore owner;

        public NpcProjectileShooter Shooter { get; set; }
        public event EventHandler ShotFired;

        public ShootAction(NpcCore owner)
        {
            if (owner == null || owner.ShootAttackObject == null)
            {
                IsReady = false;
                return;
            }

            this.owner = owner;

            Shooter = owner.ShootAttackObject.GetComponent<NpcProjectileShooter>();


            TotalShots = Shooter.TotalShotsPerAttack;
            Timer = new ActionTimer(Shooter.AutoFireInterval, ShootInterval);
            Timer.Loop = true;
            Timer.Start();
        }

        protected void ShootInterval()
        {
            FireShot();

            if (currentShot > TotalShots)
            {
                EndAction();
            }
        }

        protected virtual void FireShot()
        {
            if (DisabledAction)
                return;

            if (ShotFired != null)
            {
                ShotFired(this, EventArgs.Empty);
            }
            currentShot++;
            Shooter.Shoot();
        }

        public override void StartAction()
        {
            if (DisabledAction)
                return;

            if (!IsReady)
                return;

            currentShot = 0;
            Timer.Start();
            ShootInterval();
            base.StartAction();
        }

        public override void EndAction()
        {
            if (IsReady)
            {
                if (Shooter != null)
                    Shooter.TurnOff();
            }
            base.EndAction();
        }

        public override void Update()
        {
            if (DisabledAction)
                return;

            if (!IsReady)
                return;

            if (IsActionHappening)
            {
                owner.MovementManager.SetSpeed(Shooter.MovementSpeedWhileShooting);
                Timer.Update();
            }
        }
    }
}

