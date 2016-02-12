using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Weenus;

public class MobileUIManager : MonoBehaviour
{
    private static MobileUIManager _instance;
    public static MobileUIManager Current
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<MobileUIManager>();
            }
            return _instance;
        }
    }

    public RectTransform MenuScreen = null;
    public float TransitionDuration = 0.4f;
    public Button MenuButton = null;
    public Button BackButton = null;
    public MobileUIHeader Header = null;
    public UIScreenManager Manager { get; private set; }

    public MobileController MobileInputController = null;
    public Transform UiPagesParent = null;

    public Transform GameSceneParent = null;
    public Image ApplicationBackground = null;
    
    public ScreenRegistration[] Pages = null;

	void Start () {

        Manager = new UIScreenManager();
        Manager.SceneObjects = this;
        Manager.TransitionDuration = TransitionDuration;
        foreach (var p in this.Pages)
        {
            Manager.RegisteredScreens.Add(p);
        }

        if (MenuButton != null)
        {
            MenuButton.onClick.AddListener(delegate()
            {
                this.Manager.OpenAppMenu();

            });
        }
        if (Header != null && Header.BackButton != null)
        {
            Header.BackButton.onClick.AddListener(delegate()
            {
                this.Manager.Back();
            });
        }
	}
	

	void Update () {

        Manager.Update();
        
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (this.Manager.Mode == UIScreenManager.UiMode.Forms)
            {
                Manager.Back();
            }
            else
            {
                Manager.OpenAppMenu();
            }
        }
	}


    public void ShowBuilderStories()
    {
        this.Manager.SwitchToScreen("sbStories");
    }
    public void ShowBuilderCharacters()
    {
        this.Manager.SwitchToScreen("sbCharacters");
        //CharacterChooserPage.ShowForSingleSelect("sbCharacterSelector", null, AllCharactersSingleSelectCallback);
    }
    public void ShowBuilderAccount()
    {
        this.Manager.SwitchToScreen("sbAccount");
    }
    public void AllCharactersSingleSelectCallback(object item)
    {
        //this.Manager.SwitchToScreen("CharacterBuilder");
    }
    public void MultiSelectCallback(List<string> item)
    {
        //Debug.Log(string.Format("selected many {0}- {1}", item.Count, string.Join(",",item.ToArray())));
        this.Manager.Back();
    }
}
