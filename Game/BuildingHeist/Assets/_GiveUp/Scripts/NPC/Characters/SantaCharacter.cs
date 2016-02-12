using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using GiveUp.Core;

namespace GiveUp.Core
{
    public class SantaCharacter : NpcBossCore
    {

        public Transform BigOrbProjectile;
        public Transform SmallOrbProjectile;
        public Transform YellowOrbProjectile;

        NpcProjectileShooter bigOrbShooter;
        NpcProjectileShooter smallOrbShooter;
        NpcProjectileShooter yellowOrbShooter;

        AnimationDelayAction BigOrbAttackAction;
        AnimationDelayAction SmallOrbAttackAction;
        AnimationDelayAction YellowOrbAttackAction;

        // animation callback
        public override void Attack1()
        {
            bigOrbShooter.Shoot();
        }

        // animation callback
        public override void Attack2()
        {

            smallOrbShooter.Shoot();
        }

        // animation callback
        public override void Attack3()
        {

            yellowOrbShooter.Shoot();
        }



        public override void Start()
        {
            base.Start();

            BigOrbAttackAction = new AnimationDelayAction(this, "Attacking", 3.5f);
            this.SubscribeToActionEvents(BigOrbAttackAction);

            SmallOrbAttackAction = new AnimationDelayAction(this, "Attacking2", 3.5f);
            this.SubscribeToActionEvents(SmallOrbAttackAction);

            YellowOrbAttackAction = new AnimationDelayAction(this, "Attacking3", 3.5f);
            this.SubscribeToActionEvents(YellowOrbAttackAction);
            
            bigOrbShooter = BigOrbProjectile.GetComponent<NpcProjectileShooter>();
            smallOrbShooter = BigOrbProjectile.GetComponent<NpcProjectileShooter>();
            yellowOrbShooter = BigOrbProjectile.GetComponent<NpcProjectileShooter>();

        }

        protected override void OnActionStarted(ActionBase action)
        {
            if (Actions.Hurt == action)
            {
                AnimationManager.PlayAnimation("Hurt");
            }

            if (Actions.Die == action)
            {
                Actions.Shoot.Shooter.TurnOff();
                this.MovementManager.SetEnabled(false);
                AnimationManager.PlayAnimation("Die");
                Actions.EndAllActions(Actions.Die);
            }

            if (Actions.CloseCombat == action)
            {
                AnimationManager.PlayAnimation("Attacking");
            }
        }
        protected override void OnActionEnded(ActionBase action)
        {

            if (Actions.Die == action)
            {
                HealthManager.SetDead();
                this.gameObject.SetActiveRecursively(false);
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

        public override ActionBase ResolveNextDecision()
        {
            if (!this.HealthManager.IsAlive())
                return null;

            actionNumber++;
            if (actionNumber > 2) { actionNumber = 0; }

            switch (actionNumber)
            {
                case 0:
                    return BigOrbAttackAction;
                case 1:
                    return SmallOrbAttackAction;
                case 2:
                    return YellowOrbAttackAction;
                default:
                    return null;
            }
        }

    }
}