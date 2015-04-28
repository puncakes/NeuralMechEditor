using UnityEngine;
using System.Collections;

public class CursorUpdater : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		//only way I can think of to update the singleton
		MyCursor.Instance.DoUpdate ();
	}
}
