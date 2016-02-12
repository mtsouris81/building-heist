using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace GiveUp.Core
{
    public class NpcHealthManager
    {
        NpcCore owner;

        public NpcHealthManager(NpcCore owner, float maxHealth)
        {
            this.MaxHealth = maxHealth;
            Health = maxHealth;
            this.owner = owner;
        }
        public void RestoreHealth()
        {
            Health = MaxHealth;
        }
        public float MaxHealth { get; set; }
        public float Health { get; set; }
        public float HealthRatio
        {
            get
            {
                if (this.Health == 0)
                    return 0;

                return Health / MaxHealth;
            }
        }

        public bool IsAlive()
        {
            return Health > 0;
        }

        public bool HasDied
        {
            get;
            private set;
        }

        public void SetDead()
        {
            HasDied = true;
        }

        public void Hurt(float amount)
        {
            Health -= amount;

            if (!this.owner.HealthManager.IsAlive())
            {
                //this.owner.Actions.EndAllActions(this.owner.Actions.Die);
                this.owner.Actions.Die.StartAction();
            }
            else
            {
                //this.owner.Actions.EndAllActions(this.owner.Actions.Hurt);
                this.owner.Actions.Hurt.StartAction();
            }
        }

        public void CheckForDeath()
        {
            if (!this.owner.HealthManager.IsAlive())
            {
                this.owner.Actions.Die.StartAction();
            }
        }

    }
}