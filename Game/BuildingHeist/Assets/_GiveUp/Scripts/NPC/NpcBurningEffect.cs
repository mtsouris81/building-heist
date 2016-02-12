using UnityEngine;
using System.Collections;
using GiveUp.Core;
using System.Collections.Generic;
using System;

public class NpcBurningEffect : MonoBehaviour {


    public Material Shader = null;
    public float TextureAnimateSpeed = 3;
    public bool AnimateTexture = false;
    public bool ExpireCharacter = false;
    public bool FreezeCharacterAnimation = false;
    public bool FreezeCharacterMovement = false;
    public Transform ActivationObject = null;
    public bool CopyActivationObject = false;
    public float HealthIncrementPerSecond = 0;

    public Transform[] ExternalParticles = null;

    public NpcCore Npc { get; set; }

    public bool IsEffectActive { get; private set; }
    public string EffectName { get; set; }

    float _wrap = 0;

    List< Material> _localMaterialRef = new List<Material>();
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        if (AnimateTexture && IsEffectActive && _localMaterialRef.Count > 0)
        {
            _wrap += Time.deltaTime * TextureAnimateSpeed;
            if (_wrap > 1)
            {
                _wrap = 0 + ( _wrap - 1);
            }
            foreach (var m in _localMaterialRef)
            {
                m.SetTextureOffset("_MainTex", new Vector2(0, _wrap));
            }
        }

        if (IsEffectActive)
        {
            if (HealthIncrementPerSecond != 0)
            {
                float actualDamage = -(HealthIncrementPerSecond * Time.deltaTime);
                this.Npc.ChangeHealth(actualDamage);
                this.Npc.HealthManager.CheckForDeath();
            }
        }
	}


    public void Activate()
    {

        //this.Npc = this.GetComponentInParent<NpcCore>();

        //if (this.Npc == null)
        //{
        //    this.Npc = this.GetComponentInParent<NpcBossCore>();
        //}

        if (this.Npc == null)
        {
            throw new Exception("Npc was not assigned to this effect. Could not find NPC for special effect.");
        }

        NpcEffectExtender extender = this.GetComponent<NpcEffectExtender>();

        if (ActivationObject != null)
        {
            if (CopyActivationObject)
            {
                var t = GameObject.Instantiate(ActivationObject, this.ActivationObject.position, this.ActivationObject.rotation) as Transform;
                t.gameObject.SetActive(true);
                if (extender != null)
                {
                    extender.OnEffectActivating(t);
                }
            }
            else
            {
                ActivationObject.gameObject.SetActive(true);
                if (extender != null)
                {
                    extender.OnEffectActivating(ActivationObject);
                }
            }
        }
        else
        {
            if (extender != null)
            {
                extender.OnEffectActivating(null);
            }
        }


        if (ExpireCharacter)
        {
            this.Npc.HurtNPC(10000);
            this.Npc.Expire(false);
           GameObject.Destroy(  this.Npc.gameObject);
        }

        _localMaterialRef.Clear();

        if (FreezeCharacterAnimation)
        {
            Npc.AnimationManager.FreezeAnimation();
        }
        if (FreezeCharacterMovement)
        {
            Npc.IsFrozen = true;
        }

        if (ExternalParticles != null)
        {
            foreach (var p in ExternalParticles)
            {
                p.gameObject.SetActive(true);
            }
        }

        IsEffectActive = true;
        this.Npc.LastAppliedEffect = this.EffectName;

        if (this.Npc.Renderers == null || this.Shader == null)
            return;


        OriginalMaterials.Clear();
        foreach (var r in this.Npc.Renderers)
        {
            OriginalMaterials.Add(r, new List<Material>());
            Material[] mats = new Material[r.materials.Length];
            for (int i = 0; i < r.materials.Length; i++)
            {
                OriginalMaterials[r].Add(new Material(r.materials[i]));
                mats[i] = new Material(this.Shader);
            }
            r.materials = mats;
            //foreach (var m in r.materials)
            //{
            //    mats.Add(m);
            //}
            //Material frozen = new Material(this.Shader);
            //mats.Add(frozen);
            //r.materials = mats.ToArray();
            //_localMaterialRef.Add(frozen);
        }
    }

    public Dictionary<Renderer, List<Material>> OriginalMaterials = new Dictionary<Renderer, List<Material>>();

    public void DeActivate()
    {
        if (ActivationObject != null)
        {
            ActivationObject.gameObject.SetActive(false);
        }

        if (this.Npc == null || this.Npc.Renderers == null || this.Shader == null)
            return;

        IsEffectActive = false;

        if (FreezeCharacterAnimation)
        {
            Npc.AnimationManager.UnFreezeAnimation();
        }
        if (FreezeCharacterMovement)
        {
            Npc.IsFrozen = false;
        }

        //foreach (var r in Npc.Renderers)
        //{
        //    Material regular = new Material(r.materials[0]); // first is always regular material
        //    r.materials = new Material[]{
        //            regular
        //        };
        //}


        // reset materials
        foreach (var kv in OriginalMaterials)
        {
            kv.Key.materials = kv.Value.ToArray();
        }

        //foreach (var r in this.Npc.Renderers)
        //{
        //    List<Material> mats = new List<Material>();
        //    foreach (var m in r.materials)
        //    {
        //        mats.Add(m);
        //    }
        //    if (mats.Count > 1)
        //    {
        //        mats.RemoveAt(mats.Count - 1); // remove last 'special'
        //    }
        //    r.materials = mats.ToArray();
        //}



        if (ExternalParticles != null)
        {
            foreach (var p in ExternalParticles)
            {
                p.gameObject.SetActive(false);
            }
        }

        _localMaterialRef.Clear();
    }
}
