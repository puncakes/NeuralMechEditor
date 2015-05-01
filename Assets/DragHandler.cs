using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {

	public GameObject _buttonGhostPrefab;

	//set by PartsTreeView.cs
	public GameObject LinkedGameObject { get; set; }
	public int ButtonIndex { get; set; }

	private static GameObject _buttonGhost;
	private GameObject _canvas;

	#region IBeginDragHandler implementation

	void Start()
	{
		_canvas = GameObject.Find ("Canvas");
	}

	public void OnBeginDrag (PointerEventData eventData)
	{
		MyCursor.Instance.RequestState (this.gameObject, MyCursor.CursorState.Menu);

		_buttonGhost = Instantiate (_buttonGhostPrefab);
		_buttonGhost.GetComponentInChildren<Text> ().text = LinkedGameObject.name;
		_buttonGhost.GetComponentInChildren<RectTransform> ().sizeDelta = new Vector2 (200, 20);
		_buttonGhost.GetComponent<CanvasGroup> ().alpha = 1f;
		_buttonGhost.GetComponent<CanvasGroup> ().interactable = true;
		_buttonGhost.transform.SetParent (_canvas.transform, false);
	}

	#endregion

	#region IDragHandler implementation

	public void OnDrag (PointerEventData eventData)
	{
		_buttonGhost.transform.position = MyCursor.Instance.getScreenToWorld ();
	}

	#endregion

	#region IEndDragHandler implementation

	public void OnEndDrag (PointerEventData eventData)
	{
		MyCursor.Instance.RequestState (this.gameObject, MyCursor.CursorState.Default);
		Destroy (_buttonGhost);
	}

	#endregion
}
