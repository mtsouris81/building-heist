using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GiveUp.Core
{
	public class LevelResourceList<T> where T : Component
	{
        public string ContainerName { get; private set; }
        public List<T> List { get; private set; }
        public bool IsReady { get; private set; }

        public LevelResourceList(string containerName)
        {
            this.ContainerName = containerName;

            try
            {
                if (containerName != null)
                {
                    var o = GameObject.Find(containerName);
                    this.List = o.GetComponentsInChildren<T>().ToList();
                }
                else
                {
                    this.List = GameObject.FindObjectsOfType<T>().ToList();
                }

                this.IsReady = this.List.Count > 0;
            }
            catch 
            {
                this.List = new List<T>();
                this.IsReady = false;
            }
        }

	}

    public class LevelResource<T> where T : Component
    {
        public string ContainerName { get; private set; }
        public T Instance { get; private set; }
        public bool IsReady { get; private set; }

        public LevelResource(string containerName)
        {
            var o = GameObject.Find(ContainerName);
            this.ContainerName = containerName;

            try
            {
                if (containerName != null)
                {
                    Instance = o.GetComponentInChildren<T>();
                }
                else
                {
                    Instance = GameObject.FindObjectOfType<T>();
                }

                this.IsReady = Instance != null;
            }
            catch
            {
                this.Instance = default(T);
                this.IsReady = false;
            }
        }

    }
}
