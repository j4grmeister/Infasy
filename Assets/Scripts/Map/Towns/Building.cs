using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Building {
	public ushort id = 0;
	public ushort townID = 0;
	public Vector2Int position; //coordinate of the lower left corner (in world coordinates)
	public Vector2Int size;
	//public Vector2 insideSize;
	public Type type;
	protected Tile[,] tiles;
	protected int doorX; //only the x-coordinate is required since the y-coordinate is always 0

	public Vector2Int doorPosition {
		get {
			return new Vector2Int (position.x + doorX, position.y);
		}
	}

	//Tile[,] outside;
	//Tile[,] inside;

	public enum Type {
		weapon_shop,
		armor_shop,
		item_shop,
		inn,
		house
	};

	public Building () {

	}

	public Building (int x, int y) {
		//insideSize = new Vector2 (iwidth, iheight);
		position = new Vector2Int (x, y);
		//inside = new Tile [iwidth, iheight];
	}

	public void Generate () {
		id = Game.current.RequestID ();
		//Random.InitState (position.x * 50 - position.y / position.x * 3);
		size = new Vector2Int (Random.Range (5, 10), Random.Range (5, 10));
		tiles = new Tile[size.x, size.y];
		for (int x = 0; x < size.x; x++) {
			for (int y = 0; y < size.y; y++) {
				tiles [x, y] = new Tile ();
			}
		}
		GenerateBuilding ();
		MakeBuilding ();
	}

	protected virtual void GenerateBuilding () {
		GenerateEmpty ();
	}

	protected void GenerateEmpty () {
		//add walls
		tiles [0, 0].underType = Tile.UnderType.wall_stone_bottom;
		tiles [size.x - 1, 0].underType = Tile.UnderType.wall_stone_bottom;
		tiles [0, size.y - 1].underType = Tile.UnderType.wall_stone_top;
		tiles [size.x - 1, size.y - 1].underType = Tile.UnderType.wall_stone_top;
		for (int x = 1; x < size.x - 1; x++) {
			tiles [x, 0].underType =  Tile.UnderType.wall_stone_top_bottom;
			tiles [x, size.y - 1].underType = Tile.UnderType.wall_stone_top_bottom;
		}
		for (int y = 1; y < size.y - 1; y++) {
			tiles [0, y].underType = Tile.UnderType.wall_stone;
			tiles [size.x - 1, y].underType = Tile.UnderType.wall_stone;
		}
		//add floor
		for (int x = 1; x < size.x - 1; x++) {
			for (int y = 1; y < size.y - 1; y++) {
				tiles [x, y].underType = Tile.UnderType.floor_wood;
			}
		}
		//add door
		doorX = Random.Range (1, size.x - 1);
		tiles [doorX, 0].underType = Tile.UnderType.floor_wood;
		tiles [doorX, 0].mods.Add (Tile.ModType.door_wood);
	}

	void MakeBuilding () {
		for (int x = 0; x < size.x; x++) {
			for (int y = 0; y < size.y; y++) {
				Game.current.map.SetTile (position.x + x, position.y + y, tiles [x, y]);
			}
		}
	}

	/*
	public void SetIn (int x, int y, Tile t) {
		inside [x, y] = t;
	}
	*/

	/*
	public Tile GetOut (int x, int y) {
		return outside [x, y];
	}
	*/

	/*
	public Tile GetIn (int x, int y) {
		return inside [x, y];
	}
	*/

	/*
	public static Building GenerateBuilding(Type t) {
		string path = "Data/Buildings/" + t.ToString ();
		Tile.Block block = Tile.LoadBlockFromResource (path);

		Building building = new Building (block.width, block.height);
		building.type = t;

		building.outside = block.data;

		/*
		TextAsset bd = Resources.Load<TextAsset> (path);
		string data = bd.text;
		
		//split the data at each line break
		char[] regex = {'\n'};
		string[] lines = data.Split (regex, System.StringSplitOptions.RemoveEmptyEntries);

		int owidth = int.Parse (lines [0]);
		int oheight = int.Parse (lines [1]);
		//int iwidth = int.Parse (lines [2]);
		//int iheight = int.Parse (lines [3]);
		Building building = new Building (owidth, oheight, iwidth, iheight);
		building.type = t;
		regex [0] = ',';
		for (int i = 0; i < owidth * oheight; i++) {
			int j = i + 4;
			int x = i % owidth;
			int y = i / owidth;
			string[] line = lines [j].Split (regex, System.StringSplitOptions.None);
			Tile tile = new Tile ();
			tile.underType = (Tile.UnderType)System.Enum.Parse (typeof(Tile.UnderType), line [0]);
			tile.modType = (Tile.ModType)System.Enum.Parse (typeof(Tile.ModType), line [1]);
			tile.overType = (Tile.OverType)System.Enum.Parse (typeof(Tile.OverType), line [2]);
			building.SetOut (x, y, tile);
		}
		for (int i = 0; i < iwidth * iheight; i++) {
			int j = i + owidth * oheight + 4;
			int x = i % iwidth;
			int y = i / iwidth;
			string[] line = lines [j].Split (regex, System.StringSplitOptions.None);
			Tile tile = new Tile ();
			tile.underType = (Tile.UnderType)System.Enum.Parse (typeof(Tile.UnderType), line [0]);
			tile.modType = (Tile.ModType)System.Enum.Parse (typeof(Tile.ModType), line [1]);
			tile.overType = (Tile.OverType)System.Enum.Parse (typeof(Tile.OverType), line [2]);
			//building.SetIn (x, y, tile);
		}
		//

		return building;
	}
	*/
}