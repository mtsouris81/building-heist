using UnityEngine;
using System.Collections;

public class MountainLevel : MonoBehaviour {


    public Transform SpawnLocation;
    public Transform NPC;

    public float spawnDelay = 5;
    float currentDelay = 0;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        currentDelay += Time.deltaTime;

        if (currentDelay > spawnDelay)
        {
            currentDelay = 0;
            Instantiate(NPC, SpawnLocation.position, Quaternion.identity);
        }

	}
}
