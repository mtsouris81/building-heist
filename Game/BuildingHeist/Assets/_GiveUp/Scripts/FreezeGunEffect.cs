using UnityEngine;
using System.Collections;

public class FreezeGunEffect : GunMaterialEffect
{
    public SpecialEmitter[] Particles = null;
    public Light[] Lights = null;

    private float shinyMin = 0.01f;
    private float shinyMax = 0.1f;

    private float glossMin = 0.6f;
    private float glossMax = 1;


    public float minLight = 0;
    public float maxLight = 1.52f;


    public bool BlendMaterial = true;

    public override void Start()
    {
        base.Start();
        
        foreach (var l in Lights)
        {
            l.intensity = 0;
        }
    }

    public override void OnGunShot()
    {
        base.OnGunShot();

        if (this.Particles != null && this.Particles.Length > 0)
        {
            foreach (var p in this.Particles)
                p.Burst();
        }
    }
    public override void OnBlendEffect(float ratio)
    {

        if (BlendMaterial)
        {
            this.shooter.GunRenderer.material.SetFloat("_Shininess", Mathf.Lerp(shinyMin, shinyMax, ratio));
            this.shooter.GunRenderer.material.SetFloat("_Gloss", Mathf.Lerp(glossMax, glossMin, ratio));
        }

        foreach (var l in Lights)
        {
            l.intensity = Mathf.Lerp(maxLight, minLight, ratio);
        }
    }



}
