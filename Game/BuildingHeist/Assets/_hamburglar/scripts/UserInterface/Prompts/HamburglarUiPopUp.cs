using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class HamburglarUiPopUp : MonoBehaviour
{
    public Text TextItem = null;
    public Button OKButton = null;
    public Image Background = null;

    public Action OnOK { get; set; }

    void Start () 
    {
        OKButton.onClick.AddListener(delegate()
        {
            this.gameObject.SetActive(false);
            if (OnOK != null)
            {
                OnOK();
                OnOK = null;
            }
        });
	}
    public void Show(string message, Color background)
    {
        TextItem.text = message;
        Background.color = background;
        this.gameObject.SetActive(true);
    }
}
