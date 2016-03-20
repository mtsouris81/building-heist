using UnityEngine;
using System.Collections;
using GiveUp.Core;
using System;

public class Shaker : MonoBehaviour
{

    public int ShakeSteps = 20;
    public float ShakeStrength = 5f;
    float _strength = 1;
    ValueTimerSequence<Vector3> cameraShaker = null;
    Vector3 HomePosition;
    Transform Owner;

    void Start () {

        var shakeRandoms = this.GetRandoms(ShakeSteps, 1f, 359f);
        for (int i = 0; i < shakeRandoms.Length; i++)
        {
            if (i % 2 == 0)
            {
                shakeRandoms[i] = 0;
            }
        }
        shakeRandoms[shakeRandoms.Length - 1] = 0;
        cameraShaker = new ValueTimerSequence<Vector3>(this.GetRandoms(ShakeSteps, 0.02f, 0.05f), shakeRandoms);
        cameraShaker.DetermineValue = (seed) =>
        {
            if (seed == 0)
                return HomePosition;

            var r = Quaternion.AngleAxis(seed, Vector3.forward);
            var result = HomePosition + ((r * Vector3.up) * _strength);
            _strength -= (ShakeStrength * (1f / (float)ShakeSteps));
            return result;
        };
        cameraShaker.Lerp = Vector3.Lerp;
    }


	void Update () {
        cameraShaker.Update();
        if (cameraShaker.Timer.Enabled)
        {
            Owner.position = cameraShaker.GetCurrentValue();
        }
    }

    public void StartShaking(Transform owner)
    {
        Owner = owner;
        HomePosition = owner.position;
        _strength = ShakeStrength;
        cameraShaker.StartSequence();
    }
    public void StartShaking()
    {
        StartShaking(this.transform);
    }
}
