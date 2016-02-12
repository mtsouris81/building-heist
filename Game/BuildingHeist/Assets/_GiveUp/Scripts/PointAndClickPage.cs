using UnityEngine;

namespace GiveUp.Core
{
	public class PointAndClickPage : MonoBehaviour
    {
        protected bool HasInit { get; private set; }
        protected LockMouse _lock;
        protected MobileController mobileUi;

        public void MakeScreenActive()
        {
            MakeScreenActive(null);
        }


        public virtual void Activate()
        {
            Init();
            this.MakeScreenActive(this._lock);
        }

        public void MakeScreenActive(LockMouse locker)
        {
            if (locker != null)
            {
                locker.SetActive(false);
            }
            GlobalUI.IsPointAndClickActive = true;
            this.gameObject.SetActive(true);
        }
        public void CloseScreen()
        {
            CloseScreen(true);
        }

        public void CloseScreen(bool closeUI)
        {
            if (closeUI)
            {
                if (_lock != null && mobileUi.Mode == MobileController.MobileControllerMode.DesktopOnly)
                {
                    _lock.SetActive(true);
                }
                GlobalUI.IsPointAndClickActive = false;
            }
            this.gameObject.SetActive(false);
        }

        public SimpleEventButton[] GetButtons(Transform t)
        {
            return t.GetComponentsInChildren<SimpleEventButton>(true);
        }

        public virtual void Init()
        {
            if (HasInit)
                return;

            mobileUi = GameObject.FindObjectOfType<MobileController>();
            _lock = GameObject.FindObjectOfType<LockMouse>();
            HasInit = true;
        }

        protected int IndexOf(SimpleEventButton b, SimpleEventButton[] array)
        {
            for (int i = 0; i < array.Length; i++)
            {
                if (b == array[i])
                    return i;
            }
            return -1;
        }

	}
}
