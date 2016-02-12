using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GiveUp.Core
{
	public class Cinematic : MonoBehaviour
    {

        TimedInterpolation ImageAnimation;
        GUITexture FullScreenTexture = null;
        public Texture[] SceneTextures = new Texture[] { };
        private Vector3 StartScale = new Vector3(1, 1, 1);
        private Vector3 EndScale = new Vector3(1.18f, 1.18f, 1);
        private Vector3 StartPosition = new Vector3(0.5f, 0.5f, 1);
        private Vector3 EndPosition = new Vector3(0.55f, 0.55f, 1);

        public Cinematic()
        {

        }

        CinematicFrame lastImageFrame = null;

        private void OnImageFrameActive(CinematicFrame frame)
        {
            if (lastImageFrame != null)
            {
                lastImageFrame.gameObject.SetActive(false);
            }

            if (frame.IsImage && frame.ImageIndex < SceneTextures.Length && FullScreenTexture != null)
            {
                ImageAnimation.Reset();
                ImageAnimation.Start();
                FullScreenTexture.texture = SceneTextures[frame.ImageIndex];
                FullScreenTexture.gameObject.SetActive(true);
                lastImageFrame = frame;
            }
        }

        private void OnDialogFrameActive(CinematicFrame frame)
        {
            Debug.Log(string.Format("TEXT Frame Active {0}", frame.gameObject.name));
            LevelContext.Current.Dialog.SetText(frame.DialogText);
        }

        public float Duration;
        public bool PlayOnlyOnce = true;
        public bool CanBeSkipped = true;

        public CinematicFrameCollection ImageFrames { get; set; }
        public CinematicFrameCollection DialogFrames { get; set; }
        public ActionTimer Timer { get; set; }
        public void Start()
        {
            ImageFrames = new CinematicFrameCollection();
            DialogFrames = new CinematicFrameCollection();
            ImageFrames.FrameActiveCallback = OnImageFrameActive;
            DialogFrames.FrameActiveCallback = OnDialogFrameActive;
            CinematicFrame[] frames = GetComponentsInChildren<CinematicFrame>(true);
            
            foreach (CinematicFrame f in frames)
            {
                if (f.IsImage)
                {
                    ImageFrames.Frames.Add(f);
                }
                else
                {
                    DialogFrames.Frames.Add(f);
                }
            }

            ImageFrames.SortFrames();
            DialogFrames.SortFrames();

            Debug.Log(string.Format("TEXT FRAMES {0}", DialogFrames.Frames.Count));

            Timer = new ActionTimer(Duration, OnFinished);
            Timer.AccurateMode = true;
            Reset();
            Timer.Start();
            IsPlaying = true;

            if (ImageFrames.Frames.Count > 0)
            {
                GameObject obj = new GameObject();
                FullScreenTexture = obj.AddComponent<GUITexture>();
                obj.SetActive(false);
                FullScreenTexture.transform.position = new Vector3(0.5f, 0.5f, 9);
                FullScreenTexture.transform.localScale = new Vector3(1, 1, 9);
                ImageAnimation = new TimedInterpolation(30);
                ImageAnimation.AccurateMode = true;
                ImageAnimation.Start();
            }
        }


        public void Update()
        {
            if (IsPlaying)
            {
                if (Input.GetButton("Reload"))
                {
                    Timer.ForceEnd();
                }

                Timer.Update();
                ImageFrames.Update(Timer.Elapsed);
                DialogFrames.Update(Timer.Elapsed);

                if (FullScreenTexture != null && ImageAnimation != null)
                {
                    ImageAnimation.Update();

                    FullScreenTexture.transform.localScale =
                        Vector3.Lerp(StartScale, EndScale, ImageAnimation.Ratio);

                    //FullScreenTexture.transform.localPosition =
                    //    Vector3.Lerp(StartPosition, EndPosition, ImageAnimation.Ratio);
                }
            }
        }



        public bool IsPlaying { get; private set; }
        public bool HasPlayed { get; private set; }
        public event EventHandler Finished;

        public void Reset()
        {
            IsPlaying = false;
            HasPlayed = false;
            if (Timer != null)
            {
                Timer.Reset();
            }
        }
        protected void OnFinished()
        {
            HasPlayed = PlayOnlyOnce;

            this.gameObject.SetActive(false);

            if (FullScreenTexture != null)
            {
                FullScreenTexture.gameObject.SetActive(false);
            }

            IsPlaying = false;

            if (Finished != null)
                Finished(this, EventArgs.Empty);
        }
        public bool Play()
        {
            if (IsPlaying)
                return false;

            if (PlayOnlyOnce && HasPlayed)
                return false;

            IsPlaying = true;

            return true;
        }

        public void GVP(string arg)
        {
            //if (AnimationEventHandlers.ContainsKey(arg))
            //{
            //    AnimationEventHandlers[arg]();
            //}
        }
        //public delegate void HandleAnimationEvent();
        //public Dictionary<string, HandleAnimationEvent> AnimationEventHandlers { get; set; }

	}

}
