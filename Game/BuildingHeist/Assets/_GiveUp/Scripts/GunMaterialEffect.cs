using UnityEngine;
using System.Collections;
using GiveUp.Core;

public abstract class GunMaterialEffect : MonoBehaviour {

    public float CoolDownTime = 5;

    ActionTimer timer;

    public NpcHurtType EffectType = NpcHurtType.GunShot;

    protected CharacterProjectileShooter shooter;
	
    public virtual void Start () {
        shooter = this.GetComponent<CharacterProjectileShooter>();
        timer = new TimedInterpolation(CoolDownTime);
	}

    public virtual void Update()
    {

        timer.Update();
        if (timer.Enabled)
        {
            OnBlendEffect(timer.Ratio);
        }
	}


    public abstract void OnBlendEffect(float ratio);

    public virtual void OnGunShot()
    {
        timer.Reset();
        timer.Start();
    }


}
