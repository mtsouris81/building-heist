using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FloatingMessage : MonoBehaviour {


    public Text BackText = null;
    public Text FrontText = null;
    public Graphic BackgroundGraphic = null;
    public Color DefaultColor = Color.white;

    public float Speed = 0.2f;
    public Vector2 SpawnPosition = new Vector2(50, 50);
    RectTransform rect;

    public void SetColor(Color color)
    {
        if (BackgroundGraphic == null)
            return;

        BackgroundGraphic.color = color;
    }
    public void SetText(string content)
    {
        BackText.text = content;
        FrontText.text = content;
    }
    void Update()
    {
        rect.anchoredPosition += Vector2.down * (Speed * Time.deltaTime);
        if (rect.anchoredPosition.y < -50)
        {
            GameObject.Destroy(this.gameObject); // kill meeeee!!!!
        }
    }
    public void StartDisplay()
    {
        rect = GetComponent<RectTransform>();
        rect.anchoredPosition = new Vector2(SpawnPosition.x, Screen.height - SpawnPosition.y);
    }
    public void Start()
    {
        rect = GetComponent<RectTransform>();
    }
}
