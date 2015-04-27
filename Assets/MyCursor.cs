using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MyCursor : MonoBehaviour {

	public GameObject MechRoot;
	public GameObject CurrentObject;
	public GameObject SelectionPanel;
	public Camera _camera;

	public enum CursorState {
		Default,
		Placing,
		Dragging
	}
	public static Rect selection = new Rect(0,0,0,0);
	private Vector3 startClick = -Vector3.one;

	private CursorState _currentState, _previousState;

	// Use this for initialization
	void Start () {
		_camera = this.gameObject.GetComponent<Camera>();
		_currentState = CursorState.Default;
	}

	public void requestState(CursorState cs)
	{
		//temporary grant-all
		_currentState = cs;
	}

	public static float InvertMouseY(float y)
	{
		return Screen.height - y;
	}

	private Vector3 getScreenToWorld()
	{
		return _camera.ScreenToWorldPoint (new Vector3 (Input.mousePosition.x, Input.mousePosition.y, 10));
	}

	// Update is called once per frame
	void Update () {

		switch (_currentState) {
			case CursorState.Default:
				if (Input.GetMouseButtonDown (0)) {
					startClick = Input.mousePosition;
					SelectionPanel.GetComponent<CanvasGroup>().alpha = 1;
					requestState(CursorState.Dragging);
				}
				break;

			case CursorState.Dragging:
				if (Input.GetMouseButton (0)) {
					Vector3 mousePos = Input.mousePosition;
					
					Vector3 startPoint = startClick;
					Vector3 difference = mousePos - startClick;
					
					if(difference.x < 0)
					{
						startPoint.x = mousePos.x;
						difference.x = -difference.x;
					}
					if(difference.y < 0)
					{
						startPoint.y = mousePos.y;
						difference.y = -difference.y;
					}

				selection = new Rect(startPoint.x, startPoint.y, difference.x, difference.y);

				SelectionPanel.GetComponent<RectTransform>().anchoredPosition = new Vector3(selection.x, selection.y);
				SelectionPanel.GetComponent<RectTransform>().sizeDelta = new Vector2(selection.width, selection.height);

				} else if (Input.GetMouseButtonUp(0)) {

				Transform[] ts = MechRoot.GetComponentsInChildren<Transform>();
				if(ts.Length != 0)
				{
					foreach(Transform t in ts)
					{
						Vector3 pos = _camera.WorldToScreenPoint(t.position);

						if(selection.Contains(pos))
						{
							Renderer r = t.gameObject.GetComponent<Renderer>();
							if(r)
								r.material.color = Color.red;
						}

					}
				}

					SelectionPanel.GetComponent<CanvasGroup>().alpha = 0;
					SelectionPanel.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 0);
					requestState(CursorState.Default);

				}
				break;
			
			case CursorState.Placing:
				if (CurrentObject != null) 
				{
					CurrentObject.transform.position = getScreenToWorld();
				}
				
				if (Input.GetMouseButtonDown (0) && CurrentObject) 
				{
					CurrentObject.transform.parent = MechRoot.transform;
					CurrentObject = null;
					
					requestState(CursorState.Default);
				}
				break;
		}

		_previousState = _currentState;

	}
}
