using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GiveUp.Core
{
    public class PauseController : FormController
    {

        //WeenusSoft.MouseLook playerMouseLook = null;

        private void EnsureMouseLook()
        {
            //if (playerMouseLook == null)
            //{
            //   // playerMouseLook = this.GetComponent<WeenusSoft.MouseLook>();
            //}
        }
        public override void Start()
        {
            base.Start();
            Text["btnResume"].Clicked += Resume_Clicked;
            Text["btnExit"].Clicked += Exit_Clicked;
        }

        public void Update()
        {
            if (LevelContext.Current.IsPaused)
            {
                EnsureMouseLook();
                //if (playerMouseLook == null || playerMouseLook.SuspendMouse)
                //{
                //    return;
                //}
                LockMouse.LockCursor(false);
            }
        }

        void Resume_Clicked(object sender, EventArgs e)
        {
            LevelContext.Current.UnPause();
            // special mouse stuff
            EnsureMouseLook();
            //if (playerMouseLook == null || playerMouseLook.SuspendMouse)
            //{
            //    return;
            //}
            LockMouse.LockCursor(true);
        }

        void Exit_Clicked(object sender, EventArgs e)
        {
            LevelContext.Current.UnPause();
            LevelContext.Current.GoToLevel("Intro");
        }

    }
}
