using UnityEngine;
using System.Collections;
namespace GiveUp.Core
{
    public class CreditsScreenController : MonoBehaviour
    {
        public string NextScreen = "Intro";
        public float maxY = 4;
        public Vector3 moveInterval = new Vector3(0, 0.005f, 0);

        GUIText[] AllText;
        GameObject Credits;
        // Use this for initialization
        void Start()
        {
            return;
            Credits = GameObject.Find("CreditsText");
            TextAsset text = Resources.Load<TextAsset>("Credits");
            Credits.GetComponent<GUIText>().text = text.text;
            AllText = GameObject.FindObjectsOfType<GUIText>();
        }

        // Update is called once per frame
        void Update()
        {
            return;
            if (Credits.transform.position.y > maxY)
            {
                Application.LoadLevel("Intro");
                return;
            }

            foreach (var t in AllText)
            {
                t.transform.position += moveInterval;
            }
        }
    }
}