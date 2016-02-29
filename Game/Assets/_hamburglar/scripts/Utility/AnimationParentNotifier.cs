using UnityEngine;
using System.Collections;

public class AnimationParentNotifier : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    public void AnimationMessage(string message)
    {
        SendMessageUpwards(message, SendMessageOptions.DontRequireReceiver);
    }
    public void AnimationAttack()
    {
        SendMessageUpwards("AnimationAttackHandler", SendMessageOptions.RequireReceiver);
    }
}
