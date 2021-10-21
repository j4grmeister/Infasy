using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsTD {
	static float tileRadius = .4f;

	public static Vector2 MoveObject (Vector2 position, Vector2 movement) {
		//Vector2 pos = position;
		//check 3 different points on a circle
		//effectively performing a circle cast
		/*
		Vector2 A = position + new Vector2 (-(movement.normalized * tileRadius).y, (movement.normalized * tileRadius).x);
		Vector2 B = position + movement.normalized * tileRadius;
		Vector2 C = position + new Vector2 ((movement.normalized * tileRadius).y, -(movement.normalized * tileRadius).x);
		A += new Vector2 (.5f, .5f);
		B += new Vector2 (.5f, .5f);
		C += new Vector2 (.5f, .5f);
		for (float d = 0; d < movement.magnitude; d += .1f) {
			Vector2 nA = A + movement.normalized * d;
			Vector2 nB = B + movement.normalized * d;
			Vector2 nC = C + movement.normalized * d;
			bool cA = !IsValidPosition (position, nA);
			bool cB = !IsValidPosition (position, nB);
			bool cC = !IsValidPosition (position, nC);
			if (cA || cB || cC) {
				return position + movement.normalized * (d - .1f);
			}
		}
		return position + movement;
		*/
		//bind all coordinates to the nearest hundredth in order to simplify mathmatics
		Vector2 pos = new Vector2 (Mathf.Round (position.x * 100f) / 100f, Mathf.Round (position.y * 100f) / 100f);
		Vector2 mov = new Vector2 (Mathf.Round (movement.x * 100f) / 100f, Mathf.Round (movement.y * 100f) / 100f);
		Vector2 opposite = new Vector2 (mov.y, mov.x).normalized;
		Vector2 A = pos + new Vector2 (.5f, .5f) + new Vector2 (mov.normalized.x * tileRadius + tileRadius * opposite.x, mov.normalized.y * tileRadius + tileRadius * opposite.y);
		Vector2 B = pos + new Vector2 (.5f, .5f) + new Vector2 (mov.normalized.x * tileRadius - tileRadius * opposite.x, mov.normalized.y * tileRadius - tileRadius * opposite.y);
		for (float d = .01f; d <= mov.magnitude; d += .01f) {
			Vector2 nA = A + mov.normalized * d;
			Vector2 nB = B + mov.normalized * d;
			bool cA = !IsValidPosition (pos, nA);
			bool cB = !IsValidPosition (pos, nB);

			if (cA || cB) {
				//Debug.Log (mov.normalized * (d - .01f));
				return pos + mov.normalized * (d - .01f);
			}
		}
		return pos + mov;
	}

	public static bool IsValidPosition (Vector2 origin, Vector2 pos) {
		if (!Tile.CanTraverse (Game.current.map.currentMap.GetTile ((int)pos.x, (int)pos.y))) {
			return false;
		}
		//check character positions
		for (int i = 0; i < Game.current.characters.Count; i++) {
			//Vector2 cpos = Game.current.characters [i].position;
			//skip the character if it is the object we are moving (it is initially located on the same tile)
			//also skip this character if it is dead
			if (origin == Game.current.characters [i].position || Game.current.characters [i].IsDead ()) {
				continue;
			}
			//return false if the tile has a character on it
			//if ((int)pos.x == (int)cpos.x && (int)pos.y == (int)cpos.y) {
			//bool xc = (pos.x + 1 > cpos.x && pos.x + 1 < cpos.x + 1) || (pos.x > cpos.x && pos.x < cpos.x + 1);
			//bool yc = (pos.y + 1 > cpos.y && pos.y + 1 < cpos.y + 1) || (pos.y > cpos.y && pos.y < cpos.y + 1);
			/*
			bool xc = (pos.x > cpos.x && pos.x < cpos.x + 1);
			bool yc = (pos.y > cpos.y && pos.y < cpos.y + 1);
			if (xc && yc) {
				return false;
			}
			*/
			if (CharacterOccupies (Game.current.characters [i], pos))
				return false;
		}
		return true;
	}

	public static bool IsOccupiedByCharacter (Vector2 pos) {
		for (int i = 0; i < Game.current.characters.Count; i++) {
			//skip this character if it is dead since dead characters cannot create collisions
			if (Game.current.characters [i].IsDead ())
				continue;
			if (CharacterOccupies (Game.current.characters [i], pos))
				return true;
		}
		return false;
	}

	public static bool CharacterOccupies (Character c, Vector2 pos) {
		Vector2 cpos = c.position;
		bool xc = (pos.x > cpos.x && pos.x < cpos.x + 1);
		bool yc = (pos.y > cpos.y && pos.y < cpos.y + 1);
		return (xc && yc);
	}
		
	//returns a list of movements that can be taken in order to move from the start point to the end point
	public static List<Map.Movement> GetPathBetween (Vector2 start, Vector2 end) {
		//use a sort of flood fill algorithm to find a path, which also keeps track of the point which adds each point to the queue
		//IMPORTANT NOTE: THIS WILL BECOME INCREASINGLY MORE DEMANDING WITH GREATER DISTANCES BETWEEN THE TWO POINTS AND ALSO WITH MORE OBSTACLES BETWEEN THEM
		//Also, this probably won't work with obstancles which are super large. I imagine this will only prove to be an issue inside caves.
		Vector2Int dif = new Vector2Int ((int)end.x - (int)start.x, (int)end.y - (int)start.y);
		//Vector2Int size = new Vector2Int (Mathf.Abs (dif.x * 2), Mathf.Abs (dif.y * 2));
		Vector2Int size = new Vector2Int (50, 50);
		//Debug.Log ("size = " + size);
		Vector2Int floodMapCorner = new Vector2Int (((int)start.x + (int)end.x) / 2 - size.x / 2, ((int)start.y + (int)end.y) / 2 - size.y / 2); //world coordinate of the lower left corner of the floodmap
		//Debug.Log ("end = " + end);
		//Debug.Log ("corner = " + floodMapCorner);
		Vector2Int[,] floodMap = new Vector2Int[size.x, size.y]; //each element contains the coordinates of its "parent" element
		//initialize the floodMap
		for (int x = 0; x < size.x; x++) {
			for (int y = 0; y < size.y; y++) {
				floodMap [x, y] = new Vector2Int (-1, -1);
			}
		}
		//Debug.Log ("corner = " + floodMapCorner);
		//Debug.Log ("start = " + (start - floodMapCorner) + " - " + start);
		//Debug.Log ("end = " + (end - floodMapCorner) + " - " + end);
		//Debug.Log ("size = (" + Mathf.Abs (dif.x * 2) + ", " + Mathf.Abs (dif.y * 2) + ")"); 
		List<Vector2Int> queue = new List<Vector2Int> ();
		queue.Add (Vector2Int.FloorToInt (start) - floodMapCorner);
		while (queue.Count > 0) {
			for (int xo = -1; xo <= 1; xo++) {
				for (int yo = -1; yo <= 1; yo++) {
					if (Mathf.Abs (xo) == Mathf.Abs (yo))
						continue;
					if (queue [0].x + xo < 0 || queue [0].x + xo >= size.x || queue [0].y + yo < 0 || queue [0].y + yo >= size.y)
						continue;
					if (floodMap [queue [0].x + xo, queue [0].y + yo] == new Vector2Int (-1, -1)) {
						Vector2Int worldPos = queue [0] + floodMapCorner;
						if (Tile.CanTraverse (Game.current.map.currentMap.GetTile (worldPos.x, worldPos.y))) {
							//Debug.Log ("yes");
							Vector2Int v = new Vector2Int (queue [0].x + xo, queue [0].y + yo);
							floodMap [v.x, v.y] = queue [0];
							//Debug.Log ("flood " + queue [0]);
							queue.Add (v);
						} else {
							//Debug.Log ("no");
						}
					}
				}
			}
			queue.RemoveAt (0);
			//Debug.Log ("index = " + new Vector2Int ((int)end.x - floodMapCorner.x, (int)end.y - floodMapCorner.y));
			if (!floodMap [(int)end.x - floodMapCorner.x, (int)end.y - floodMapCorner.y].Equals (new Vector2Int (-1, -1))) {
				break;
			}
		}
		//Debug.Log ("finished flood");


		List<Vector2Int> tilePath = new List<Vector2Int> ();
		tilePath.Add (Vector2Int.FloorToInt (end) - floodMapCorner);
		//Debug.Log (Vector2Int.FloorToInt (end) - floodMapCorner);
		//int i = 0;
		while (tilePath [tilePath.Count - 1] != new Vector2Int (-1, -1)) {
			//if (!(i < 10))
			//	break;
			if (tilePath [tilePath.Count - 1] == Vector2Int.FloorToInt (start) - floodMapCorner)
				break;
			Vector2Int v = tilePath [tilePath.Count - 1];
			tilePath.Add (floodMap [v.x, v.y]);


			//Debug.Log (floodMap [v.x, v.y]);
			//i++;
		}

		//Debug.Log ("finished compile");


		List<Map.Direction> dirs = new List<Map.Direction> ();
		for (int i = tilePath.Count - 1; i > 0; i--) {
			Vector2 v = tilePath [i - 1] - tilePath [i];
			dirs.Add (Map.DirectionFromVector2 (v.normalized));
		}

		/*
		for (int i = 0; i < dirs.Count; i++) {
			Debug.Log (dirs [i].ToString ());
		}
		*/

		//clean up some other time
		Map.Direction currentDirection = Map.Direction.down;
		float currentDistance = 0f;
		List<Map.Movement> moves = new List<Map.Movement> ();
		if (dirs.Count > 0) {
			currentDirection = dirs [0];
			currentDistance += 1f;
		}
		for (int i = 1; i < dirs.Count; i++) {
			if (currentDirection == dirs [i]) {
				currentDistance += 1f;
			} else {
				Map.Movement m;
				m.direction = currentDirection;
				m.distance = currentDistance;
				moves.Add (m);
				currentDirection = dirs [i];
			}
		}
		Map.Movement mm;
		mm.direction = currentDirection;
		mm.distance = currentDistance;
		moves.Add (mm);

		Vector2 so = start - new Vector2 (Mathf.FloorToInt (start.x), Mathf.FloorToInt (start.y));
		mm.direction = Map.Direction.left;
		mm.distance = so.x;
		moves.Insert (0, mm);
		mm.direction = Map.Direction.down;
		mm.distance = so.y;
		moves.Insert (0, mm);
		Vector2 eo = end - new Vector2 (Mathf.FloorToInt (end.x), Mathf.FloorToInt (end.y));
		mm.direction = Map.Direction.right;
		mm.distance = eo.x;
		moves.Add (mm);
		mm.direction = Map.Direction.up;
		mm.distance = eo.y;
		moves.Add (mm);

		/*
		for (int i = 0; i < moves.Count; i++) {
			Debug.Log (moves [i].direction + " : " + moves [i].distance);
		}
		*/

		return moves;
	}
}