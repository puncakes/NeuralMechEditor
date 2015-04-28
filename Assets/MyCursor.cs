﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

//ain't nobody called danburg >:\

public class MyCursor : MonoBehaviour {

	public GameObject MechRoot;
	public GameObject CurrentObject;
	public GameObject SelectionPanel;
	public Camera _camera;

	public List<Transform> _selectedTransforms = new List<Transform>();

	public enum CursorState {
		Default,
		Placing,
		Dragging,
		Selecting,
		Panning,
		Moving
	}
	public static Rect selection = new Rect(0,0,0,0);
	private Vector3 startClick = -Vector3.one;

	private CursorState _currentState, _previousState;

	// Use this for initialization
	void Start () {
		_camera = this.gameObject.GetComponent<Camera>();
		_currentState = CursorState.Default;
		SelectionPanel.GetComponent<RectTransform>().sizeDelta = Vector2.zero;
	}

	public void requestState(CursorState cs)
	{
		//temporary grant-all
		_currentState = cs;
	}

	private Vector3 getScreenToWorld()
	{
		return _camera.ScreenToWorldPoint (new Vector3 (Input.mousePosition.x, Input.mousePosition.y, 10));
	}

	private void checkPosForSelection(Vector2 vec)
	{
		Vector3 worldVec = _camera.ScreenToWorldPoint (new Vector3 (vec.x, vec.y, 10));
		RaycastHit2D hit = Physics2D.Raycast(new Vector2(worldVec.x, worldVec.y), Vector2.zero);
		if (hit) {
			Renderer r = hit.transform.gameObject.GetComponent<Renderer>();
			if(!r)
				return;
			r.material.color = Color.green;
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
	void Update () {

		switch (_currentState) {
		case CursorState.Default:
			if (Input.GetMouseButtonDown (0)) {
				startClick = Input.mousePosition;
				SelectionPanel.GetComponent<CanvasGroup>().alpha = 1;
				requestState(CursorState.Selecting);
			}
			break;

		case CursorState.Selecting:
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
					foreach(Transform t in ts)
					{
						Vector3 pos = _camera.WorldToScreenPoint(t.position);
						if(selection.Contains(pos))
						{
							_selectedTransforms.Add(t);
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

		_previousState = _currentState;

	}
}
