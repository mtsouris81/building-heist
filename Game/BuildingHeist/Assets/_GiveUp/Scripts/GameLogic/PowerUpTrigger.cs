using UnityEngine;
using System.Collections;
namespace GiveUp.Core
{

    public class PowerUpTrigger : MonoBehaviour
    {

        public Transform Model;


        public void Update()
        {
            if (Model != null)
            {
                Model.transform.Rotate(0, 2, 0);
            }
        }

        public void OnTriggerEnter(Collider col)
        {
            if (col.gameObject != null)
            {
                if (col.gameObject.name == "Hero")
                {
                    SendMessage("PlayerPowerUp", null, SendMessageOptions.RequireReceiver);
                    return;
                }
                if (col.gameObject.tag == NpcUtility.TagName)
                {
                    SendMessage("NpcPowerUp", col.gameObject, SendMessageOptions.RequireReceiver);
                }
            }
        }
    }


}