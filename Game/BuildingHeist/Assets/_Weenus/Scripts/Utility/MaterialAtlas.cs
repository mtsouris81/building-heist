using UnityEngine;
using System.Collections;
using System;

public class MaterialAtlas : MonoBehaviour {

    public LookUpEntry[] Entries = null;

    public T GetRandom<T>() where T : class
    {
       if (Entries == null || Entries.Length == 0)
            return null;

       return Entries[UnityEngine.Random.Range(0, Entries.Length - 1)].Item as T;
    }

    public T Get<T>(string name) where T : class
    {
        if (Entries == null || Entries.Length == 0)
        {
            throw new Exception(this.gameObject.name + " material atlas has no entries");
        }
        for (int i = 0; i < Entries.Length; i++)
        {
            if (Entries[i].Name.Equals(name, System.StringComparison.OrdinalIgnoreCase))
            {
                return Entries[i].Item as T;
            }
        }
        return null;
    }

    [Serializable]
    public class LookUpEntry
    {
        public string Name = "";
        public UnityEngine.Object Item = null;
    }

    public T Get<T>(int index) where T : class
    {
        if (Entries == null || Entries.Length == 0)
        {
            throw new Exception(this.gameObject.name + " material atlas has no entries");
        }

        if (index >= Entries.Length)
        {
            throw new Exception(this.gameObject.name + " material atlas index out of bounds");
        }

        return Entries[index].Item as T;
    }
}
