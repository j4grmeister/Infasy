using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class House : Building {
	ushort residentID = 0;
	Character resident {
		get {
			return Game.current.GetCharacterByID (residentID);
		}
	}

	Vector2Int bedPosition;
	Vector2Int drawerPosition;
	Vector2 residentPosition;

	public House (int x, int y) : base (x, y) {

	}

	protected override void GenerateBuilding () {
		GenerateEmpty ();
		PlaceBed ();
		PlaceDrawer ();
		MakeHouse ();
		GenerateResident ();
	}

	void GenerateResident () {
		Character c = Character.CreateCharacter (Character.Race.man, Character.Alignment.good, typeof(TownsPersonAI));
		c.name = Character.GenerateName (Character.Race.man);
		c.mapID = Game.current.map.id;
		residentPosition = new Vector2 (doorX, 1);
		c.position = position + residentPosition;
		c.characterInfo.Add ("townID", (object)townID);
		c.characterInfo.Add ("houseID", (object)id);
		Game.current.AddCharacter (c);
		residentID = c.id;
	}

	void PlaceBed () {
		//get valid bed locations
		//this consists of any corner that isn't directly in front of the door
		List<Vector2Int> validPos = new List<Vector2Int> ();
		validPos.Add (new Vector2Int (1, size.y - 2));
		validPos.Add (new Vector2Int (size.x - 2, size.y - 2));
		if (!tiles [1, 0].mods.Contains (Tile.ModType.door_wood)) {
			validPos.Add (new Vector2Int (1, 1));
		}
		if (!tiles [size.x - 2, 0].mods.Contains (Tile.ModType.door_wood)) {
			validPos.Add (new Vector2Int (size.x - 2, 1));
		}
		//decide which position to use
		int bedIndex = Random.Range (0, validPos.Count);
		bedPosition = validPos [bedIndex];
	}

	void PlaceDrawer () {
		//place a drawer on a north wall, at least two tiles from the bed
		int o = 0;
		if (bedPosition.y == size.y - 2) {
			if (bedPosition.x == 1) {
				o = Random.Range (3, size.x - 2);
			} else if (bedPosition.x == size.x - 2) {
				o = Random.Range (1, size.x - 4);
			}
		} else {
			o = Random.Range (1, size.x - 2);
		}
		drawerPosition = new Vector2Int (o, size.y - 2);
	}

	void MakeHouse () {
		//place the bed
		tiles [bedPosition.x, bedPosition.y].mods.Add (Tile.ModType.bed_bottom);
		tiles [bedPosition.x, bedPosition.y + 1].overType = Tile.OverType.bed_top;
		//place the drawer
		tiles [drawerPosition.x, drawerPosition.y].mods.Add (Tile.ModType.drawer_wood);
	}
}
