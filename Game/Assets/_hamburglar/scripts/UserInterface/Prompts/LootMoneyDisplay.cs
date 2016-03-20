using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using GiveUp.Core;

public class LootMoneyDisplay : MonoBehaviour {

    public Text Text = null;
    public float Speed = 0.2f;

    public Transform FollowObject { get; private set; }
    public Vector3 FollowOffset { get; private set; }

    float moveDistance = 0;
    RectTransform rect;

    public void SetAmount(int amount)
    {
        Text.text = string.Format("${0}", amount);
    }
	void Start ()
    {
        rect = GetComponent<RectTransform>();
	}
	void Update ()
    {
        moveDistance += (Speed * Time.deltaTime);
        
        if (FollowObject == null)
            return;

        if (HamburglarContext.Instance.Mode != HamburglarViewMode.Room)
        {
            GameObject.Destroy(this.gameObject);
            return;
        }
        else
        {
            var screenPos = HamburglarContext.Instance.RoomCamera.WorldToScreenPoint(FollowObject.position + FollowOffset);
            rect.anchoredPosition = screenPos + (moveDistance * Vector3.up);
        }
        if (rect.anchoredPosition.y > Screen.height)
        {
            GameObject.Destroy(this.gameObject); // kill meeeee!!!!
        }
	}
    public void StartDisplay(RoomItem item)
    {
        moveDistance = 0;
        FollowOffset = Vector3.up * 1.3f;
        FollowObject = item.transform;
        rect = GetComponent<RectTransform>();
    }
}
