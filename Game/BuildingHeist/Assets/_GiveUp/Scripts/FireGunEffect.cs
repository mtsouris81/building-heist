using UnityEngine;
using System.Collections;

public class FireGunEffect : GunMaterialEffect
{
    public SpecialEmitter[] Particles = null;
    public Light[] Lights = null;

    private float shinyMin = 0.01f;
    private float shinyMax = 0.1f;

    private float glossMin = 0.6f;
    private float glossMax = 1;


    private float minLight = 0;
    private float maxLight = 4.79f;


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

        foreach (var p in this.Particles)
            p.Burst();
    }
    public override void OnBlendEffect(float ratio)
    {
        //this.shooter.GunRenderer.material.SetFloat("_Shininess", Mathf.Lerp(shinyMin, shinyMax, ratio));
        //this.shooter.GunRenderer.material.SetFloat("_Gloss", Mathf.Lerp(glossMax, glossMin, ratio));

        foreach (var l in Lights)
        {
            l.intensity = Mathf.Lerp(maxLight, minLight, ratio);
        }
    }



}
