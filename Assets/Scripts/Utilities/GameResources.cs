using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//THIS CLASS MUST HAVE ONE, AND EXACTLY ONE, INSTANCE ATTACHED TO A GAMEOBJECT!
public class GameResources : MonoBehaviour {
	[System.Serializable]
	public struct CharacterSprite {
		public Character.Race race;
		public Sprite sprite;
	}
	[System.Serializable]
	public struct TileUnderSprite {
		public Tile.UnderType type;
		public Sprite sprite;
		public Color color;
	}
	[System.Serializable]
	public struct TileModSprite {
		public Tile.ModType type;
		public Sprite sprite;
	}
	[System.Serializable]
	public struct TileOverSprite {
		public Tile.OverType type;
		public Sprite sprite;
	}

	//static pointer the the instance of this object
	public static GameResources instance;
	public static List<Font> fonts = new List<Font> ();
	public static Font defaultFont {
		get {
			return fonts [0];
		}
	}

	public static Sprite GetCharacterSprite (Character.Race race) {
		for (int i = 0; i < instance.characterSprites.Count; i++) {
			if (instance.characterSprites [i].race == race) {
				return instance.characterSprites [i].sprite;
			}
		}
		return null;
	}

	public static Sprite GetTileSprite (Tile.UnderType t) {
		for (int i = 0; i < instance.tileUnderSprites.Count; i++) {
			if (instance.tileUnderSprites [i].type == t)
				return instance.tileUnderSprites [i].sprite;
		}
		return null;
	}
	public static Sprite GetTileSprite (Tile.ModType t) {
		for (int i = 0; i < instance.tileModSprites.Count; i++) {
			if (instance.tileModSprites [i].type == t)
				return instance.tileModSprites [i].sprite;
		}
		return null;
	}
	public static Sprite GetTileSprite (Tile.OverType t) {
		for (int i = 0; i < instance.tileOverSprites.Count; i++) {
			if (instance.tileOverSprites [i].type == t)
				return instance.tileOverSprites [i].sprite;
		}
		return null;
	}
	public static Color GetTileColor (Tile.UnderType t) {
		for (int i = 0; i < instance.tileUnderSprites.Count; i++) {
			if (instance.tileUnderSprites [i].type == t)
				return instance.tileUnderSprites [i].color;
		}
		return Color.black;
	}


	public List<CharacterSprite> characterSprites;
	public List<TileUnderSprite> tileUnderSprites;
	public List<TileModSprite> tileModSprites;
	public List<TileOverSprite> tileOverSprites;

	void Awake () {
		instance = this;
		fonts.Add (Resources.Load<Font> ("Fonts/Ringbearer"));
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
