using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GiveUp.Core
{
	public static class GameObjectUtilities
	{

        public static void SetEmission(this ParticleSystem particles, bool enabled)
        {
            var e = particles.emission;
            e.enabled = enabled;
        }

        public static T GetComponentFromParent<T>(Transform currentNode, int maxParentsToCheckForCamera) where T : UnityEngine.Component
        {
            T result = null;

            Transform nodeParent = currentNode.parent;

            for (int i = 0; i < maxParentsToCheckForCamera; i++)
            {
                if (nodeParent == null)
                    break;

                result = nodeParent.GetComponent<T>();

                if (result != null)
                    break;

                nodeParent = nodeParent.parent;
            }

            return result;
        }

        public static void GetAllComponentsInChildTree<T>(Transform obj, List<T> result) where T : Component
        {
            if (obj == null)
            {
                return;
            }
            var c = obj.GetComponent<T>();

            if (c != null)
            {
                result.Add(c);
            }

            foreach (Transform child in obj.transform)
            {
                if (child == obj)
                {
                    continue;
                }
                GetAllComponentsInChildTree<T>(child, result);
            }
        }


        //public static T GetComponentFromParent<T>(Transform currentNode, int maxParentsToCheckForCamera) where T : UnityEngine.Component
        //{
        //    T result = null;

        //    Transform nodeParent = currentNode.parent;

        //    for (int i = 0; i < maxParentsToCheckForCamera; i++)
        //    {
        //        if (nodeParent == null)
        //            break;

        //        result = nodeParent.GetComponent<T>();

        //        if (result != null)
        //            break;

        //        nodeParent = nodeParent.parent;
        //    }

        //    return result;
        //}


        public static GameObject InstantiatePrefab(UnityEngine.Object original, Vector3 position, Quaternion rotation)
        {
            Transform transform = GameObject.Instantiate(original, position, rotation) as Transform;
            return transform.gameObject;
        }

        public static T InstantiatePrefab<T>(UnityEngine.Object original, Vector3 position, Quaternion rotation) where T : UnityEngine.Component
        {
            Transform transform = GameObject.Instantiate(original, position, rotation) as Transform;
            return transform.gameObject.GetComponent<T>();
        }



	}
}
