using UnityEngine;
using System.Collections;
namespace GiveUp.Core
{
    public class CinematicTrigger : MonoBehaviour
    {
        public string CinematicName;

        public void OnTriggerEnter(Collider col)
        {
            LevelContext.Current.PlayCinematic(CinematicName);
        }
    }
}