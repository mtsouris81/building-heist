using UnityEngine;
using System.Collections;
using GiveUp.Core;

public class TutorialManager : MonoBehaviour {

    public float StartDelay = 2;
    public float TimeBetweenSlides = 5;
    int tutorialIndex = -1;
    ActionTimer TutorialTimer;

    static bool HasSeenTutorial = false;
    GUIText[] Texts;

    protected void DisableAllTexts()
    {
        foreach (var t in Texts)
        {
            t.enabled = false;
        }

    }

	// Use this for initialization
	void Start () {

    


        TutorialTimer = new ActionTimer(StartDelay, delegate()
            {
                TutorialTimer.TimeLimit = TimeBetweenSlides;

                // first, disable all
                DisableAllTexts();

                tutorialIndex++;

                if (tutorialIndex >= 0 && tutorialIndex < Texts.Length)
                {
                    Texts[tutorialIndex].enabled = true;
                }

                // after last one
                if (tutorialIndex >= Texts.Length)
                {
                    HasSeenTutorial = true;
                    this.gameObject.SetActive(false);
                }

                TutorialTimer.Reset();
            });

        TutorialTimer.Loop = true;
        TutorialTimer.Start();

        Texts = this.GetComponentsInChildren<GUIText>();

        for (int i = 0; i < Texts.Length; i++)
        {
            switch (i)
            {
                case 0:
                    Texts[i].text = "WALK \n[ w , a , s , d ] keys \nRUN \nhold [ shift ]";
                    break;
                case 1:
                    Texts[i].text = "JUMP \n[ space ] key";
                    break;
                case 2:
                    Texts[i].text = "SHOOT \n[ left click ] mouse";
                    break;
                case 3:
                    Texts[i].text = "ZOOM \n[ right click  ] mouse";
                    break;
                case 4:
                    Texts[i].text = "CHANGE GUNS \n[ mouse wheel ]";
                    break;
                default:
                    break;
            }
        }

        DisableAllTexts();



        if (HasSeenTutorial)
        {
            this.gameObject.SetActive(false);
            return;
        }
	}
	
	// Update is called once per frame
	void Update () {

        if (TutorialTimer != null)
        {
            TutorialTimer.Update();
        }
	}
}
