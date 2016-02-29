using System;
namespace GiveUp.Core
{
	public class ActionTimer
	{
        private bool _accurateStartRequest = false;
        float _accurateLastTime = 0;
        float _accurateStartTime = 0;
        public bool AccurateMode { get; set; }

        public float Ratio
        {
            get
            {
                if (!AlternateDirection)
                {
                    if (_elapsed == 0 || TimeLimit == 0)
                        return 0;

                    return _elapsed / TimeLimit;
                }
                else
                {
                    if (_elapsed == 0 || TimeLimit == 0)
                        return _isLoopBack ? 1 : 0;

                    return _isLoopBack
                            ? 1 - (_elapsed / TimeLimit)
                            : _elapsed / TimeLimit;
                }
            }
        }
        public bool Loop { get; set; }
		float _elapsed = 0;


		public delegate void TimerExpireDelegate();

		public float TimeLimit = 0;

		public TimerExpireDelegate TimerExpireCallback ;

        public bool Enabled { get; private set; }
        public float Elapsed { get { return _elapsed; } }

		public ActionTimer (float time, TimerExpireDelegate callback)
		{
			TimeLimit = time;
			TimerExpireCallback = callback;
			Enabled = false;
		}

		public void Start()
		{
            if (AccurateMode)
            {
                _accurateLastTime = UnityEngine.Time.realtimeSinceStartup;
                _accurateStartTime = _accurateLastTime;
            }
			Enabled = true;
		}

		public void Reset()
		{
			_elapsed = 0;

            if (AccurateMode)
            {
                _accurateLastTime = UnityEngine.Time.realtimeSinceStartup;
                _accurateStartTime = _accurateLastTime;
            }
		}

		public void Stop()
		{
			Enabled = false;
		}

		public void Update()
		{
			if (!Enabled)
				return;

            if (AccurateMode)
            {
                //_elapsed += UnityEngine.Time.realtimeSinceStartup - _accurateLastTime;
                _elapsed = UnityEngine.Time.realtimeSinceStartup - _accurateStartTime;
                _accurateLastTime = UnityEngine.Time.realtimeSinceStartup;
            }
            else
            {
                _elapsed += UnityEngine.Time.deltaTime;
            }

			if (_elapsed >= TimeLimit)
			{
                if (Loop)
                {
                    if (AlternateDirection)
                    {
                        _isLoopBack = !_isLoopBack;
                    }
                    Reset();
                }
                else
                {
                    Stop();
                }                
                
                if (TimerExpireCallback != null)
    				TimerExpireCallback();
			}

		}


        public void ForceEnd()
        {
            Reset();
            Stop();

            if (TimerExpireCallback != null)
                TimerExpireCallback();
        }

        bool _isLoopBack = false;

        public bool AlternateDirection { get; set; }
    }
}

