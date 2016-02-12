using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GiveUp.Core;

public class CharacterSoundManager : MonoBehaviour {

    protected SoundPack[] Sounds;
    
    protected Dictionary<string, SoundPack> SoundLookup = new Dictionary<string, SoundPack>();

    protected Dictionary<CharacterSoundType, SoundPack> TypedSounds = new Dictionary<CharacterSoundType, SoundPack>();


    public bool Supports(CharacterSoundType t)
    {
        return TypedSounds.ContainsKey(t);
    }


	// Use this for initialization
	void Start () {
        Sounds = this.GetComponentsInChildren<SoundPack>(true);
        foreach (var s in Sounds)
        {
            s.transform.localPosition = Vector3.zero;
            SoundLookup.Add(s.gameObject.name, s);
            AddSound(s.Type, s);
        }
	}

    public void AddSound(CharacterSoundType type, SoundPack pack)
    {
        if (!TypedSounds.ContainsKey(type))
        {
            TypedSounds.Add(type, pack);
        }
        else
        {
            TypedSounds[type] = pack;
        }
    }


    public void Play(CharacterSoundType type)
    {
        if (TypedSounds.ContainsKey(type))
        {
            TypedSounds[type].Play();
        }
    }


    public void PlaySoundFX(string name)
    {
        if (SoundLookup.ContainsKey(name))
        {
            SoundLookup[name].Play();
        }
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
