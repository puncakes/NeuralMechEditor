using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class PartsTreeView : MonoBehaviour {

	public GameObject HierarchButton;

	private static int _maxButtons = 100;
	private GameObject[] _buttons = new GameObject[_maxButtons];

	private int count;

	// Use this for initialization
	void Start () {
		for (int i = 0; i < _maxButtons; i++) {
			_buttons[i] = (GameObject)Instantiate (HierarchButton);
			_buttons[i].transform.SetParent (this.transform, false);
			_buttons[i].GetComponent<DragHandler>().ButtonIndex = i;
		}
	}
	
	// Update is called once per frame
	void Update () {

		UpdateTree ();

	}

	public void UpdateTree()
	{
		Transform root = InputHandler.Instance.MechRoot.transform;

		clearStructure ();
		count = 0;
		createStructure (root, 0);
	}

	void clearStructure ()
	{
		for (int i = 0; i < count; i++) {
			_buttons[i].GetComponent<Button>().image.color = Color.white;
			CanvasGroup cg = _buttons[i].GetComponent<CanvasGroup>();
			cg.alpha = 0;
			cg.interactable = false;
		}
	}

	void createStructure (Transform node, int lvl)
	{
		for (int i = 0; i < node.childCount; i++) {
			Transform t = node.GetChild (i);
			if(t.gameObject.name.Contains("Selection Border"))
				continue;

			DragHandler drag = _buttons[count].GetComponent<DragHandler>();
			drag.LinkedGameObject = t.gameObject;
			if(drag.LinkedGameObject.GetComponent<RobotPart>().Selected)
			{
				_buttons[count].GetComponent<Button>().image.color = Color.green;
			}


			GameObject b = _buttons[count];
		
			CanvasGroup cg = b.GetComponent<CanvasGroup>();
			cg.alpha = 1;
			cg.interactable = true;

			Button bu = b.GetComponent<Button> ();
			bu.GetComponentInChildren<Text> ().text = "".PadLeft (lvl * 2) + t.gameObject.name;

			count++;

			//for (int x = 0; x < node.childCount; x++) {
				createStructure(t, lvl + 1);
			//}
		}		

	
	}
}
