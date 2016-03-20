using System;

namespace GiveUp.Core
{
    
    public class TimerSequence
    {
        public SelectionList<TimerSequenceItem> Items { get; protected set; }
        public ActionTimer Timer { get; private set; }
        public Action FinishedCallback { get; set; }
        public void StartSequence()
        {
            Timer.Reset();
            Items.ResetIndex();
            Timer.TimeLimit = Items[0].Time;
            Timer.Start();
        }
        public void Populate(params TimerSequenceItem[] items)
        {
            Items = new SelectionList<TimerSequenceItem>();
            Items.AddRange(items);
            Timer = new ActionTimer(Items[0].Time, () =>
            {
                if (Items.IsNextAvailable())
                {
                    Items.Next();
                    Timer.TimeLimit = Items.CurrentValue.Time;
                    Timer.Reset();
                    Timer.Start();
                }
                else
                {
                    if (FinishedCallback != null)
                    {
                        FinishedCallback();
                        return;
                    }
                }
                Items.CurrentValue.Callback();
            });
        }
        public void Update()
        {
            Timer.Update();
        }
    }

}
