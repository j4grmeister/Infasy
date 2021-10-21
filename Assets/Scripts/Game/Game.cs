using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

[System.Serializable]
public class Game {
	public static MapScript mapScript;
	public static CodeCanvas codeCanvas;
	public static ActionMenu actionMenu;
	public static CameraScript camera;
	public static UIScript ui;
	public static DialogueMenu dialogueMenu;
	public static ShopMenu shopMenu;
	public static LootMenu lootMenu;

	public static List<GameSave> savedGames = new List<GameSave>();
	public static Game current;

	public static GameSave currentGameSave {
		get {
			return savedGames [GetSaveIndex (current.playerCharacter.name)];
		}
	}

	public static int GetSaveIndex (string name) {
		LoadGameSaves ();
		for (int i = 0; i < savedGames.Count; i++) {
			if (savedGames [i].name == name)
				return i;
		}
		return -1;
	}

	public static void SaveGameSaves () {
		string fp = Application.persistentDataPath + "/savedGames.sgl";
		BinaryFormatter bf = new BinaryFormatter ();
		if (File.Exists (fp)) {
			File.Delete (fp);
		}
		FileStream file = File.Create (fp);
		bf.Serialize (file, savedGames);
		file.Close ();
	}
	public static void LoadGameSaves () {
		string fp = Application.persistentDataPath + "/savedGames.sgl";
		if(File.Exists(fp)) {
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open(fp, FileMode.Open);
			savedGames = (List<GameSave>)bf.Deserialize(file);
			file.Close();
		}
	}

	public static void UpdateGameInfo () {
		current.info.timePlayed = current.info.timePlayed.Add (GameTime.playTime);
		GameTime.ResetPlayTime ();
	}

	//saves the current game as a new save
	public static void Save () {
		int index = GetSaveIndex (current.playerCharacter.name);
		if (index == -1) {
			savedGames.Add (new GameSave (current.playerCharacter.name));
			index = savedGames.Count - 1;
		}
		UpdateGameInfo ();
		savedGames [index].Save (current);
	}
	//saves the current game, overwriting the save at the given index
	public static void Save (int i) {
		int index = GetSaveIndex (current.playerCharacter.name);
		UpdateGameInfo ();
		savedGames [index].Save (i, current);
	}
	//load the latest save of the game with the given name
	public static void Load (string name) {
		current = savedGames [GetSaveIndex (name)].Load ();
	}
	public static void Load (int i) {
		current = savedGames [GetSaveIndex (current.playerCharacter.name)].Load (i);
	}

	public static void Start () {
		Skill.Init ();
		GameInput.Init ();
		GameTime.Init ();
	}

	//performs anything that must be done on a per frame basis outside of a monobehaviour
	public static void Update () {
		GameTime.Update ();
		ActionSchedule.Update ();
		current.GameUpdate ();
	}

	//for dev purposes only
	public static void CreateDeveloperGame () {
		current = new Game ();
		current.map = new WorldMap ();
		current.map.Generate ();

		//Character player = Character.CreateCharacter (Character.Race.human_male, Character.Alignment.good);
		//player.gameObject.AddComponent<PlayerController> ();
		current.playerCharacter = Character.CreateCharacter (Character.Race.man, Character.Alignment.good, typeof(PlayerController));
		//current.playerCharacter.position = new Vector2 (105, 140);
		current.playerCharacter.position = new Vector2 (114, 164); //places the player on a cave entrance to load a cave immediately
		current.playerCharacter.name = "magnus burnsides";
		//Weapon sword = new Weapon (Weapon.Type.sword, 10, 5);
		current.playerCharacter.inventory.gold = 1000;
		current.playerCharacter.LearnSkill ("Fire Bolt");
		current.playerCharacter.inventory.Add (Weapon.CreateNewWeapon (typeof(Sword.RustySword)));
		current.playerCharacter.inventory.Equip (current.playerCharacter.inventory.weapons [0]);


		Character skeleton = Character.CreateCharacter (Character.Race.skeleton, Character.Alignment.evil, typeof(SkeletonAI));
		skeleton.name = "skeleton";
		skeleton.position = new Vector2 (114, 176);
		skeleton.inventory.Add (Weapon.CreateNewWeapon (typeof(Sword.RustySword)));
		skeleton.inventory.Equip (skeleton.inventory.weapons [0]);
		current.AddCharacter (skeleton);
	}



	public GameSave.GameInfo info;
	public int seed = 0;
	//public GameScript gameScript;
	public WorldMap map;
	public ushort playerCharacterID = 0;
	public List<Character> characters = new List<Character> ();
	public Character playerCharacter {
		get {
			return GetCharacterByID (playerCharacterID);
		}
		set {
			if (!characters.Contains (value)) {
				characters.Add (value);
				if (nextID == value.id)
					NextID ();
				else if (nextID < value.id)
					usedIDs.Add (value.id);
			}
			playerCharacterID = value.id;
		}
	}
	public List<Battle> battles = new List<Battle> (); //list of all currently ongoing battles
	//all the possible ids combined take up a total of about 128 KB
	public ushort nextID = 0;
	public List<ushort> usedIDs = new List<ushort> ();
	public List<ushort> retiredIDs = new List<ushort> ();

	//time of day in game time
	//military time with decimal
	//example: 18.5 would mean 6:30 PM
	public int day = 0;
	public float timeOfDay = 7f;
	public float inGameSecondsPerSecond = 36f;


	/*
	public CodeCanvas codeCanvas;
	public ActionMenu actionMenu;
	public CameraScript camera;
	public UIScript ui;
	*/

	public Game () {
		Init ();
		//seed = (int)Random.Range (0, 2147483648); //all possible int values
		seed = 49235403; //for dev purposes only
		//Debug.Log(Game.current);
		//map = new Map (); //for dev purposes only
		//playerCharacter.position = new Vector2(0, 0); //for dev purposes only
	}
	public Game (int s) {
		Init ();
		seed = s;
		map = new WorldMap ();
	}

	void Init () {
		//Character player = Character.CreateCharacter (Character.Race.human_male, Character.Alignment.good);
		//player.gameObject.AddComponent<PlayerController> ();
		//characters.Add (player);
		//playerCharacterIndex = 0;
		//add unused ids
		//for (int i = 1; i <= 65535; i++) {
		/*
		for (ushort i = 1; i <= 255; i++) {
			unusedIDs.Add (i);
		}
		*/
	}

	public void GameUpdate () {
		map.Update ();
		if (map.mapID != map.id) {
			map.currentMap.Update ();
		}

		//update the time of day
		timeOfDay += inGameSecondsPerSecond * GameTime.deltaTime / 3600f;
		if (timeOfDay >= 24f) {
			timeOfDay -= 24f;
			day++;
			NewDay ();
		}
	}

	//executed at midnight of each in-game day
	public void NewDay () {
		for (int i = 0; i < characters.Count; i++) {
			characters [i].controller.DailyUpdate ();
		}
	}

	public void NextID () {
		nextID++;
		while (usedIDs.Contains (nextID)) {
			usedIDs.Remove (nextID);
			nextID++;
		}
	}

	//returns an unused id
	public ushort RequestID () {
		//ushort id = unusedIDs [0];
		//unusedIDs.RemoveAt (0);
		ushort id = nextID;
		NextID ();
		return id;
	}

	//adds an id to the list of unused ids
	public void RetireID (ushort id) {
		//unusedIDs.Add (id);
		retiredIDs.Add (id);
	}

	public List<ushort> GetCharactersOnMap (ushort mapID) {
		List<ushort> c = new List<ushort> ();
		for (int i = 0; i < characters.Count; i++) {
			if (characters [i].mapID == mapID) {
				c.Add (characters [i].id);
			}
		}
		return c;
	}

	public void AddCharacter (Character c) {
		//assign the character a unique ID before adding it to the character list if it hasn't been assigned an ID yet
		if (c.id == 0) {
			c.id = RequestID ();
			//c.controller.DailyUpdate ();
		} else {
			if (nextID == c.id) {
				NextID ();
				//c.controller.DailyUpdate ();
			} else if (nextID < c.id && !usedIDs.Contains (c.id)) {
				usedIDs.Add (c.id);
				//c.controller.DailyUpdate ();
			}
		}
		characters.Add (c);
		//enable/disable the character depending on whether they are located in the currently active map
		c.gameObject.SetActive (c.mapID == map.mapID);
	}

	//FIX LATER! I'M TOO DAMN LAZY RIGHT NOW
	public void RemoveCharacter (Character c) {
		//add the character's ID to the list of unused IDs before removing it
		retiredIDs.Add (c.id);
		characters.Remove (c);
	}

	public void EnableCharactersOnMap (ushort mapID) {
		for (int i = 0; i < characters.Count; i++) {
			if (characters [i].mapID == mapID) {
				characters [i].gameObject.SetActive (true);
			}
		}
	}

	public void DisableCharactersOnMap (ushort mapID) {
		for (int i = 0; i < characters.Count; i++) {
			if (characters [i].mapID == mapID) {
				characters [i].gameObject.SetActive (false);
			}
		}
	}

	public void DestroyCharacter (ushort id) {
		for (int i = 0; i < characters.Count; i++) {
			if (characters [i].id == id) {
				RetireID (id);
				GameObject.Destroy (characters [i].gameObject);
				characters.RemoveAt (i);
				break;
			}
		}
	}

	public Character GetCharacterByID (ushort id) {
		for (int i = 0; i < characters.Count; i++) {
			if (characters [i].id == id)
				return characters [i];
		}
		return null;
	}

	/*
	public Character GetCharacterOnTile (int x, int y) {
		for (int i = 0; i < characters.Count; i++) {
			if ((int)characters [i].position.x == x && (int)characters [i].position.y == y) {
				return characters [i];
			}
		}
		return null;
	}
	*/

	public Character GetCharacterAt (Vector2 pos) {
		for (int i = 0; i < characters.Count; i++) {
			//skip this character if it is dead
			if (characters [i].IsDead ())
				continue;
			if (PhysicsTD.CharacterOccupies (characters [i], pos))
				return characters [i];
		}
		return null;
	}

	public List<Character> GetDeadCharactersAt (Vector2 pos) {
		List<Character> cl = new List<Character> ();
		for (int i = 0; i < characters.Count; i++) {
			//skip this character if it is alive
			if (!characters [i].IsDead ())
				continue;
			if (PhysicsTD.CharacterOccupies (characters [i], pos))
				cl.Add (characters [i]);
		}
		return cl;
	}

	//engages both of the two characters with each other
	public void Engage (Character c0, Character c1) {
		c0.Engage (c1);
		c1.Engage (c0);
		//create a battle if one doens't already exist
		//if a battle doesn exist, simply add one character to the battle
		int i0 = BattleWithCharacter (c0);
		int i1 = BattleWithCharacter (c1);
		if (i0 != -1) {
			if (i1 == -1) {
				battles [i0].Add (c1);
			}
		} else if (i1 != -1) {
			battles [i1].Add (c0);
		} else {
			Battle b = new Battle ();
			b.Add (c0);
			b.Add (c1);
			b.Start ();
			battles.Add (b);
		}
	}

	public void Disengage (Character c0, Character c1) {
		Debug.Log ("disengage");
		c0.Disengage (c1);
		c1.Disengage (c0);
	}

	//searches for and returns the index of the battle with the given character
	//returns -1 if the given character is not in a battle
	public int BattleWithCharacter (Character c) {
		for (int i = 0; i < battles.Count; i++) {
			if (battles [i].IsCombatant (c)) {
				return i;
			}
		}
		return -1;
	}
		
	public static T CreateObject<T> (GameObject parent) {
		GameObject obj = new GameObject ();
		//GameObject obj = GameObject.Instantiate();
		obj.transform.SetParent (parent.transform);
		obj.AddComponent(typeof(T));
		T c = obj.GetComponent<T> ();

		//add additional type specific components
		if (c.GetType () == typeof(Animator)) {
			obj.AddComponent (typeof(SpriteRenderer));
		}
			
		return c;
	}

	public static T CreateObject<T> () {
		GameObject obj = new GameObject ();
		//GameObject obj = GameObject.Instantiate();
		obj.AddComponent(typeof(T));
		T c = obj.GetComponent<T> ();

		//add additional type specific components
		if (c.GetType () == typeof(Animator)) {
			obj.AddComponent (typeof(SpriteRenderer));
		}

		return c;
	}

	public static void DestroyObjectList (List<GameObject> l) {
		for (int i = 0; i < l.Count;) {
			GameObject.Destroy (l [i]);
			l.RemoveAt (i);
		}
	}

	public static void DestroyComponentList (List<GameObject> l) {
		for (int i = 0; i < l.Count;) {
			GameObject.Destroy (l [i].gameObject);
			l.RemoveAt (i);
		}
	}

	public static Vector2 UnityToGame (Vector2 t) {
		return t * 100 / Tile.tileSize;
	}
	public static float UnityToGame (float t) {
		return t * 100 / Tile.tileSize;
	}
	public static Vector2 GameToUnity (Vector2 w) {
		return w * Tile.tileSize / 100;
	}
	public static float GameToUnity (float w) {
		return w * Tile.tileSize / 100;
	}

	public static int RollDice (int nofsides) {
		return Random.Range (1, nofsides + 1);
	}
}