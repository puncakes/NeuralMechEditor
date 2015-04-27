using UnityEngine;
using System.Collections;

public class MyCursor : MonoBehaviour {

	public GameObject CurrentObject;
	public Camera _camera;

	// Use this for initialization
	void Start () {
		_camera = this.gameObject.GetComponent<Camera>();
	}
	
	// Update is called once per frame
	void Update () {
		if (CurrentObject != null)
			CurrentObject.transform.position = _camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10));
		if (Input.GetMouseButtonDown (0) && CurrentObject)
			CurrentObject = null;
	}
}
