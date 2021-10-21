using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileBlockScript : MonoBehaviour {

	Tile.Block block;
	GameObject[,] tileObjects;
	SpriteRenderer[,] under;
	SpriteRenderer[,] mod;
	SpriteRenderer[,] over ;

	public void Load (Tile.Block b) {
		block = b;
		Init ();

		gameObject.transform.position = new Vector2 (-block.width - 50, -block.height - 50); //set the position of the block's transform

		for (int y = 0; y < block.height; y++) {
			for (int x = 0; x < block.width; x++) {
				//tileObjects [x, y].GetComponent<SpriteRenderer> ().sprite = Tile.GetSprite (data [x, y]);
				//tileObjects [x, y].GetComponent<BoxCollider2D> ().enabled = !Tile.CanTraverse (block.data [x, y]);
				under [x, y].sprite = Tile.GetSprite (block.data [x, y].underType);
				//mod [x, y].sprite = Tile.GetSprite (block.data [x, y].modType);
				over [x, y].sprite = Tile.GetSprite (block.data [x, y].overType);
			}
		}
	}

	public void Init() {
		tileObjects = new GameObject[block.width, block.height];
		under = new SpriteRenderer[block.width, block.height];
		mod = new SpriteRenderer[block.width, block.height];
		over = new SpriteRenderer[block.width, block.height];
		for (int y = 0; y < block.height; y++) {
			for (int x = 0; x < block.width; x++) {
				//tileObjects [x, y] = Game.CreateObject<Tile> (gameObject).gameObject;
				//tileObjects [x, y].AddComponent (typeof(SpriteRenderer));
				tileObjects [x, y] = new GameObject ();
				tileObjects [x, y].transform.SetParent (gameObject.transform);
				tileObjects [x, y].transform.localPosition = new Vector2 (x * Tile.tileSize, y * Tile.tileSize) / 100;
				//tileObjects [x, y].AddComponent (typeof(BoxCollider2D));
				//tileObjects [x, y].GetComponent<BoxCollider2D> ().size = new Vector2 ((float)Tile.tileSize / 100, (float)Tile.tileSize / 100);
				under [x, y] = Game.CreateObject<SpriteRenderer> (tileObjects [x, y]);
				under [x, y].gameObject.transform.localPosition = new Vector3 (0, 0, 2);
				mod [x, y] = Game.CreateObject<SpriteRenderer> (tileObjects [x, y]);
				mod [x, y].gameObject.transform.localPosition = new Vector3 (0, 0, 1);
				over [x, y] = Game.CreateObject<SpriteRenderer> (tileObjects [x, y]);
				over [x, y].gameObject.transform.localPosition = new Vector3 (0, 0, -1);
			}
		}
	}
}