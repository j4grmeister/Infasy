using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Map {
	[System.Serializable]
	public struct OnCharacterEnterTileAction {
		public int x;
		public int y;
		public List<System.Action<Character>> actions;
		//list of IDs of characters who have already entered this tile
		//used to prevent the action from being executed more than once on the same character
		public List<ushort> characters;
	}

	public enum Direction {
		up,
		down,
		left,
		right
	}
	[System.Serializable]
	public struct Movement {
		public Direction direction;
		public float distance;
	}

	//unique id
	//0 indicates an unassigned id
	public ushort id = 0;

	//public int mapSize = 3; //width of the map in chunks
	public Vector2 mapSize = new Vector2 (150, 150); //map size in tiles

	public float light = 1f;

	public Chunk[,] chunks;

	public List<OnCharacterEnterTileAction> tileActions = new List<OnCharacterEnterTileAction> ();
	public List<OnCharacterEnterTileAction> tileObjectActions = new List<OnCharacterEnterTileAction> ();

	public MapImage mapImage;

	public Map () {
		
	}

	public void InitMap () {
		chunks = new Chunk [(int)GetSizeInChunks ().x, (int)GetSizeInChunks ().y];
		for (int y = 0; y < GetSizeInChunks ().y; y++) {
			for (int x = 0; x < GetSizeInChunks ().x; x++) {
				chunks [x, y] = new Chunk (x, y);
				//chunks [x, y].SetPosition (x, y);
			}
		}
	}

	public virtual void Update () {
		//Debug.Log ("update map");
		//execute CharacterEnterTileActions
		for (int i = 0; i < tileActions.Count; i++) {
			//create a list of characters who are in the tile right now
			List<ushort> newCharacters = new List<ushort> ();
			for (int ic = 0; ic < Game.current.characters.Count; ic++) {
				Vector2 pos = Game.current.characters [ic].position;
				if ((int)pos.x == tileActions [i].x && (int)pos.y == tileActions [i].y) {
					//Debug.Log (Game.current.characters [ic].name + " has entered the tile.");
					newCharacters.Add (Game.current.characters [ic].id);
				}
			}

			//iterate through each character in the tile
			for (int ic = 0; ic < newCharacters.Count; ic++) {
				//skip this character if it was already in the tile
				if (tileActions [i].characters.Contains (newCharacters [ic]))
					continue;
				//execute each action
				for (int ia = 0; ia < tileActions [i].actions.Count; ia++) {
					tileActions [i].actions [ia] (Game.current.GetCharacterByID (newCharacters [ic]));
				}
			}

			//update the list of characters on this tile
			OnCharacterEnterTileAction newTileAction = tileActions [i];
			newTileAction.characters = newCharacters;
			tileActions [i] = newTileAction;
		}
		//CLEAN UP LATER, CUZ I'M A LAZY BASTARD... AGAIN
		//execute temporary CharacterEnterTileActions
		for (int i = 0; i < tileObjectActions.Count; i++) {
			//create a list of characters who are in the tile right now
			List<ushort> newCharacters = new List<ushort> ();
			for (int ic = 0; ic < Game.current.characters.Count; ic++) {
				Vector2 pos = Game.current.characters [ic].position;
				if ((int)pos.x == tileObjectActions [i].x && (int)pos.y == tileObjectActions [i].y) {
					//Debug.Log (Game.current.characters [ic].name + " has entered the tile.");
					newCharacters.Add (Game.current.characters [ic].id);
				}
			}

			//iterate through each character in the tile
			for (int ic = 0; ic < newCharacters.Count; ic++) {
				//skip this character if it was already in the tile
				if (tileObjectActions [i].characters.Contains (newCharacters [ic]))
					continue;
				//execute each action
				for (int ia = 0; ia < tileObjectActions [i].actions.Count; ia++) {
					tileObjectActions [i].actions [ia] (Game.current.GetCharacterByID (newCharacters [ic]));
				}
			}
			if (newCharacters.Count > 0) {
				tileObjectActions.RemoveAt (i);
			}
		}
	}

	public virtual void Generate () {

	}

	public void AddOnCharacterEnterTileAction (int x, int y, System.Action<Character> action) {
		int index = -1;
		for (int i = 0; i < tileActions.Count; i++) {
			if (tileActions [i].x == x && tileActions [i].y == y) {
				index = i;
				break;
			}
		}
		if (index == -1) {
			OnCharacterEnterTileAction a;
			a.x = x;
			a.y = y;
			a.actions = new List<System.Action<Character>> ();
			a.characters = new List<ushort> ();
			tileActions.Add (a);
			index = tileActions.Count - 1;
		}
		tileActions [index].actions.Add (action);
	}

	//this one adds temporary enter tile actions that are destroyed after the current map is switched or after they are executed once
	public void AddOnCharacterEnterTileObjectAction (int x, int y, System.Action<Character> action) {
		int index = -1;
		for (int i = 0; i < tileObjectActions.Count; i++) {
			if (tileObjectActions [i].x == x && tileObjectActions [i].y == y) {
				index = i;
				break;
			}
		}
		if (index == -1) {
			OnCharacterEnterTileAction a;
			a.x = x;
			a.y = y;
			a.actions = new List<System.Action<Character>> ();
			a.characters = new List<ushort> ();
			tileObjectActions.Add (a);
			index = tileObjectActions.Count - 1;
		}
		tileObjectActions [index].actions.Add (action);
	}

	public void DestroyTileObjectActions () {
		tileObjectActions.Clear ();
	}

	public Vector2 GetCenter() {
		return new Vector2 (mapSize.x / 2, mapSize.y / 2);
	}

	public Vector2 GetCenterChunk() {
		return new Vector2 ((int)(GetSizeInChunks ().x / 2),  (int)(GetSizeInChunks ().y / 2));
	}

	public Vector2 GetSize() {
		return mapSize;
	}

	public Vector2 GetSizeInChunks() {
		return new Vector2 (Mathf.Ceil (mapSize.x / 50), Mathf.Ceil (mapSize.y / 50));
	}

	public void SetTile(int x, int y, Tile t) {
		chunks [x / 50, y / 50].SetTile (x % 50, y % 50, t);
	}

	public void SetTile(int x, int y, Tile.UnderType t) {
		chunks [x / 50, y / 50].SetTile (x % 50, y % 50, t);
	}
	public void SetTile(int x, int y, Tile.OverType t) {
		chunks [x / 50, y / 50].SetTile (x % 50, y % 50, t);
	}
	/*
	public void SetTile(int x, int y, Tile.Portal p) {
		chunks [x / 50, y / 50].SetTile (x % 50, y % 50, p);
	}
	*/

	public void AddMod(int x, int y, Tile.ModType t) {
		chunks [x / 50, y / 50].AddMod (x % 50, y % 50, t);
	}

	public Tile GetTile(int x, int y) {
		return chunks [x / 50, y / 50].GetTile (x % 50, y % 50);
	}

	public static Vector2 Vector2FromDirection (Direction d) {
		if (d == Direction.up) {
			return Vector2.up;
		}
		if (d == Direction.down) {
			return Vector2.down;
		}
		if (d == Direction.left) {
			return Vector2.left;
		}
		if (d == Direction.right) {
			return Vector2.right;
		}
		return Vector2.zero;
	}
	public static Direction DirectionFromVector2 (Vector2 v) {
		if (v.normalized.Equals (Vector2.up)) {
			return Direction.up;
		} else if (v.normalized.Equals (Vector2.down)) {
			return Direction.down;
		} else if (v.normalized.Equals (Vector2.left)) {
			return Direction.left;
		} else if (v.normalized.Equals (Vector2.right)) {
			return Direction.right;
		}
		return Direction.down;
	}
}