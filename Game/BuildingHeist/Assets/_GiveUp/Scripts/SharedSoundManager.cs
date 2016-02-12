using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SharedSoundManager : MonoBehaviour
{

    public static SharedSoundManager GetCurrent()
    {
        return GameObject.FindObjectOfType<SharedSoundManager>();
    }

    protected SoundPack[] Sounds;

    protected Dictionary<string, SoundPack> SoundLookup = new Dictionary<string, SoundPack>();

    // Use this for initialization
    void Start()
    {
        Sounds = this.GetComponentsInChildren<SoundPack>(true);
        foreach (var s in Sounds)
        {
            s.transform.localPosition = Vector3.zero;
            SoundLookup.Add(s.gameObject.name, s);
        }
    }

    public void Play(string name)
    {
        Play(name, null);
    }

    public void Play(string name, Vector3? position)
    {
        if (SoundLookup.ContainsKey(name))
        {
            if (position.HasValue)
            {
                SoundLookup[name].transform.position = position.Value;
            }
            else
            {
                SoundLookup[name].transform.localPosition = Vector3.zero;
            }
            SoundLookup[name].Play();
        }
    }
}
