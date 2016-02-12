using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Tsouris.StoryBuilder.StoryParts;

namespace Weenus
{
	public static class weenusExtensions
	{
        public static Vector3 ToVector3(this MeetingLocation loc)
        {
            return new Vector3(loc.Position.X, loc.Position.Y, loc.Position.Z);
        }
	}
}
