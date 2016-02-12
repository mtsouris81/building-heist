using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace GiveUp.Core
{
    public class NpcProjectileShooter : MonoBehaviour
    {

        public static float PlayerPositionYOffset = 0.39f;
        public float AutoFireInterval = 1;
        public int TotalShotsPerAttack = 3;

        ActionTimer timer;
        GameObject player;
        AudioSource shotSound = null;
        public float MovementSpeedWhileShooting = 0.7f;
        public float ShootForce;
        public Transform BulletPrefab;
        public bool IsThrowing = false;

        [RangeAttribute(0.0001f, 1)]
        public float UpwardForce = 0.2f;

        public float MinThrowForce = 10;
        public float MaxThrowForce = 30;
        public float MaxThrowDistance = 100;


        public bool IsDoubleShot = false;
        public Transform DoubleShotLocation1 = null;
        public Transform DoubleShotLocation2 = null;


        public float GetThrowForce()
        {
            float distance = Vector3.Distance(player.transform.position, this.transform.position);
            if (distance < 1)
            {
                distance = 1;
            }
            float ratio = distance / MaxThrowDistance;
            return Mathf.Lerp(MinThrowForce, MaxThrowForce, ratio);
        }


        // Use this for initialization
        void Start()
        {
            player = GameObject.Find("Hero");
            timer = new ActionTimer(1, Shoot);
            timer.Loop = true;
            shotSound = this.GetComponentInChildren<AudioSource>();
            //timer.Start();
        }

        // Update is called once per frame
        void Update()
        {
            //timer.Update();
        }

        public void TurnOff()
        {
            if (timer != null)
                timer.Stop();
        }




        public void Shoot()
        {
            if (IsDoubleShot)
            {
                //Debug.Log("double shoot");
                ShootProjectile(DoubleShotLocation1);
                ShootProjectile(DoubleShotLocation2);
            }
            else
            {
                ShootProjectile(this.transform);
            }

            if (shotSound != null)
            {
                shotSound.Play();
            }
        }

        private void ShootProjectile(Transform originTransform)
        {

            NpcProjectile projectile = GameObjectUtilities.InstantiatePrefab<NpcProjectile>(BulletPrefab, originTransform.position, Quaternion.identity);
            projectile.gameObject.SetActive(true);
            Vector3 target = player.transform.position + new Vector3(0, PlayerPositionYOffset, 0);
            //float distance = Vector3.Distance(target, originTransform.position);
            Vector3 dir = Vector3.Normalize(target - originTransform.position);
            Vector3 velocity = IsThrowing
                                    ? (dir + new Vector3(0, UpwardForce, 0)) * (GetThrowForce())// * distance)
                                    : dir * ShootForce;

            if (IsThrowing)
            {
                projectile.GetComponent<Rigidbody>().useGravity = true;
            }

            projectile.GetComponent<Rigidbody>().velocity = velocity;
            projectile.Origin = originTransform.position;
            projectile.transform.rotation = Quaternion.LookRotation(dir);
        }

    }
}