using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TutorialPage : MonoBehaviour {

    [TextAreaAttribute]
    public string TextContent = null;
    public bool ButtonEnabled = true;
    public Button Button = null;
    public Text BackText = null;
    public Text ForeText = null;

    public void SetText(string message)
    {
        BackText.text = message;
        ForeText.text = message;
    }
	void Start () {
        SetText(this.TextContent);
        Button.gameObject.SetActive(ButtonEnabled);
        if (ButtonEnabled)
        {
            Button.onClick.AddListener(() =>
            {
                var tutorial = GameObject.FindObjectOfType<HamburglarTutorial>();
                if (tutorial != null)
                {
                    tutorial.NextStep();
                }
            });
        }
	}
}
