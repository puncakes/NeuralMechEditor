using UnityEngine;
using System.Collections;

public class RobotPart : MonoBehaviour {

	// Use this for initialization
	void Start () {
		//Collider2D[] col = this.gameObject.transform.root.gameObject.GetComponents<Collider2D> ();
		//foreach (Collider2D c in col) {
		//	c.enabled = false;
		//}
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnMouseOver()
	{
		MyCursor.Instance.RequestState (this.gameObject, MyCursor.CursorState.Hovering);
	}

	void OnMouseExit()
	{
		MyCursor.Instance.RequestState (this.gameObject, MyCursor.CursorState.Dummy);
	}
}
