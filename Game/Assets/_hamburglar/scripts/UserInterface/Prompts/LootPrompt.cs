using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using GiveUp.Core;
using Hamburglar.Core;
using System;

public class LootPrompt : HamburglarDisplayPrompt
{
    public LootMoneyDisplay MoneyDisplay = null;
    public TrapButton TrapButton = null;
    public Text ValueDisplay = null;
    public RectTransform TrapPart = null;
    public RectTransform ClosePart = null;
    public Button TrapYesButton = null;
    public Button TrapNoButton = null;
    public Button OkButton = null;
    public Color GoodColor = Color.green;
    public Color BadColor = Color.red;

    public static Action InterceptAction { get; set; }
    public int LootIndex { get; set; }

    public void ClearLoot(bool withTrap)
    {
        if (InterceptAction != null)
        {
            // tutorial mode, don't do anything
            return;
        }

        if (withTrap)
        {
            HamburglarContext.Instance.BuildingData.SetRoomLootTrap(
                                            HamburglarContext.Instance.PlayerId,
                                            HamburglarContext.Instance.Floor,
                                            HamburglarContext.Instance.Room,
                                            this.LootIndex,
                                            1); // trapId
        }
        CallLootService(withTrap ? 1 : 0);
    }

    private void CallLootService(int trapId)
    {
        HamburglarContext.Instance.Service.LootServiceCall(LootIndex, trapId);
        CloseDisplay();
    }

    public void ShowDisplay(Vector3 screenPos, RoomItem roomItem, float value, bool showTrap, int lootIndex)
    {
        LootIndex = lootIndex;
        HamburglarContext.Instance.UISpawnContainer.gameObject.SetActive(true);
        var itemScreenPos = HamburglarContext.Instance.RoomCamera.WorldToScreenPoint(roomItem.transform.position);

        if (value > 0)
        {
            var display = GameObject.Instantiate(MoneyDisplay) as LootMoneyDisplay;
            display.transform.SetParent(HamburglarContext.Instance.UISpawnContainer.transform);
            display.StartDisplay(roomItem);
            display.gameObject.SetActive(true);
            display.SetAmount((int)value);
        }
        if (showTrap)
        {
            TrapButton.GetComponent<RectTransform>().anchoredPosition = itemScreenPos + Vector3.down * 50;
            TrapButton.SetData(roomItem);
            TrapButton.ActivateButton();
        }
        if (InterceptAction != null)
        {
            InterceptAction();
            return;
        }
        ClearLoot(false);
    }
}
