using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using GiveUp.Core;

namespace GiveUp.Core
{
    public class BossCharacter : NpcBossCore
    {
        public Transform ThrowProjectile;
        public Transform ShootProjectile;

        NpcProjectileShooter throwShooter;
        NpcProjectileShooter bulletShooter;

        AnimationDelayAction ThrowAttackAction;
        AnimationDelayAction ShootAttackAction;

        // animation callback
        public override void Attack1()
        {
            bulletShooter.Shoot();
        }
        public override void Attack2()
        {
            throwShooter.Shoot();
        }

        public override void Start()
        {
            base.Start();

            ThrowAttackAction = new AnimationDelayAction(this, "Attacking2", 3.5f);
            ShootAttackAction = new AnimationDelayAction(this, "Attacking", 3.5f);

            this.SubscribeToActionEvents(ThrowAttackAction);
            this.SubscribeToActionEvents(ShootAttackAction);

            throwShooter = ThrowProjectile.GetComponent<NpcProjectileShooter>();
            bulletShooter = ShootProjectile.GetComponent<NpcProjectileShooter>();

            this.Actions.Shoot.DisabledAction = true;
            this.Actions.CloseCombat.DisabledAction = true;
        }

        protected override void OnActionStarted(ActionBase action)
        {
            if (Actions.Hurt == action)
            {
                AnimationManager.PlayAnimation("Hurt");
            }

            if (Actions.Die == action)
            {
                if (Actions.Shoot != null && Actions.Shoot.Shooter != null)
                {
                    Actions.Shoot.Shooter.TurnOff();
                }
                this.MovementManager.SetEnabled(false);
                AnimationManager.PlayAnimation("Die");
                Actions.EndAllActions(Actions.Die);
            }

            if ( Actions.CloseCombat == action)
            {
                AnimationManager.PlayAnimation("Attacking");
            }
        }
        protected override void OnActionEnded(ActionBase action)
        {

            if (Actions.Die == action)
            {
                HealthManager.SetDead();
                this.gameObject.SetActive(false);
                Expire(false);
                return;
            }

            if (!HealthManager.IsAlive())
            {
                return;
            }

            if (Actions.Hurt == action)
            {
                RestoreSpeed();
                if (HealthManager.IsAlive())
                {
                    AnimationManager.PlayAnimation("Walking");
                }
                return;
            }

            if (Actions.Shoot == action || Actions.CloseCombat == action)
            {
                RestoreSpeed();
                AnimationManager.PlayAnimation("Walking");
                return;
            }

            AnimationManager.PlayAnimation("Walking");
        }
        public override void HurtNPC(float amount)
        {
            if (!HealthManager.IsAlive() || Actions.Die.IsActionHappening)
                return;

            HealthManager.Hurt(amount);
        }
        public override void Update()
        {
            RegisterSelf();

            if (HealthManager.HasDied)
                return;

            Actions.Update();

            if (!HealthManager.IsAlive())
                return;

            if (Actions.Hurt.IsActionHappening || Actions.Die.IsActionHappening)
            {
                return;
            }

            Vector3? target = MovementManager.GetTargetMethod(this);

            if (target.HasValue)
            {
                MovementManager.SetDestination(target.Value);
            }

            if (!Actions.AreAnyActionsHappening())
            {
                Actions.UpdateWaitingForNewDecision();
            }

        }

        int actionNumber = 0;




        [RangeAttribute(0.00001f, 1)]
        public float ShootProbability = 0.2f;
        private bool IsShooting()
        {
            float chance = UnityEngine.Random.Range(0f, 100f);
            return chance < ShootProbability;
        }
        public override ActionBase ResolveNextDecision()
        {
            if (!this.HealthManager.IsAlive())
                return null;

            if (IsShooting())
            {
                return ShootAttackAction;
            }
            else
            {
                return ThrowAttackAction;
            }
        }

    }
}