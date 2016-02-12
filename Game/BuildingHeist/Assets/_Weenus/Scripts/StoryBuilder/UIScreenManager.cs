using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using GiveUp.Core;

namespace Weenus
{

	public class UIScreenManager
	{
        public enum UiMode
        {
            GamePlay,
            Forms
        }

        public UiMode Mode = UiMode.Forms;

        public MobileUIManager SceneObjects { get; set; }
        public List<ScreenRegistration> RegisteredScreens = new List<ScreenRegistration>();
        private Stack<string> Stack = new Stack<string>();
        public bool IsTransitioning { get; private set; }
        public bool IsTransitioningForModal { get; private set; }

        public Stack<string> History
        {
            get
            {
                return this.Stack;
            }
        }
        Dictionary<string, object> viewData = new Dictionary<string, object>();


        public void SetViewData(string name, object value)
        {
            if (!viewData.ContainsKey(name))
            {
                viewData.Add(name, null);
            }
            viewData[name] = value;
        }
        public T GetViewData<T>(string name)
        {
            if (viewData.ContainsKey(name))
            {
                return (T)viewData[name];
            }
            return default(T);
        }



        public bool IsMenuOpen { get; private set;}

        public void OpenAppMenu()
        {
            SceneObjects.MenuScreen.gameObject.SetActive(true);
            SceneObjects.MenuScreen.SendMessage("OpenAppMenu");
            IsMenuOpen = true;
            //if (Mode == UiMode.Forms)
            //{
            //    SceneObjects.MenuScreen.OpenAppMenu();
            //}
            //else
            //{
            //    OpenPageRequirements();
            //    SceneObjects.MenuScreen.OpenAppMenu();
            //}
        }
        public void CloseAppMenu()
        {
            if (SceneObjects.MenuScreen.gameObject.activeSelf)
            {
                SceneObjects.MenuScreen.SendMessage("CloseAppMenu", SendMessageOptions.DontRequireReceiver);
            }
            IsMenuOpen = false;
        }


        public string CurrentScreen { get; private set; }

        public bool PageRequirementsOpen { get; private set; }

        public string TransitioningOut { get; private set; }
        public string TransitioningIn { get; private set; }

        public RectTransform GetScreenByName(string name)
        {
            if (name == null)
                return null;

            return RegisteredScreens.Where(s => s.Name.Equals(name, StringComparison.OrdinalIgnoreCase)).FirstOrDefault().Screen;
        }
        public ScreenRegistration GetRegistrationByName(string name)
        {
            return RegisteredScreens.Where(s => s.Name.Equals(name, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
        }


        public UIScreenManager()
        {
            TransitionTimer = new ActionTimer(0.25f, CompleteTransition);
            TransitionTimer.AccurateMode = true;

            rotateRight = Quaternion.Euler(new Vector3(0, 30, 0));
            rotateLeft = Quaternion.Euler(new Vector3(0, 330, 0));
        }



        protected void CompleteTransition()
        {
            CurrentScreen = TransitioningIn;
            IsTransitioning = false;
            var from = GetScreenByName(TransitioningOut);
            var to = GetScreenByName(CurrentScreen);
            to.anchoredPosition = toEnd;

            if (from != null && from != to && !IsTransitioningForModal)
            {
                from.SendMessage("UiDeactivated", SendMessageOptions.DontRequireReceiver);
                from.gameObject.SetActive(false);
            }

            //if (!CurrentTransitionIsBack && to != null)
            if (to != null)
            {
                to.SendMessage("UiActivated", SendMessageOptions.DontRequireReceiver);
            }

            var reg = GetRegistrationByName(CurrentScreen);
            
            this.SceneObjects.Header.Title.text = reg.DisplayTitle;

            if (AppUiTutorial.Current != null && AppUiTutorial.Current.PageTutorials != null && AppUiTutorial.Current.PageTutorials.Count > 0)
            {
                var tutorials = AppUiTutorial.Current.PageTutorials;
                if (tutorials.ContainsKey(CurrentScreen))
                {
                    string accomplishmentKey = string.Format("tutorial-{0}", CurrentScreen);

                }
            }

        }

        public void ClosePageRequirements()
        {

            var curr = GetScreenByName(CurrentScreen);
            if (curr != null)
            {
                curr.SendMessage("UiDeactivated", SendMessageOptions.DontRequireReceiver);
            }

            this.Stack.Clear();

            foreach (var s in RegisteredScreens)
            {
                if (s.Screen != null)
                {
                    s.Screen.gameObject.SetActive(false);
                }
            }

            if (this.SceneObjects.MobileInputController != null)
            {
                this.SceneObjects.MobileInputController.gameObject.SetActive(true);
            }

            this.SceneObjects.ApplicationBackground.gameObject.SetActive(false);

            if (this.SceneObjects.GameSceneParent != null)
            {
                this.SceneObjects.GameSceneParent.gameObject.SetActive(true);
            }

            this.SceneObjects.Header.gameObject.SetActive(false);

            if (LevelContext.Current != null)
            {
                LevelContext.Current.UnPause();
            }
            PageRequirementsOpen = false;
        }

        public void OpenPageRequirements()
        {
            if (PageRequirementsOpen)
            {
                return;
            }
            PageRequirementsOpen = true;

            if (LevelContext.Current != null)
            {
                LevelContext.Current.Pause();
            }

            this.SceneObjects.ApplicationBackground.gameObject.SetActive(true);

            if (this.SceneObjects.GameSceneParent != null)
            {
                this.SceneObjects.GameSceneParent.gameObject.SetActive(false);
            }

            this.SceneObjects.Header.gameObject.SetActive(true);

            if (this.SceneObjects.MobileInputController != null)
            {
                this.SceneObjects.MobileInputController.gameObject.SetActive(false);
            }

        }

        float factor;
        public void Update()
        {

            TransitionTimer.Update();

            if (IsTransitioning)
            {
                var to = GetScreenByName(TransitioningIn);
                if (to != null)
                {
                    to.anchoredPosition = HermiteVector(toStart, toEnd, TransitionTimer.Ratio);
                    //to.rotation = Quaternion.Lerp(toRotate, Quaternion.identity, TransitionTimer.Ratio);
                }

                if (!IsTransitioningForModal)
                {
                    var from = GetScreenByName(TransitioningOut);
                    if (from != null && from != to)
                    {
                        from.anchoredPosition = HermiteVector(fromStart, fromEnd, TransitionTimer.Ratio);
                    }
                }
            }
            else
            {
                if (ScreenQueue.Count > 0)
                {
                    string nextScreen = ScreenQueue.Dequeue();
                    SwitchToScreen(nextScreen);
                }
            }

        }

        ActionTimer TransitionTimer;

        public Vector2 HermiteVector(Vector2 start, Vector2 end, float amount)
        {
            return Vector2.Lerp(start, end, amount);
            //return new Vector2(
            //    Mathfx.Hermite(start.x, end.x, amount),
            //    Mathfx.Hermite(start.y, end.y, amount));
        }
        protected bool CurrentTransitionIsBack { get; private set; }

        public void StartTransition()
        {
            var from = GetScreenByName(TransitioningOut);
            var to = GetScreenByName(TransitioningIn);

            TransitionTimer.TimeLimit = TransitionDuration;
            TransitionTimer.Reset();
            TransitionTimer.Start();

            //to.rotation = Quaternion.identity;
            to.gameObject.SetActive(true);

            toStart = CurrentTransitionIsBack ? new Vector2(-Screen.width, 0) : new Vector2(Screen.width, 0);
            toEnd = new Vector2(0, 0);
            to.anchoredPosition = toStart;

            toRotate = CurrentTransitionIsBack ? rotateLeft : rotateRight;
            fromRotate = !CurrentTransitionIsBack ? rotateLeft : rotateRight;

            fromStart = new Vector2(0, 0);
            fromEnd = CurrentTransitionIsBack ? new Vector2(Screen.width, 0) : new Vector2(-Screen.width, 0);


        }

        Quaternion rotateRight, rotateLeft;

        Quaternion toRotate, fromRotate;
        Vector2 toStart, toEnd;
        Vector2 fromStart, fromEnd;

        public Queue<string> ScreenQueue = new Queue<string>();

        public void SetScreenImmediately(string name)
        {
            OpenPageRequirements();
            IsTransitioningForModal = false;
            CurrentTransitionIsBack = false;
            Stack.Push(name);
            var screen = GetScreenByName(name);
            toEnd = new Vector2(0, 0);
            TransitioningIn = name;


            screen.gameObject.SetActive(true);
            //screen.gameObject.SendMessage("UiActivated", SendMessageOptions.DontRequireReceiver);
            //CurrentScreen = name;

            CompleteTransition();
            
            SupportBackButton(name);
        }

        public void SwitchToScreen(string name)
        {
            if (IsTransitioning)
            {
                ScreenQueue.Enqueue(name);
                return;
            }

            Mode = UIScreenManager.UiMode.Forms;

            IsTransitioning = true;

            OpenPageRequirements();

            if (Stack.Count < 1)
            {
                TransitioningOut = FindAnyOpenPages(name);
            }
            else
            {
                TransitioningOut = Stack.Peek();
            }




            IsTransitioningForModal = false;
            TransitioningIn = name;
            Stack.Push(name);

            CurrentTransitionIsBack = false;
            StartTransition();

            SupportBackButton(name);

        }

        protected void SupportBackButton(string name)
        {
            var screenRegistration = GetRegistrationByName(name);
            if (screenRegistration != null)
            {
                bool showBackButton = screenRegistration.SupportsBackButton && History.Count > 1;
                this.SceneObjects.MenuButton.gameObject.SetActive(!showBackButton && screenRegistration.SupportsMenuButton);
                this.SceneObjects.BackButton.gameObject.SetActive(showBackButton);
            }
        }

        private string FindAnyOpenPages(string defaultResult)
        {
            foreach (var page in RegisteredScreens)
            {
                if (page.Screen != null)
                {
                    if (page.Screen.gameObject.activeSelf && !page.Name.Equals(defaultResult, StringComparison.OrdinalIgnoreCase))
                    {
                        return page.Name;
                    }
                }
            }
            return defaultResult;
        }
        public void OpenModalScreen(string name)
        {
            OpenModalScreen(name, false);
        }
        public void OpenModalScreen(string name, bool deActivateMainPageParent)
        {
            if (IsTransitioning)
            {
                return;
            }
            var to = GetScreenByName(name);
            OpenPageRequirements();
            to.gameObject.SetActive(true);
            to.gameObject.SendMessage("UiActivated", SendMessageOptions.DontRequireReceiver);

            if (deActivateMainPageParent)
            {
                SceneObjects.UiPagesParent.gameObject.SetActive(false);
            }
        }
        public void OpenModalOverGame(string name)
        {
            if (IsTransitioning)
            {
                return;
            }
            var to = GetScreenByName(name);
            to.gameObject.SetActive(true);
            to.gameObject.SendMessage("UiActivated", SendMessageOptions.DontRequireReceiver);
        }
        public void CloseModalScreen(string name)
        {
            if (IsTransitioning)
            {
                return;
            }
            var to = GetScreenByName(name);
            to.gameObject.SendMessage("UiDeactivated", SendMessageOptions.DontRequireReceiver);
            to.gameObject.SetActive(false);

            if (Mode == UiMode.Forms)
            {
                SceneObjects.UiPagesParent.gameObject.SetActive(true);
            }
        }
        public void Back()
        {
            if (IsTransitioning)
            {
                return;
            }
            if (Stack.Count < 2)
            {
                return;
            }

            IsTransitioning = true;

            var current = Stack.Pop();

            var beforeCurrent = Stack.Peek();
            //Stack.Pop();


            TransitioningOut = current;
            TransitioningIn = beforeCurrent;

            IsTransitioningForModal = false;
            CurrentTransitionIsBack = true;
            StartTransition();

            SupportBackButton(TransitioningIn);
        }


        public void StartPlayMode()
        {
            Mode = UIScreenManager.UiMode.GamePlay;
            CloseAppMenu();
            ClosePageRequirements();
            this.SceneObjects.MenuButton.gameObject.SetActive(true);

            if (OnStartingPlayMode != null)
            {
                OnStartingPlayMode();
            }
        }

        public Action OnStartingPlayMode { get; set; }

        public float TransitionDuration { get; set; }

        public void ReloadCurrentScreen()
        {
            this.SetScreenImmediately(this.CurrentScreen);
        }

    }

    [System.Serializable]
    public class ScreenRegistration
    {
        public string Name = null;
        public RectTransform Screen = null;
        public string DisplayTitle = string.Empty;
        public bool SupportsBackButton = false;
        public bool SupportsMenuButton = true;
    }

}
