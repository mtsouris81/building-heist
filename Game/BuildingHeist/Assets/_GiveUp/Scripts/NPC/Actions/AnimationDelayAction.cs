using System;
namespace GiveUp.Core
{
    public class AnimationDelayAction : ActionBase
    {
        public string AnimationName { get; set; }
        public float Duration { get { return this.Timer.TimeLimit; } set { this.Timer.TimeLimit = value; } }

        NpcCore _npc;

        public AnimationDelayAction(NpcCore npc, string animationName, float actionTotalTime)
        {
            this.AnimationName = animationName;
            this._npc = npc;
            this.Timer = new ActionTimer(actionTotalTime, EndAction);
        }

        public override void Update()
        {
            if (IsActionHappening)
            {
                Timer.Update();
            }
        }

        public override void StartAction()
        {
            base.StartAction();

            if (IsActionHappening)
            {
                this.Timer.Reset();
            }

            this.Timer.Start();
            _npc.AnimationManager.PlayAnimation(this.AnimationName);
            
        }

        public virtual void HandleAnimationEvent()
        {

        }
    }
}

