using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GiveUp.Core
{
	public class CinematicFrameCollection
	{
        public CinematicFrameCollection()
        {
            Frames = new List<CinematicFrame>();
        }
        public void SortFrames()
        {
            Frames = Frames.OrderBy(x => x.Time).ToList();
        }
        public delegate void OnFrameActive(CinematicFrame frame); 
        public List<CinematicFrame> Frames { get; private set; }
        public int CurrentIndex { get; private set; }
        public OnFrameActive FrameActiveCallback { get; set; }
        public void Update(double sceneTime)
        {
            if (Frames == null || CurrentIndex >= Frames.Count)
            {
                return;
            }
            if (sceneTime >= Frames[CurrentIndex].Time)
            {
                FrameActiveCallback(Frames[CurrentIndex]);
                CurrentIndex++;
            }
        }
        public void Reset()
        {
            CurrentIndex = 0;
        }
	}
}
