using UnityEngine;
using System.Collections;

public class ReferneceActivator : MonoBehaviour {

    public int ActiveReferences { get; private set; }

    public void AddReference()
    {
        ActiveReferences++;
        this.gameObject.SetActive(this.ActiveReferences > 0);
    }
    public void RemoveReference()
    {
        ActiveReferences--;
        this.gameObject.SetActive(this.ActiveReferences > 0);
    }

	void Start () {
	
	}
	
}
