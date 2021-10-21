using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CaveMap : Map {
	public struct CaveSystem {
		//the coordinates of the lower leftmost tile
		public int x;
		public int y;
		public int width;
		public int height;
		public bool[,] cells; //true = alive/not open; false = dead/open
		public int size {
			get {
				int n = 0;
				for (int x = 0; x < width; x++) {
					for (int y = 0; y < height; y++) {
						if (!cells [x, y])
							n++;
					}
				}
				return n;
			}
		}
	}

	static float intersectionRadius = 5f; //the max distance which intersections must be from each other in order to be considered "near" each other
	static int intersectionsForEnemy = 3;
	static float minEnemySpawnDistance = 20f;

	static float aliveProbability = .45f;
	static int starvationLimit = 3;
	static int birthLimit = 4;
	//static int overPopulationLimit = 7;
	static int iterations = 10;

	static int automatonSize = 150; //the size (in tiles) of the full cellular automaton, before it is cut down in size
	//static int idealCaveSize = 3000; //the number of open tiles in an ideal cave

	public string name;
	public Vector2 worldMapEntrance; //the coordinates of the entrance to this cave on the world map
	public Vector2 caveEntrance; //the coordinates of the entrance to this cave in the cave's own coordinate system
	public bool generated = false;
	public List<Vector2> spawnPoints = new List<Vector2> ();

	public CaveMap () {
		worldMapEntrance = Vector2.zero;
	}

	public CaveMap (Vector2 pos) {
		worldMapEntrance = pos;
	}

	void Init () {
		//mapSize = 1;
		base.InitMap ();
	}

	public void SpawnEnemies () {
		for (int i = 0; i < spawnPoints.Count; i++) {
			Character c = Character.CreateCharacter (Character.Race.skeleton, Character.Alignment.evil, typeof(SkeletonAI));
			c.mapID = id;
			c.position = spawnPoints [i];
			Game.current.AddCharacter (c);
		}
	}

	public override void Generate () {
		Random.InitState (Game.current.seed * (int)worldMapEntrance.x - (int)worldMapEntrance.y);
		GenerateMap ();
		ReviseMap ();
		DetermineEntrance ();
		GenerateSpawnPoints ();
		CleanUpMap ();
		generated = true;
		SpawnEnemies ();
	}

	void GenerateMap () {
		//use a cellular automaton (similar to Conway's Game of Life) to generate a cave
		//temporary array of alive and dead cells
		//true = alive; false = dead
		bool[,] cells = new bool [automatonSize, automatonSize];

		//generate initial state
		for (int y = 0; y < automatonSize; y++) {
			for (int x = 0; x < automatonSize; x++) {
				float r = Random.Range (0f, 1f);
				bool alive = (r <= aliveProbability);
				cells [x, y] = alive;
			}
		}

		//iterate through the cellular automaton
		for (int i = 0; i < iterations; i++) {
			//the values must be temporarily stored in another array so as not to affect the results of other tiles operated on
			bool[,] cellsTemp = new bool [automatonSize, automatonSize];
			for (int y = 0; y < automatonSize; y++) {
				for (int x = 0; x < automatonSize; x++) {
					cellsTemp [x, y] = cells [x, y]; //set the initial value

					//count the neighbors of the cell
					//anything out of bounds of the array is considered alive
					int neighbors = 0;
					for (int ly = -1; ly <= 1; ly++) {
						for (int lx = -1; lx <= 1; lx++) {
							if (lx == 0 && ly == 0) {//this coordinate is this cell. skip it because we only want to count neighbors, not including ourself
								continue;
							}
							int xc = x + lx;
							int yc = y + ly;
							if (xc < 0 || xc >= automatonSize || yc < 0 || yc >= automatonSize) { //this coordinate is out of bounds
								neighbors++;
								continue;
							}
							if (cells [xc, yc]) {
								neighbors++;
							}
						}
					}

					//perform the operation on this cell
					if (cells [x, y]) { //cell is alive
						if (neighbors < starvationLimit) {
							cellsTemp [x, y] = false;
						} else {
							cellsTemp [x, y] = true;
						}
					} else { //cell is dead
						if (neighbors > birthLimit) {
							cellsTemp [x, y] = true;
						} else {
							cellsTemp [x, y] = false;
						}
					}
				}
			}
			//load the temporary array into the real one before continuing
			cells = cellsTemp;
		}


		List<CaveSystem> systems = FloodFill (ref cells, automatonSize, automatonSize);
		/*
		for (int i = 0; i < systems.Count; i++) {
			Debug.Log ("(" + systems [i].x + ", " + systems [i].y + ")\n" + systems [i].size + ":(" + systems [i].width + ", " + systems [i].height + ")");
		}
		*/

		//determine the best system to use
		//chooses the largest cave system
		int bestIndex = 0;
		int bestSize = systems [0].size;
		for (int i = 1; i < systems.Count; i++) {
			int size = systems [i].size;
			//if (Mathf.Abs (idealCaveSize - size) < Mathf.Abs (idealCaveSize - bestSize)) { //this system is a better fit than the last one
			if (size > bestSize) {
				bestSize = size;
				bestIndex = i;
			}
		}

		//use the final state of the cell map from the best system fit to create the map
		mapSize = new Vector2 (systems [bestIndex].width, systems [bestIndex].height);
		Init ();
		for (int y = 0; y < systems [bestIndex].height; y++) {
			for (int x = 0; x < systems [bestIndex].width; x++) {
				SetTile (x, y, (systems [bestIndex].cells [x, y]) ? Tile.UnderType.none : Tile.UnderType.floor_stone);
			}
		}
	}

	//obtains and returns information about each cave system by "filling the caves with water"
	List<CaveSystem> FloodFill (ref bool[,] cells, int width, int height) {
		List<CaveSystem> systems = new List<CaveSystem> ();
		bool[,] filled = new bool[automatonSize, automatonSize];
		bool[,] filledTemp =  new bool[automatonSize, automatonSize];
		//initialize the filled array
		for (int x = 0; x < automatonSize; x++) {
			for (int y = 0; y < automatonSize; y++) {
				filled [x, y] = false;
				filledTemp [x, y] = false;
			}
		}
		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {
				if (cells [x, y] || filled [x, y]) {
					continue;
				}
				CaveSystem s = new CaveSystem ();
				s.x = x;
				s.y = y;
				s.width = 1;
				s.height = 1;
				List<Vector2> queue = new List<Vector2> ();
				queue.Add (new Vector2 (x, y));
				filled [x, y] = true;
				filledTemp [x, y] = true;
				/*
				Debug.DrawLine (Game.GameToUnity (new Vector2 (x - .5f, y - .5f)), Game.GameToUnity (new Vector2 (x + .5f, y - .5f)), Color.magenta, 1000);
				Debug.DrawLine (Game.GameToUnity (new Vector2 (x + .5f, y - .5f)), Game.GameToUnity (new Vector2 (x + .5f, y + .5f)), Color.magenta, 1000);
				Debug.DrawLine (Game.GameToUnity (new Vector2 (x + .5f, y + .5f)), Game.GameToUnity (new Vector2 (x - .5f, y + .5f)), Color.magenta, 1000);
				Debug.DrawLine (Game.GameToUnity (new Vector2 (x - .5f, y + .5f)), Game.GameToUnity (new Vector2 (x - .5f, y - .5f)), Color.magenta, 1000);
				*/
				while (queue.Count > 0) {
					Vector2 v = queue [0];
					queue.RemoveAt (0);
					for (int xo = -1; xo <= 1; xo++) {
						for (int yo = -1; yo <= 1; yo++) {
							if (Mathf.Abs (xo) == Mathf.Abs (yo)) {
								continue;
							}
							int xc = (int)v.x + xo;
							int yc = (int)v.y + yo;
							if (xc < 0 || xc >= width || yc < 0 || yc >= height) {
								continue;
							}
							if (cells [xc, yc] || filled [xc, yc]) {
								continue;
							}
							filled [xc, yc] = true;
							filledTemp [xc, yc] = true;
							/*
							Debug.DrawLine (Game.GameToUnity (new Vector2 (xc - .5f, yc - .5f)), Game.GameToUnity (new Vector2 (xc + .5f, yc - .5f)), Color.magenta, 1000);
							Debug.DrawLine (Game.GameToUnity (new Vector2 (xc + .5f, yc - .5f)), Game.GameToUnity (new Vector2 (xc + .5f, yc + .5f)), Color.magenta, 1000);
							Debug.DrawLine (Game.GameToUnity (new Vector2 (xc + .5f, yc + .5f)), Game.GameToUnity (new Vector2 (xc - .5f, yc + .5f)), Color.magenta, 1000);
							Debug.DrawLine (Game.GameToUnity (new Vector2 (xc - .5f, yc + .5f)), Game.GameToUnity (new Vector2 (xc - .5f, yc - .5f)), Color.magenta, 1000);
							*/
							queue.Add (new Vector2 (xc, yc));
						}
					}
					if ((int)v.x < s.x) {
						//Debug.Log ("expand west");
						s.width += s.x - (int)v.x;
						s.x = (int)v.x;
					} else if ((int)v.x >= s.x + s.width) {
						//Debug.Log ("expand east");
						s.width += (int)v.x - s.width - s.x + 1;
					}
					if ((int)v.y < s.y) {
						//Debug.Log ("expand south");
						s.height += s.y - (int)v.y;
						s.y = (int)v.y;
					} else if ((int)v.y >= s.y + s.height) {
						//Debug.Log ("expand north");
						s.height += (int)v.y - s.height - s.y + 1;
					}
				}
				s.cells = new bool[s.width, s.height];
				for (int xb = 0; xb < s.width; xb++) {
					for (int yb = 0; yb < s.height; yb++) {
						s.cells [xb, yb] = !filledTemp [s.x + xb, s.y + yb];
					}
				}
				for (int xr = 0; xr < width; xr++) {
					for (int yr = 0; yr < height; yr++) {
						filledTemp [xr, yr] = false;
					}
				}
				systems.Add (s);
			}
		}
		return systems;
	}

	void ReviseMap () {
		for (int x = 0; x < (int)mapSize.x; x++) {
			for (int y = 0; y < (int)mapSize.y; y++) {
				if (GetTile (x, y).underType == Tile.UnderType.none) {
					//count the number of not open neighbors this tile has
					int neighbors = 0;
					for (int xo = -1; xo <= 1; xo++) {
						for (int yo = -1; yo <= 1; yo++) {
							if (Mathf.Abs (xo) == Mathf.Abs (yo)) {
								continue;
							}
							int xc = x + xo;
							int yc = y + yo;
							if (xc < 0 || xc >= (int)mapSize.x || yc < 0 || yc >= (int)mapSize.y) {
								continue;
							}
							if (GetTile (xc, yc).underType == Tile.UnderType.none) {
								neighbors++;
							}
						}
					}
					//make this tile open if it has one or less neighbor
					if (neighbors <= 1) {
						SetTile (x, y, Tile.UnderType.floor_stone);
					}
				}
			}
		}
	}

	//determines the location of the cave's entrance/exit
	void DetermineEntrance () {
		//list of possible locations
		List<Vector2> possibleLocations = new List<Vector2> ();
		for (int x = 1; x < (int)mapSize.x - 1; x++) {
			for (int y = 2; y < (int)mapSize.y - 1; y++) {
				if (GetTile (x, y).underType == Tile.UnderType.floor_stone) {
					continue;
				}
				if (GetTile (x - 1, y).underType == Tile.UnderType.floor_stone) {
					continue;
				}
				if (GetTile (x + 1, y).underType == Tile.UnderType.floor_stone) {
					continue;
				}

				if (GetTile (x, y + 1).underType == Tile.UnderType.none) {
					continue;
				}
				if (GetTile (x - 1, y + 1).underType == Tile.UnderType.none) {
					continue;
				}
				if (GetTile (x + 1, y + 1).underType == Tile.UnderType.none) {
					continue;
				}

				if (GetTile (x, y - 1).underType == Tile.UnderType.floor_stone) {
					continue;
				}
				if (GetTile (x - 1, y - 1).underType == Tile.UnderType.floor_stone) {
					continue;
				}
				if (GetTile (x + 1, y - 1).underType == Tile.UnderType.floor_stone) {
					continue;
				}

				if (GetTile (x, y - 2).underType == Tile.UnderType.floor_stone) {
					continue;
				}
				if (GetTile (x - 1, y - 2).underType == Tile.UnderType.floor_stone) {
					continue;
				}
				if (GetTile (x + 1, y - 2).underType == Tile.UnderType.floor_stone) {
					continue;
				}

				possibleLocations.Add (new Vector2 (x, y));
			}
		}
		int index = Random.Range (0, possibleLocations.Count);
		caveEntrance = possibleLocations [index];

		System.Action<Character> backToWorldMap = (Character c) => {
			c.mapID = Game.current.map.id;
			c.position = worldMapEntrance + Vector2.down;
			//destroy the characters in this cave
			List<ushort> mc = Game.current.GetCharactersOnMap (id);
			for (int i = 0; i < mc.Count; i++) {
				Game.current.DestroyCharacter (mc [i]);
			}
		};
		AddOnCharacterEnterTileAction ((int)caveEntrance.x, (int)caveEntrance.y, backToWorldMap);
	}

	//Draw lines originating from every corner on the map. "Pockets" within the cave will tend to have a higher density of intersections.
	//Please be aware that this is probably very time consuming; but less so than other algorithms I considered.
	void GenerateSpawnPoints () {
		//generate lines
		//lists of the start and end point of each line
		List<Vector2> point0 = new List<Vector2> ();
		List<Vector2> point1 = new List<Vector2> ();
		for (int x = 0; x < (int)mapSize.x; x++) {
			for (int y = 0; y < (int)mapSize.y; y++) {
				if (GetTile (x, y).underType == Tile.UnderType.floor_stone) {
					continue;
				}
				//count the number of neighbors this tile has; a corner will always have 2 neighbors which are not opposite each other
				List<Vector2> neighborOffsets = new List<Vector2> ();
				for (int xo = -1; xo <= 1; xo++) {
					for (int yo = -1; yo <= 1; yo++) {
						if (Mathf.Abs (xo) == Mathf.Abs (yo)) {
							continue;
						}
						int xc = x + xo;
						int yc = y + yo;
						if (xc < 0 || xc >= (int)mapSize.x || yc < 0 || yc >= (int)mapSize.y) {
							continue;
						}
						if (GetTile (xc, yc).underType == Tile.UnderType.none) {
							neighborOffsets.Add (new Vector2 (xo, yo));
						}
					}
				}
				if (neighborOffsets.Count == 2) {
					//the sum of the neighbor offsets will be 0 if they are exactly opposite each other
					if (neighborOffsets [0] + neighborOffsets [1] != Vector2.zero) {
						point0.Add (new Vector2 (x, y));
						Vector2 dir = (neighborOffsets [0] + neighborOffsets [1]) * -1;
						//Debug.Log (neighborOffsets [0] + "\n" + neighborOffsets [1]);
						Vector2 p = new Vector2 (x, y);
						while (true) {
							p += dir;
							if (p.x < 0 || p.x >= mapSize.x || p.y < 0 || p.y >= mapSize.y) {
								break;
							}
							if (GetTile ((int)p.x, (int)p.y).underType == Tile.UnderType.none || GetTile ((int)p.x - (int)dir.x, (int)p.y).underType == Tile.UnderType.none || GetTile ((int)p.x, (int)p.y - (int)dir.y).underType == Tile.UnderType.none) {
								break;
							}
						}
						point1.Add (p);
					}
				}
			}
		}
		//find intersections
		List<Vector2> intersections = new List<Vector2> ();
		for (int i = 0; i < point0.Count; i++) {
			//Debug.Log (point0 [i] + "\n" + point1 [i]);
			//Debug.DrawLine (Game.GameToUnity (point0 [i] + new Vector2 (.5f, .5f)), Game.GameToUnity (point1 [i] + new Vector2 (.5f, .5f)), Color.magenta, 1000f);
			float m1 = (point1 [i].y - point0 [i].y) / (point1 [i].x - point0 [i].x);
			float x1 = point0 [i].x;
			float y1 = point0 [i].y;
			for (int j = i + 1; j < point0.Count; j++) {
				float m2 = (point1 [j].y - point0 [j].y) / (point1 [j].x - point0 [j].x);
				float x2 = point0 [j].x;
				float y2 = point0 [j].y;

				float x = (m1 * x1 - m2 * x2 - y1 + y2) / (m1 - m2);
				float y = m1 * (x - x1) + y1;
				intersections.Add (new Vector2 (x, y));
			}
		}

		//iterate through tiles
		for (int x = 0; x < (int)mapSize.x; x++) {
			for (int y = 0; y < (int)mapSize.y; y++) {
				//skip this tile if it is not open
				if (GetTile (x, y).underType == Tile.UnderType.none) {
					continue;
				}
				//skip this tile if it is too close to another enemy spawn point
				bool tooClose = false;
				for (int i = 0; i < spawnPoints.Count; i++) {
					if (Vector2.Distance (new Vector2 (x, y), spawnPoints [i]) < minEnemySpawnDistance) {
						tooClose = true;
						break;
					}
				}
				if (tooClose) {
					continue;
				}

				//count the number of intersections "near" this tile
				int ic = 0;
				for (int i = 0; i < intersections.Count; i++) {
					if (Vector2.Distance (new Vector2 (x, y), intersections [i]) < intersectionRadius) {
						ic++;
					}
				}
				if (ic >= intersectionsForEnemy) {
					spawnPoints.Add (new Vector2 (x, y));
				}
			}
		}
	}

	//replace tiles to make the cave look nicer
	void CleanUpMap () {
		//change the tile at the entrance/exit of the cave
		SetTile ((int)caveEntrance.x, (int)caveEntrance.y, Tile.UnderType.floor_stone);
		AddMod ((int)caveEntrance.x, (int)caveEntrance.y, Tile.ModType.cave_exit);

		//make changes that must be made by iterating
		for (int x = 0; x < (int)mapSize.x; x++) {
			for (int y = 0; y < (int)mapSize.y; y++) {
				if (GetTile (x, y).underType == Tile.UnderType.none) {
					if (y > 0) { //the tile below this one is not out of bounds
						//change to cliff if needed
						if (GetTile (x, y - 1).underType == Tile.UnderType.floor_stone) {
							SetTile (x, y, Tile.UnderType.cliff);
							continue;
						}
						if (GetTile (x, y - 1).underType == Tile.UnderType.cliff) {
							AddMod (x, y, Tile.ModType.land_edge_bottom);
						}
					}
					if (y < (int)mapSize.y - 1) { //the tile above this one is not out of bounds
						if (GetTile (x, y + 1).underType == Tile.UnderType.floor_stone && !GetTile (x, y + 1).mods.Contains (Tile.ModType.cave_exit)) {
							AddMod (x, y, Tile.ModType.land_edge_top);
						}
					}
					if (x > 0) { //the tile to the left of this one is not out of bounds
						if (GetTile (x - 1, y).underType == Tile.UnderType.floor_stone || GetTile (x - 1, y).underType == Tile.UnderType.cliff) {
							AddMod (x, y, Tile.ModType.land_edge_left);
						}
					}
					if (x < (int)mapSize.x - 1) { //the tile to the right of this one is not out of bounds
						if (GetTile (x + 1, y).underType == Tile.UnderType.floor_stone) {
							AddMod (x, y, Tile.ModType.land_edge_right);
						}
						//repeat the previuos step with different conditions because of the direction we are iterating
						if (y > 0) {
							if (GetTile (x + 1, y).underType == Tile.UnderType.none && GetTile (x + 1, y - 1).underType == Tile.UnderType.floor_stone) {
								AddMod (x, y, Tile.ModType.land_edge_right);
							}
						}
					}
				}
			}
		}
	}

	//TODO: refine later; consider using distance from entrance to determine the validity of the spawn point
	public Vector2 GetSignificantSpawnPoint () {
		return spawnPoints [Random.Range (0, spawnPoints.Count)];
	}

	public static string GenerateName () {
		string path = "Data/Random/caveNames";
		TextAsset file = Resources.Load<TextAsset> (path);
		string data = file.text;
		string adj = "";
		string noun = "";

		//find beginning index of adjectives
		int i = 0;
		for (; true; i++) {
			if (data [i] == '<') {
				string match = data.Substring (i + 1, 9);
				if (match == "adjective") {
					i += 12;
					break;
				}
			}
		}
		//create a list of adjectives
		List<string> adjs = new List<string> ();
		for (int l = 0; true; l++) {
			if (data [i] == '<') {
				string match = data.Substring (i + 1, 10);
				if (match == "/adjective") {
					break;
				}
			}
			if (data [i + l] == '\n') {
				adjs.Add (data.Substring (i, l));
				i += l + 1;
				l = 0;
			}
		}

		//find beginning index of nouns
		i = 0;
		for (; true; i++) {
			if (data [i] == '<') {
				string match = data.Substring (i + 1, 4);
				if (match == "noun") {
					break;
				}
			}
		}
		//create a list of nouns
		List<string> nouns = new List<string> ();
		for (int l = 0; true; l++) {
			if (data [i] == '<') {
				string match = data.Substring (i + 1, 5);
				if (match == "/noun") {
					break;
				}
			}
			if (data [i + l] == '\n') {
				nouns.Add (data.Substring (i, l));
				i += l + 1;
				l = 0;
			}
		}

		//generate and return a random name
		adj = adjs [Random.Range(0, adjs.Count)];
		noun = nouns [Random.Range(0, nouns.Count)];
		return (adj + " " + noun).ToLower ();
	}
}