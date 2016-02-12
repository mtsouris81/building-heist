using UnityEngine;

public class UiAppStartUp : MonoBehaviour
{

    public string InitialScreen = null;
    public bool StartInstant = true;

    public void Start()
    {
        if (StartInstant)
        {
            MobileUIManager.Current.Manager.SetScreenImmediately(InitialScreen);
        }
        else
        {
            MobileUIManager.Current.Manager.SwitchToScreen(InitialScreen);
        }
    }
}
