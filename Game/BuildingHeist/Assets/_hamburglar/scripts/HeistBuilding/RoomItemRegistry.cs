using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RoomItemRegistry : MonoBehaviour {

    public static RoomItemRegistry Instance { get; private set; }


    Dictionary<FurnitureType, List<RoomItem>> ItemLookUp = new Dictionary<FurnitureType, List<RoomItem>>();


	void Start () {
        Instance = this;
        RoomItem[] items = GetComponentsInChildren<RoomItem>(true);
        foreach (var i in items)
        {
            if (!ItemLookUp.ContainsKey(i.Type))
            {
                ItemLookUp.Add(i.Type, new List<RoomItem>());
            }
            ItemLookUp[i.Type].Add(i);
            i.gameObject.SetActive(false);
        }
	}
    public RoomItem GetItem(FurnitureType type)
    {
        return ItemLookUp[type][Random.Range(0, ItemLookUp[type].Count - 1)];
    }
    public RoomItem GetItem(FurnitureType type, int index)
    {
        return ItemLookUp[type][index];
    }
	// Update is called once per frame
	void Update () {
	
	}
}
