using UnityEngine;
using System.Collections;
using GiveUp.Core;

public abstract class NpcDecisionResolver : MonoBehaviour {

	// Use this for initialization
	void Start () {
        //NpcCore npc = this.GetComponent < NpcCore>();
        //InjectActions(npc);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public abstract void InjectActions(NpcCore npc);

    public abstract ActionBase ResolveNextAction();

}
