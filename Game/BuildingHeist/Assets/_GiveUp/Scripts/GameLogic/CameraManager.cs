using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GiveUp.Core
{
	public class CameraManager
	{

        public Camera CurrentCamera { get; private set; }
        List<Camera> Cameras = new List<Camera>();
        public Camera PlayerCamera;

        public CameraManager()
        {
            PlayerCamera = PlayerUtility.Current.GetComponentInChildren<Camera>();

            this.Cameras = GameObject.FindObjectsOfType<Camera>().ToList();
        }

        public void DisableAll()
        {
            foreach (var c in this.Cameras)
            {
                c.enabled = false;
            }
        }

        public void SetGameCameraActive()
        {
            DisableAll();
            PlayerCamera.enabled = true;
            CurrentCamera = PlayerCamera;
        }

        public void SetCameraActive(Camera camera)
        {
            DisableAll();
            camera.enabled = true;
            CurrentCamera = camera;
        }
	}
}
