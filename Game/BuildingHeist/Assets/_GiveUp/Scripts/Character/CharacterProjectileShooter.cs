using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GiveUp.Character;
using System.Linq;

namespace GiveUp.Core
{
    public class CharacterProjectileShooter : MonoBehaviour
    {
        // DESIGNER PROPS
        public bool CameraNormalReversed = false;
        public float ShootForce;
        public Transform BulletPrefab;
        public AudioSource ShotSound = null;

        //Blast Display
        public Transform BlastDisplay;
        public float BlastDisplayDecayTime = 0.2f;
        
        Transform CameraTransform;
        Vector3 BlastDisplayScale;
        TimedInterpolation BlastFadeTimer;
        Quaternion blastRotation;

        public bool IsRayCast = false;
        public float MaxRayCastDistance = 1000;
        public float RaycastDamage = 100;
        public bool RaycastCausesExplosion = false;
        public string RaycastOwnerTag = "Player";
        public string RaycastTargetTag = "NPC";
        public Transform RayHitParticles = null;



        public Renderer GunRenderer = null;


        void Start()
        {
            Camera cam = GameObjectUtilities.GetComponentFromParent<Camera>(this.transform, 4);
            CameraTransform = cam.transform;

            if (BlastDisplay != null)
            {
                BlastFadeTimer = new TimedInterpolation(2);
                BlastDisplayScale = BlastDisplay.transform.localScale;
                blastRotation = BlastDisplay.transform.localRotation;
                BlastDisplay.transform.localScale = Vector3.zero;
            }
        }

        void Update()
        {
            if (BlastFadeTimer != null && BlastFadeTimer.Enabled)
            {
                BlastFadeTimer.Update();

                BlastDisplay.rotation = Quaternion.LookRotation(
                                    -Vector3.Normalize(
                                        LevelContext.Current.Cameras.PlayerCamera.transform.position -
                                        BlastDisplay.position)); 
                
                BlastDisplay.transform.localScale = Vector3.Lerp(BlastDisplayScale, Vector3.zero, BlastFadeTimer.Ratio);
            }
        }


        bool actuallyHitTarget = false;
        public void Shoot(Ray? ray)
        {
            Vector3 origin = ray.HasValue ? ray.Value.origin : this.transform.position;
            Vector3 camForward = ray.HasValue ? ray.Value.direction : CameraNormalReversed ? -CameraTransform.transform.forward : CameraTransform.transform.forward;
            Vector3 targetPos = CameraTransform.transform.position + (camForward * 1000);
            Vector3 shotNormal = Vector3.Normalize(targetPos - origin);
            if (float.IsNaN(shotNormal.x))
            {
                Debug.Log("invalid shot normal NAN");
            }
            Vector3 velocity = shotNormal * ShootForce;
            if (ShotSound != null)
            {
                ShotSound.PlayOneShot(ShotSound.clip);
            }
            if (IsRayCast)
            {
                //Debug.DrawLine(origin, origin + (shotNormal * MaxRayCastDistance), Color.green, 100, true);
                RaycastHit[] hits = Physics.RaycastAll(origin, shotNormal, MaxRayCastDistance).OrderBy(x => x.distance).ToArray();
                bool hitAnything = hits != null && hits.Length > 0;
                if (hitAnything)
                {
                    int index = 0;
                    //Debug.Log(string.Join(" >> ", hits.Select(x => x.transform.name).ToArray()));
                    while (
                        // is player
                        (hits[index].collider != null && hits[index].collider.HasTag(RaycastOwnerTag)) ||
                        // is trigger and not NPC
                        (hits[index].collider != null && hits[index].collider.isTrigger && !hits[index].collider.HasTag(RaycastTargetTag)))
                    {
                        index++;
                        if (index >= hits.Length)
                        {
                            hitAnything = false;
                            break;
                        }
                    }

                    if (hitAnything)
                    {
                        actuallyHitTarget = NpcProjectile.CheckCollisionForAttackHit(
                                            hits[index].collider.gameObject,
                                            hits[index].point,
                                            shotNormal,
                                            RaycastDamage,
                                            RaycastCausesExplosion,
                                            RaycastOwnerTag,
                                            RaycastTargetTag,
                                            true,
                                            NpcHurtType.GunShot);

                        if (!actuallyHitTarget && RayHitParticles != null) // only show raycast particles if NOT hitting enemy 
                        {
                            Transform o = GameObject.Instantiate(RayHitParticles, hits[index].point, Quaternion.identity) as Transform;
                            o.gameObject.SetActive(true);
                        }
                    }
                }
            }
            else
            {
                // physical bullet
                Transform projectile = (Transform)Instantiate(BulletPrefab, origin, Quaternion.identity);
                NpcProjectile bullet = projectile.GetComponent<NpcProjectile>();
                bullet.gameObject.SetActive(true);
                projectile.GetComponent<Rigidbody>().velocity = velocity;
                bullet.Origin = origin;
            }

            if (BlastFadeTimer != null)
            {
                BlastFadeTimer.TimeLimit = this.BlastDisplayDecayTime;
                BlastFadeTimer.Reset();
                BlastFadeTimer.Start();
            }

            this.SendMessage("OnGunShot", SendMessageOptions.DontRequireReceiver);
        }
    }
}