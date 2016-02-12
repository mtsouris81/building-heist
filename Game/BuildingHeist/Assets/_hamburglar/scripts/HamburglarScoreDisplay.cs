using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HamburglarScoreDisplay : MonoBehaviour {

    public Text BuildingScore = null;
    public Text PlayerScore = null;
    public RectTransform DisplayContainer = null;
	void Start () {
	
	}
	
	void Update () {
        bool showDisplay = (MobileUIManager.Current.Manager.Mode == Weenus.UIScreenManager.UiMode.GamePlay);
        DisplayContainer.gameObject.SetActive(showDisplay);
	}


    public void SetScores(int building, int player)
    {
        BuildingScore.text = string.Format("${0}", building);
        PlayerScore.text = string.Format("${0}", player);
    }



}
