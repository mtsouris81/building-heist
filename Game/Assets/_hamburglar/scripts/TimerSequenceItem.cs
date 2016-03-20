using System;

namespace GiveUp.Core
{

    public class TimerSequenceItem
    {
        public static TimerSequenceItem Create(float t, Action cb)
        {
            return new TimerSequenceItem()
            {
                Time = t,
                Callback = cb
            };
        }
        public TimerSequenceItem() { }
        public Action Callback { get; set; }
        public float Time { get; set; }
    }
}
