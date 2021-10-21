using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapImage {
	[System.Serializable]
	public class Node {
		public string name;
		public Vector2Int position; //lower left corner
		public Vector2Int size;
		public bool draw; //draw a visible node on the map?
		public bool visible; //visible to player; discovered?
	}

	public Texture2D image;
	public Texture2D mask; //a mask representing the unexplored areas of the map which the player cannot see from the map menu
	public List<Node> nodes = new List<Node> ();
}