using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GiveUp.Core
{
    public class CinematicFrame : MonoBehaviour
	{
        public double Time = 0;
        public string DialogCharacter = string.Empty;
        public string DialogText = string.Empty;
        public int ImageIndex = 0;
        public bool IsImage = false;
        public ImagePanType ImagePan = ImagePanType.None;
	}
}
