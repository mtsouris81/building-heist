using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GiveUp.Core
{
	public static class GlobalUI
	{
        public static bool IsPointAndClickActive { get; set; }
        public static bool AllowLockedCursor
        {
            get { return !IsPointAndClickActive; }
        }
	}
}
