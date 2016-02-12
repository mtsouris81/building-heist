using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GiveUp.Core
{
	public class CinematicManager : MonoBehaviour
	{

        //vp_FPInput playerInput;
        
        private AudioListener PlayerAudioListener;

        public bool IsCinematicPlaying
        {
            get
            {
                return (CurrentCinematic != null && CurrentCinematic.IsPlaying);
            }
        }

        public Cinematic CurrentCinematic { get; private set; }

        public delegate void CinematicFinishedHandler();

        public Dictionary<string, CinematicFinishedHandler> CinematicEndActions = new Dictionary<string, CinematicFinishedHandler>();

        public List<Cinematic> Cinematics { get; set; }

        public Cinematic FindCinematic(string name)
        {
            foreach (var c in Cinematics)
            {
                if (c.name.Equals(name, StringComparison.CurrentCultureIgnoreCase))
                {
                    return c;
                }
            }

            return null;
        }

        public void Start()
        {
            Cinematics = GameObject.FindObjectsOfType<Cinematic>().ToList();

            foreach (var c in this.Cinematics)
            {
                c.Finished += c_Finished;
                c.gameObject.SetActive(false);
            }

            //playerInput = PlayerUtility.Current.GetComponent<vp_FPInput>();
            PlayerAudioListener = PlayerUtility.Current.GetComponent<AudioListener>();
        }

        void c_Finished(object sender, EventArgs e)
        {
            Cinematic cinematic = sender as Cinematic;

            //playerInput.enabled = true;
            
            //LevelContext.Current.Cameras.SetGameCameraActive();

            if (cinematic == null)
                return;

            LevelContext.Current.Dialog.Hide();

            if (CinematicEndActions.ContainsKey(cinematic.name))
            {
                CinematicEndActions[cinematic.name]();
                return;
            }

            if (CinematicEndActions.ContainsKey("*"))
            {
                CinematicEndActions["*"]();
                return;
            }
        }

        public void Update()
        {
        }

        public bool Play(string cinematicName)
        {
            CurrentCinematic = FindCinematic(cinematicName);

            if (CurrentCinematic == null)
            {
                //Debug.Log("no cinematic " + cinematicName);
                return false;
            }

            LevelContext.Current.Dialog.Hide();

            CurrentCinematic.gameObject.SetActive(true);

            bool startedSuccessfully = CurrentCinematic.Play();

            if (startedSuccessfully)
            {
                //Debug.Log("started cinematic " + cinematicName);
            }
            else // cinematic wouldn't play for some reason
            {
                //Debug.Log("did not start cinematic " + cinematicName);
                CurrentCinematic.gameObject.SetActive(false);
                CurrentCinematic = null;
            }

            return startedSuccessfully;
        }

	}
}
