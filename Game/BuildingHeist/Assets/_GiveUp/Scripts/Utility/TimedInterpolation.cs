using UnityEngine;
using System.Collections;
namespace GiveUp.Core
{
    public class TimedInterpolation : ActionTimer
    {
        public TimedInterpolation(float time)
            : base(time, null)
        {
        }
    }
}