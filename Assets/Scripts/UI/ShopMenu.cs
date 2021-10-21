using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopMenu : MonoBehaviour {
	public enum ShopAction {
		buy,
		sell
	}

	public ShopAction action;
	public GameObject itemContainer;
	public ItemEntry itemEntry;
	public WeaponEntry weaponEntry;
	public ArmorEntry armorEntry;
	public GameObject marker;
	public Vector2 markerOffset;
	public Text playerGold;
	public Text shopkeeperGold;
	public Text description;
	public float itemSpacing;

	ushort townID = 0;
	ushort shopID = 0;
	Shop shop;

	List<ItemEntry> items = new List<ItemEntry> ();
	int index = 0;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		GameTime.paused = true;
		if (GameInput.GetKeyDown (GameInput.Bind.down)) {
			index++;
			if (action == ShopAction.buy) {
				if (index >= shop.inventory.Count)
					index = 0;
			} else if (action == ShopAction.sell) {
				if (shop.GetType () == typeof(ItemShop)) {

				} else if (shop.GetType () == typeof(WeaponShop)) {
					if (index >= Game.current.playerCharacter.inventory.weapons.Count)
						index = 0;
				} else if (shop.GetType () == typeof(ArmorShop)) {

				}
			}
		}
		if (GameInput.GetKeyDown (GameInput.Bind.up)) {
			index--;
			if (index < 0) {
				if (action == ShopAction.buy) {
					index = shop.inventory.Count - 1;
				} else if (action == ShopAction.sell) {
					if (shop.GetType () == typeof(ItemShop)) {

					} else if (shop.GetType () == typeof(WeaponShop)) {
						index = Game.current.playerCharacter.inventory.weapons.Count - 1;
					} else if (shop.GetType () == typeof(ArmorShop)) {

					}
				}
			}
		}
		if (GameInput.GetKeyDown (GameInput.Bind.interact)) {
			if (action == ShopAction.buy) {
				if (Game.current.playerCharacter.inventory.gold >= shop.inventory [index].cost) {
					shop.Buy (index);
					if (index >= shop.inventory.Count)
						index = shop.inventory.Count - 1;
					Load ();
				}
			} else if (action == ShopAction.sell) {
				if (shop.GetType () == typeof(ItemShop)) {

				} else if (shop.GetType () == typeof(WeaponShop)) {
					shop.Sell (index);
					if (index >= Game.current.playerCharacter.inventory.weapons.Count)
						index = Game.current.playerCharacter.inventory.weapons.Count - 1;
					Load ();
				} else if (shop.GetType () == typeof(ArmorShop)) {

				}
			}
		}
		if (GameInput.GetKeyDown (GameInput.Bind.back)) {
			Close ();
		}
		ShowInfo ();
	}

	void ShowInfo () {
		if (action == ShopAction.buy) {
			if (shop.inventory.Count > 0) {
				marker.GetComponent<RectTransform> ().localPosition = items [index].gameObject.GetComponent<RectTransform> ().localPosition + (Vector3)markerOffset;
				description.text = shop.inventory [index].description;
			}
		} else if (action == ShopAction.sell) {
			if (shop.GetType () == typeof(ItemShop)) {
				if (Game.current.playerCharacter.inventory.items.Count > 0) {
					marker.GetComponent<RectTransform> ().localPosition = items [index].gameObject.GetComponent<RectTransform> ().localPosition + (Vector3)markerOffset;
				}
			} else if (shop.GetType () == typeof(WeaponShop)) {
				if (Game.current.playerCharacter.inventory.weapons.Count > 0) {
					marker.GetComponent<RectTransform> ().localPosition = items [index].gameObject.GetComponent<RectTransform> ().localPosition + (Vector3)markerOffset;
					description.text = Game.current.playerCharacter.inventory.weapons [index].description;
				}
			} else if (shop.GetType () == typeof(ArmorShop)) {
				if (Game.current.playerCharacter.inventory.armor.Count > 0) {
					marker.GetComponent<RectTransform> ().localPosition = items [index].gameObject.GetComponent<RectTransform> ().localPosition + (Vector3)markerOffset;
				}
			}
		}
	}

	public void Close () {
		GameTime.paused = false;
		gameObject.SetActive (false);
		shop.shopkeeper.controller.OnPlayerInteract ();
	}

	public void Buy (ushort townID, ushort shopID) {
		index = 0;
		//GameTime.paused = true;
		action = ShopAction.buy;
		this.townID = townID;
		this.shopID = shopID;
		shop = (Shop)Game.current.map.GetTownByID (townID).GetBuildingByID (shopID);
		Load ();
		gameObject.SetActive (true);
	}

	public void Sell (ushort townID, ushort shopID) {
		index = 0;
		//GameTime.paused = true;
		action = ShopAction.sell;
		this.townID = townID;
		this.shopID = shopID;
		shop = (Shop)Game.current.map.GetTownByID (townID).GetBuildingByID (shopID);
		Load ();
		gameObject.SetActive (true);
	}

	void Load () {
		if (action == ShopAction.buy) {
			if (shop.GetType () == typeof(ItemShop)) {
				
			} else if (shop.GetType () == typeof(WeaponShop)) {
				for (int i = 0; i < shop.inventory.Count; i++) {
					if (i >= items.Count) {
						ItemEntry ie = (ItemEntry)GameObject.Instantiate (weaponEntry, itemContainer.transform);
						ie.gameObject.GetComponent<RectTransform> ().localPosition = new Vector2 (0, itemContainer.GetComponent<RectTransform> ().rect.height - (weaponEntry.gameObject.GetComponent<RectTransform> ().rect.height + itemSpacing) * i);
						items.Add (ie);
					}
					items [i].SetItem (shop.inventory [i]);
				}
				for (int i = shop.inventory.Count; i < items.Count;) {
					GameObject.Destroy (items [i].gameObject);
					items.RemoveAt (i);
				}
			} else if (shop.GetType () == typeof(ArmorShop)) {

			}
		} else if (action == ShopAction.sell) {
			if (shop.GetType () == typeof(ItemShop)) {

			} else if (shop.GetType () == typeof(WeaponShop)) {
				for (int i = 0; i < Game.current.playerCharacter.inventory.weapons.Count; i++) {
					if (i >= items.Count) {
						ItemEntry ie = (ItemEntry)GameObject.Instantiate (weaponEntry, itemContainer.transform);
						ie.gameObject.GetComponent<RectTransform> ().localPosition = new Vector2 (0, itemContainer.GetComponent<RectTransform> ().rect.height - (weaponEntry.gameObject.GetComponent<RectTransform> ().rect.height + itemSpacing) * i);
						items.Add (ie);
					}
					items [i].SetItem (Game.current.playerCharacter.inventory.weapons [i]);
				}
				for (int i = Game.current.playerCharacter.inventory.weapons.Count; i < items.Count;) {
					GameObject.Destroy (items [i].gameObject);
					items.RemoveAt (i);
				}
			} else if (shop.GetType () == typeof(ArmorShop)) {

			}
		}
		playerGold.text = Game.current.playerCharacter.inventory.gold.ToString () + " g";
		shopkeeperGold.text = shop.gold.ToString () + " g";
	}
}
