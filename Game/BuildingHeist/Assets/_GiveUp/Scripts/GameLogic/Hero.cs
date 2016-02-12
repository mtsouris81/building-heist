using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GiveUp.Core
{
	public class Hero : MonoBehaviour
    {


        public bool Invincible { get; set; }
        SharedSoundManager sharedSounds;
        public FirstPersonDrifter Controller { get; private set; }
        public object HelperArrow = null;
        public float MaxHealth = 500;
        public float Health = 500;
        public bool IsAlive
        {
            get
            {
                return Health > 0;
            }
        }
        public bool HasDied { get; private set; }
        public event EventHandler Died;
        public event EventHandler HealthRestored;

        CharacterHurtIndicator hurtIndicator;


        public void Awake()
        {
            PlayerUtility.Current = this.gameObject;
        }

        void Start()
        {
            sharedSounds = SharedSoundManager.GetCurrent();
            Controller = this.GetComponent<FirstPersonDrifter>();
            GunController = this.GetComponentInChildren<CharacterGunController>();
            hurtIndicator = this.GetComponentInChildren<CharacterHurtIndicator>();
            HasDied = false;
        }



        public CharacterGunController GunController { get; private set; }

        public void PlayerHurt(float damage)
        {
            if (Invincible)
                return;

            if (this.Health < 0)
            {
                return;
            }

            Health -= damage;
            if (Health < 0)
            {
                Health = 0;
            }
        }

        public void Update()
        {
            if (!IsAlive && !HasDied)
            {
                Die();
            }
        }

        public void KillPlayer()
        {
            PlayerHurt(1000000);
            Die();
        }

        
        protected void Die()
        {
            if (HasDied)
                return;

            HasDied = true;
            
            if (Died != null)
                Died(this, EventArgs.Empty);
        }


        public void Reset()
        {
            this.HasDied = false;
            this.Health = MaxHealth;
            sharedSounds.Play("PlayerRespawn", this.transform.position);
        }

        public void RestoreHealth()
        {
            this.Health = MaxHealth;
            
            if (HealthRestored != null)
                HealthRestored(this, EventArgs.Empty);



        }

        public void Jump()
        {
            Controller.Jump();
        }

        public void AcceptHurt(HurtInfo info)
        {
            if (Invincible)
                return;
            
            if (HasDied)
            {
                return;
            }
            PlayerHurt(info.Damage);
            hurtIndicator.OnHurt(info);
        }

    }
}
