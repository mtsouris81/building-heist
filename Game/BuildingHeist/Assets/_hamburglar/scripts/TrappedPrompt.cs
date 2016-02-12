using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Hamburglar;

public class TrappedPrompt : HamburglarDisplayPrompt
{
    public Button OkButton = null;
    public Text Text = null;
    public Color Color = Color.red;

    public int Index { get; private set; }
    public override void Start()
    {
        base.Start();
        OkButton.onClick.AddListener(delegate()
        {
            CloseDisplay();
        });
    }


    public void ShowDisplay(string playerId)
    {
        var player = HamburglarContext.Instance.GetPlayerData(playerId);
        if (player == null)
        {
            this.Text.gameObject.SetActive(false);
        }
        else
        {
            this.Text.text = string.Format("{0} got you!", player.Username);
        }
        this.ShowDisplay(Vector3.zero);
        SetColor(Color);
    }



}
