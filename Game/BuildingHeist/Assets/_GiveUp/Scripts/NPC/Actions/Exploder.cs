using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GiveUp.Core;

public class Exploder : MonoBehaviour {

    public Transform ParticlesPrefab = null;
    public float Radius = 30;
    public int TotalSphereCasts = 500;

    public static Vector3[] PointCollection = null;

    private bool hasExploded = false;

	// Use this for initialization
	void Start () {
        if (PointCollection == null)
        {
            PointCollection = UniformPointsOnSphere(TotalSphereCasts);
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    List<Transform> spawnedParticles = new List<Transform>();

    public void SpawnParticles(Vector3 pos)
    {
        Transform t = GameObject.Instantiate(this.ParticlesPrefab, pos, Quaternion.identity) as Transform;
        if (t != null)
        {
            spawnedParticles.Add(t);
        }
    }


    public void ClearExplosion()
    {
        foreach (var p in spawnedParticles)
        {
            p.gameObject.SetActive(false);
            GameObject.Destroy(p.gameObject);
        }

    }

    public void Explode(float damage)
    {
        if (hasExploded)
        {
            return;
        }

        hasExploded = true;
        Vector3 workingPosition = this.transform.position + new Vector3(0, 2.5f, 0);
        RaycastHit[] hits = null;
        foreach (var p in PointCollection)
        {
            hits = Physics.RaycastAll(workingPosition, p, Radius);
            if (hits != null && hits.Length > 0)
            {
                SpawnParticles(hits[hits.Length - 1].point);
            }
        }

        // try to hit player
        Vector3 dir = Vector3.Normalize(LevelContext.Current.Hero.transform.position - workingPosition);
        hits = Physics.RaycastAll(workingPosition, dir, this.Radius);
        if (hits.Length > 0)
        {
            if (hits[hits.Length - 1].transform.gameObject == LevelContext.Current.Hero.gameObject)
            {
                LevelContext.Current.Hero.AcceptHurt(new HurtInfo()
                {
                    Damage = damage,
                    Force = 200,
                    Owner = this.gameObject,
                    Position = workingPosition
                });
            }
        }

    }

    public static Vector3[] UniformPointsOnSphere(float numberOfPoints)
    {
        List<Vector3> points = new List<Vector3>();
        float i = Mathf.PI * (3 - Mathf.Sqrt(5));
        float offset = 2 / numberOfPoints;
        float halfOffset = 0.5f * offset;
        float y = 0;
        float r = 0;
        float phi = 0;
        int currPoint = 0;
        for (; currPoint < numberOfPoints; currPoint++)
        {
            y = currPoint * offset - 1 + halfOffset;
            r = Mathf.Sqrt(1 - y * y);
            phi = currPoint * i;
            Vector3 point = new Vector3(Mathf.Cos(phi) * r, y, Mathf.Sin(phi) * r);
            if (!points.Contains(point)) 
                points.Add(point);
        }
        return points.ToArray();
    }
}
