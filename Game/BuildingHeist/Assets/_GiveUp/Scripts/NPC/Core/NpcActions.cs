using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GiveUp.Core;

namespace GiveUp.Core
{
    public class NpcActions
    {


        ActionTimer NewDecisionTimer;
        NpcCore owner;
        public List<ActionBase> Actions = new List<ActionBase>();
        public HurtAction Hurt { get; set; }
        public DieAction Die { get; set; }
        public ShootAction Shoot { get; set; }
        public CloseCombatAction CloseCombat { get; set; }
        public bool IsEndingAllActions { get; private set; }
        public delegate ActionBase ResolveDecisionDelegate();

        public float DecisionInterval = 1.89f;

        public ResolveDecisionDelegate ResolveDecisionMethod { get; set; }


        public float DeathDuration = 5;

        public NpcActions(NpcCore npc)
        {
            owner = npc;
        }
        public void Initialize()
        {
            Hurt = new HurtAction(owner, owner.MovementManager, owner.HurtTime, owner.HurtAffectsSpeed ? 0 : owner.MoveSpeed);
            Die = new DieAction(DeathDuration);
            Shoot = new ShootAction(owner);
            CloseCombat = new CloseCombatAction(owner, 2.2f);
            NewDecisionTimer = new ActionTimer(DecisionInterval, ResolveNewDecision);
            NewDecisionTimer.Loop = true;
            NewDecisionTimer.Start();
            IsEndingAllActions = false;
        }

        public ActionBase IgnoreAction { get; set; }

        public void ResolveNewDecision()
        {
            if (AreAnyActionsHappening())
                return;

            if (ResolveDecisionMethod != null)
            {
                ActionBase action = ResolveDecisionMethod();
                
                if (action != null)
                {
                    action.StartAction();
                }
            }
        }

        public void UpdateWaitingForNewDecision()
        {
            NewDecisionTimer.Update();
        }

        public bool AreAnyActionsHappening()
        {
            foreach (ActionBase a in Actions)
            {
                if (a.IsActionHappening && a != IgnoreAction)
                    return true;
            }

            return false;
        }

        public void EndAllActions()
        {
            EndAllActions(null);
        }
        public void EndAllActions(ActionBase except)
        {
            IsEndingAllActions = true;
            foreach (var a in Actions)
            {
                if (except != null && a == except)
                {
                    continue;
                }

                a.EndAction();
            }
            IsEndingAllActions = false;
        }
        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        public void Update()
        {
            foreach (ActionBase a in Actions)
            {
                a.Update();
            }
        }
    }
}