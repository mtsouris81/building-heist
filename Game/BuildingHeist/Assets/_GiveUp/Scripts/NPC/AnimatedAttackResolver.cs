using UnityEngine;
using System.Collections;

using GiveUp.Core;

public class AnimatedAttackResolver : NpcDecisionResolver
{

    public enum AnimatedAttackType
    {
        Shoot,
        CloseCombat,
        Custom
    }
    public string AnimationName = null;
    public string RestoreAnimationName = null;
    public float ActionDuration = 3;
    public AnimatedAttackType Type = AnimatedAttackType.Shoot;
    AnimationDelayAction ThrowAttackAction;
    NpcProjectileShooter shooter;
    NpcCore _npc;
    public override void InjectActions(NpcCore npc)
    {
        _npc = npc;
        shooter = this.GetComponentInChildren<NpcProjectileShooter>();
        ThrowAttackAction = new AnimationDelayAction(npc, AnimationName, ActionDuration);
        ThrowAttackAction.ActionEnding += new System.EventHandler(ThrowAttackAction_ActionEnding);
        npc.SubscribeToActionEvents(ThrowAttackAction);
    }

    void ThrowAttackAction_ActionEnding(object sender, System.EventArgs e)
    {
        _npc.AnimationManager.PlayAnimation(RestoreAnimationName);
    }

    public override ActionBase ResolveNextAction()
    {
        return ThrowAttackAction;
    }

    public void AnimationAttackHandler()
    {
        switch (this.Type)
        {
            case AnimatedAttackType.Shoot:
                shooter.Shoot();
                break;
            case AnimatedAttackType.CloseCombat:
                //_npc.Actions.CloseCombat.CloseCombatAttackObject.StartAction();
                break;
            case AnimatedAttackType.Custom:
                break;
            default:
                break;
        }
        
    }


}
