using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GiveUp.Core
{
	public class DialogDisplay : MonoBehaviour
	{
        GUITexture CharacterBackground;
        GUITexture DialogBackground;
        GUITexture CharacterPicture;
        GUIText DialogText;

        public virtual void Start()
        {
            GUITexture[] textures = this.GetComponentsInChildren<GUITexture>(true);
            DialogText = this.GetComponentsInChildren<GUIText>(true)[0];

            foreach (var i in textures)
            {
                if (i.name.Equals("character background", System.StringComparison.CurrentCultureIgnoreCase))
                {
                    CharacterBackground = i;
                }
                if (i.name.Equals("character picture", System.StringComparison.CurrentCultureIgnoreCase))
                {
                    CharacterPicture = i;
                }
                if (i.name.Equals("dialog background", System.StringComparison.CurrentCultureIgnoreCase))
                {
                    DialogBackground = i;
                }
            }
        }
        public void SetDialogEnabled(bool enabled)
        {
            if (CharacterBackground != null)
            {
                CharacterBackground.gameObject.SetActive(enabled);
            }
            if (DialogBackground != null)
            {
                DialogBackground.gameObject.SetActive(enabled);
            }
            if (CharacterPicture != null)
            {
                CharacterPicture.gameObject.SetActive(enabled);
            }
            if (DialogText != null)
            {
                DialogText.gameObject.SetActive(enabled);
            }
        }
        public void SetText(string text)
        {
            SetDialogEnabled(true);

            if (DialogText != null)
            {
                Debug.Log(string.Format("show text : {0}", text));
                DialogText.text = text;
            }
        }
        public void Hide()
        {
            SetDialogEnabled(false);
        }
	}
}
