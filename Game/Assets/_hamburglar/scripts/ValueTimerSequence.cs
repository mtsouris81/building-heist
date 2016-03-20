using System;

namespace GiveUp.Core
{
    public class ValueTimerSequence<T> : TimerSequence
    {
        public T StartValue { get; set; }
        public T EndValue { get; set; }
        public T GetCurrentValue()
        {
            return Lerp(StartValue, EndValue, this.Timer.Ratio);
        }
        public Func<T, T, float, T> Lerp { get; set; }
        public void StepCallback(T value)
        {
            StartValue = EndValue;
            EndValue = value;
        }
        public float[] ValueSeeds;
        public Func<float, T> DetermineValue { get; set; }
        public ValueTimerSequence(float[] times, float[] valueSeeds)
        {
            var array = new TimerSequenceItem[times.Length];
            for (int i = 0; i < times.Length; i++)
            {
                var seed = valueSeeds[i];
                array[i] = new TimerSequenceItem()
                {
                    Time = times[i],
                    Callback = () =>
                    {
                        StepCallback(DetermineValue(seed));
                    }
                };
            }
            this.Populate(array);
        }

    }
}
