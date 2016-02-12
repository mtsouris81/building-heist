using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GiveUp.Core
{
    public class NpcBossAnimationControlOwner : MonoBehaviour
	{
        public Transform Owner;
        NpcBossCore Boss;

        public void Start()
        {
            Boss = Owner.GetComponent<NpcBossCore>();
        }

        public void Attack1()
        {
            Owner.SendMessageUpwards("Attack1");
        }
        public void Attack2()
        {
            Owner.SendMessageUpwards("Attack2");
        }
	}
}
