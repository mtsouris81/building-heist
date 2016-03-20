using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Weenus;
using GiveUp.Core;

public class HamburglarAppMenu : MonoBehaviour {

    public RectTransform GameOptions = null;
    public Button MenuBacking = null;
    public Image Background = null;

    public float MenuAnimationSpeed = 2f;
    public RectTransform MenuScreen = null;


    bool hasInitialized = false;
    WeenusUI View = null;

    void Start()
    {
        Init();
    }

    void Update()
    {
        UpdateMenuAnimation();
    }

    public void Init()
    {
        if (hasInitialized)
            return;

        hasInitialized = true;
        View = new WeenusUI(this);

        MenuAnimationTimer = new ActionTimer(MenuAnimationSpeed, OnMenuAnimationComplete);
        MenuAnimationTimer.AccurateMode = true;


        View.SetClickHandler("games", true, delegate ()
        {
            View.UI.SwitchToScreen("games");
            View.UI.CloseAppMenu();
        });
        View.SetClickHandler("friends", true, delegate ()
        {
            View.UI.SwitchToScreen("friends");
            View.UI.CloseAppMenu();
        });
        View.SetClickHandler("exit", true, delegate()
        {
            Application.Quit();
        });
        View.SetClickHandler("backtogame", true, delegate()
        {
            MobileUIManager.Current.Manager.StartPlayMode();
        });
        MenuBacking.onClick.AddListener(delegate()
        {
            this.CloseAppMenu();
        });
    }

    public void UiActivated()
    {
        Init();
        GameOptions.gameObject.SetActive(true);
    }

    public void UiDeactivated()
    {

    }





    private ActionTimer MenuAnimationTimer = null;
    private Vector2 menuStartPos = new Vector2(0, 0);
    private Vector2 menuEndPos = new Vector2(0, 0);
    public bool IsMenuTransitioning { get; private set; }
    public bool IsMenuTransitionOpen { get; private set; }

    private void StartMenuTimer()
    {
        MenuAnimationTimer.Reset();
        MenuAnimationTimer.Start();
        IsMenuTransitioning = true;
    }
    public void OpenAppMenu()
    {
        if (IsMenuTransitioning)
            return;

        gameObject.SetActive(true);
        UiActivated();
        IsMenuTransitionOpen = true;
        menuStartPos = new Vector2(-MenuScreen.rect.width, 0);
        menuEndPos = new Vector2(0, 0);
        MenuScreen.anchoredPosition = menuStartPos;
        StartMenuTimer();
        Background.gameObject.SetActive(true);
    }
    public void CloseAppMenu()
    {
        if (IsMenuTransitioning)
            return;

        StartMenuTimer();
        IsMenuTransitionOpen = false;
        menuStartPos = new Vector2(0, 0);
        menuEndPos = new Vector2(-MenuScreen.rect.width, 0);
        MenuScreen.anchoredPosition = menuStartPos;
        Background.gameObject.SetActive(false);
    }
    private void UpdateMenuAnimation()
    {
        MenuAnimationTimer.Update();
        if (IsMenuTransitioning)
        {
            MenuScreen.anchoredPosition = Vector2.Lerp(menuStartPos, menuEndPos, MenuAnimationTimer.Ratio);
        }
    }
    private void OnMenuAnimationComplete()
    {
        if (!IsMenuTransitionOpen)
        {
            gameObject.SetActive(false);
        }
        MenuScreen.anchoredPosition = menuEndPos;
        IsMenuTransitioning = false;
    }

}
