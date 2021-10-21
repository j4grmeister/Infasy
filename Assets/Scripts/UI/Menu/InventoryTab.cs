using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryTab : MenuTab {
	public Text itemDescription;
	public ItemEntry itemObject;
	public WeaponEntry weaponObject;
	public ArmorEntry armorObject;
	public GameObject itemContainer;
	public GameObject subMenuOptionContainer;
	public GameObject optionMarker;
	public GameObject marker;
	public Vector2 markerOffset;
	public Vector2 itemMarkerOffset;
	public Vector2 itemOffset;

	public List<Text> subMenuOptions;

	List<ItemEntry> items = new List<ItemEntry> ();
	//List<WeaponEntry> weapons = new List<WeaponEntry> ();
	//List<ArmorEntry> armor = new List<ArmorEntry> ();
	//int index = 0;
	List<int> index = new List<int> ();
	//int column = 0;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (GameInput.GetKeyDown (GameInput.Bind.down)) {
			index [index.Count - 1]++;
			if (index.Count == 1) {
				if (index [index.Count - 1] >= subMenuOptions.Count)
					index [index.Count - 1] = 0;
			} else if (index.Count == 2) {
				if (index [index.Count - 1] >= items.Count)
					index [index.Count - 1] = 0;
			}
			MoveMarker ();
		}
		if (GameInput.GetKeyDown (GameInput.Bind.up)) {
			index [index.Count - 1]--;
			if (index [index.Count - 1] < 0) {
				if (index.Count == 1)
					index [index.Count - 1] = subMenuOptions.Count - 1;
				else if (index.Count == 2)
					index [index.Count - 1] = items.Count - 1;
			}
			MoveMarker ();
		}
		if (GameInput.GetKeyDown (GameInput.Bind.interact)) {
			if (index.Count == 1) {
				index.Add (0);
				UpdateItems ();
			} else if (index.Count == 2) {
				if (index [0] == 1) { //weapons
					Game.current.playerCharacter.inventory.Equip (Game.current.playerCharacter.inventory.weapons [index [1]]);
				} else if (index [0] == 2) { //armor
					Game.current.playerCharacter.inventory.Equip (Game.current.playerCharacter.inventory.armor [index [1]]);
				}
			}
		}
		if (GameInput.GetKeyDown (GameInput.Bind.back)) {
			if (index.Count == 1) {
				Game.ui.menu.GoHome ();
			} else if (index.Count == 2) {
				index.RemoveAt (1);
				UpdateItems ();
			}
		}
	}

	void MoveMarker () {
		if (index.Count == 1) {
			optionMarker.GetComponent<RectTransform> ().localPosition = subMenuOptions [index [0]].gameObject.GetComponent<RectTransform> ().localPosition + (Vector3)markerOffset;
		} else if (index.Count == 2) {
			marker.GetComponent<RectTransform> ().localPosition = items [index [1]].gameObject.GetComponent<RectTransform> ().localPosition + new Vector3 (-20, itemObject.gameObject.GetComponent<RectTransform> ().rect.height / 2);
			itemDescription.text = items [index [1]].GetItem ().description;
		}
	}

	void UpdateItems () {
		if (index.Count == 1) {
			itemContainer.SetActive (false);
			itemDescription.gameObject.SetActive (false);
			subMenuOptionContainer.SetActive (true);
			Game.ui.menu.home.ShowStats ();
			Game.ui.menu.home.HideOptions ();
		} else if (index.Count == 2) {
			subMenuOptionContainer.SetActive (false);
			itemContainer.SetActive (true);
			itemDescription.gameObject.SetActive (true);
			Game.ui.menu.home.gameObject.SetActive (false);

			if (index [0] == 0) { //items
				for (int i = 0; i < Game.current.playerCharacter.inventory.items.Count; i++) {
					if (i >= items.Count) {
						ItemEntry e = GameObject.Instantiate (itemObject, itemContainer.transform);
						e.gameObject.GetComponent<RectTransform> ().localPosition = new Vector3 (0, itemContainer.GetComponent<RectTransform> ().rect.height / 2 - itemObject.gameObject.GetComponent<RectTransform> ().rect.height * (i + 1));
						items.Add (e);
					}
					items [i].SetItem (Game.current.playerCharacter.inventory.items [i]);
				}
				for (int i = Game.current.playerCharacter.inventory.items.Count; i < items.Count;) {
					GameObject.Destroy (items [i].gameObject);
					items.RemoveAt (i);
				}
			} else if (index [0] == 1) { //weapons
				for (int i = 0; i < Game.current.playerCharacter.inventory.weapons.Count; i++) {
					if (i >= items.Count) {
						WeaponEntry e = GameObject.Instantiate (weaponObject, itemContainer.transform);
						e.gameObject.GetComponent<RectTransform> ().localPosition = new Vector3 (0, itemContainer.GetComponent<RectTransform> ().rect.height / 2 - weaponObject.gameObject.GetComponent<RectTransform> ().rect.height * (i + 1));
						items.Add (e);
					}
					items [i].SetItem (Game.current.playerCharacter.inventory.weapons [i]);
				}
				for (int i = Game.current.playerCharacter.inventory.weapons.Count; i < items.Count;) {
					GameObject.Destroy (items [i].gameObject);
					items.RemoveAt (i);
				}
			} else if (index [0] == 2) { //armor
				for (int i = 0; i < Game.current.playerCharacter.inventory.armor.Count; i++) {
					if (i >= items.Count) {
						ArmorEntry e = GameObject.Instantiate (armorObject, itemContainer.transform);
						e.gameObject.GetComponent<RectTransform> ().localPosition = new Vector3 (0, itemContainer.GetComponent<RectTransform> ().rect.height / 2 - weaponObject.gameObject.GetComponent<RectTransform> ().rect.height * (i + 1));
						items.Add (e);
					}
					items [i].SetItem (Game.current.playerCharacter.inventory.armor [i]);
				}
				for (int i = Game.current.playerCharacter.inventory.armor.Count; i < items.Count;) {
					GameObject.Destroy (items [i].gameObject);
					items.RemoveAt (i);
				}
			}
		}
		MoveMarker ();
	}

	public override void UpdateTab () {
		/*
		List<Item> items = new List<Item> ();
		for (int i = 0; i < Game.current.playerCharacter.inventory.items.Count; i++) {
			items.Add (Game.current.playerCharacter.inventory.items [i]);
		}
		List<Item> weapons = new List<Item> ();
		for (int i = 0; i < Game.current.playerCharacter.inventory.weapons.Count; i++) {
			Weapon w = Game.current.playerCharacter.inventory.weapons [i];
			weapons.Add ((Item)w);
			if (Game.current.playerCharacter.inventory.Equipped (w)) {
				
			}
		}
		List<Item> armor = new List<Item> ();
		for (int i = 0; i < Game.current.playerCharacter.inventory.armor.Count; i++) {
			Armor a = Game.current.playerCharacter.inventory.armor [i];
			armor.Add ((Item)a);
			if (Game.current.playerCharacter.inventory.Equipped (a)) {

			}
		}
		*/

		index.RemoveRange (0, index.Count);
		index.Add (0);

		UpdateItems ();
	}
}
