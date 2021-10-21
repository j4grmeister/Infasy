using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapScript : MonoBehaviour {

	/*
	static int mapSize = 5; //width of the map in chunks
	static int nOfVPoints = 100;
	static float[] townDensity = {0, 0, 0, 0, 40, 75, 95}; //index = # of towns - 1; value = threshold probability (0-100, the lowest value that will produce this many towns)
	static int minTownDistance = 100;
	static float[] townSize = { }; //index = size of town (in tiles) / 10
	*/
	static int loadMapSize = 3;

	//public GameObject emptyMapTile;
	//public GameObject emptyChunk;

	/*
	public Chunk Chunk_0_0;
	public Chunk Chunk_1_0;
	public Chunk Chunk_2_0;
	public Chunk Chunk_0_1;
	public Chunk Chunk_1_1;
	public Chunk Chunk_2_1;
	public Chunk Chunk_0_2;
	public Chunk Chunk_1_2;
	public Chunk Chunk_2_2;
	*/

	//public ChunkScript[,] chunks = new ChunkScript[Game.current.map.mapSize, Game.current.map.mapSize];


	//lower-leftmost tile/chunk is (0,0)
	ChunkScript[,] loadedMap = new ChunkScript[loadMapSize, loadMapSize];

	//kept loaded in the negative part of the filesystem (quadrant III)
	//used for entering spaces which are separate from the world map
	//these spaces have a maximum size of one chunk and are used for things such as entering small buildings
	//anything larger than one chunk must be implemented separately (such as caves, castles, etc.)
	//TileBlockScript alternateArea;

	void Awake() {
		//Game.current = new Game (); //for dev purposes only
	}

	// Use this for initialization
	void Start () {
		Game.mapScript = this;
		//Game.current = new Game (); //for dev purposes only
		//Game.current.playerCharacter = new Character (); //for dev purposes only
		//Debug.Log(Game.current);

		//Game.current.playerCharacter.characterObject = CharacterObject; //for dev purposes only
		//Game.current.map = this;


		//initialize array of entire map
		/*
		for (int y = 0; y < Game.current.map.mapSize; y++) {
			for (int x = 0; x < Game.current.map.mapSize; x++) {
				GameObject c = Game.CreateObject<ChunkScript> (gameObject).gameObject;
				c.SetActive (false);
				chunks [x, y] = c.GetComponent<ChunkScript> ();
				chunks [x, y].SetPosition (x, y);
			}
		}
		*/

		//generate the map
		//Generate ();


		//initialize loaded chunks
		for (int a = 0; a < loadMapSize; a++) {
			for (int b = 0; b < loadMapSize; b++) {
				loadedMap [a, b] = Game.CreateObject<ChunkScript> (gameObject);
				//loadedMap [a, b].Init (emptyMapTile);
				loadedMap [a, b].Init ();
				loadedMap [a, b].Set (a, b);
			}
		}

		//initialize the alternate chunk
		//alternateArea = Game.CreateObject <TileBlockScript> (gameObject);
		//alternateArea.Init ();
		//alternateArea.SetPositionAndTransform (-2, -2); //load chunk at (-2, -2), if it's at (-1, -1) the world map will be visible from the upper-right corner
	}

	Vector2 lastplc;
	ushort lastMapID;

	// Update is called once per frame
	void Update () {
		//only update chunks if the player is actually in the world map
		//otherwise, load the alternate area if it hasn't already been loaded ***THIS IS AN OLD COMMENT FROM OLD IMPLEMENTATION! DO NOT LET IT TRICK YOU, OR ELSE YOU WILL HAVE BEEN BAMBOOZLED!
		//if (Game.current.playerCharacter.location.Count == 0) {
		Vector2 pl = Game.current.playerCharacter.position;
		Vector2 plc = new Vector2 ((int)(pl.x / 50), (int)(pl.y / 50));
		ushort mapID = Game.current.map.mapID;
		//if (loadedMap [loadMapSize / 2, loadMapSize / 2].position != plc) {
		if (plc != lastplc || mapID != lastMapID) {
			Game.current.map.currentMap.DestroyTileObjectActions ();
			//Debug.Log ("update");
			for (int a = 0; a < loadMapSize; a++) {
				for (int b = 0; b < loadMapSize; b++) {
					int x = (int)plc.x - (int)(loadMapSize / 2) + a;
						int y = (int)plc.y - (int)(loadMapSize / 2) + b;
					if (x - a <= 0) {
						x = a;
					} else if (x + loadMapSize - a >= Game.current.map.currentMap.GetSizeInChunks ().x) {
						x = (int)Game.current.map.currentMap.GetSizeInChunks ().x - loadMapSize + a;
					}
					if (y - b <= 0) {
						y = b;
					} else if (y + loadMapSize - b >= Game.current.map.currentMap.GetSizeInChunks ().y) {
						y = (int)Game.current.map.currentMap.GetSizeInChunks ().y - loadMapSize + b;
					}
					loadedMap [a, b].Set (x, y);
				}
			}
		}
		lastplc = plc;
		lastMapID = mapID;
		/*} else if (lastPlayerLocationCount != Game.current.playerCharacter.location.Count) {
			//alternateArea.Load (Game.current.playerCharacter.location [Game.current.playerCharacter.location.Count - 1].block);
			lastPlayerLocationCount = Game.current.playerCharacter.location.Count;
		}*/


		/*
		for (int a = 0; a < 3; a++) {
			for (int b = 0; b < 3; b++) {
				loadedMap [a, b].Update ();
			}
		}
		*/
	}

	public TileScript GetTileObject (int x, int y) {
		Vector2Int localCoord = new Vector2Int (x - (int)lastplc.x + 1, y - (int)lastplc.y + 1);
		Vector2Int chunkc = new Vector2Int (localCoord.x / 50, localCoord.y / 50);
		Vector2Int tilec = new Vector2Int (localCoord.x % 50, localCoord.y % 50);
		return loadedMap [chunkc.x, chunkc.y].tileObjects [tilec.x, tilec.y]; 
	}
}