using UnityEngine;
using System.Collections;

namespace GiveUp.Core
{
	public class HurtInfo
	{
        public float Damage { get; set; }
        public float Force { get; set; }
        public bool CausesExplosion { get; set; }
        public Vector3 Position { get; set; }
        public Vector3 Normal { get; set; }
        public GameObject Owner { get; set; }
        public NpcHurtType HurtType { get; set; }
	}

    public enum NpcHurtType
    {
        None = 0,
        GunShot = 1,
        Freeze = 2,
        Fire = 3,
        Explode = 4
    }
}
