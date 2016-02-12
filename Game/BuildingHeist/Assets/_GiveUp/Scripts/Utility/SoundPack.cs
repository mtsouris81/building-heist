using UnityEngine;
using System.Collections;
using GiveUp.Core;

public class SoundPack : MonoBehaviour {

    public enum PlayMode
    {
        CutoffSelf,
        ExclusiveWait,
        PlayAll
    }


    public CharacterSoundType Type = CharacterSoundType.None;

    public PlayMode Mode = PlayMode.ExclusiveWait;

    int currPlayIndex = 0;
    AudioSource[] sounds;
	// Use this for initialization
	void Start () {

        sounds = this.GetComponentsInChildren<AudioSource>(true);
        currPlayIndex = 0;
        this.transform.localPosition = Vector3.zero;

        foreach (var s in sounds)
        {
            s.transform.localPosition = Vector3.zero;
            //s.maxDistance = 250;
            //s.minDistance = 0;
            s.rolloffMode = AudioRolloffMode.Linear;
        }
	}

    public void Play()
    {
        if (sounds == null || sounds.Length < 1)
        {
            Debug.Log("sounds are null");
            return;
        }

        if (Mode == PlayMode.ExclusiveWait && AnySoundsArePlaying())
        {
            return;
        }

        if (Mode == PlayMode.CutoffSelf)
        {
            StopAllSounds();
        }

        currPlayIndex++;

        if (currPlayIndex >= sounds.Length || currPlayIndex < 0)
        {
            currPlayIndex = 0;
        }

        if (currPlayIndex < sounds.Length)
        {
            sounds[currPlayIndex].PlayOneShot(sounds[currPlayIndex].clip);
        }
    }

    private void StopAllSounds()
    {
        foreach (var s in this.sounds)
        {
            if (s.isPlaying)
            {
                s.Stop();
            }
        }
    }

    private bool AnySoundsArePlaying()
    {
        foreach (var s in this.sounds)
        {
            if (s.isPlaying)
            {
                return true;
            }
        }
        return false;
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
