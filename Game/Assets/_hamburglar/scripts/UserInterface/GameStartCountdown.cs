using UnityEngine;
using System.Collections;
using GiveUp.Core;
using UnityEngine.UI;
using System;

public class GameStartCountdown : MonoBehaviour {

    ActionTimer timer;
    public Text TimerDisplay = null;

    public void StartCountdown(float time, Action action)
    {
        if (timer == null)
        {
            timer = new ActionTimer(time, () =>
            {
                if (action != null)
                {
                    action();
                }
                this.gameObject.SetActive(false);
                HamburglarContext.Instance.WaitingForPlayersDisplay.gameObject.SetActive(false);
            });
        }
        timer.AccurateMode = true;
        timer.TimeLimit = time;
        timer.Reset();
        timer.Start();
        this.gameObject.SetActive(true);
    }
    
	void Update ()
    {
        timer.Update();
        if (timer.Enabled)
        {
            TimerDisplay.text = string.Format("{0:00.0}", (timer.TimeLimit - timer.Elapsed));
        }
	}
}
