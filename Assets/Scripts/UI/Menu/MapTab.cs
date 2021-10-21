using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapTab : MenuTab {
	public Image mapImage;
	public GameObject objectiveMarker;
	public GameObject cursor;
	public Text nodeLabel;
	public float cursorSpeed;

	public Vector2 cursorPosition;
	Vector2Int imageCenter; //this is in world coordinates
	float zoom;
	int noZoomSize; //the size of the map image (which is a square) in tiles when zoom is equal to 1
	int imageSize {
		get {
			return (int)(noZoomSize / zoom);
		}
	}
	Vector2Int min {
		get {
			return new Vector2Int (imageCenter.x - imageSize / 2, imageCenter.y - imageSize / 2);
		}
	}
	Vector2Int max {
		get {
			return new Vector2Int (imageCenter.x + imageSize / 2, imageCenter.y + imageSize / 2);
		}
	}
	Vector2Int cursorWorldPosition {
		get {
			return min + new Vector2Int ((int)(imageSize * cursorPosition.x), (int)(imageSize * cursorPosition.y));
		}
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (GameInput.GetKey (GameInput.Bind.left)) {
			cursorPosition.x += -1 * cursorSpeed * Time.deltaTime;
			if (cursorPosition.x < 0f) {
				int nv = Mathf.Max (0, (int)(min.x - (0 - cursorPosition.x)));
				imageCenter.x += nv - min.x;
				cursorPosition.x = 0f;
				UpdateImage ();
			}
		}
		if (GameInput.GetKey (GameInput.Bind.right)) {
			cursorPosition.x += cursorSpeed * Time.deltaTime;
			if (cursorPosition.x > 1f) {
				int nv = Mathf.Min (1, (int)(max.x + (cursorPosition.x - 1)));
				imageCenter.x += nv - max.x;
				cursorPosition.x = 1f;
				UpdateImage ();
			}
		}
		if (GameInput.GetKey (GameInput.Bind.down)) {
			cursorPosition.y += -1 * cursorSpeed * Time.deltaTime;
			if (cursorPosition.y < 0f) {
				int nv = Mathf.Max (0, (int)(min.y - (0 - cursorPosition.y)));
				imageCenter.y += nv - min.y;
				cursorPosition.y = 0f;
				UpdateImage ();
			}
		}
		if (GameInput.GetKey (GameInput.Bind.up)) {
			cursorPosition.y += cursorSpeed * Time.deltaTime;
			if (cursorPosition.y > 1f) {
				int nv = Mathf.Min (1, (int)(max.y + (cursorPosition.y - 1)));
				imageCenter.y += nv - max.y;
				cursorPosition.y = 1f;
				UpdateImage ();
			}
		}
		if (GameInput.GetKeyDown (GameInput.Bind.back)) {
			Game.ui.menu.GoHome ();
		}
		cursor.GetComponent<RectTransform> ().localPosition = new Vector3 (mapImage.gameObject.GetComponent<RectTransform> ().rect.width * cursorPosition.x - mapImage.gameObject.GetComponent<RectTransform> ().rect.width / 2, mapImage.gameObject.GetComponent<RectTransform> ().rect.height * cursorPosition.y - mapImage.gameObject.GetComponent<RectTransform> ().rect.height / 2);
		CheckNodes ();
	}


	void CheckNodes () {
		//Debug.Log (cursorWorldPosition);
		//find the smallest node that the cursor is hovering over
		MapImage.Node currentNode = null;
		for (int i = 0; i < Game.current.map.currentMap.mapImage.nodes.Count; i++) {
			MapImage.Node n = Game.current.map.currentMap.mapImage.nodes [i];
			if (cursorWorldPosition.x >= n.position.x && cursorWorldPosition.x <= n.position.x + n.size.x && cursorWorldPosition.y >= n.position.y && cursorWorldPosition.y <= n.position.y + n.size.y) {
				if (currentNode == null)
					currentNode = n;
				else if (n.size.x * n.size.y < currentNode.size.x * currentNode.size.y)
					currentNode = n;
			}
		}
		//display the node (if there is one)
		if (currentNode != null) {
			nodeLabel.text = currentNode.name;
			nodeLabel.gameObject.SetActive (true);
		} else {
			nodeLabel.gameObject.SetActive (false);
		}
	}

	void UpdateImage () {
		//calculate dimension of the map image in tiles
		Sprite s = Sprite.Create (Game.current.map.currentMap.mapImage.image, new Rect (min.x, min.y, imageSize, imageSize), new Vector2 (.5f, .5f));
		mapImage.sprite = s;
	}

	public override void UpdateTab () {
		zoom = 1f;
		noZoomSize = (int)Mathf.Min (Game.current.map.currentMap.mapSize.x, Game.current.map.currentMap.mapSize.y);
		imageCenter = new Vector2Int ((int)Game.current.map.currentMap.mapSize.x / 2, (int)Game.current.map.currentMap.mapSize.y / 2);
		cursorPosition = new Vector2 (.5f, .5f);
		UpdateImage ();
		/*
		//place a marker at the player's current quest's location
		if (Game.current.playerCharacter.currentQuestID != 0) {
			objectiveMarker.SetActive (true);
			Vector2 ol = Game.current.playerCharacter.currentQuest.GetObjectiveLocation ();
			objectiveMarker.GetComponent<RectTransform> ().localPosition = new Vector2 (ol.x * 850 / Game.current.map.mapSize.x, ol.y * 850 / Game.current.map.mapSize.y);
		} else
			objectiveMarker.SetActive (false);
		*/
	}

	/*
	public Vector2 WorldToUI (Vector2 v) {
		
	}
	public Vector2 UIToWorld (Vector2 v) {
		
	}
	*/
}
