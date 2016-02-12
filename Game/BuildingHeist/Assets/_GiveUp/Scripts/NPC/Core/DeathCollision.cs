using UnityEngine;
using System.Collections;
namespace GiveUp.Core
{

    public class DeathCollision : MonoBehaviour
    {
        public AudioSource DeathSound = null;
        public void OnTriggerEnter(Collider col)
        {
            if (col.gameObject.tag == PlayerUtility.TagName)
            {
                Debug.Log("kill!");
                PlayerUtility.Hero.KillPlayer();
                if (DeathSound != null)
                {
                    DeathSound.Play();
                }
            }
        }
    }


}