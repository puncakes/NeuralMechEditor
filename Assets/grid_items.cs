using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class grid_items : MonoBehaviour {

	public GameObject buttonPrefab;
	public Transform layoutPanel;

	public string ItemFolderPath = "";

	private GameObject[] _prefabs;
	// Use this for initialization
	void Start () {
		createGrid (ItemFolderPath);
	}

	public void createGrid(string path)
	{
		_prefabs = Resources.LoadAll<GameObject> (path);
		for(int i = 0; i < _prefabs.Length; i++) {
			int crappyscopehack = i;

			GameObject b = (GameObject)Instantiate(buttonPrefab);

			b.transform.SetParent(layoutPanel, false);

			Button bu = b.GetComponent<Button>();

			GameObject go = Instantiate(_prefabs[i]);

			Collider2D[] col = go.GetComponents<Collider2D>();
			for(int x = 0; x < col.Length; x++)
			{
				col[x].enabled = false;
			}
			SpriteRenderer[] ren = go.GetComponents<SpriteRenderer>();
			for(int x = 0; x < ren.Length; x++)
			{
				ren[x].sortingOrder = 1;
			}

			go.transform.SetParent(b.transform, false);
			go.transform.localScale = new Vector3(40,40,1);
			bu.onClick.AddListener(() => genPrefab(crappyscopehack));
		}
	}

	private void genPrefab(int index)
	{
		GameObject go = Instantiate (_prefabs[index]);
		//call cursor script and hand the prefab to it

		MyCursor.Instance.RequestState (go, MyCursor.CursorState.Placing);

	}

	// Update is called once per frame
	void Update () {
	
	}
}
