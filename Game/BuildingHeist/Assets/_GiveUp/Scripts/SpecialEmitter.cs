using UnityEngine;
using System.Collections;

public class SpecialEmitter : MonoBehaviour {


    public int BurstAmount = 100;

    ParticleSystem Particles = null;

	void Start () {
        Particles = this.GetComponent<ParticleSystem>();
	}

    public void Burst()
    {
        Particles.Emit(BurstAmount);
    }
	
	void Update () {
	    
	}

}
