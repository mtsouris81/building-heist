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

    public List<Transform> boundItems = new List<Transform>();
    public bool TrackSelection { get; set; }



    public void ClearBindings()
    {
        foreach (var i in boundItems)
        {
            GameObject.Destroy(i.gameObject);
        }
        boundItems.Clear();
    }

    public void BindList<T>(IEnumerable<T> list, Func<T, string> displayProperty, Func<T, object> valueProperty)
    {
        BindList<T, T>(list, displayProperty, valueProperty, null);
    }
    public void BindList<T>(IEnumerable<T> list, Func<T, string> displayProperty, Func<T, object> valueProperty, bool appendToList)
    {
        BindList<T, T>(list, displayProperty, valueProperty, null, appendToList);
    }
    public void BindList<T, TCustomProcessType>(IEnumerable<T> list, Func<T, string> displayProperty, Func<T, object> valueProperty, Action<TCustomProcessType, string, object> postProcess)
    {
        BindList<T, TCustomProcessType>(list, displayProperty, valueProperty, postProcess, false);
    }
    public void BindList<T, TCustomProcessType>(IEnumerable<T> list, Func<T, string> displayProperty, Func<T, object> valueProperty, Action<TCustomProcessType, string, object> postProcess, bool appendToList)
    {
        if (!appendToList)
        {
            ClearBindings();
        }
        if (list == null || list.Count() < 1)
        {
            return;
        }
        foreach (T item in list)
        {
            BindableListItem newItem = AddItemToList<BindableListItem>(ItemTemplate);
            string display = displayProperty(item);
            object value = valueProperty(item);
            newItem.TrackSelection = this.TrackSelection;
            newItem.SetData(display, value);
            newItem.OnClicked = OnItemClicked;
            if (postProcess != null)
            {
                postProcess(newItem.GetComponent<TCustomProcessType>(), display, value);
            }
        }
    }
    public T AddItemToList<T>(T original) where T : Component
    {
        T newItem = GameObject.Instantiate<T>(original);
        newItem.gameObject.SetActive(true);
        boundItems.Add(newItem.transform);
        newItem.transform.SetParent(this.transform, BindingConstants.WorldTransformStaysSame);
        return newItem;
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
