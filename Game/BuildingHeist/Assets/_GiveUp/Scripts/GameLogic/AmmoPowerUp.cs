using UnityEngine;
using System.Collections;
namespace GiveUp.Core
{

    public class AmmoPowerUp : MonoBehaviour
    {
        public void PlayerPowerUp()
        {
            PlayerUtility.Hero.GunController.ReloadAllGunAmmo();
        }

        public void NpcPowerUp(GameObject npc)
        {
            //npc.HealthManager.RestoreHealth();
        }

        void Update()
        {

        }
    }


}