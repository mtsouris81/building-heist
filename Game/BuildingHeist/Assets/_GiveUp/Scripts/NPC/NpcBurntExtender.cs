using UnityEngine;
using System.Collections;
using GiveUp.Core;

public class NpcBurntExtender : NpcEffectExtender
{
    public override void OnEffectActivating(Transform ActivationObject)
    {
        NpcBurningEffect parentEffect = this.GetComponent<NpcBurningEffect>();
        if (parentEffect != null)
        {
            var obj = parentEffect.Npc.gameObject;

            var agent = obj.GetComponent<NavMeshAgent>();
            var npc = obj.GetComponent<NpcCore>();

            if (agent != null)
                GameObject.Destroy(agent);

            if (npc != null)
                GameObject.Destroy(npc);

            var body = obj.AddComponent<Rigidbody>();
            
            //body.isKinematic = true;
            body.mass = 10;
            body.drag = 2;
            //body.useGravity = true;


            var exp = obj.AddComponent<TimedExpiration>();
            exp.TimeToExpire = 6;
            exp.DestoryObject = true;


            obj.transform.rotation *= Quaternion.Euler(new Vector3(9, 0, 0)); // tilt so it falls forward
        }
    }

    public void SetInertia(float h, float r)
    {
        Vector3 inertia;
        inertia.x = GetComponent<Rigidbody>().mass * (1.0f / 12.0f) * h * h + GetComponent<Rigidbody>().mass * 0.25f * r * r;
        inertia.y = GetComponent<Rigidbody>().mass * 0.5f * r * r;
        inertia.z = inertia.x;
        GetComponent<Rigidbody>().inertiaTensor = inertia;
    }

}
