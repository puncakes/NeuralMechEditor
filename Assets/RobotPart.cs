using UnityEngine;
using System.Collections;

public class RobotPart : MonoBehaviour {

	public bool Selected { get; set; }

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
		InputHandler.Instance.RequestState (this.gameObject, InputHandler.CursorState.Hovering);
	}

	void OnMouseExit()
	{
		InputHandler.Instance.RequestState (this.gameObject, InputHandler.CursorState.Dummy);
	}
}
