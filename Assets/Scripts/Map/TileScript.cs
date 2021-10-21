using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileScript : MonoBehaviour {

	public int x;
	public int y;
	SpriteRenderer under;
	SpriteRenderer over;
	List<SpriteRenderer> mods = new List<SpriteRenderer> ();
	List<Animator> anim = new List<Animator> ();

	//keep a copy of the attributes of the target tile
	//that way we can compare it to the tile's copy and then we will know when it changes
	//public Library<bool> boolAttributes = new Library<bool> ();

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		//check for changes that require some kind of action to be performed
		/*
		for (int i = 0; i < Game.current.map.currentMap.GetTile (x, y).boolAttributes.Size (); i++) {
			string att = Game.current.map.currentMap.GetTile (x, y).boolAttributes.GetName (i);
			bool val = Game.current.map.currentMap.GetTile (x, y).boolAttributes.Get (i);
			if(Game.current.map.currentMap.GetTile (x, y).mods.Contains (Tile.ModType.door_wood)) {
				anim [Game.current.map.currentMap.GetTile (x, y).mods.IndexOf (Tile.ModType.door_wood)].SetBool (att, val);
			}
			/*
			bool val = boolAttributes.Get (att);
			bool tval = Game.current.map.GetTile (x, y).boolAttributes.Get (att);
			//this attribute has changed
			if (val != tval) {
				if (att == "closed") {
					anim.SetBool ("closed", tval);
				}
			}

		}
		*/

		//update info about the target tile which must stay current
		//boolAttributes = Game.current.map.GetTile (x, y).boolAttributes;
	}

	public void Init () {
		under = Game.CreateObject<SpriteRenderer> (gameObject);
		under.gameObject.transform.localPosition = new Vector3 (0, 0, 2);
		over = Game.CreateObject<SpriteRenderer> (gameObject);
		over.gameObject.transform.localPosition = new Vector3 (0, 0, -1);
	}

	public void Load (int x, int y) {
		//update info about the target tile
		this.x = x;
		this.y = y;
		//boolAttributes = Game.current.map.GetTile (x, y).boolAttributes;

		//only continue if this tile is actually within bounds of the map
		if (!(x < 0 || x >= Game.current.map.currentMap.GetSize ().x || y < 0 || y >= Game.current.map.currentMap.GetSize ().y)) {
			Tile tile = Game.current.map.currentMap.GetTile (x, y);
			under.sprite = Tile.GetSprite (tile.underType);
			over.sprite = Tile.GetSprite (tile.overType);

			for (int i = 0; i < tile.mods.Count; i++) {
				if (i >= mods.Count) {
					SpriteRenderer sr = Game.CreateObject<SpriteRenderer> (gameObject);
					mods.Add (sr);
				}
				mods [i].gameObject.transform.localPosition = new Vector3 (0, 0, 1 - (1 / (float)(int)Tile.ModType.number_of_values * (float)(int)tile.mods [i]));
				mods [i].sprite = Tile.GetSprite (tile.mods [i]);
				mods [i].gameObject.SetActive (true);
				//add animators if necessary
				if (tile.mods [i] == Tile.ModType.door_wood) {
					mods [i].gameObject.AddComponent<Animator> ();
					Animator a = mods [i].gameObject.GetComponent<Animator> ();
					a.runtimeAnimatorController = Resources.Load <RuntimeAnimatorController> ("Controllers/Animator Controllers/Tiles/door_wood");
					System.Action<Character> openDoor = (Character c) => {
						a.SetBool ("closed", false);
					};
					Game.current.map.currentMap.AddOnCharacterEnterTileObjectAction (x, y, openDoor);
				}
			}
			for (int i = tile.mods.Count; i < mods.Count;) {
				//mods [mods.Count - 1].sprite = null;
				//GameObject.Destroy (mods [mods.Count - 1].gameObject);
				//mods.RemoveAt (mods.Count - 1);
				GameObject.Destroy (mods [i].gameObject);
				mods.RemoveAt (i);
			}

			/*
		if (tile.mods.Contains (Tile.ModType.door_wood)) {
			Animator a = mods [tile.mods.IndexOf (Tile.ModType.door_wood)].gameObject.GetComponent<Animator> ();
			if (a == null) {
				mods [tile.mods.IndexOf (Tile.ModType.door_wood)].gameObject.AddComponent<Animator> ();
				a = mods [tile.mods.IndexOf (Tile.ModType.door_wood)].gameObject.GetComponent<Animator> ();
			}
			a.runtimeAnimatorController = Resources.Load <RuntimeAnimatorController> ("Controllers/Animator Controllers/Tiles/door_wood");
			anim.Insert (tile.mods.IndexOf (Tile.ModType.door_wood), a);
		}
		*/
		} else { //the tile is out of bounds, so set its sprite it to an empty tile
			//deactivate mods
			for (int i = 0; i < mods.Count;) {
				//mods [mods.Count - 1].sprite = null;
				//GameObject.Destroy (mods [mods.Count - 1].gameObject);
				//mods.RemoveAt (mods.Count - 1);
				GameObject.Destroy (mods [i].gameObject);
				mods.RemoveAt (i);
			}
			under.sprite = Tile.GetSprite (Tile.UnderType.none);
			over.sprite = Tile.GetSprite (Tile.OverType.none);
		}
	}
}