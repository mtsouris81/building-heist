using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using Tsouris.StoryBuilder;
using Tsouris.StoryBuilder.Data.StoryParts;
using Weenus;

public class AuthorChooserList : MonoBehaviour {

    UnityXmlService<List<KeyVal>> AllStoriesRequest = new UnityXmlService<List<KeyVal>>();

    ScrollableItemList scrollList = null;
    WWW www;
    GameObject parentObject = null;
    WeenusUI View;

	void Start () 
    {
        View = new WeenusUI(this);
        parentObject = this.transform.parent.gameObject;
        scrollList = this.GetComponent<ScrollableItemList>();
        scrollList.BindableList.OnItemClicked = delegate(BindableListItem item)
        {
            View.UI.SetViewData("authorid", long.Parse(item.Value.ToString()));
            View.UI.SwitchToScreen("Stories");
        };
        AllStoriesRequest.StartRequest(ServiceUrl.Get(Urls.Authors));
	}

	void Update () {

        if (AllStoriesRequest.AttemptResolveReesponse())
        {
            scrollList.BindableList.BindList(AllStoriesRequest.Result,
                                                    x => x.Key,
                                                    x => x.Value);
        }
	}


}
