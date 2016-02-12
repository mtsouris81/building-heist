using UnityEngine;
using System.Collections;

namespace GiveUp.Core
{
    public class NpcProjectile : MonoBehaviour
    {

        public const string IGNORE_TAG = "projectileIgnore";

        public NpcHurtType Type = NpcHurtType.None;

        public bool EnableDebug = false;
        public bool HasExpired { get; private set; }
        public bool CausesExplosion = false;
        public Transform Visual;
        public Transform ExpirationParticles;
        public float Damage = 10;
        public float MaxDistanceTraveled = 150;
        public string TargetTag = "Player";
        public string SelfTag = "NPC";
        public float SlowBurnOut = 0;
        public Transform[] SlowBurnImmediateDestroy = null;

        public Vector3 Origin { get; set; }
        float distanceTraveled;
        string tagHit = string.Empty;
        string nameHit = string.Empty;
        bool hasExploded = false;
        Vector3 frozedPos;

        ActionTimer ExposionTimer;
        ActionTimer BurnOutTimer;
        void Start()
        {
            BurnOutTimer = new ActionTimer(SlowBurnOut, delegate()
            {
                GameObject.Destroy(this.gameObject);
            });
            BurnOutTimer.Stop();
            HasExpired = false;
            if (CausesExplosion)
            {
                ExposionTimer = new ActionTimer(5, delegate()
                    {
                        ExpireProjectile();
                    });
            }
        }

        public void OnCollisionEnter(Collision col)
        {
            if (HasExpired || hasExploded || col.HasSameTagAs(this) || col.HasTag(IGNORE_TAG))
            {
                return;
            }

            if (col.HasTag(SelfTag))
            {
                return;
            }

            if (this.CausesExplosion)
            {
                hasExploded = true;
                this.SendMessage("Explode", this.Damage, SendMessageOptions.DontRequireReceiver);
                ExposionTimer.Start();
                frozedPos = this.transform.position;
            }
            else
            {
                CheckCollisionForAttackHit(col.gameObject, this.transform.position, Vector3.Normalize( this.GetComponent<Rigidbody>().velocity),   this.Damage, this.CausesExplosion, this.tag, this.TargetTag, this.EnableDebug, this.Type);
                ExpireProjectile();
            }
        }


        public static bool CheckCollisionForAttackHit(GameObject obj, Vector3 position, Vector3 normal, float damage, bool causesExplosion, string ownerTag, string targetTag, bool EnableDebug, NpcHurtType t)
        {
            if (obj != null)
            {
                string nameHit = obj.name;
                string tagHit = obj.tag;

                if (ownerTag == tagHit)
                {
                    return false;
                }

                if (tagHit == targetTag)
                {
                    HurtInfo hurtInfo = new HurtInfo();
                    hurtInfo.CausesExplosion = causesExplosion;
                    hurtInfo.Damage = damage;
                    hurtInfo.Owner = null;
                    hurtInfo.Position = position;
                    hurtInfo.Normal = normal;
                    hurtInfo.HurtType = t;
                    obj.SendMessage("AcceptHurt", hurtInfo, SendMessageOptions.DontRequireReceiver);
                    return true;
                }
            }

            return false;
        }

        void Update()
        {
            BurnOutTimer.Update();

            if (!CausesExplosion)
            {
                 distanceTraveled = Vector3.Distance(Origin, this.transform.position);
                 if (distanceTraveled > MaxDistanceTraveled)
                 {
                     ExpireProjectile();
                 }
            }
            else
            {
                if (hasExploded)
                {
                    this.transform.position = frozedPos;
                    ExposionTimer.Update();
                }
            }
        }

        public void ExpireProjectile()
        {
            HasExpired = true;
            SpawnExplosionParticles();

            if (this.CausesExplosion)
            {
                SendMessage("ClearExplosion", SendMessageOptions.DontRequireReceiver);
            }


            if (SlowBurnOut > 0)
            {
                BurnOutTimer.Start();
                this.GetComponent<Rigidbody>().velocity = Vector3.zero;
                this.GetComponent<Rigidbody>().Sleep();
                if (SlowBurnImmediateDestroy != null)
                {
                    foreach (var t in SlowBurnImmediateDestroy)
                    {
                        t.gameObject.SetActive(false);
                    }
                }
            }
            else
            {
                Destroy(this.gameObject);
            }
        }

        protected virtual void SpawnExplosionParticles()
        {
            if (ExpirationParticles != null)
            {
                Transform obj = GameObject.Instantiate(ExpirationParticles, this.transform.position, Quaternion.identity) as Transform;
                obj.gameObject.SetActive(true);
            }
        }
    }
}