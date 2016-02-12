using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Weenus;
using System;
using GiveUp.Core;

public class AppUiTutorial : MonoBehaviour
{
    public RectTransform TutorialPanel = null;
    public Text TutorialText = null;
    public static AppUiTutorial Current { get; private set; }

    Vector2 toStart, toEnd;

    ActionTimer TransitionTimer;


	private bool hasInitialized = false;

	void Start () 
    {
        Init();
	}

    void Update()
    {
        TransitionTimer.Update();

        if (TransitionTimer.Enabled)
        {
            TutorialPanel.anchoredPosition = Vector2.Lerp(toStart, toEnd, TransitionTimer.Ratio);
        }
    }

    public void Init()
    {
        if (hasInitialized)
            return;

        hasInitialized = true;

        Current = this;

        TextAsset text = Resources.Load<TextAsset>("page_tutorials");
        ParseTutorialPages(text.text);

        TransitionTimer = new ActionTimer(0.4f, delegate() {

            TutorialPanel.anchoredPosition = toEnd;

        });
        TransitionTimer.AccurateMode = true;
       // Debug.Log(PageTutorials.Count);
    }

    public void TutorialPanelClicked()
    {

        CloseTutorial();
    }

    public void ParseTutorialPages(string source)
    {
        PageTutorials.Clear();
        string[] lines = source.ToLines();

        string key = null;
        string value = null;

        foreach (var line in lines)
        {
            string t = line.Trim();
            if (t.StartsWith("#"))
            {
                if (key != null)
                {
                    PageTutorials.Add(key, value);
                }
                key = t.TrimStart('#');
            }
            else if (!string.IsNullOrEmpty(t))
            {
                value = t;
            }
        }
        if (!PageTutorials.ContainsKey(key))
        {
            PageTutorials.Add(key, value);
        }
    }

    public Dictionary<string, string> PageTutorials = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);


    public void OpenTutorial(string text)
    {
        if (TransitionTimer.Enabled || string.IsNullOrEmpty(text))
            return;

        TutorialText.text = text;
        TransitionTimer.Reset();
        TransitionTimer.Start();
        toStart = new Vector2(TutorialPanel.rect.width + 2, 0);
        toEnd = new Vector2(0, 0);
        TutorialPanel.anchoredPosition = toStart;
        TutorialPanel.gameObject.SetActive(true);
    }
    public void CloseTutorial()
    {
        if (TransitionTimer.Enabled)
            return;

        TransitionTimer.Reset();
        TransitionTimer.Start();
        toStart = new Vector2(0, 0);
        toEnd = new Vector2(TutorialPanel.rect.width + 2, 0);
        TutorialPanel.anchoredPosition = toStart;
    }
}
