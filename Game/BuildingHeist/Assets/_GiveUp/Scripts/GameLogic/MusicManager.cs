using UnityEngine;
using System.Collections;
using GiveUp.Core;
using System.Collections.Generic;
using System;

public class MusicManager : MonoBehaviour {


    public float BackgroundMusicVolume = 0.75f;

    public enum LevelMusicType
    {
        Level,
        Intense,
        Boss
    }

    Dictionary<string, AudioSource> NamedAudioSources = new Dictionary<string, AudioSource>();
    Dictionary<string, LevelMusicType> LevelMusicTypeMappings = new Dictionary<string, LevelMusicType>(StringComparer.OrdinalIgnoreCase);
    AudioSource LevelMusic;
    AudioSource IntenseMusic;
    AudioSource BossMusic;
    AudioSource[] audios;

    AudioSource CurrentSong;

    MusicFade SongFadeIn = new MusicFade(10);
    MusicFade SongFadeOut = new MusicFade(10);

    public class MusicFade
    {
        public TimedInterpolation Timer;

        public bool IsActiveFade { get; private set; }

        public MusicFade(float _fadeTime)
        {
            Timer = new TimedInterpolation(_fadeTime);
            Timer.TimerExpireCallback = FadeFinished;
            FadeTime = _fadeTime;
        }

        public AudioSource Song;
        public bool IsFadingIn;
        public float FadeTime;
        public void SetForFade(AudioSource audio, bool isFadingIn)
        {
            Timer.Reset();
            Timer.Start();
            IsFadingIn = isFadingIn;
            Song = audio;
            IsActiveFade = true;

            if (IsFadingIn)
            {
                Song.volume = 0;

                if (!Song.isPlaying)
                    Song.Play();
            }
            else
            {
                Song.volume = 1;
            }
        }

        void FadeFinished()
        {
            IsActiveFade = false;
            if (!IsFadingIn)
            {
                if (Song.isPlaying)
                    Song.Stop();
            }
        }
    }

    public LevelMusicType CurrentMusicType { get; private set; }

    float _lastSetVolume = 0;

    public MusicManager() : base()
    {
        LevelMusicTypeMappings.Add("Level", LevelMusicType.Level);
        LevelMusicTypeMappings.Add("Intense", LevelMusicType.Intense);
        LevelMusicTypeMappings.Add("Boss", LevelMusicType.Boss);
    }

    public void StopAllMusic()
    {
        try
        {
            if (CurrentSong != null)
            {
                CurrentSong.Stop();
            }
        }
        catch (Exception ex)
        {
            Debug.Log("Couldn't stop song. " + ex.Message);
        }
    }

    public void PlayBackgroundMusic(LevelMusicType t)
    {
        PlayBackgroundMusic(t.ToString());
    }
    public bool PlayBackgroundMusic(string name)
    {
        if (LevelMusic == null)
            return false;

        AudioSource newSong = NamedAudioSources[name];
        
        if (newSong == null)
            return false;

        if (newSong == CurrentSong)
            return false;

        #region HARD CUTS BETWEEN SONGS

        if (CurrentSong != null && CurrentSong.isPlaying)
        {
            CurrentSong.Stop();
        }

        newSong.volume = BackgroundMusicVolume;
        newSong.Play();

        CurrentSong = newSong;

        //Debug.Log(string.Format("new song {0}", CurrentSong));

        #endregion



        #region CROSS FADES ( NOT SUPPORTED YET )



        #endregion


        if (LevelMusicTypeMappings.ContainsKey(name))
        {
            CurrentMusicType = LevelMusicTypeMappings[name];
        }
        else
        {
            CurrentMusicType = LevelMusicType.Level;
        }

        return true;
    }

	// Use this for initialization
	void Start () {

            audios = this.GetComponentsInChildren<AudioSource>();
            if (audios.Length > 0)
            {
                foreach (var a in audios)
                {
                    if (a.gameObject.name.Equals("Level", System.StringComparison.CurrentCultureIgnoreCase))
                    {
                        LevelMusic = a;
                    }
                    else if (a.gameObject.name.Equals("Intense", System.StringComparison.CurrentCultureIgnoreCase))
                    {
                        IntenseMusic = a;
                    }
                    else if (a.gameObject.name.Equals("Boss", System.StringComparison.CurrentCultureIgnoreCase))
                    {
                        BossMusic = a;
                    }

                    NamedAudioSources.Add(a.gameObject.name, a);
                }
            }

            if (LevelMusic != null)
            {
                if (IntenseMusic == null)
                {
                    IntenseMusic = LevelMusic;
                }

                if (BossMusic == null)
                {
                    BossMusic = LevelMusic;
                }

            }
        
	}
	
	// Update is called once per frame
    void FixedUpdate()
    {
        isFade = false;
        if (SongFadeIn.IsActiveFade)
        {
            isFade = true;
            SongFadeIn.Timer.Update();
            SongFadeIn.Song.volume = Mathf.Lerp(0, BackgroundMusicVolume, SongFadeIn.Timer.Ratio);
            if (SongFadeIn.Song.isPlaying)
            {
                SongFadeIn.Song.Play();
            }
        }
        if (SongFadeOut.IsActiveFade)
        {
            isFade = true;
            SongFadeOut.Timer.Update();
            SongFadeOut.Song.volume = Mathf.Lerp(BackgroundMusicVolume, 0, SongFadeOut.Timer.Ratio);
        }


        //if (BackgroundMusicVolume != XmasGameOptions.Instance.MusicVolume)
        //{
        //    BackgroundMusicVolume = XmasGameOptions.Instance.MusicVolume;
        //    if (!isFade && CurrentSong != null && CurrentSong.isPlaying)
        //    {
        //        CurrentSong.volume = BackgroundMusicVolume;
        //    }
        //}
	}

    bool isFade = false;
}
