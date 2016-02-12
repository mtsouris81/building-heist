using UnityEngine;
using System.Collections;
using GiveUp.Core;

public class BlinkingLight : MonoBehaviour {

    public Vector2 RandomOffRange = new Vector2(1, 4);
    public Vector2 RandomOnRange = new Vector2(0.5f, 1.5f);

    float offTime;
    float onTime;
    ActionTimer timer;
    bool isOn;

	// Use this for initialization
    void Start()
    {
        offTime = Random.Range(RandomOffRange.x, RandomOffRange.y);
        onTime = Random.Range(RandomOnRange.x, RandomOnRange.y);

        timer = new ActionTimer(offTime, delegate()
        {
            isOn = !isOn;
            this.GetComponent<Light>().enabled = isOn;
            timer.TimeLimit = isOn ? onTime : offTime;
            timer.Reset();
            timer.Start();
        });
        timer.Start();
	}
	
	// Update is called once per frame
	void Update () {
        timer.Update();
	}
}
