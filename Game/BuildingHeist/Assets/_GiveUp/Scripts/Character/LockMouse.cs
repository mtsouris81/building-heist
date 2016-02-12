using UnityEngine;
using System.Collections;
using GiveUp.Core;

public class LockMouse : MonoBehaviour
{
    public bool InitiallyEnabled = false;
    public bool InitiallyChecking = true;

    bool _isChecking = false;


    public void SetActive(bool active)
    {
        Screen.lockCursor = active;
        _isChecking = active;
    }

	void Start()
	{
        LockCursor(InitiallyEnabled);
        _isChecking = InitiallyChecking;
	}


    void Update()
    {
        if (_isChecking && GlobalUI.AllowLockedCursor)
        {
            if (Input.GetMouseButtonDown(0) && !Screen.lockCursor)
            {
                LockCursor(true);
            }
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            LockCursor(false);
        }
    }
    
    public static void LockCursor(bool lockCursor)
    {
    	Screen.lockCursor = lockCursor;
    }
}