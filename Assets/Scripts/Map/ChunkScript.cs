using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ChunkScript : MonoBehaviour {
	public Vector2 position;

	//Tile[,] data = new Tile [50, 50];
	public TileScript[,] tileObjects = new TileScript [50, 50];
	/*
	GameObject[,] tileObjects = new GameObject [50, 50];
	SpriteRenderer[,] under = new SpriteRenderer [50, 50];
	List<SpriteRenderer>[,] mod = new List<SpriteRenderer> [50, 50];
	SpriteRenderer[,] over = new SpriteRenderer[50, 50];
	*/
	//GameObject emptyMapTile;

	/*
	public void Init(GameObject t) {
		//emptyMapTile = t;
		for (int y = 0; y < 50; y++) {
			for (int x = 0; x < 50; x++) {
				tileObjects[x, y] = GameObject.Instantiate (emptyMapTile);
				tileObjects[x, y].name = "(" + x + "," + y + ")";
				tileObjects[x, y].transform.SetParent (gameObject.transform);
				tileObjects[x, y].transform.position = new Vector2 (x * tileSize, y * tileSize) / 100;
			}
		}
	}
	*/

	public void Init() {
		for (int y = 0; y < 50; y++) {
			for (int x = 0; x < 50; x++) {
				tileObjects [x, y] = Game.CreateObject<TileScript> (gameObject);
				//tileObjects [x, y].gameObject.transform.localPosition = new Vector2 (x * Tile.tileSize, y * Tile.tileSize) / 100;
				tileObjects [x, y].gameObject.transform.localPosition = Game.GameToUnity (new Vector2 (x, y));
				tileObjects [x, y].Init ();
			}
		}

		/*
		for (int y = 0; y < 50; y++) {
			for (int x = 0; x < 50; x++) {
				//tileObjects [x, y] = Game.CreateObject<Tile> (gameObject).gameObject;
				//tileObjects [x, y].AddComponent (typeof(SpriteRenderer));
				tileObjects [x, y] = new GameObject ();
				tileObjects [x, y].transform.SetParent (gameObject.transform);
				tileObjects [x, y].transform.localPosition = new Vector2 (x * Tile.tileSize, y * Tile.tileSize) / 100;
				//tileObjects [x, y].AddComponent (typeof(BoxCollider2D));
				//tileObjects [x, y].GetComponent<BoxCollider2D> ().size = new Vector2 ((float)Tile.tileSize / 100, (float)Tile.tileSize / 100);
				under [x, y] = Game.CreateObject<SpriteRenderer> (tileObjects [x, y]);
				under [x, y].gameObject.transform.localPosition = new Vector3 (0, 0, 2);
				over [x, y] = Game.CreateObject<SpriteRenderer> (tileObjects [x, y]);
				over [x, y].gameObject.transform.localPosition = new Vector3 (0, 0, -1);
			}
		}
		*/
	}
		
	//draws the chunk
	public void Update() {

	}

	public void Set(int x, int y) {
		position = new Vector2 (x, y);
		//Debug.Log (position);
		if (gameObject != null) {
			//gameObject.transform.position = Game.GameToUnity (position);
			//gameObject.transform.position = new Vector2 (position.x * Tile.tileSize * 50, position.y * Tile.tileSize * 50) / 100;
			gameObject.transform.localPosition = Game.GameToUnity (position * 50);
			Load ();
		}
	}

	public void SetPosition(int x, int y) {
		position = new Vector2 (x, y);
	}

	public void SetPositionAndTransform (int x, int y) {
		position = new Vector2 (x, y);
		if (gameObject != null) {
			gameObject.transform.position = new Vector2 (position.x * Tile.tileSize * 50, position.y * Tile.tileSize * 50) / 100;
		}
	}

	public Vector2 GetPosition() {
		return position;
	}

	/*
	public Tile GetTile(int x, int y) {
		return data [x, y];
	}
	*/

	/*
	public Tile.Type[,] GetTiles() {
		return data;
	}
	*/

	/*
	public void SetTile(int x, int y, Tile t) {
		data [x, y] = t;
	}
	*/

	public void Load() {
		//old (super shitty) algorithm 
		/*
		for (int y = 0; y < 50; y++) {
			for (int x = 0; x < 50; x++) {
				
				float xp = (float)(position.x * 50 + x) + 0.1f;
				float yp = (float)(position.y * 50 + y) + 0.1f;
				float v = Mathf.PerlinNoise(xp, yp);
				//Debug.Log (x + " , " + y);
				//Debug.Log (v);
				Tile.Type t = Tile.Type.grass;
				if (v < .39)
					t = Tile.Type.water;
				data [x, y] = t;
				tileObjects[x, y].GetComponent<SpriteRenderer>().color = new Color(0, 255 / 2 * (int)t, 0); //for dev purposes only

				//tileObjects[xo, yo].GetComponent<SpriteRenderer>().sprite = Tile.GetSprite(t);
			}
		}
		*/
		//Debug.Log (position);
		for (int y = 0; y < 50; y++) {
			for (int x = 0; x < 50; x++) {
				tileObjects [x, y].Load ((int)position.x * 50 + x, (int)position.y * 50 + y);
			}
		}

		/*
		for (int y = 0; y < 50; y++) {
			for (int x = 0; x < 50; x++) {
				//tileObjects [x, y].GetComponent<SpriteRenderer> ().sprite = Tile.GetSprite (data [x, y]);
				//tileObjects [x, y].GetComponent<BoxCollider2D> ().enabled = !Tile.CanTraverse (data [x, y]);
				under [x, y].sprite = Tile.GetSprite (data [x, y].underType);
				over [x, y].sprite = Tile.GetSprite (data [x, y].overType);

				//add mods to tile
				int q = data [x, y].mods.Count;
			}
		}
		*/
	}
}