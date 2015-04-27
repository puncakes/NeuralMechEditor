using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEditor;

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
			Texture2D t = AssetPreview.GetAssetPreview(_prefabs[i]);
			b.GetComponentInChildren<Image>().sprite = Sprite.Create(t, new Rect(0,0,t.width,t.height), new Vector2(0.5f,0.5f));
			b.transform.SetParent(layoutPanel, false);

			Button bu = b.GetComponent<Button>();
			bu.onClick.AddListener(() => genPrefab(crappyscopehack));
		}
	}

	private void genPrefab(int index)
	{
		GameObject go = Instantiate (_prefabs[index]);
		//call cursor script and hand the prefab to it
		MyCursor c = (MyCursor)GameObject.Find ("Main Camera").GetComponent (typeof(MyCursor));
		c.CurrentObject = go;

	}

	// Update is called once per frame
	void Update () {
	
	}
}
