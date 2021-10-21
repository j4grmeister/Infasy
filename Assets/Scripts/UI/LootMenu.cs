using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LootMenu : MonoBehaviour {
	public Text textObject;
	public GameObject itemContainer;
	public GameObject marker;
	public Text description;

	List<Inventory> inventories;
	List<Item> allItems {
		get {
			List<Item> l = new List<Item> ();
			for (int i = 0; i < inventories.Count; i++) {
				for (int j = 0; j < inventories [i].allItems.Count; j++) {
					l.Add (inventories [i].allItems [j]);
				}
			}
			return l;
		}
	}
	List<Text> itemObjects = new List<Text> ();
	int inventoryIndex = 0;
	int itemIndex = 0;
	int index {
		get {
			int v = 0;
			for (int i = 0; i <= inventoryIndex; i++) {
				v += (i == inventoryIndex) ? itemIndex : inventories [i].allItems.Count;
			}
			return v;
		}
		set {
			int i;
			for (i = 0; i < inventories.Count; i++) {
				if (value - inventories [i].allItems.Count < 0)
					break;
				value -= inventories [i].allItems.Count;
			}
			inventoryIndex = i;
			itemIndex = value;
		}
	}
	int itemCount {
		get {
			int c = 0;
			for (int i = 0; i < inventories.Count; i++) {
				c += inventories [i].allItems.Count;
			}
			return c;
		}
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (GameInput.GetKeyDown (GameInput.Bind.down)) {
			index++;
			if (index >= itemObjects.Count)
				index = 0;
		}
		if (GameInput.GetKeyDown (GameInput.Bind.up)) {
			index--;
			if (index < 0)
				index = itemObjects.Count - 1;
		}
		if (GameInput.GetKeyDown (GameInput.Bind.interact)) {
			Game.current.playerCharacter.inventory.Add (inventories [inventoryIndex].allItems [itemIndex]);
			inventories [inventoryIndex].RemoveAt (itemIndex);
			Load ();
			if (index >= itemObjects.Count)
				index = itemObjects.Count - 1;
		}
		if (GameInput.GetKeyDown (GameInput.Bind.back)) {
			Hide ();
		}
		marker.GetComponent<RectTransform> ().localPosition = (Vector2)itemObjects [index].gameObject.GetComponent<RectTransform> ().localPosition + new Vector2 (-20, textObject.gameObject.GetComponent<RectTransform> ().rect.height / 2);
		description.text = inventories [inventoryIndex].allItems [itemIndex].description;
	}

	void Load () {
		for (int i = 0; i < itemCount; i++) {
			if (itemObjects.Count <= i) {
				Text t = GameObject.Instantiate (textObject, itemContainer.transform);
				t.gameObject.GetComponent<RectTransform> ().localPosition = new Vector2 (0, itemContainer.GetComponent<RectTransform> ().rect.height / 2 - textObject.gameObject.GetComponent<RectTransform> ().rect.height * (i + 1));
				t.gameObject.SetActive (true);
				itemObjects.Add (t);
			}
			itemObjects [i].text = allItems [i].InfoString ();
		}
		while (itemObjects.Count > itemCount) {
			GameObject.Destroy (itemObjects [itemCount]);
			itemObjects.RemoveAt (itemCount);
		}
	}

	public void	Loot (List<Inventory> i) {
		//Debug.Log (i [0].allItems.Count);
		inventories = i;
		index = 0;
		Load ();
		Show ();
	}

	public void Show () {
		gameObject.SetActive (true);
		GameTime.paused = true;
	}

	public void Hide () {
		gameObject.SetActive (false);
		GameTime.paused = false;
	}
}