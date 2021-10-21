using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Chunk {
	public Vector2 position;
	Tile[,] data = new Tile[50, 50];

	public Chunk(int x, int y) {
		position = new Vector2 (x, y);
	}

	public Vector2 GetPosition() {
		return position;
	}

	public Tile GetTile(int x, int y) {
		return data [x, y];
	}

	public Tile[,] GetTiles() {
		return data;
	}

	public void SetTile(int x, int y, Tile t) {
		data [x, y] = t;
	}

	public void SetTile(int x, int y, Tile.UnderType t) {
		if (data [x, y] == null)
			data [x, y] = new Tile (t);
		else
			data [x, y].underType = t;
	}
	public void SetTile(int x, int y, Tile.OverType t) {
		if (data [x, y] == null)
			data [x, y] = new Tile (t);
		else
			data [x, y].overType = t;
	}
	/*
	public void SetTile(int x, int y, Tile.Portal p) {
		if (data [x, y] == null)
			data [x, y] = new Tile ();
		data [x, y].AddPortal (p);
	}
	*/

	public void AddMod(int x, int y, Tile.ModType t) {
		data [x, y].mods.Add (t);
	}
}