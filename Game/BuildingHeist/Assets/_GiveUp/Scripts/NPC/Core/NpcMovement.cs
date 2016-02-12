using UnityEngine;
using System.Collections;
using System;
namespace GiveUp.Core
{
    public class NpcMovement
    {

        public Vector3 Velocity
        {
            get
            {
                return this.navAgent.velocity;
            }
        }

        private bool _enabled = true;
        public void SetEnabled(bool enabled)
        {
            if (enabled == false)
            {

                if (this.navAgent != null)
                {
                    try
                    {
                        this.navAgent.Stop(true);
                    }
                    catch(Exception ex) { }
                }
            }

            _enabled = enabled;
            //this.navAgent.active = enabled;
            //this.navAgent.enabled = enabled;
        }

        public float Speed
        {
            get { return navAgent.speed; }
        }
        MonoBehaviour owner;
        private bool CantFindHero = false;
        public delegate Vector3? GetCurrentTarget(NpcCore owner);
        private Vector3? DefaultGetTarget(NpcCore owner)
        {
            if (!_enabled)
                return null;

            if (TargetObject == null)
            {
                if (CantFindHero)
                {
                    return null;
                }
                else
                {
                    TargetObject = GameObject.Find("Hero");

                    if (TargetObject == null)
                    {
                        CantFindHero = true;
                        return null;
                    }
                }
            }

            return TargetObject.transform.position;
        }


        public NpcMovement(MonoBehaviour owner)
        {
            this.owner = owner;

            //var externalResolver = new object;// owner.GetComponent<PathTargetResolver>();

            //if (externalResolver != null)
            //{
            //    GetTargetMethod = externalResolver.GetPathfindingTarget;
            //    //Debug.Log("external path target resolver found!");
            //}
            //else
            //{
            //    GetTargetMethod = this.DefaultGetTarget;
            //}
        }

        public GetCurrentTarget GetTargetMethod { get; private set; }

        public void SetGetTargetMethod(GetCurrentTarget method)
        {
            GetTargetMethod = null;
            GetTargetMethod = method;
        }

        public void SetSpeed(float speed)
        {
            if (!_enabled)
                return;

            navAgent.speed = speed;
        }
        public void SetDestination(Vector3 destination)
        {
            if (!_enabled)
                return;

            navAgent.SetDestination(destination);
        }
        public float RemainingDistance
        {
            get
            {
                if (!_enabled)
                    return 5000;

                return navAgent.remainingDistance;
            }
        }
        NavMeshAgent navAgent;
        GameObject TargetObject;

        public void Initialize(bool movementEnabled)
        {
            SetEnabled(movementEnabled);

            if (movementEnabled)
            {
                navAgent = owner.GetComponent<NavMeshAgent>();
                TargetObject = GameObject.Find("Hero");
            }
            else
            {
                var p = owner.GetComponent<NavMeshAgent>();
                GameObject.Destroy(p);
            }
        }


    }
}