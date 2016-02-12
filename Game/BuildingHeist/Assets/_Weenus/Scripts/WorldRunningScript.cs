using UnityEngine;
using System.Collections;
using GiveUp.Core;

public class WorldRunningScript : MonoBehaviour {

    LockMouse _lockMouse = null;
    public MobileController MobileControls = null;

	// Use this for initialization
	void Start () {

        _lockMouse = PlayerUtility.Hero.GetComponent<LockMouse>();
	}
	
	// Update is called once per frame
	void Update () {

        if (MobileControls.Mode == MobileController.MobileControllerMode.DesktopOnly)
        {
            if (_lockMouse != null)
            {
                if (MobileUIManager.Current.Manager.Mode == Weenus.UIScreenManager.UiMode.GamePlay)
                {
                    if (!MobileUIManager.Current.Manager.IsMenuOpen)
                    {
                        _lockMouse.SetActive(true);
                    }
                    else
                    {
                        _lockMouse.SetActive(false);
                    }
                }
            }
        }
        


	}
}
