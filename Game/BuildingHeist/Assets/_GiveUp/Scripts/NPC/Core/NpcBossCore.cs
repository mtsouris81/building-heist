using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using GiveUp.Core;

namespace GiveUp.Core
{
    [AddComponentMenu("GIVEUP NPC CONTROLLER")]
    public class NpcBossCore : NpcCore
    {

        // animation callback
        public virtual void Attack1()
        {
        }

        // animation callback
        public virtual void Attack2()
        {
        }

        // animation callback
        public virtual void Attack3()
        {
        }

        // animation callback
        public override void Start()
        {
            base.Start();
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
                this.gameObject.SetActive(false);
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
            }

            if (Actions.Shoot == action || Actions.CloseCombat == action)
            {
                RestoreSpeed();
                AnimationManager.PlayAnimation("Walking");
            }
        }

        public override void HurtNPC(float amount)
        {
            if (!HealthManager.IsAlive() || Actions.Die.IsActionHappening)
                return;

            HealthManager.Hurt(amount);
        }

        public override void Update()
        {
            if (HealthManager.HasDied)
                return;

            Actions.Update();

            if (Actions.Hurt.IsActionHappening || Actions.Die.IsActionHappening)
            {
                return;
            }

            if (!HealthManager.IsAlive())
                return;

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

        public override ActionBase ResolveNextDecision()
        {
            if (this.HealthManager.IsAlive())
            {
                if (MovementManager.RemainingDistance < 8)
                {
                    return Actions.CloseCombat;
                }

                return null;// Actions.Shoot;
            }
            else
            {
                return null;
            }
        }

    }
}