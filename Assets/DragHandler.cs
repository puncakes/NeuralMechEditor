using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler {

	public GameObject _buttonGhostPrefab;
	
	//set by PartsTreeView.cs
	public GameObject LinkedGameObject { get; set; }
	public int ButtonIndex { get; set; }

	private static GameObject _linkedGameObject;
	private static GameObject _buttonGhost;
	private static GameObject _canvas;

	#region IBeginDragHandler implementation

	void Start()
	{
		if(!_buttonGhost)
			_buttonGhost = Instantiate (_buttonGhostPrefab);
		_buttonGhost.GetComponent<CanvasGroup> ().alpha = 0f;
		_buttonGhost.GetComponent<CanvasGroup> ().interactable = false;

		_canvas = GameObject.Find ("Canvas");
		this.gameObject.GetComponent<Button> ().onClick.AddListener (OnClick);
	}

	void OnClick()
	{
		MyCursor.Instance.selectGameObject (LinkedGameObject);
	}

	public void OnBeginDrag (PointerEventData eventData)
	{
		MyCursor.Instance.RequestState (this.gameObject, MyCursor.CursorState.Menu);

		//static so when dropped, other scripts will be able to get the correct linked object
		_linkedGameObject = LinkedGameObject;
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
		_buttonGhost.GetComponent<CanvasGroup> ().alpha = 0f;
		_buttonGhost.GetComponent<CanvasGroup> ().interactable = false;
		//_linkedGameObject = null;
		//Destroy (_buttonGhost);
	}

	#endregion

	#region IDropHandler implementation

	public void OnDrop (PointerEventData eventData)
	{
		//"this" is the item the dragged item was dropped onto
		GameObject child = _linkedGameObject;
		GameObject parent = this.gameObject.GetComponent<DragHandler> ().LinkedGameObject;
		child.transform.SetParent (parent.transform, true);
	}

	#endregion
}
