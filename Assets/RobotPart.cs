using UnityEngine;
using System.Collections;

public class RobotPart : MonoBehaviour {

	public bool Selected { get; set; }

	private GameObject _selectionBorder;
	private RectTransform _rekt;
	private CanvasGroup _canvasGroup;

	// Use this for initialization
	void Start () {
		//Collider2D[] col = this.gameObject.transform.root.gameObject.GetComponents<Collider2D> ();
		//foreach (Collider2D c in col) {
		//	c.enabled = false;
		//}
		_selectionBorder = Instantiate (Resources.Load<GameObject> ("Selection Border"));
		_rekt = _selectionBorder.GetComponent<RectTransform> ();

		Bounds bounds = this.gameObject.GetComponent<SpriteRenderer> ().bounds;
		Vector2 pos = new Vector2 (bounds.size.x / 2, bounds.size.y / 2);
		_rekt.anchoredPosition -= pos;
		_rekt.sizeDelta = new Vector2(bounds.size.x, bounds.size.y);
		_selectionBorder.transform.SetParent (this.gameObject.transform, false);
		_canvasGroup = _selectionBorder.GetComponent<CanvasGroup> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (Selected) {
			_canvasGroup.alpha = 1;
		} else {
			_canvasGroup.alpha = 0;
		}
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
