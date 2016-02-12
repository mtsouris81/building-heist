using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using GiveUp.Core;

namespace GiveUp.Core
{
    [AddComponentMenu("GIVEUP NPC CONTROLLER")]
    public class NpcCore : MonoBehaviour, IDifficultySettable
    {

        #region PROPERTIES / FIELDS

        public const string EXPLOSION_FX = "Explosion";

        // UNITY EDITOR PROPERTIES
        public EnemyType Type = EnemyType.WalkingShooter;
        public Renderer[] Renderers = null;
        public float MoveSpeed = 4;
        public float MaxHealth = 100;
        public float HurtTime = 0.7f;
        public string StartAnimationName = "Walking";
        public float MeleeDistance = 11;
        public Transform CloseCombatAttackObject;
        public Transform ShootAttackObject;
        public bool AnimationOnAttack = false;
        public bool PlayDeathAnimation = true;
        public bool DeathSupportsSpecialFX = true;
        public bool HurtAffectsSpeed = true;
        public float DeathDuration = 5;
        public CustomHurtOverride[] HurtOverrides = null;
        public bool ResolveDecisionsDuringHurt = false;




        public bool IsSuspended = false;

        // CODE PROPERTIES
        public bool LastEnabledState { get; set; }
        public bool HasRegistered { get; private set; }
        public float DefaultSpeed { get; private set; }
        public NpcActions Actions { get; set; }
        public NpcMovement MovementManager { get; set; }
        public NpcAnimation AnimationManager { get; set; }
        public NpcHealthManager HealthManager { get; set; }
        public CharacterSoundManager Sounds { get; set; }
        public NpcHealthDisplay HealthDisplay { get; set; }
        public bool IsFrozen { get; set; }
        public string LastAppliedEffect { get; set; }
        public NpcSpecialEffectManager SpecialEffects { get; private set; }
        public bool IsFalling { get; private set; }
        public HurtInfo LastHurtInfo { get; private set; }
        public bool Invincible { get; set; }
        public Func<ActionBase> DecisionResolverOverride { get; set; }

        // EVENTS
        public event EventHandler Expired;
        
        
        // PRIVATE

        private DifficultySetting diff_MaxHealth;
        private DifficultySetting diff_MoveSpeed;
        private DifficultySetting diff_HurtTime;


        private ActionBase possibleCloseCombat = null;
        private ActionBase possibleShoot = null;
        private Vector3 deadFallPosition;
        private Vector3 deadFallDirection;
        private float fallrate = 11;
        private Vector3 fallOrigin;
        private ActionTimer _hurtOverrideTimer;

        string _hurtEffect = null;
        #endregion

        #region LIFE TIME

        public NpcCore()
        {
            Actions = new NpcActions(this);
        }

        public virtual void Start()
        {

            diff_HurtTime = new DifficultySetting(this.HurtTime, 1.25f, 0.8f);
            diff_MaxHealth = new DifficultySetting(this.MaxHealth, 0.4f, 1.8f);
            diff_MoveSpeed = new DifficultySetting(this.MoveSpeed, 0.85f, 1.7f);

            HasRegistered = false;

            DefaultSpeed = MoveSpeed;
            
            AnimationManager = new NpcAnimation(this);
            AnimationManager.Initialize();
            MovementManager = new NpcMovement(this);
            MovementManager.Initialize(SupportsMovement());

            RestoreSpeed();

            HealthManager = new NpcHealthManager(this, MaxHealth);


            if (this.Type == EnemyType.WalkingMelee)
            {
                Actions.DecisionInterval = 0.4f;
            }

            Actions.DeathDuration = DeathDuration;
            Actions.Initialize();
            Actions.ResolveDecisionMethod = this.ResolveNextDecision;

            SubscribeToActionEvents(Actions.Hurt);
            SubscribeToActionEvents(Actions.Shoot);
            SubscribeToActionEvents(Actions.Die);
            SubscribeToActionEvents(Actions.CloseCombat);

            if (!IsSuspended)
            {
                AnimationManager.PlayAnimation(StartAnimationName);
            }
            else
            {
                try
                {
                    AnimationManager.PlayAnimation("Inactive", UnityEngine.Random.Range(0f, 1f));
                }
                catch { }
            }

            possibleCloseCombat = Actions.CloseCombat.IsReady ? Actions.CloseCombat : null;
            possibleShoot = Actions.Shoot.IsReady ? Actions.Shoot : null;


            NpcDecisionResolver[] extendedDecisionResolvers = this.GetComponents<NpcDecisionResolver>();

            if (extendedDecisionResolvers != null && extendedDecisionResolvers.Length > 0)
            {
                foreach (var d in extendedDecisionResolvers)
                {
                    this.DecisionResolverOverride = d.ResolveNextAction;
                    d.InjectActions(this);
                }
            }

                if (possibleCloseCombat == null)
                {
                    possibleCloseCombat = possibleShoot;
                }
           


            if (possibleShoot == null)
            {
                possibleShoot = possibleCloseCombat;
            }

            if (Actions.CloseCombat != null)
            {
                Actions.CloseCombat.ActionStarting += PlayPrimaryAttackSound;
            }
            if (Actions.Shoot != null)
            {
                Actions.Shoot.ShotFired += PlayPrimaryAttackSound;
            }

            Sounds = this.GetComponentInChildren<CharacterSoundManager>();

            HealthDisplay = this.GetComponentInChildren<NpcHealthDisplay>();

            SpecialEffects = GetComponentInChildren<NpcSpecialEffectManager>();
            if (SpecialEffects != null)
            {
                SpecialEffects.RegisterSpecialEffects(this);
            }
            _hurtOverrideTimer = new ActionTimer(10, this.SpecialEffects.ClearAllEffects);

            //PlaySound(CharacterSoundType.Alive);

        }



        public virtual void Update()
        {
            RegisterSelf();

            if (HealthManager.HasDied || IsSuspended)
                return;

            if (LevelContext.Current.Cinematics.Instance.IsCinematicPlaying)
            {
                return;
            }

            // this.HealthManager.CheckForDeath();

            Actions.Update();

            _hurtOverrideTimer.Update();

            if (IsFrozen)
            {
                this.MovementManager.SetSpeed(0);
                return;
            }



            if (Actions.Hurt.IsActionHappening || Actions.Die.IsActionHappening)
            {
                if (IsFalling)
                {
                    this.transform.position += (deadFallDirection * fallrate * Time.deltaTime);
                    if (this.transform.position.y < deadFallPosition.y)
                    {
                        this.transform.position = deadFallPosition;
                        IsFalling = false;
                    }
                }
                if (ResolveDecisionsDuringHurt)
                {
                    UpdateDecisionResolver(Actions.Hurt);
                }
                return;
            }

            if (!HealthManager.IsAlive())
                return;

            if (SupportsMovement())
            {
                Vector3? target = MovementManager.GetTargetMethod(this);

                if (target.HasValue)
                {
                    MovementManager.SetDestination(target.Value);
                }
            }
            else
            {
                Quaternion rrr = Quaternion.LookRotation(Vector3.Normalize(PlayerUtility.Current.transform.position - this.transform.position));
                rrr = Quaternion.Euler(new Vector3(0, rrr.eulerAngles.y, 0));
                this.transform.rotation = Quaternion.Lerp(this.transform.rotation, rrr, 0.42f * Time.deltaTime);
            }

            UpdateDecisionResolver(null);

        }
        public void SuperExpire()
        {
            Expire(false);
            this.gameObject.SetActive(false);
        }
        public void Expire(bool fromSystem)
        {
            if (fromSystem)
            {
                DeRegisterSelf();
            }
            else
            {
                if (Expired != null)
                    Expired(this, EventArgs.Empty);

                DeRegisterSelf();
            }

            if (this.Renderers != null)
            {
                foreach (var r in Renderers)
                {
                    if (r != null)
                    {
                        r.enabled = false;
                    }
                }
            }


        }
        public void RegisterSelf()
        {
            if (HasRegistered)
                return;

            HasRegistered = true;
            //SetDifficulty(XmasGameOptions.Instance.Difficulty);

            this.LastEnabledState = this.gameObject.activeInHierarchy;


            if (!LevelContext.Current.NPCs.List.Contains(this))
            {
                LevelContext.Current.NPCs.List.Add(this);
            }
        }
        public void DeRegisterSelf()
        {
            if (LevelContext.Current.NPCs.List.Contains(this))
            {
                LevelContext.Current.NPCs.List.Remove(this);
            }
        }

        #endregion


        public void SubscribeToActionEvents(ActionBase action)
        {
            Actions.Actions.Add(action);
            action.ActionEnding += actionEndHandler;
            action.ActionStarting += actionStartHandler;
        }


        protected void PlayPrimaryAttackSound(object sender, EventArgs e)
        {
            if (Sounds != null)
                Sounds.Play(CharacterSoundType.PrimaryAttack);
        }



        public void RestoreSpeed()
        {
            MovementManager.SetSpeed(DefaultSpeed);
        }

        public string GetEffectByHurtType()
        {
            switch (this.LastHurtInfo.HurtType)
            {
                case NpcHurtType.Freeze:
                    return "Freeze";
                case NpcHurtType.Fire:
                    return "Burn";
                case NpcHurtType.Explode:
                    NpcExplosionExtender.ExplosionOrigin = this.LastHurtInfo.Position;
                    return EXPLOSION_FX;
                default:
                    return "ShotBleed";
            }
        }

        public void TriggerExplosionEffect()
        {
            NpcExplosionExtender.ExplosionOrigin = this.LastHurtInfo.Position;
            if (SpecialEffects.SupportsEffect(EXPLOSION_FX))
            {
                SpecialEffects.TriggerEffect(EXPLOSION_FX);
            }
        }


        bool deathWasHandled = false;
        protected virtual void OnActionStarted(ActionBase action)
        {
            if (Actions.Hurt == action)
            {
                AnimationManager.PlayAnimation("Hurt");
                _hurtEffect = GetEffectByHurtType();

                //Debug.Log(string.Format("Effect Request {0} - supported {1}", _hurtEffect, SpecialEffects.SupportsEffect(_hurtEffect)));
                if (SpecialEffects.SupportsEffect(_hurtEffect))
                {
                    SpecialEffects.TriggerEffect(_hurtEffect);
                    //Debug.Log(string.Format("{0} - {1}", this.LastHurtInfo.HurtType, _hurtEffect));
                }
            }

            if (Actions.Die == action)
            {
              //  NpcDeathHandler[] handlers = this.GetComponents<NpcDeathHandler>();
                //if (handlers != null && handlers.Length > 0)
                //{
                //    bool handledDeath = handlers[0].OnDied(this);
                //    bool shouldExpire = handlers[0].ExpireNPC;
                //    if (handledDeath)
                //    {
                //        deathWasHandled = true;
                //        Actions.EndAllActions();
                //        HealthManager.SetDead();
                //        if (shouldExpire)
                //        {
                //            Expire(false);
                //            this.gameObject.SetActive(false);
                //            Debug.Log("expired by handler");
                //        }
                //        return;
                //    }
                //}

                if (DeathSupportsSpecialFX && this.Type != EnemyType.Boss)
                {
                    if (this.Type == EnemyType.FloatingShooter)
                    {
                        Vector3 floorPos = FindFloorPosition();
                        SetFalling(floorPos);
                    }

                    if (this.LastAppliedEffect == "Burn")
                    {
                        SpecialEffects.TriggerEffect("Burnt");
                        return;
                    }

                    if (this.LastAppliedEffect == "Freeze")
                    {
                        NpcExplosionExtender.ExplosionOrigin = this.transform.position;
                        SpecialEffects.TriggerEffect("CubeSplosion");
                        return;
                    }
                }
                else
                {
                    this.SpecialEffects.ClearAllEffects();
                }


                this.MovementManager.SetEnabled(false);
                Actions.EndAllActions(Actions.Die);
                AnimationManager.PlayAnimation("Die");

                if (HealthDisplay != null)
                    HealthDisplay.SetHealthRatio(0, this.HealthManager.MaxHealth);

                if (this.GetComponent<Collider>() != null)
                {
                    this.GetComponent<Collider>().enabled = false;
                }
                if (this.GetComponent<Rigidbody>() != null)
                {
                    this.GetComponent<Rigidbody>().Sleep();
                }
            }

            if (Actions.CloseCombat == action && AnimationOnAttack)
            {
                AnimationManager.PlayAnimation("Attacking");
            }

            if (Actions.Shoot == action && AnimationOnAttack)
            {
                AnimationManager.PlayAnimation("Attacking");
            }
        }
        protected virtual void OnActionEnded(ActionBase action)
        {

            if (Actions.Die == action && !deathWasHandled)
            {
                HealthManager.SetDead();
                Expire(false);
                this.gameObject.SetActive(false);
                GameObject.Destroy(this.gameObject);
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
                    if (!this.Actions.IsEndingAllActions)
                    {
                        AnimationManager.PlayAnimation("Walking");
                    }
                }
            }

            if (Actions.Shoot == action || Actions.CloseCombat == action)
            {
                RestoreSpeed();
                if (!this.Actions.IsEndingAllActions)
                {
                    AnimationManager.PlayAnimation("Walking");
                }
            }
        }
        

        private void SetFalling(Vector3 floorPos)
        {
            IsFalling = true;
            deadFallPosition = floorPos;
            deadFallDirection = Vector3.Normalize( deadFallPosition - this.transform.position);
            if (float.IsNaN(deadFallDirection.x))
            {
                deadFallDirection = Vector3.zero;
            }
        }
        private bool IsGround(RaycastHit h)
        {
            return (h.collider.tag == "Untagged");
        }
        private Vector3 FindFloorPosition()
        {
            Vector3 result = this.transform.position;
            var hits = Physics.RaycastAll(this.transform.position, Vector3.down, 20);
            if (hits != null && hits.Length != 0)
            {
                foreach (var h in hits)
                {
                    if (h.collider != null)
                    {
                        if (IsGround(h))
                        {
                            return h.point;
                        }
                    }
                }
            }
            return result;
        }

        private void actionStartHandler(object sender, EventArgs e)
        {
            OnActionStarted(sender as ActionBase);
        }
        private void actionEndHandler(object sender, EventArgs e)
        {
            OnActionEnded(sender as ActionBase);
        }
        
        public virtual void HurtNPC(float amount)
        {
            if (!HealthManager.IsAlive() || Actions.Die.IsActionHappening)
                return;

            HealthManager.Hurt(amount);

            UpdateHealthDisplay();

            if (HealthManager.IsAlive())
            {
                PlaySound(CharacterSoundType.Hurt);
            }
            else
            {
                PlaySound(CharacterSoundType.Die);
            }
        }

        public void PlaySound(CharacterSoundType type)
        {
            if (Sounds != null && Sounds.Supports(type))
            {
                Sounds.Play(type);
            }
        }

        public event EventHandler OnNpcHurt;

        public void AcceptHurt(HurtInfo info)
        {
            if (this.Invincible)
            {
                return;
            }

            if (OnNpcHurt != null)
            {
                OnNpcHurt(this, EventArgs.Empty);
            }

            LastHurtInfo = info;

            if (HurtOverrides != null && HurtOverrides.Length > 0)
            {
                var hurtOverride = HurtOverrides.Where(x => x.Name == info.HurtType).FirstOrDefault();
                if (hurtOverride != null)
                {
                    ProcessHurtOverride(info, hurtOverride);
                    return;
                }
            }

            HurtNPC(info.Damage);
        }

        public void UpdateHealthDisplay()
        {
            if (HealthDisplay != null)
            {
                HealthDisplay.SetHealthRatio(this.HealthManager.Health, this.HealthManager.MaxHealth);
            }
        }
        
        private void ProcessHurtOverride(HurtInfo info, CustomHurtOverride hurtOverride)
        {
            switch (hurtOverride.behavior)
            {
                case CustomHurtOverrideBehavior.None:
                    HurtNPC(info.Damage);
                    break;
                case CustomHurtOverrideBehavior.HurtWithNoEffect:
                    info.HurtType = NpcHurtType.None;
                    float damage = (hurtOverride.numberValue != 0) ? hurtOverride.numberValue : info.Damage;
                    HurtNPC(damage);
                    break;
                case CustomHurtOverrideBehavior.NotHurt:
                    // do nothing
                    return;
                case CustomHurtOverrideBehavior.NotHurtAboveY:
                    float maxY = hurtOverride.numberValue + this.transform.position.y;
                    if (info.Position.y < maxY)
                    {
                        HurtNPC(info.Damage);
                    }
                    break;
                case CustomHurtOverrideBehavior.NotHurtGainHealth:
                    this.ChangeHealth(hurtOverride.numberValue);
                    break;
                case CustomHurtOverrideBehavior.InstantDeath:
                    HurtNPC(this.MaxHealth * 1000);
                    break;
                case CustomHurtOverrideBehavior.InstantDeathWithEffect:
                    HurtNPC(this.MaxHealth * 1000);
                    this.SpecialEffects.TriggerEffect(hurtOverride.effectName);
                    break;
                case CustomHurtOverrideBehavior.InstantDeathWithReplace:
                    this.gameObject.SetActive(false);
                    this.Expire(true);
                    Instantiate(hurtOverride.transform, this.transform.position, this.transform.rotation);
                    break;
                case CustomHurtOverrideBehavior.ClearEffectAfterXSeconds:
                    _hurtOverrideTimer.TimeLimit = hurtOverride.numberValue;
                    _hurtOverrideTimer.Reset();
                    _hurtOverrideTimer.Start();
                    HurtNPC(info.Damage);
                    break;
                default:
                    break;
            }
        }
       


        private void UpdateDecisionResolver(ActionBase ignoreAction)
        {
            Actions.IgnoreAction = ignoreAction;
            if (!Actions.AreAnyActionsHappening())
            {
                Actions.UpdateWaitingForNewDecision();
            }
        }

        private bool SupportsMovement()
        {
            return this.Type != EnemyType.FloatingShooter &&
                this.Type != EnemyType.StandingThrower;
        }

        public virtual ActionBase ResolveNextDecision()
        {
            if (!this.HealthManager.IsAlive())
                return null;

            if (this.DecisionResolverOverride != null)
            {
                return this.DecisionResolverOverride();
            }

            if (this.Type == EnemyType.WalkingMelee)
            {
                return (MovementManager.RemainingDistance < MeleeDistance) ? possibleCloseCombat : (ActionBase)null;
            }
            else
            {
                return possibleShoot;
            }
        }

        public void ChangeHealth(float HealthIncrement)
        {
            this.HealthManager.Health += HealthIncrement;
            if (HealthIncrement != 0)
            {
                UpdateHealthDisplay();
            }
        }






        #region NESTED TYPES

        [Serializable]
        public class CustomHurtOverride
        {
            public NpcHurtType Name = NpcHurtType.None;
            public CustomHurtOverrideBehavior behavior = CustomHurtOverrideBehavior.None;
            public float numberValue = 0;
            public string effectName = null;
            public Transform transform = null;
        }

        [Serializable]
        public enum CustomHurtOverrideBehavior
        {
            None,
            HurtWithNoEffect,
            NotHurt,
            NotHurtAboveY,
            NotHurtGainHealth,
            InstantDeath,
            InstantDeathWithEffect,
            InstantDeathWithReplace,
            ClearEffectAfterXSeconds
        }

        #endregion



        public void SetDifficulty(int diff)
        {
            float currentHeathRatio = 0;
            if (this.HealthManager != null && this.HealthManager.Health != 0)
            {
                currentHeathRatio = this.HealthManager.Health / this.MaxHealth;
                this.HealthManager.MaxHealth = diff_MaxHealth.GetValue(diff);
            }
            this.MaxHealth = diff_MaxHealth.GetValue(diff);
            this.HurtTime = diff_HurtTime.GetValue(diff);
            this.MoveSpeed = diff_MoveSpeed.GetValue(diff);
            try
            {
                this.HealthManager.Health = this.MaxHealth * currentHeathRatio;
                this.MovementManager.SetSpeed(this.MoveSpeed);
            }
            catch { }
        }



    }
}