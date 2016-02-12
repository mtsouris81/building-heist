using UnityEngine;
using System.Collections;
using System;
namespace GiveUp.Core
{

    public class CloseCombatCollision : MonoBehaviour
    {
        float lastHurt = 0;

        public float AttackDamage = 30;
        public float MinTimeBetweenDamage = 0.7f;

        public bool BumbleBeeMode = false;

        void Start()
        {
           // this.collider.enabled = true;
        }


        public void Update()
        {
            float diff = (Time.timeSinceLevelLoad - lastHurt);
            if (diff > MinTimeBetweenDamage)
            {
                Collider[] hits = Physics.OverlapSphere(this.transform.position, 4);
                if (hits != null)
                {
                    foreach (var h in hits)
                    {
                        TestCollider(h);
                    }
                }
            }
        }

        private void TestCollider(Collider col)
        {
            if (col.gameObject != null && col.gameObject.tag == "Player")
            {
                lastHurt = Time.timeSinceLevelLoad;
                PlayerUtility.Hero.AcceptHurt(new HurtInfo()
                {
                    CausesExplosion = false,
                    Damage = AttackDamage,
                    Force = 2,
                    Owner = this.transform.parent.gameObject,
                    Position = this.transform.position
                });

                if (BumbleBeeMode)
                {
                    NpcCore npc = GetComponentInParent<NpcCore>();
                    if (npc != null)
                    {
                        npc.HealthManager.Hurt(npc.MaxHealth * 2);
                        Debug.Log("kill self");
                    }
                }
            }

        }

        //public void OnTriggerStay(Collider col)
        //{
        //    float diff = (Time.timeSinceLevelLoad - lastHurt);
        //    if (diff > MinTimeBetweenDamage)
        //    {
        //        TestCollider(col);
        //    }

        //}
    }


}