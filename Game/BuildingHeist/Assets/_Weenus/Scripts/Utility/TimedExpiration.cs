using UnityEngine;
using System.Collections;
using GiveUp.Core;

public class TimedExpiration : MonoBehaviour {



    public float TimeToExpire = 2;
    public bool DestoryObject = true;
    public bool SupportEditor { get; set; }
    ActionTimer timer;
	// Use this for initialization
	void Start () {

        if (DestoryObject)
        {
            timer = new ActionTimer(TimeToExpire, ExpireSelf);
        }
        else
        {
            timer = new ActionTimer(TimeToExpire, DisableSelf);
        }

        timer.Reset();
        timer.Start();
	}

    public void ExpireSelf()
    {
        GameObject.Destroy(this.gameObject);
    }

    public void DisableSelf()
    {
        this.gameObject.SetActive(false);
    }

	// Update is called once per frame
	void Update () {
        timer.Update();
	}
}
