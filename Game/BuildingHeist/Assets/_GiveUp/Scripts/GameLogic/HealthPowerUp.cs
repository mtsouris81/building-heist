using UnityEngine;
using System.Collections;
namespace GiveUp.Core
{
    public class HealthPowerUp : MonoBehaviour
    {
        public void PlayerPowerUp()
        {
            PlayerUtility.Hero.RestoreHealth();
        }

        public void NpcPowerUp(GameObject npc)
        {
            if (npc == null)
                return;

            NpcCore npcCore = npc.GetComponent<NpcCore>();
            npcCore.HealthManager.RestoreHealth();
        }

        void Update()
        {

        }
    }


}