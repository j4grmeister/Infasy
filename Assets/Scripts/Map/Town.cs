using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Town {
	[System.Serializable]
	public struct Road {
		public Vector2Int start;
		public Vector2Int end;
	}

	static int blockSize = 10;
	static int roadWidth = 2;

	static System.Type[] buildingPossibilities = {typeof(WeaponShop), typeof(House)};
	static float[] buildingThresholds = {.5f, 1f};

	static int nearbyCaveDistance = 75; //in tiles

	public ushort id = 0;
	public string name;
	public Vector2Int position;
	public Vector2Int sizeInBlocks;
	public Vector2Int sizeInTiles {
		get {
			return new Vector2Int (blockSize + roadWidth, blockSize + roadWidth) * sizeInBlocks;
		}
	}
	//the following are all in local coordinates
	public Vector2Int min = Vector2Int.zero;
	public Vector2Int max = Vector2Int.zero;
	public Vector2Int geometricCenter {
		get {
			return new Vector2Int ((min.x + max.x) / 2, (min.y + max.y) / 2);
		}
	}
	public Vector2Int centerOffset {
		get {
			return new Vector2Int ((min.x + max.x) / -2, (min.y + max.y) / -2);
		}
	}

	public List<Road> roads = new List<Road> ();
	public List<Building> buildings = new List<Building> ();

	public List<ushort> nearbyCaveIDs = new List<ushort> (); //list of ids of caves that are near this town

	public Town (Vector2Int pos) {
		position = pos;
	}

	public Building GetBuildingByID (ushort bID) {
		for (int i = 0; i < buildings.Count; i++) {
			if (buildings [i].id == bID)
				return buildings [i];
		}
		return null;
	}

	//position required in order to seed the rng so that this town's generation doesn't affect that of others
	public void Generate () {
		id = Game.current.RequestID ();
		Random.InitState (Game.current.seed / position.x + position.y / 3 - position.x * position.y);
		name = GenerateName ();
		GenerateRoads (); //generate roads first
		//CompileRoads ();
		SetSize ();
		GenerateBuildings ();
		ScaleRoads ();
		MakeRoads ();
	}

	void SetSize () {
		for (int i = 0; i < roads.Count; i++) {
			if (roads [i].start.x < min.x) {
				min.x = roads [i].start.x;
			} else if (roads [i].start.x > max.x) {
				max.x = roads [i].start.x;
			}
			if (roads [i].end.x < min.x) {
				min.x = roads [i].end.x;
			} else if (roads [i].end.x > max.x) {
				max.x = roads [i].end.x;
			}
			if (roads [i].start.y < min.y) {
				min.y = roads [i].start.y;
			} else if (roads [i].start.y > max.y) {
				max.y = roads [i].start.y;
			}
			if (roads [i].end.y < min.y) {
				min.y = roads [i].end.y;
			} else if (roads [i].end.y > max.y) {
				max.y = roads [i].end.y;
			}
		}
		sizeInBlocks = new Vector2Int (max.x - min.x, max.y - min.y);
	}

	//generates road data
	void GenerateRoads () {
		List<Vector2Int> roadSegmentStart = new List<Vector2Int> ();
		List<Vector2Int> roadSegmentDir = new List<Vector2Int> ();
		List<Vector2Int> extendPoints = new List<Vector2Int> ();
		roadSegmentStart.Add (Vector2Int.zero);
		roadSegmentDir.Add (Vector2Int.up);
		extendPoints.Add (Vector2Int.up);
		roadSegmentStart.Add (Vector2Int.zero);
		roadSegmentDir.Add (Vector2Int.down);
		extendPoints.Add (Vector2Int.down);
		roadSegmentStart.Add (Vector2Int.zero);
		roadSegmentDir.Add (Vector2Int.left);
		extendPoints.Add (Vector2Int.left);
		roadSegmentStart.Add (Vector2Int.zero);
		roadSegmentDir.Add (Vector2Int.right);
		extendPoints.Add (Vector2Int.right);
		for (int r = 2; r > 0; r--) {
			int s = extendPoints.Count;
			for (int i = 0; i < s; i++) {
				List<Vector2Int> posibilities = new List<Vector2Int> ();
				posibilities.Add (Vector2Int.up);
				posibilities.Add (Vector2Int.down);
				posibilities.Add (Vector2Int.left);
				posibilities.Add (Vector2Int.right);
				for (int j = 0; j < roadSegmentStart.Count; j++) {
					if (roadSegmentStart [j] + roadSegmentDir [j] == extendPoints [0]) {
						posibilities.Remove (roadSegmentDir [j] * -1);
					}
				}
				int ne = Mathf.Min (r, posibilities.Count);
				for (int j = 0; j < ne; j++) {
					Vector2Int rdir = posibilities [Random.Range (0, posibilities.Count)];
					roadSegmentStart.Add (extendPoints [0]);
					roadSegmentDir.Add (rdir);
					extendPoints.Add (extendPoints [0] + rdir);
					posibilities.Remove (rdir);
				}
				extendPoints.RemoveAt (0);
			}
		}

		for (int i = 0; i < roadSegmentStart.Count; i++) {
			Road r;
			r.start = roadSegmentStart [i];
			r.end = roadSegmentStart [i] + roadSegmentDir [i];

			//make the road direction in the positive direction before adding it
			//this makes it easier to implement later on
			Road newRoad = r;
			if (newRoad.start.x > newRoad.end.x || newRoad.start.y > newRoad.end.y) {
				newRoad.start = r.end;
				newRoad.end = r.start;
			}
			roads.Add (r);
		}

		/*
		//compile the road segments together with other segments that extend each other
		//int nOfIntersections = 0;
		Road currentRoad;
		currentRoad.start = roadSegmentStart [0];
		currentRoad.end = roadSegmentStart [0] + roadSegmentDir [0];
		roadSegmentStart.RemoveAt (0);
		roadSegmentDir.RemoveAt (0);
		while (roadSegmentStart.Count > 0) {
			Vector2Int cdir = currentRoad.end - currentRoad.start;
			cdir.x = (cdir.x > 0) ? 1 : ((cdir.x < 0) ? -1 : 0);
			cdir.y = (cdir.y > 0) ? 1 : ((cdir.y < 0) ? -1 : 0);
			bool skip = false;
			for (int i = 0; i < roadSegmentStart.Count; i++) {
				if (roadSegmentStart [i] == currentRoad.end && roadSegmentDir [i] == cdir) {
					//nOfIntersections++;
					currentRoad.end = roadSegmentStart [i] + roadSegmentDir [i];
					roadSegmentStart.RemoveAt (i);
					roadSegmentDir.RemoveAt (i);
					skip = true;
					break;
				}
				if (roadSegmentStart [i] + roadSegmentDir [i] == currentRoad.end && roadSegmentDir [i] == cdir * -1) {
					//nOfIntersections++;
					currentRoad.end = roadSegmentStart [i];
					roadSegmentStart.RemoveAt (i);
					roadSegmentDir.RemoveAt (i);
					skip = true;
					break;
				}
				if (roadSegmentStart [i] + roadSegmentDir [i] == currentRoad.start && roadSegmentDir [i] == cdir) {
					//nOfIntersections++;
					currentRoad.start = roadSegmentStart [i];
					roadSegmentStart.RemoveAt (i);
					roadSegmentDir.RemoveAt (i);
					skip = true;
					break;
				}
				if (roadSegmentStart [i] == currentRoad.start && roadSegmentDir [i] == cdir * -1) {
					//nOfIntersections++;
					currentRoad.start = roadSegmentStart [i] + roadSegmentDir [i];
					roadSegmentStart.RemoveAt (i);
					roadSegmentDir.RemoveAt (i);
					skip = true;
					break;
				}
			}
			if (skip) {
				continue;
			}
			//make the road direction in the positive direction before adding it
			//this makes it easier to implement later on
			Road newRoad = currentRoad;
			if (newRoad.start.x > newRoad.end.x || newRoad.start.y > newRoad.end.y) {
				newRoad.start = currentRoad.end;
				newRoad.end = currentRoad.start;
			}
			roads.Add (newRoad);
			//nOfIntersections = 0;
			currentRoad.start = roadSegmentStart [0];
			currentRoad.end = roadSegmentStart [0] + roadSegmentDir [0];
			roadSegmentStart.RemoveAt (0);
			roadSegmentDir.RemoveAt (0);
		}
		*/
	}

	void CompileRoads () {
		//compile the road segments together with other segments that extend each other
		//int nOfIntersections = 0;
		List<Vector2Int> roadSegmentStart = new List<Vector2Int> ();
		List<Vector2Int> roadSegmentDir = new List<Vector2Int> ();
		for (int i = 0; i < roads.Count; i++) {
			Vector2Int cdir = roads [i].end - roads [i].start;
			cdir.x = (cdir.x > 0) ? 1 : ((cdir.x < 0) ? -1 : 0);
			cdir.y = (cdir.y > 0) ? 1 : ((cdir.y < 0) ? -1 : 0);
			roadSegmentStart.Add (roads [i].start);
			roadSegmentDir.Add (cdir);
		}
		roads.Clear ();

		Road currentRoad;
		currentRoad.start = roadSegmentStart [0];
		currentRoad.end = roadSegmentStart [0] + roadSegmentDir [0];
		roadSegmentStart.RemoveAt (0);
		roadSegmentDir.RemoveAt (0);
		while (roadSegmentStart.Count > 0) {
			Vector2Int cdir = currentRoad.end - currentRoad.start;
			cdir.x = (cdir.x > 0) ? 1 : ((cdir.x < 0) ? -1 : 0);
			cdir.y = (cdir.y > 0) ? 1 : ((cdir.y < 0) ? -1 : 0);
			bool skip = false;
			for (int i = 0; i < roadSegmentStart.Count; i++) {
				if (roadSegmentStart [i] == currentRoad.end && roadSegmentDir [i] == cdir) {
					//nOfIntersections++;
					currentRoad.end = roadSegmentStart [i] + roadSegmentDir [i];
					roadSegmentStart.RemoveAt (i);
					roadSegmentDir.RemoveAt (i);
					skip = true;
					break;
				}
				if (roadSegmentStart [i] + roadSegmentDir [i] == currentRoad.end && roadSegmentDir [i] == cdir * -1) {
					//nOfIntersections++;
					currentRoad.end = roadSegmentStart [i];
					roadSegmentStart.RemoveAt (i);
					roadSegmentDir.RemoveAt (i);
					skip = true;
					break;
				}
				if (roadSegmentStart [i] + roadSegmentDir [i] == currentRoad.start && roadSegmentDir [i] == cdir) {
					//nOfIntersections++;
					currentRoad.start = roadSegmentStart [i];
					roadSegmentStart.RemoveAt (i);
					roadSegmentDir.RemoveAt (i);
					skip = true;
					break;
				}
				if (roadSegmentStart [i] == currentRoad.start && roadSegmentDir [i] == cdir * -1) {
					//nOfIntersections++;
					currentRoad.start = roadSegmentStart [i] + roadSegmentDir [i];
					roadSegmentStart.RemoveAt (i);
					roadSegmentDir.RemoveAt (i);
					skip = true;
					break;
				}
			}
			if (skip) {
				continue;
			}
			roads.Add (currentRoad);
			//nOfIntersections = 0;
			currentRoad.start = roadSegmentStart [0];
			currentRoad.end = roadSegmentStart [0] + roadSegmentDir [0];
			roadSegmentStart.RemoveAt (0);
			roadSegmentDir.RemoveAt (0);
		}
	}

	//scales roads to their actual size
	void ScaleRoads () {
		for (int i = 0; i < roads.Count; i++) {
			Road currentRoad = roads [i];
			currentRoad.start *= blockSize + roadWidth;
			currentRoad.end *= blockSize + roadWidth;
			//quick workaround to a wierd result where roads with positive x or y directions wouldn't extend by roadWidth
			if ((currentRoad.end - currentRoad.start).x > 0) {
				currentRoad.end += Vector2Int.right * roadWidth;
			}
			if ((currentRoad.end - currentRoad.start).y > 0) {
				currentRoad.end += Vector2Int.up * roadWidth;
			}
			roads [i] = currentRoad;
		}
	}

	//changes the game's map data to match the road data
	void MakeRoads () {
		for (int i = 0; i < roads.Count; i++) {
			Vector2Int start = new Vector2Int (Mathf.Min (roads [i].start.x, roads [i].end.x), Mathf.Min (roads [i].start.y, roads [i].end.y));
			Vector2Int dir = roads [i].end - roads [i].start;
			dir.x = (dir.x != 0) ? 1 : 0;
			dir.y = (dir.y != 0) ? 1 : 0;
			Vector2Int roadSize = new Vector2Int (((dir.x == 1) ? Mathf.Abs ((roads [i].end - roads [i].start).x) : roadWidth), ((dir.y == 1) ? Mathf.Abs ((roads [i].end - roads [i].start).y) : roadWidth));
			for (int x = 0; x < roadSize.x; x++) {
				for (int y = 0; y < roadSize.y; y++) {
					Game.current.map.SetTile (position.x + start.x + x, position.y + start.y + y, Tile.UnderType.sand);
				}
			}
		}
	}

	void GenerateBuildings () {
		//create an array of the validity of all possible lots in this town
		//true = valid lot; false = invalid lot
		//each block is a single lot
		//block coordinates are relative from the lower left hand corner of the town
		bool[,] build = new bool[sizeInBlocks.x + 2, sizeInBlocks.y + 2];
		for (int x = 0; x < sizeInBlocks.x + 2; x++) {
			for (int y = 0; y < sizeInBlocks.y + 2; y++) {
				Vector2Int roadCoord = new Vector2Int (x, y) - new Vector2Int (1, 1) - centerOffset + min;
				bool roadSouth = roadBetween (roadCoord, roadCoord + Vector2Int.right);
				bool roadEast = roadBetween (roadCoord + Vector2Int.right, roadCoord + Vector2Int.right + Vector2Int.up);
				bool roadNorth = roadBetween (roadCoord + Vector2Int.up, roadCoord + Vector2Int.right + Vector2Int.up);
				bool roadWest = roadBetween (roadCoord, roadCoord + Vector2Int.up);
				if (roadSouth || roadEast || roadNorth || roadWest) {
					float d = roadCoord.magnitude;
					float buildingProbability = .8f - (d * .1f);
					build [x, y] = (Random.Range (0f, 1f) <= buildingProbability);
				}
			}
		}
			
		//generate buildings
		for (int x = 0; x < sizeInBlocks.x + 2; x++) {
			for (int y = 0; y < sizeInBlocks.y + 2; y++) {
				if (build [x, y]) {
					//decide what type of building to generate
					float r = Random.Range (0f, 1f);
					int i = 0;
					for (; i < buildingThresholds.Length; i++) {
						if (r <= buildingThresholds [i]) {
							break;
						}
					}
					System.Type buildingType = buildingPossibilities [i];
					//create the building
					Vector2Int pos = (min - new Vector2Int (1, 1) + new Vector2Int (x, y)) * (blockSize + roadWidth) + position + new Vector2Int (roadWidth, roadWidth);
					Building b = (Building)System.Activator.CreateInstance (buildingType, new object[]{pos.x, pos.y});
					b.townID = id;
					//b.id = Game.current.RequestID ();
					b.Generate ();
					buildings.Add (b);
				}
			}
		}
			
		/*
		//debug
		for (int x = 0; x < sizeInBlocks.x + 2; x++) {
			for (int y = 0; y < sizeInBlocks.y + 2; y++) {
				if (build [x, y]) {
					Vector2Int roadCoord = new Vector2Int (x, y) - new Vector2Int (1, 1) - centerOffset + min;
					Vector2Int pos = position + roadCoord * (blockSize + roadWidth);
					Debug.DrawLine (Game.GameToUnity (pos), Game.GameToUnity (pos + Vector2Int.right * (blockSize)), Color.magenta, 1000f);
					Debug.DrawLine (Game.GameToUnity (pos + Vector2Int.right * (blockSize)), Game.GameToUnity (pos + Vector2Int.right * (blockSize) + Vector2Int.up * (blockSize)), Color.magenta, 1000f);
					Debug.DrawLine (Game.GameToUnity (pos + Vector2Int.right * (blockSize) + Vector2Int.up * (blockSize)), Game.GameToUnity (pos + Vector2Int.up * (blockSize)), Color.magenta, 1000f);
					Debug.DrawLine (Game.GameToUnity (pos + Vector2Int.up * (blockSize)), Game.GameToUnity (pos), Color.magenta, 1000f);
				}
			}
		}
		*/
	}

	//P1 SHOULD ALWAYS BE IN A POSITIVE DIRECTION RELATIVE TO P0!!!!
	bool roadBetween (Vector2Int p0, Vector2Int p1) {
		Vector2Int dir = p1 - p0;
		dir.x = (dir.x != 0) ? 1 : 0;
		dir.y = (dir.y != 0) ? 1 : 0;
		for (int i = 0; i < roads.Count; i++) {
			Vector2Int rdir = roads [i].end - roads [i].start;
			rdir.x = (rdir.x != 0) ? 1 : 0;
			rdir.y = (rdir.y != 0) ? 1 : 0;
			if (rdir != dir) {
				continue;
			}
			if (dir.x == 1) {
				if (p0.y != roads [i].start.y) {
					continue;
				}
				if (p0.x >= roads [i].start.x && p1.x <= roads [i].end.x) {
					return true;
				}
			} else if (dir.y == 1) {
				if (p0.x != roads [i].start.x) {
					continue;
				}
				if (p0.y >= roads [i].start.y && p1.y <= roads [i].end.y) {
					return true;
				}
			}
		}
		return false;
	}

	public List<Road> RoadsAt (Vector2Int v) {
		List<Road> rl = new List<Road> ();
		for (int i = 0; i < roads.Count; i++) {
			if (roads [i].start.x == v.x && roads [i].end.x == v.x) {
				if (v.y >= roads [i].start.y && v.y <= roads [i].end.y)
					rl.Add (roads [i]);
			} else if (roads [i].start.y == v.y && roads [i].end.y == v.y) {
				if (v.x >= roads [i].start.x && v.x <= roads [i].end.x)
					rl.Add (roads [i]);
			}
		}
		return rl;
	}

	/// <summary>
	/// returns all posible directions and distances that a character can travel starting at <c>origin</c> while staying on a road
	/// </summary>
	/// <returns>
	/// a <c>List</c> of <c>Map.Movement</c>s which represent each valid direction and the maximum distance which a character can move in
	/// </returns>
	/// <param name="origin">the origin of the movement in question given in world coordinates</param>
	public List<Map.Movement> GetValidMovementsOnRoad (Vector2 origin) {
		Vector2 localPos = (origin - position) / (blockSize + roadWidth);
		Vector2Int roundPos = Vector2Int.RoundToInt (localPos);
		List<Road> possibleRoads = RoadsAt (roundPos);
		List<Map.Movement> moves = new List<Map.Movement> ();
		for (int i = 0; i < possibleRoads.Count; i++) {
			if ((possibleRoads [i].end - possibleRoads [i].start).x > 0) { //horizontal road
				Map.Movement m;
				m.direction = Map.Direction.left;
				m.distance = (localPos - (Vector2)possibleRoads [i].start).x;
				moves.Add (m);
				m.direction = Map.Direction.right;
				m.distance = ((Vector2)possibleRoads [i].end - localPos).x;
				moves.Add (m);
			} else if ((possibleRoads [i].end - possibleRoads [i].start).y > 0) { //vertical road
				Map.Movement m;
				m.direction = Map.Direction.down;
				m.distance = (localPos - (Vector2)possibleRoads [i].start).y;
				moves.Add (m);
				m.direction = Map.Direction.up;
				m.distance = ((Vector2)possibleRoads [i].end - localPos).y;
				moves.Add (m);
			}
		}
		return moves;
	}

	//sets various types of informations that are used throughout code to generate other things
	//such as a list of the IDs of nearby caves
	public void SetInfo () {
		//set nearby cave IDs
		/*
		for (int i = 0; i < Game.current.map.caveIDs.Count; i++) {
			Vector2 cavePos = ((CaveMap)Game.current.map.GetMapByID (Game.current.map.caveIDs [i])).worldMapEntrance;
			if (Vector2.Distance (cavePos, position + geometricCenter) <= nearbyCaveDistance + Mathf.Max (sizeInTiles.x, sizeInTiles.y) / 2) {
				//at this point, the cave is within range of the town to be considered "nearby"
				nearbyCaveIDs.Add (Game.current.map.caveIDs [i]);
			}
		}
		*/
		nearbyCaveIDs.Add (Game.current.map.caveIDs [0]);
	}

	public static string GenerateName () {
		string path = "Data/Random/townNames";
		TextAsset file = Resources.Load<TextAsset> (path);
		string data = file.text;
		//create a list of town names
		List<string> tnames = new List<string> ();
		int l = 0;
		for (int i = 0; true; l++) {
			if (i + l > data.Length)
				break;
			if (data [i + l] == '\n') {
				tnames.Add (data.Substring (i, l));
				i += l + 1;
				l = 0;
			}
		}
		//pick a town name at random
		string tn = tnames [Random.Range (0, tnames.Count)];
		return tn.ToLower ();
	}
}