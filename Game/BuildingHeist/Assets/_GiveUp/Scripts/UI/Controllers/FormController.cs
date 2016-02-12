using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GiveUp.Core
{

    public class FormController : MonoBehaviour
    {

        public Dictionary<string, GameImageButton> Images = new Dictionary<string, GameImageButton>();
        public Dictionary<string, GameTextButton> Text = new Dictionary<string, GameTextButton>();
        public Dictionary<string, Game3DTextButton> Text3D = new Dictionary<string, Game3DTextButton>();

        public virtual void Start()
        {
            AssignUIComponents<GameImageButton>(Images);
            AssignUIComponents<GameTextButton>(Text);
            AssignUIComponents<Game3DTextButton>(Text3D);
        }

        private void AssignUIComponents<T>(Dictionary<string, T> collection) where T : Component
        {
            List<T> list = this.GetComponentsInChildren<T>().ToList();
            foreach (T t in list)
            {
                collection.Add(t.name, t);
            }
        }






    }
}
