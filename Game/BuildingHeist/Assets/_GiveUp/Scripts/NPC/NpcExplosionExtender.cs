using UnityEngine;
using System.Collections;

public class NpcExplosionExtender : NpcEffectExtender
{

    public static Vector3 ExplosionOrigin = Vector3.zero;


    public float ExplosionForce = 30;
    public float ExplosionRadius = 10;

	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public override void OnEffectActivating(Transform ActivationObject)
    {
        string soundName = "Explosion";
        if (this.gameObject.name.IndexOf("Ice", System.StringComparison.OrdinalIgnoreCase) >= 0)
        {
            soundName = "IceExplosion";
        }
        SharedSoundManager.GetCurrent().Play(soundName, ExplosionOrigin);
        Rigidbody[] bodies = ActivationObject.GetComponentsInChildren<Rigidbody>();
        if (bodies != null)
        {
            foreach (var b in bodies)
            {
                b.mass = 0.3f;
                b.AddExplosionForce(ExplosionForce, ExplosionOrigin, ExplosionRadius, -1, ForceMode.VelocityChange);
               // b.gameObject.AddComponent<BodyExplosionPart>();
            }
        }
    }
}
