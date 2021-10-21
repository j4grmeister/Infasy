using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WorldMap : Map {
	//static int nOfVPoints = 47;
	//static int smoothMapRadius = 68;
	static int nOfVPoints = 10;
	static int smoothMapRadius = 10;
	static float[] townDensity = {0, 0, 0, 0, 40, 75, 95}; //index = # of towns - 1; value = threshold probability (0-100, the lowest value that will produce this many towns)
	static int minTownDistance = 100;
	static int nOfTowns = 1;
	//static float[] townSize = { }; //index = size of town (in tiles) / 10

	//static int cavesPerChunk = 5;
	static int cavesPerChunk = 1;

	//these are all in game time
	public static float sunriseTime = 6f;
	public static float sunsetTime = 21.5f;
	static float dayNightTransitionTime = .833f; //about 5 minutes
	static float nightLight = .2f;

	List<Map> maps = new List<Map> ();
	public ushort mapID = 0; //id of the current map
	public Map currentMap {
		get {
			if (mapID == id)
				return this;
			else
				return GetMapByID (mapID);
		}
	}
	public List<ushort> caveIDs = new List<ushort> ();

	public List<Town> towns = new List<Town> ();

	//public Chunk[,] chunks = new Chunk[mapSize, mapSize];

	//public Texture2D mapImage;

	public WorldMap () {
		//initialize the map
		if (id == 0) {
			id = Game.current.RequestID ();
		}
		mapID = id;
		mapSize = new Vector2 (250, 250);

		base.InitMap ();

		//generate the map
		//Generate (); //now called in Game.cs
	}

	public override void Update () {
		//update the amount of light based on the time of day
		if (Game.current.timeOfDay >= sunriseTime + dayNightTransitionTime / 2f && Game.current.timeOfDay <= sunsetTime - dayNightTransitionTime / 2f) {
			light = 1f;
		} else if (Game.current.timeOfDay <= sunriseTime - dayNightTransitionTime / 2f || Game.current.timeOfDay >= sunsetTime + dayNightTransitionTime / 2f) {
			light = nightLight;
		} else if (Game.current.timeOfDay > sunriseTime - dayNightTransitionTime / 2f && Game.current.timeOfDay < sunriseTime + dayNightTransitionTime / 2f) {
			float t = (Game.current.timeOfDay - (sunriseTime - dayNightTransitionTime / 2f)) / dayNightTransitionTime;
			light = nightLight + (1f - nightLight) * t;
		} else if (Game.current.timeOfDay > sunsetTime - dayNightTransitionTime / 2f && Game.current.timeOfDay < sunsetTime + dayNightTransitionTime / 2f) {
			float t = (Game.current.timeOfDay - (sunsetTime - dayNightTransitionTime / 2f)) / dayNightTransitionTime;
			light = 1f + (nightLight - 1f) * t;
		}

		base.Update ();
		//update all the other maps
		for (int i = 0; i < maps.Count; i++) {
			//maps [i].Update ();
		}

		//set the current map to the map that the player is in
		if (mapID != Game.current.playerCharacter.mapID) {
			currentMap.DestroyTileObjectActions ();
			Game.current.DisableCharactersOnMap (mapID);
			mapID = Game.current.playerCharacter.mapID;
			Game.current.EnableCharactersOnMap (mapID);
		}
	}

	public void AddMap (Map m) {
		if (m.id == 0) {
			m.id = Game.current.RequestID ();
		} else {
			if (Game.current.nextID == m.id)
				Game.current.NextID ();
			else
				Game.current.usedIDs.Add (m.id);
		}
		maps.Add (m);
	}
		
	public Map GetMapByID (ushort mID) {
		for (int i = 0; i < maps.Count; i++) {
			if (maps [i].id == mID)
				return maps [i];
		}
		return null;
	}

	public Town GetTownByID (ushort tID) {
		for (int i = 0; i < towns.Count; i++) {
			if (towns [i].id == tID)
				return towns [i];
		}
		return null;
	}

	public override void Generate () {
		GenerateDeveloperMap (); //for dev purposes only
		/*
		GenerateMap ();
		SmoothMap (smoothMapRadius);
		SmoothMap (15);
		CleanUpMap ();
		GenerateTowns ();
		ReviseMap ();
		GenerateImage ();
		*/
	}

	//remove before building game
	void GenerateDeveloperMap () {
		for (int y = 0; y < GetSize ().y; y++) {
			for (int x = 0; x < GetSize ().x; x++) {
				SetTile (x, y, Tile.UnderType.grass);
			}
		}
		CleanUpMap ();
		GenerateTowns ();
		GenerateCaves ();
		GenerateImage ();
		SetInfo ();
	}
		
	void GenerateMap () {
		Random.InitState (Game.current.seed);
		//generate 
		//create a voronoi map
		Vector2[] p = new Vector2[nOfVPoints];
		bool[] isLand = new bool[nOfVPoints];
		for (int i = 0; i < nOfVPoints; i++) {
			p[i] = new Vector2(Random.Range(0, GetSize().x), Random.Range(0, GetSize().y));
			float vf = Random.Range (0, 2);
			isLand [i] = ((int)(vf) == 1) ? true : false;
		}
		for (int y = 0; y < GetSize ().y; y++) {
			for (int x = 0; x < GetSize ().x; x++) {
				//get index of the nearest point to this tile;
				int closest = 0;
				float d = Mathf.Max (GetSize ().x, GetSize ().y);
				for (int i = 0; i < nOfVPoints; i++) {
					if (Vector2.Distance (new Vector2 (x, y), p [i]) < d) {
						closest = i;
						d = Vector2.Distance (new Vector2 (x, y), p [i]);
					}
				}

				//set this tile equal to that of the nearest voronoi point
				SetTile (x, y, (isLand[closest]) ? Tile.UnderType.grass : Tile.UnderType.water);
			}
		}
	}

	void SmoothMap (int radius) {
		//smooth the map by averaging tiles together
		//keep a buffer so that the initial tiles that are still needed can be used
		if (radius > 0) {
			bool[,] buffer = new bool [(int)GetSize ().x, radius]; //true = land
			for (int y = 0; y < GetSize ().y; y++) {
				for (int x = 0; x < GetSize ().x; x++) {
					//load this tile into the buffer
					buffer [x, radius - 1] = Tile.IsLand(GetTile (x, y));
					//calculate the average of this tiles surrounding tiles
					float average = 0;
					int divide = 0;
					//go in a circle and move outwards
					for (float a = 0; a < 2; a += .2f) {
						Vector2 dir = new Vector2 (Mathf.Cos (a), Mathf.Sin (a)).normalized; //it should already be normalized, but normalize it just to be extra sure
						for (int dis = 0; dis < radius; dis++) {
							Vector2 point = dir * dis;
							point.x = (int)point.x;
							point.y = (int)point.y;
							if ((point.x <= 0 && point.y == 0) || point.y < 0) {
								if (!(x + point.x < 0 || y + point.y < 0)) {
									average += (buffer [x + (int)point.x, radius - 1 + (int)point.y]) ? 1 : 0;
									divide++;
								}
							} else if (x + (int)point.x >= 0) {
								if (!(x + point.x >= GetSize ().x || y + point.y >= GetSize ().y)) {
									average += (Tile.IsLand(GetTile (x + (int)point.x, y + (int)point.y))) ? 1 : 0;
									divide++;
								}
							}
						}
					}
					average /= (float)divide;
					bool land = (Mathf.RoundToInt (average) == 1);
					SetTile (x, y, (land) ? Tile.UnderType.grass : Tile.UnderType.water);
				}
				//update the buffer
				for (int i = 0; i < radius - 1; i++) {
					for (int j = 0; j < GetSize ().x; j++) {
						buffer [j, i] = buffer [j, i + 1];
					}
				}
			}
		}
	}

	//makes the map look cleaner and replaces sprites to make it look nicer
	//ex. replacing the sprite on a tile in between water and land
	void CleanUpMap () {
		for (int y = 25; y < GetSize ().y - 25; y++) {
			for (int x = 25; x < GetSize ().x - 25; x++) {
				//check the tiles adjacent to this one
				bool here = Tile.IsLand (GetTile (x, y));
				bool up = Tile.IsLand (GetTile (x, y + 1));
				bool right = Tile.IsLand (GetTile (x + 1, y));
				bool down = Tile.IsLand (GetTile (x, y - 1));
				bool left = Tile.IsLand (GetTile (x - 1, y));
				int identicalNeighbors = 0;
				if (up == here)
					identicalNeighbors++;
				if (right == here)
					identicalNeighbors++;
				if (down == here)
					identicalNeighbors++;
				if (left == here)
					identicalNeighbors++;
				//if this tile has less than 2 identical neighbors, switch the tile. this helps make the map look more smooth
				if (identicalNeighbors < 2) {
					SetTile (x, y, (Tile.IsLand (GetTile (x, y)) ? Tile.UnderType.water : Tile.UnderType.grass));
					here = !here;
					identicalNeighbors = 4 - identicalNeighbors;
				}
				//up = (up == here);
				//right = (right == here);
				//down = (down == here);
				//left = (left == here);
				//only continue if there is at least one unidentical neighbor
				if (identicalNeighbors < 4) {
					if (here && up && right && down && !left) {
						AddMod (x, y, Tile.ModType.land_edge_left);
					} else if (here && up && !right && down && left) {
						AddMod (x, y, Tile.ModType.land_edge_right);
					} else if (here && !up && right & down && left) {
						AddMod (x, y, Tile.ModType.land_edge_top);
					} else if (here && !up && right && down && !left) {
						AddMod (x, y, Tile.ModType.land_edge_top_left);
					} else if (here && !up && !right && down && left) {
						AddMod (x, y, Tile.ModType.land_edge_top_right);
					} else if (here && up && right && !down && left) {
						AddMod (x, y, Tile.ModType.land_edge_bottom);
					} else if (here && up && right && !down && !left) {
						AddMod (x, y, Tile.ModType.land_edge_bottom_left);
					} else if (here && !right && !down && left) {
						AddMod (x, y, Tile.ModType.land_edge_bottom_right);
					}
				}
			}
		}
	}

	void GenerateTowns () {
		/*
		 * towns are generated in several stages
		 * each stage is performed on each town individually before moving to the next stage
		 * each stage begins with reseeding the RNG, unless otherwise specified
		 * 1: generate the number of towns in the chunk and the coordinates of lower left corner of each town
		 * 2: generate the town size and the town itself
		*/
		//generate the town coordinates
		//generate using a town density paired with probability per chunk
		//don't generate towns for the outter edge chunks
		Random.InitState (Game.current.seed);
		List<Vector2Int> townPos = new List<Vector2Int> ();
		//generate coordinates of each town
		for (int i = 0; i < nOfTowns; i++) {
			//there is a minimum distance between towns, we need to keep that when we generate the coordinates
			//instead of adjusting coordinates by comparing each town,
			//generate the coordinates on a scale that is (1 / minDistance) and then scale up by minDistance
			//Vector2 size = new Vector2 (Random.Range(30, 50), Random.Range(30, 50));
			while(true) {
				Vector2 town = new Vector2 (Random.Range (65, (int)mapSize.x - 65), Random.Range (65, (int)mapSize.y - 65));
				//scale the town coordinates back up and convert its values to int values
				//town *= minTownDistance;


				if (!townPos.Contains (Vector2Int.RoundToInt (town)) && IsValidTown (town)) {
					townPos.Add (Vector2Int.RoundToInt (town));
					break;
				}
			}
		}

		//create the towns
		for (int i = 0; i < nOfTowns; i++) {
			Town t = new Town (townPos [i]);
			towns.Add (t);
		}
		//generate towns
		//this is a separate loop so that the generation of a town doesn't affect the generation of the one after it
		for (int i = 0; i < towns.Count; i++) {
			towns [i].Generate ();
		}
	}

	bool IsValidTown(Vector2 pos) {
		for (int y = 0; y < 130; y++) {
			for (int x = 0; x < 130; x++) {
				if (!Tile.IsLand (GetTile ((int)pos.x - 65 + x, (int)pos.y - 65 + y)))
					return false;
			}
		}
		return true;
	}

	//revises the tiles on the map based on the towns
	void ReviseMap () {
		/*
		for (int i = 0; i < towns.Count; i++) {
			for (int j = 0; j < towns [i].buildings.Count; j++) {
				for (int y = 0; y < towns [i].buildings [j].outsideSize.y; y++) {
					for (int x = 0; x < towns [i].buildings [j].outsideSize.x; x++) {
						//SetTile ((int)towns [i].position.x + x, (int)towns [i].position.y + y, towns [i].buildings [j].GetOut (x, y));
						Tile tile = towns [i].buildings [j].GetOut (x, y);
						Vector2 bpos = towns [i].buildings [j].position;
						if (tile.underType != Tile.UnderType.none)
							SetTile ((int)towns [i].position.x + (int)bpos.x + x, (int)towns [i].position.y + (int)bpos.y + y, tile.underType);
						if (tile.overType != Tile.OverType.none)
							SetTile ((int)towns [i].position.x + (int)bpos.x + x, (int)towns [i].position.y + (int)bpos.y + y, tile.overType);
						for (int k = 0; k < tile.mods.Count; k++) {
							AddMod ((int)towns [i].position.x + (int)bpos.x + x, (int)towns [i].position.y + (int)bpos.y + y, tile.mods [k]);
						}
						/*
						if (tile.isPortal)
							SetTile ((int)towns [i].position.x + (int)bpos.x + x, (int)towns [i].position.y + (int)bpos.y + y, tile.portal);
						//
					}
				}
			}
		}
		*/
	}

	void GenerateCaves () {
		Random.InitState (Game.current.seed);
		for (int cy = 0; cy < GetSizeInChunks ().y; cy++) {
			for (int cx = 0; cx < GetSizeInChunks ().y; cx++) {
				//generate random local chunk coordinates for cave entrances
				List<Vector2> cavePos = new List<Vector2> ();
				for (int i = 0; i < cavesPerChunk; i++) {
					Vector2 v = new Vector2 (Random.Range (0, 50), Random.Range (0, 50));
					if (cavePos.Contains (v)) {
						i--;
						continue;
					}
					cavePos.Add (v);
				}

				//create the caves and add them to the map list
				for (int i = 0; i < cavePos.Count; i++) {
					CaveMap cm = new CaveMap (new Vector2 (cx * 50, cy * 50) + cavePos [i]);
					cm.name = CaveMap.GenerateName ();
					//Debug.Log ("Cave at " + (new Vector2 (cx * 50, cy * 50) + cavePos [i]));
					SetTile (cx * 50 + (int)cavePos [i].x, cy * 50 + (int)cavePos [i].y, Tile.UnderType.steps_stone_down);
					AddMap (cm);
					caveIDs.Add (cm.id);
					//add a new CharacterEnterTileAction to transport the character to the cave
					System.Action<Character> transportCharacter = (Character c) => {
						//generate the cave map if it hasnt been generated yet
						if (!cm.generated) {
							cm.Generate ();
						}
						c.mapID = cm.id;
						//spawn the character one tile above the cave's entrance/exit
						//otherwise the character will immediately exit the cave
						c.position = cm.caveEntrance + Vector2.up;
					};
					AddOnCharacterEnterTileAction ((int)cm.worldMapEntrance.x, (int)cm.worldMapEntrance.y, transportCharacter);
				}
			}
		}
	}
		
	void GenerateImage() {
		mapImage = new MapImage ();
		mapImage.image = new Texture2D ((int)mapSize.x, (int)mapSize.y);
		for (int x = 0; x < (int)mapSize.x; x++) {
			for (int y = 0; y < (int)mapSize.y; y++) {
				mapImage.image.SetPixel (x, y, Tile.GetColor (GetTile (x, y)));
			}
		}
		mapImage.image.filterMode = FilterMode.Point;
		mapImage.image.Apply ();

		//add town nodes
		for (int i = 0; i < towns.Count; i++) {
			Town t = towns [i];
			MapImage.Node n = new MapImage.Node ();
			n.name = t.name;
			n.position = t.position + t.centerOffset - new Vector2Int (t.sizeInTiles.x / 2, t.sizeInTiles.y / 2);
			n.size = t.sizeInTiles;
			n.draw = false;
			n.visible = true;
			mapImage.nodes.Add (n);
		}

		//add cave nodes
		for (int i = 0; i < caveIDs.Count; i++) {
			CaveMap c = (CaveMap)GetMapByID (caveIDs [i]);
			MapImage.Node n = new MapImage.Node ();
			n.name = c.name;
			n.position = Vector2Int.FloorToInt (c.worldMapEntrance) - new Vector2Int (1, 1);
			n.size = new Vector2Int (3, 3);
			n.draw = false;
			n.visible = true;
			mapImage.nodes.Add (n);
		}
	}

	void SetInfo () {
		for (int i = 0; i < towns.Count; i++) {
			towns [i].SetInfo ();
		}
	}
}