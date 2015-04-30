using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

//ain't nobody called danburg >:\
//actually there is
using System;

public class MyCursor : MonoBehaviour {

	public GameObject MechRoot;
	public static GameObject CurrentObject;
	public GameObject SelectionPanel;
	public Camera _camera;

	//Cursor textures
	public Texture2D _hovering, _moving;

	private List<Transform> _selectedTransforms = new List<Transform>();
	public List<Transform> SelectedTransforms { get {return _selectedTransforms;} }

	[Flags]
	public enum CursorState {
		None		=0,
		Default		=1,
		Hovering	=2,
		Placing		=4,
		Dragging	=8,
		Selecting	=16,
		Panning		=32,
		Moving		=64,
		Dummy		=128
	}

	public static Rect selection = new Rect(0,0,0,0);
	private Vector3 startClick = -Vector3.one;

	private CursorState _currentState, _previousState;
	private Vector3 _prevMousePos;

	private Dictionary<CursorState, CursorState> _allowedTransitions = new Dictionary<CursorState, CursorState> ();


	public static MyCursor Instance { get; private set; }
	
	void Awake()
	{
		init ();
		Instance = this;
	}

	private void init()
	{
		setTransitions ();

		_camera = this.gameObject.GetComponent<Camera>();
		_currentState = CursorState.Default;
		SelectionPanel.GetComponent<RectTransform>().sizeDelta = Vector2.zero;
	}

	private void setTransitions()
	{
		//value containing all enums
		CursorState All = (CursorState)Math.Pow(2, Enum.GetNames(typeof(CursorState)).Length-1) - 1;

		_allowedTransitions.Add (CursorState.Default, All & ~(CursorState.Default)); 	
		_allowedTransitions.Add (CursorState.Hovering, All & ~(CursorState.Hovering));
		_allowedTransitions.Add (CursorState.Placing, All & ~(CursorState.Placing | CursorState.Hovering | CursorState.Dummy));
		_allowedTransitions.Add (CursorState.Dragging, All & ~(CursorState.Dragging));
		_allowedTransitions.Add (CursorState.Selecting, All & ~(CursorState.Selecting | CursorState.Hovering | CursorState.Dummy));
		_allowedTransitions.Add (CursorState.Panning, All & ~(CursorState.Panning));
		_allowedTransitions.Add (CursorState.Moving, All & ~(CursorState.Moving | CursorState.Hovering | CursorState.Dummy));
		_allowedTransitions.Add (CursorState.Dummy, All & ~(CursorState.Dummy));
	}

	private bool isRequestAllowed(CursorState cs)
	{
		CursorState transitions;

		_allowedTransitions.TryGetValue (_currentState, out transitions);

		if ((transitions & cs) == cs) {
			return true;
		}

		return false;
	}

	public void RequestState(GameObject go, CursorState cs)
	{
		//allow other scripts to change the cursor state only when in the default state
		//Debug.Log ("Current State: " + _currentState.ToString () + "  Requested State: " +cs.ToString());

		if(isRequestAllowed(cs))
		{
			CurrentObject = go;
			_previousState = _currentState;
			_currentState = cs;
		}
	}

	private void requestState(CursorState cs)
	{
		RequestState(null, cs);
	}

	private Vector3 getScreenToWorld()
	{
		return _camera.ScreenToWorldPoint (new Vector3 (Input.mousePosition.x, Input.mousePosition.y, 10));
	}

	private void checkPosForSelection(Vector2 vec)
	{
		Vector3 worldVec = _camera.ScreenToWorldPoint (new Vector3 (vec.x, vec.y, 10));
		RaycastHit2D hit = Physics2D.Raycast(new Vector2(worldVec.x, worldVec.y), Vector2.zero);
		if (hit && hit.transform.parent == MechRoot.transform) {
			if(!_selectedTransforms.Contains(hit.transform))
				_selectedTransforms.Add(hit.transform);
		}
	}

	public void UnselectAll()
	{
		Transform[] ts = MechRoot.GetComponentsInChildren<Transform>();
		if (ts.Length != 0) {
			foreach (Transform t in ts) {
				Renderer r = t.gameObject.GetComponent<Renderer>();
				if(!r)
					continue;
				r.material.color = Color.white;
			}

		}
		_selectedTransforms.Clear();
	}

	// Update is called once per frame
	public void DoUpdate () {

		CursorState stateAtStartOfFrame = _currentState;

		switch (_currentState) {
		case CursorState.Default:
			if(_previousState != CursorState.Default)
			{
				//Debug.Log("Default");
				Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
			}
			if (Input.GetMouseButtonDown (0)) {
				startClick = Input.mousePosition;
				SelectionPanel.GetComponent<CanvasGroup>().alpha = 1;
				requestState(CursorState.Selecting);
			}
			break;

		case CursorState.Dummy:
			requestState(CursorState.Default);
			break;

		case CursorState.Hovering:
			if(_previousState != CursorState.Hovering)
			{
				//Debug.Log("Hovering");
				Cursor.SetCursor(_hovering, new Vector2(_hovering.width / 2, _hovering.height / 2), CursorMode.Auto);
			}
			if(Input.GetKey(KeyCode.T) && Input.GetMouseButtonDown(0))
			{
				Vector3 localCoords = CurrentObject.transform.InverseTransformPoint(getScreenToWorld());
				Debug.Log("Local Coords: " + localCoords.ToString());
			}
			if (Input.GetMouseButton (0)) {
				if(_selectedTransforms.Count > 0 && !Input.GetKey(KeyCode.LeftControl)) //if there are selections and more are not trying to be added
				{
					//if the cursor is actually above a selected item
					Vector3 worldVec = getScreenToWorld();
					RaycastHit2D hit = Physics2D.Raycast(new Vector2(worldVec.x, worldVec.y), Vector2.zero);
					if (hit && !_selectedTransforms.Contains(hit.transform))
					{
						UnselectAll();
						_selectedTransforms.Add(hit.transform);
					}
					requestState(CursorState.Moving);
				} else {
					checkPosForSelection(Input.mousePosition);
				}
			}
			break;

		case CursorState.Moving:
			if(_previousState != CursorState.Moving)
			{
				//Debug.Log("Moving");
				Cursor.SetCursor(_moving, new Vector2(_moving.width / 2, _moving.height / 2), CursorMode.Auto);
			}

			if (Input.GetMouseButton (0)) {
				Vector3 prevWorldPos = _camera.ScreenToWorldPoint(new Vector3(_prevMousePos.x, _prevMousePos.y, 10));
				Vector3 curWorld = getScreenToWorld();

				Vector3 worldDelta = curWorld - prevWorldPos;
				worldDelta.z = 0;

				foreach(Transform t in _selectedTransforms)
				{
					t.position += worldDelta;
				}
			} else {
				requestState(CursorState.Default);
			}

			//TODO:move selected objects from mouse delta

			break;
		case CursorState.Selecting:
			if(_previousState != CursorState.Selecting)
			{
				//Debug.Log("Selecting");
			}
			if (Input.GetMouseButton (0)) {

				//TODO: toggle selections
				bool isControlPressed = Input.GetKey(KeyCode.LeftControl);

				//don't clear if control is down
				if(!isControlPressed)
					UnselectAll();					

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

			} 
			else if (Input.GetMouseButtonUp(0)) {
				Transform[] ts = MechRoot.GetComponentsInChildren<Transform>();
				if(ts.Length != 0)
				{
					List<Transform> tsList = new List<Transform>(ts);
					tsList.Remove(MechRoot.transform);
					foreach(Transform t in tsList)
					{
						Vector3 pos = _camera.WorldToScreenPoint(t.position);
						if(selection.Contains(pos))
						{
							if(!_selectedTransforms.Contains(t.transform))
								_selectedTransforms.Add(t);
						}
					}

					//loop for making sure topmost transform is selected
					foreach(Transform t in tsList)
					{
						for(int i = 0; i < t.childCount; i++)
						{
							_selectedTransforms.Remove(t.GetChild(i));
						}
					}

					//check four corners of selection box for intersection with robot parts
					//have to convert to world space for collider intersections. done in function
					checkPosForSelection(selection.position);
					checkPosForSelection(selection.position + new Vector2(selection.width, 0));
					checkPosForSelection(selection.position + new Vector2(0, selection.height));
					checkPosForSelection(selection.position + new Vector2(selection.width, selection.height));
				}

				SelectionPanel.GetComponent<CanvasGroup>().alpha = 0;
				SelectionPanel.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 0);
				requestState(CursorState.Default);
			}
			break;
		
		case CursorState.Placing:
			//Debug.Log("Placing");
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

		foreach (Transform t in _selectedTransforms) {
			Renderer r = t.gameObject.GetComponent<Renderer>();
			if(!r)
				continue;
			r.material.color = Color.green;
		}
		_previousState = stateAtStartOfFrame;
		_prevMousePos = Input.mousePosition;
	}
}
