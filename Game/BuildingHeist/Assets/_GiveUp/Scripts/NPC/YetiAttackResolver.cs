using UnityEngine;
using System.Collections;
using GiveUp.Core;
using System;
using System.Linq;
using System.Collections.Generic;

public class YetiAttackResolver : NpcDecisionResolver
{
    public BossAttackInfo[] Attacks = null;
    public string RestoreAnimationName = null;
    
    BossAttackInfo _currentAttack = null;
    AnimationDelayAction ThrowAttackAction;
    NpcCore _npc;
    float _normalSpeed = 1;
    float distance;
    ActionTimer autoFireTimer;


    List<BossAttackInfo> attacksList = new List<BossAttackInfo>();

    public override void InjectActions(NpcCore npc)
    {
        _npc = npc;
        attacksList = new List<BossAttackInfo>(this.Attacks);
        _normalSpeed = _npc.MovementManager.Speed;
        ThrowAttackAction = new AnimationDelayAction(npc, Attacks[0].AnimationName, Attacks[0].Duration);
        ThrowAttackAction.ActionEnding += new System.EventHandler(ThrowAttackAction_ActionEnding);
        ThrowAttackAction.ActionStarting += new EventHandler(ThrowAttackAction_ActionStarting);
        npc.SubscribeToActionEvents(ThrowAttackAction);
        autoFireTimer = new ActionTimer(100, ActualAttack);
        autoFireTimer.Stop();
    }

    void ThrowAttackAction_ActionStarting(object sender, EventArgs e)
    {

    }

    void ThrowAttackAction_ActionEnding(object sender, System.EventArgs e)
    {
        _npc.AnimationManager.PlayAnimation(RestoreAnimationName);
        _npc.MovementManager.SetSpeed(_normalSpeed);
        autoFireTimer.Stop();
        _npc.Invincible = false;
    }

    public void Update()
    {
        autoFireTimer.Update();
    }

    List<BossAttackInfo> EligibleAttacks = new List<BossAttackInfo>();

    public override ActionBase ResolveNextAction()
    {
        distance = Vector3.Distance(_npc.transform.position, PlayerUtility.Current.transform.position);
        EligibleAttacks.Clear();

        foreach (var a in this.Attacks)
        {
            bool isGood = true;
            foreach (var condition in a.Conditions)
            {
                if (!CheckCondition(condition))
                {
                    isGood = false;
                    break;
                }
            }

            if (isGood)
            {
                EligibleAttacks.Add(a);
            }
        }

        if (EligibleAttacks.Count < 1) // force atleast one eligible attack
        {
            EligibleAttacks.Add(this.Attacks[0]);
        }

        int index = 0;

        if (EligibleAttacks.Count > 1)
        {
            // alternate between actions that share the same conditions
            index = SetAllAlternatingIndex(EligibleAttacks);
        }

        ThrowAttackAction.Duration = EligibleAttacks[index].Duration;
        ThrowAttackAction.AnimationName = EligibleAttacks[index].AnimationName;
        _currentAttack = EligibleAttacks[index];
        _npc.MovementManager.SetSpeed(EligibleAttacks[index].MoveSpeed * _normalSpeed);
        _npc.Invincible = _currentAttack.InvincibleDuringAttack;

        if (_currentAttack.InvincibleDuringAttack)
        {
            if (_npc.Actions.Hurt.IsActionHappening)
            {
                _npc.Actions.Hurt.EndAction();
            }
        }

        if (!EligibleAttacks[index].SoundOnShotOnly)
        {
            PlaySoundByIndex(index);
        }
        return ThrowAttackAction;
    }

    private void PlaySoundByIndex(int index)
    {
        if (_npc == null)
        {
            _npc = GetComponent<NpcCore>();
        }
        if (_npc.Sounds == null)
        {
            return;
        }

        CharacterSoundType t = GetSoundTypeForAttack(index);
        PlayAttackSound(t);

    }

    private void PlayAttackSound(CharacterSoundType t)
    {
        if (_npc.Sounds.Supports(t))
        {
            _npc.PlaySound(t);
        }
        else if (_npc.Sounds.Supports(CharacterSoundType.PrimaryAttack))
        {
            _npc.PlaySound(CharacterSoundType.PrimaryAttack);
        }
    }

    public CharacterSoundType GetSoundTypeForAttack(int index)
    {
        switch (index)
        {
            case 0:
                return (CharacterSoundType.PrimaryAttack);
            case 1:
                return (CharacterSoundType.SecondaryAttack);
            case 2:
                return (CharacterSoundType.TertiaryAttack);
            default:
                return CharacterSoundType.PrimaryAttack;
        }
    }

    private int SetAllAlternatingIndex(List<BossAttackInfo> eligibleAttacks)
    {
        if (eligibleAttacks == null || eligibleAttacks.Count < 1)
            return 0;

        eligibleAttacks[0].AlternationIndex++;
        if (eligibleAttacks[0].AlternationIndex >= eligibleAttacks.Count)
        {
            eligibleAttacks[0].AlternationIndex = 0;
        }

        int index = eligibleAttacks[0].AlternationIndex;
        foreach (var a in eligibleAttacks)
        {
            a.AlternationIndex = index;
        }
        return index;
    }

    private bool CheckCondition(BossAttackConditionInfo condition)
    {
        switch (condition.Type)
        {
            case BossAttackCondition.HealthRange:
                {
                    return _npc.HealthManager.Health > (condition.MinVal * _npc.MaxHealth) && _npc.HealthManager.Health < (condition.MaxVal * _npc.MaxHealth);
                }

            case BossAttackCondition.DistanceRange:
                {
                    return distance > condition.MinVal && distance < condition.MaxVal;
                }

            default: return true;
        }
    }

    public void AnimationAttackHandler()
    {
        ActualAttack();

        if (_currentAttack.AutoFire)
        {
            autoFireTimer.TimeLimit = _currentAttack.AutoFireInterval;
            autoFireTimer.Reset();
            autoFireTimer.Loop = true;
            autoFireTimer.Start();
        }

    }

    private void ActualAttack()
    {
        if (_currentAttack != null)
        {
            _currentAttack.Shooter.SendMessage("Shoot");
            if (_currentAttack.SoundOnShotOnly)
            {
                int index = attacksList.IndexOf(_currentAttack);
                CharacterSoundType t = GetSoundTypeForAttack(index);
                PlayAttackSound(t);
            }
        }
    }


    [Serializable]
    public class BossAttackInfo
    {
        public string Name = null;
        public string AnimationName = null;
        public bool SoundOnShotOnly = true;
        public float Duration = 3;
        public float MoveSpeed = 1;
        public Transform Shooter;
        public BossAttackConditionInfo[] Conditions = null;
        internal int AlternationIndex = 0;
        public bool AutoFire = false;
        public float AutoFireInterval = 0;
        public bool InvincibleDuringAttack = false;
    }

    public enum BossAttackCondition
    {
        None,
        HealthRange,
        DistanceRange,
    }

    [Serializable]
    public class BossAttackConditionInfo
    {
        public BossAttackCondition Type = BossAttackCondition.None;
        public float MinVal = 0;
        public float MaxVal = 0;
    }



}
