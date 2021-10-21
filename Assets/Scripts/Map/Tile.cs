using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Tile {
	public enum UnderType {
		none,
		grass,
		water,
		wall_stone,
		wall_stone_top,
		wall_stone_bottom,
		wall_stone_top_bottom,
		wall_stone_red_top_bottom,
		cliff,
		floor_wood,
		floor_stone,
		sign_weapon,
		sign_armor,
		sign_item,
		sign_inn,
		sand,
		steps_stone_down,
		entrance_wood
	};

	//the order should be in the order they should be displayed
	//0 = bottom
	public enum ModType {
		land_edge_left,
		land_edge_right,
		land_edge_top,
		land_edge_top_left,
		land_edge_top_right,
		land_edge_bottom,
		land_edge_bottom_left,
		land_edge_bottom_right,
		well_stone,
		fence_horizontal,
		fence_vertical,
		cave_exit,
		table_wood,
		drawer_wood,
		bed_bottom,
		pot,
		sign,
		door_wood,
		number_of_values //used to reference the possible number of values
	};

	public enum OverType {
		none,
		bed_top
	};

	public struct Block {
		public int width;
		public int height;
		public Tile[,] data;
	};

	/*
	public struct Portal {
		public bool targetWorldMap; //does this portal lead to the world map?
		public Tile.Block target; //the tile data of the target if this portal doesn not target the world map
		//the position in the target block where this portal leads
		//a value of (-1, -1) indicates that the system should figure out the value itself
		//for example, position = (-1, -1) and the tile containing this portal is inside an alternate area,
		//then the system should figure out that position should be equal to the position of the player in the area he/she was in before he/she entered the alternate area
		//whether that be the world map or another alternate area
		public Vector2 position;
	};
	*/

	public UnderType underType; //rendered below characters
	public List<ModType> mods = new List<ModType> (); //rendered below characters but above underTile; typically for modifying tiles for cosmetic purposes
	public OverType overType; //rendered above characters
	public bool isPortal;
	//public Portal portal;
	public Library<bool> boolAttributes = new Library<bool> ();

	public Tile () {
		underType = UnderType.none;
		overType = OverType.none;
		isPortal = false;
		Init ();
	}

	public Tile (UnderType ut) {
		underType = ut;
		overType = OverType.none;
		isPortal = false;
		Init ();
	}

	public Tile (OverType ot) {
		underType = UnderType.none;
		overType = ot;
		isPortal = false;
		Init ();
	}

	public Tile (UnderType ut, OverType ot) {
		underType = ut;
		overType = ot;
		isPortal = false;
		Init ();
	}

	/*
	public void AddPortal (Portal p) {
		portal = p;
		isPortal = true;
		Init ();
	}
	*/

	void Init () {
		if (mods.Contains (ModType.door_wood)) {
			boolAttributes.Add ("closed", true);
			//boolAttributes.Add ("locked", false);
		}
	}

	public void DoAction (string action) {
		if (action == "open door") {
			
		}
	}

	//public static int tileSize = 32;
	public static int tileSize = 16;

	public static Sprite GetSprite (UnderType t) {
		/*
		switch (t) {
		case Type.grass:
			return master.grass;
		case Type.water:
			return master.water;
		default:
			return new Sprite ();
		}
		*/
		if (t == UnderType.none) {
			return null;
		}
		//return Resources.Load<Sprite> ("Sprites/Tiles/Under/" + t.ToString ());
		return GameResources.GetTileSprite (t);
	}
	public static Sprite GetSprite (ModType t) {
		//return Resources.Load<Sprite> ("Sprites/Tiles/Mod/" + t.ToString ());
		return GameResources.GetTileSprite (t);
	}
	public static Sprite GetSprite (OverType t) {
		if (t == OverType.none) {
			return null;
		}
		//return Resources.Load<Sprite> ("Sprites/Tiles/Over/" + t.ToString ());
		return GameResources.GetTileSprite (t);
	}

	public static Color GetColor(Tile t) {
		/*
		switch (t) {
		case Type.grass:
			return Color.green;
		case Type.water:
			return Color.blue;
		default:
			return Color.black;
		}
		*/
		/*
		if (t.underType.ToString ().StartsWith ("grass"))
			return Color.green;
		else if (t.underType.ToString ().StartsWith ("water"))
			return Color.blue;
		return Color.black;
		*/
		return GameResources.GetTileColor (t.underType);
	}

	public static bool IsLand(Tile t) {
		return t.underType.ToString ().StartsWith ("grass");
	}

	//returns whether or not a character can traverse a certain type of tile
	//used to decide which tiles receive colliders
	//for Christ's sake, we wouldn't wan't the player walking on water or anything like that (pun intended)
	public static bool CanTraverse(Tile t) {
		if (t.mods.Contains (ModType.bed_bottom))
			return false;
		if (t.mods.Contains (ModType.drawer_wood))
			return false;
		if (t == null)
			return false;
		if (t.underType.ToString ().StartsWith ("grass"))
			return true;
		if (t.underType.ToString ().StartsWith ("sand"))
			return true;
		if (t.underType.ToString ().StartsWith ("floor"))
			return true;
		if (t.isPortal)
			return true;
		return false;
	}

	//loads a block of tiles from the TextAsset resource at the provided path
	public static Block LoadBlockFromResource (string path) {
		TextAsset ta = Resources.Load<TextAsset> (path);
		string fileData = ta.text;

		//split the data at each line break
		char[] regex = {'\n'};
		string[] lines = fileData.Split (regex, System.StringSplitOptions.RemoveEmptyEntries);

		Block block;
		block.width = int.Parse (lines [0]);
		block.height = int.Parse (lines [1]);
		block.data = new Tile [block.width, block.height];
		//int iwidth = int.Parse (lines [2]);
		//int iheight = int.Parse (lines [3]);
		for (int i = 0; i < block.width * block.height; i++) {
			int j = i + 2;
			int x = i % block.width;
			int y = i / block.width;
			regex [0] = ',';
			string[] line = lines [j].Split (regex, System.StringSplitOptions.None);
			Tile tile = new Tile ();
			tile.underType = (Tile.UnderType)System.Enum.Parse (typeof(Tile.UnderType), line [0]);
			tile.overType = (Tile.OverType)System.Enum.Parse (typeof(Tile.OverType), line [1]);
			regex [0] = '+';
			string[] mods = line [2].Split (regex, System.StringSplitOptions.RemoveEmptyEntries);
			for (int k = 0; k < mods.Length; k++) {
				tile.mods.Add ((Tile.ModType)System.Enum.Parse (typeof(Tile.ModType), mods [k]));
			}
			/*
			if (line.Length >= 4) {
				Portal p = new Portal ();
				p.targetWorldMap = (line [3] == "$world_map");
				if (!p.targetWorldMap) {
					p.target = Tile.LoadBlockFromResource (line [3]);
				}
				p.position = new Vector2 (-1, -1);
				if (line.Length == 6) {
					int px = int.Parse (line [4]);
					int py = int.Parse (line [5]);
					p.position = new Vector2 (px, py);
				}
				tile.AddPortal (p);
			}
			*/
			block.data [x, y] = tile;
		}
		return block;
	}
}