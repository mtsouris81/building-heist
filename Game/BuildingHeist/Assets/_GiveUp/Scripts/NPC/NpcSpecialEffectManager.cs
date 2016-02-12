using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GiveUp.Core;
using System;

public class NpcSpecialEffectManager : MonoBehaviour {

    public NpcSpecialEffectRegistration[] RegisteredEffectsArray = null;
    protected Dictionary<string, NpcSpecialEffectRegistration> SpecialEffects = new Dictionary<string, NpcSpecialEffectRegistration>();
    public void RegisterSpecialEffects(NpcCore npc)
    {
        SpecialEffects.Clear();
        foreach (var e in this.RegisteredEffectsArray)
        {
            NpcBurningEffect effect = e.EffectPrefab.GetComponent<NpcBurningEffect>();
            effect.EffectName = e.Name;
            effect.Npc = npc;
            effect.DeActivate();
            SpecialEffects.Add(e.Name, e);
        }
    }
    public void TriggerEffect(string name)
    {
        ClearAllEffects();
        if (SpecialEffects.ContainsKey(name))
        {
            //Debug.Log(name);
            SpecialEffects[name].EffectPrefab.gameObject.SetActive(true);
            SpecialEffects[name].EffectPrefab.gameObject.SendMessage("Activate", SendMessageOptions.DontRequireReceiver);
        }
    }
    public bool SupportsEffect(string name)
    {
        return this.SpecialEffects.ContainsKey(name);
    }
    public void ClearAllEffects()
    {
        foreach (var e in this.SpecialEffects)
        {
            e.Value.EffectPrefab.gameObject.SendMessage("DeActivate", SendMessageOptions.DontRequireReceiver);
        }
    }

    [Serializable]
    public class NpcSpecialEffectRegistration
    {
        public string Name = null;
        public Transform EffectPrefab = null;
    }
}
