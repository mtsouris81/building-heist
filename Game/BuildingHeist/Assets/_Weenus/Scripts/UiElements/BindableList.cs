using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using System.Linq;

public class BindableList : MonoBehaviour {


    public BindableListItem ItemTemplate = null;
    public VerticalLayoutGroup layoutGroup = null;
    public Action<BindableListItem> OnItemClicked { get; set; }

    public List<BindableListItem> boundItems = new List<BindableListItem>();
    public bool TrackSelection { get; set; }

    public void BindList<T>(IEnumerable<T> list, Func<T, string> displayProperty, Func<T, object> valueProperty)
    {
        BindList<T>(list, displayProperty, valueProperty, null);
    }

    public void ClearBindings()
    {
        foreach (var i in boundItems)
        {
            GameObject.Destroy(i.gameObject);
        }
        boundItems.Clear();
    }

    public void BindList<T>(IEnumerable<T> list, Func<T, string> displayProperty, Func<T, object> valueProperty, Action<BindableListItem> postProcess)
    {
        ClearBindings();
        if (list == null || list.Count() < 1)
        {
            return;
        }
        foreach (T item in list)
        {
            BindableListItem newItem = GameObject.Instantiate(ItemTemplate) as BindableListItem;
            newItem.gameObject.SetActive(true);
            newItem.TrackSelection = this.TrackSelection;
            boundItems.Add(newItem);
            newItem.transform.SetParent(this.transform, BindingConstants.WorldTransformStaysSame);
            newItem.SetData(displayProperty(item), valueProperty(item));
            newItem.OnClicked = OnItemClicked;
            if (postProcess != null)
            {
                postProcess(newItem);
            }
        }
    }


	// Use this for initialization
	void Start () {
	
	}




	
	// Update is called once per frame
	void Update () {
	
	}


    public class KeyValPairThing
    {
        public int ID { get; set; }
        public string Name { get; set; }
    }

}
