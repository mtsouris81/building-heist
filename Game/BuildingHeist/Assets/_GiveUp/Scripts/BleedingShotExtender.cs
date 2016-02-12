using UnityEngine;
using System.Collections;
using GiveUp.Core;

public class BleedingShotExtender : NpcEffectExtender
{

    private int BloodParticlesPerShot = 40;

    public ParticleSystem Particles = null;
    public ParticleSystem Particles2 = null;
    NpcBurningEffect parentEffect;

    ActionTimer timer;
    public float bleedTime = 0.1f;
    public void Update()
    {
        timer.Update();
    }
    public void Start()
    {
        Particles.SetEmission(false);

        if (Particles2 != null)
            Particles2.SetEmission(false);

        timer = new ActionTimer(bleedTime, delegate()
            {
                this.Particles.SetEmission(false);

                if (Particles2 != null)
                    this.Particles2.SetEmission(false);
            });
        parentEffect = this.GetComponent<NpcBurningEffect>();
    }

    public override void OnEffectActivating(Transform ActivationObject)
    {
        Particles.transform.position = parentEffect.Npc.LastHurtInfo.Position;
        Particles.transform.rotation = Quaternion.LookRotation(-parentEffect.Npc.LastHurtInfo.Normal);
        Particles.SetEmission(true);

        if (Particles2 != null)
        {
            Particles2.transform.position = parentEffect.Npc.LastHurtInfo.Position;
            Particles2.transform.rotation = Quaternion.LookRotation(parentEffect.Npc.LastHurtInfo.Normal);
            Particles2.SetEmission(true);
        }
        timer.Reset();
        timer.Start();
        

    }



}
