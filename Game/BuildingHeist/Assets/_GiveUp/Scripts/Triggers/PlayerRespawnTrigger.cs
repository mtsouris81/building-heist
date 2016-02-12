using UnityEngine;
using System.Collections;
using System;
namespace GiveUp.Core
{

    public class PlayerRespawnTrigger : MonoBehaviour
    {
        public Vector3 RespawnPosition { get;set; }

        public void TeleportPlayer()
        {
            PlayerUtility.Current.transform.position = RespawnPosition;
        }

        public void Start()
        {
            Transform t = this.transform.GetChild(0).GetComponent<Transform>();
            try
            {
                RespawnPosition = t.position;
            }
            catch (Exception ex)
            {
                throw new Exception("HEY!!!! Make the player respawn contain a transform you stupid fuck face!!!", ex);
            }
        }

        public void OnTriggerEnter(Collider col)
        {
            if (col.gameObject.name == PlayerUtility.ObjectName)
            {
                LevelContext.Current.AttemptToSetPlayerRespawn(this);
            }
        }
    }


}