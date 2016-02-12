using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using System.Linq;

public class BindableListBase<TListItem, TDataType> : MonoBehaviour where TListItem : MonoBehaviour {


    public Transform ItemTemplate = null;
    public Transform layoutGroup = null;
    public Action<TListItem> OnItemClicked { get; set; }
    List<TListItem> boundItems = new List<TListItem>();


    public void ClearBindings()
    {
        foreach (var i in boundItems)
        {
            GameObject.Destroy(i.gameObject);
        }
        boundItems.Clear();
    }



    public virtual void ApplyBinding(TListItem ui, TDataType data)
    {

    }

    public void BindList(IEnumerable<TDataType> list)
    {
        BindList<TDataType>(list, ApplyBinding);
    }

    public void BindList<T>(IEnumerable<T> list, Action<TListItem, T> applyValues)
    {
        ClearBindings();
        if (list == null || list.Count() < 1)
        {
            return;
        }
        foreach (T item in list)
        {
            Transform newItem = GameObject.Instantiate(ItemTemplate) as Transform;
            TListItem typedInstance = newItem.GetComponent<TListItem>();
            boundItems.Add(typedInstance);
            applyValues(typedInstance, item);
            newItem.transform.SetParent(layoutGroup.transform, BindingConstants.WorldTransformStaysSame);
        }
    }


	void Start () {
	
	}
	void Update () {
	
	}


    public class KeyValPairThing
    {
        public int ID { get; set; }
        public string Name { get; set; }
    }

}
