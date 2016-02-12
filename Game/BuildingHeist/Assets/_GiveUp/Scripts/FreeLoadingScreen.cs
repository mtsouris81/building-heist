using UnityEngine;
using System.Collections;

public class FreeLoadingScreen : MonoBehaviour {

    public Transform Overlay = null;
    private int WaitFrames = 4;

	public Transform[] DestoryObjects = null;
    public string TargetScreen { get; private set; }
    public int? TargetScreenIndex { get; private set; }

    bool isWaitingForChange = false;
    int waitingFrames = 0;

	void Start () 
    {
	
	}


    public static void SwitchToScene(string sceneName)
    {
        FreeLoadingScreen obj = GameObject.FindObjectOfType<FreeLoadingScreen>();
        if (obj != null)
        {
            obj.TargetScreen = sceneName;
            obj.TargetScreenIndex = null;
            obj.StartScreenSwitch();
        }
        else
        {
            throw new System.Exception(string.Format("could not find instance of {0}, but it is required for calls to SwitchToScene(string sceneName)", typeof(FreeLoadingScreen).Name));
        }
    }    
    
    public static void SwitchToScene(int sceneIndex)
    {
        FreeLoadingScreen obj = GameObject.FindObjectOfType<FreeLoadingScreen>();
        if (obj != null)
        {

            obj.TargetScreen = null;
            obj.TargetScreenIndex = sceneIndex;
            obj.StartScreenSwitch();
        }
        else
        {
            throw new System.Exception(string.Format("could not find instance of {0}, but it is required for calls to SwitchToScene(string sceneName)", typeof(FreeLoadingScreen).Name));
        }
    }

	private void CleanUpObjects()
	{
		if (this.DestoryObjects != null && this.DestoryObjects.Length > 0) {
			foreach(var o in this.DestoryObjects)
			{
                if (o == null || o.gameObject == null)
                    continue;

				GameObject.Destroy(o.gameObject);
			}
		}

		Resources.UnloadUnusedAssets ();
	}


    private void StartScreenSwitch()
    {
        isWaitingForChange = true;
        waitingFrames = 0;
        if (Overlay != null)
        {
            Overlay.gameObject.SetActive(true);
		}
		CleanUpObjects();
    }
	
	void Update () 
    {

        if (isWaitingForChange && waitingFrames < WaitFrames)
        {
            waitingFrames++;
            if (waitingFrames >= WaitFrames)
            {
                if (TargetScreenIndex.HasValue)
                {
                    Application.LoadLevel(TargetScreenIndex.Value);
                }
                else
                {
                    Application.LoadLevel(TargetScreen);
                }
            }
        }
		else
		{
			if (Input.GetKeyDown(KeyCode.I))
			{
				SwitchToScene("Credits");
			}
		}
	}


}
