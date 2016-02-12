using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GiveUp.Core
{
    public class StartPageController : FormController
    {
        public override void Start()
        {
            base.Start();
            LockMouse.LockCursor(false);
            Text["btnStart"].Clicked += Start_Clicked;
            Text["btnOptions"].Clicked += Options_Clicked;
        }

        void Start_Clicked(object sender, EventArgs e)
        {
            LevelContext.ClearCurrent();
            PlayerUtility.ClearCurrent();
            Application.LoadLevel(0);
        }
        void Options_Clicked(object sender, EventArgs e)
        {
        }

    }
}
