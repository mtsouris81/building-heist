using UnityEngine;
using System.Collections;
using GiveUp.Core;

public class NpcHealthDisplay : MonoBehaviour {

    public Transform HealthQuad = null;
    public Transform BackgroundQuad = null;
    public float HealthMaxWidth = 2;

	// Use this for initialization
	void Start () {
	
	}

    public void SetHealthRatio(float currentHealth, float maxHealth)
    {
        float ratio = 0;

        if (currentHealth < 0)
        {
            currentHealth = 0;
        }
        if (currentHealth == 0 || maxHealth == 0)
        {
            ratio = 0;
        }
        else
        {
            ratio = currentHealth / maxHealth;
        }

        if (ratio == 0)
        {
            if (this.enabled)
            {
                this.gameObject.SetActive(false);
            }
        }
        else
        {
            if (!this.enabled)
            {
                this.gameObject.SetActive(true);
            }
        }

        HealthQuad.transform.localScale = new Vector3(
                                                (HealthMaxWidth * ratio),
                                                HealthQuad.transform.localScale.y,
                                                HealthQuad.transform.localScale.z);
    }

	// Update is called once per frame
	void Update () {
        this.transform.LookAt(Camera.main.transform.position, Vector3.up);
	}
}
