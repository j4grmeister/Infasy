using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[System.Serializable]
public class GameSave {
	//contains information about game saves
	[System.Serializable]
	public struct GameInfo {
		public System.TimeSpan timePlayed;
	}

	public string name;

	public int Count {
		get {
			return filePaths.Count;
		}
	}
	//the filepaths of each save
	//index 0 = oldest
	List<string> filePaths = new List<string> ();
	List<GameInfo> gameInfo = new List<GameInfo> ();

	public GameSave (string str) {
		name = str;
	}

	//save the given game as a new save
	public void Save (Game game) {
		string fp = Application.persistentDataPath + "/Saves/" + game.playerCharacter.name + "/" + game.playerCharacter.name + " " + (filePaths.Count + 1).ToString () + ".inf";
		Directory.CreateDirectory (Application.persistentDataPath + "/Saves");
		Directory.CreateDirectory (Application.persistentDataPath + "/Saves/" + game.playerCharacter.name);
		BinaryFormatter bf = new BinaryFormatter ();
		SurrogateSelector ss = new SurrogateSelector ();
		ss.AddSurrogate (typeof(Vector2), new StreamingContext (StreamingContextStates.All), new Vector2SerializationSurrogate ());
		ss.AddSurrogate (typeof(Character), new StreamingContext (StreamingContextStates.All), new CharacterSerializationSurrogate ());
		bf.SurrogateSelector = ss;
		if (File.Exists (fp)) {
			File.Delete (fp);
		}
		FileStream file = File.Create (fp);
		bf.Serialize (file, game);
		file.Close ();
		filePaths.Add (fp);
		gameInfo.Add (game.info);
	}
	//save the given game, overwriting the save at the given index
	public void Save (int i, Game game) {
		string fp = Application.persistentDataPath + "/Saves/" + game.playerCharacter.name + "/" + game.playerCharacter.name + " " + (i + 1).ToString () + ".inf";
		Directory.CreateDirectory (Application.persistentDataPath + "/Saves");
		Directory.CreateDirectory (Application.persistentDataPath + "/Saves/" + game.playerCharacter.name);
		BinaryFormatter bf = new BinaryFormatter ();
		SurrogateSelector ss = new SurrogateSelector ();
		ss.AddSurrogate (typeof(Vector2), new StreamingContext (StreamingContextStates.All), new Vector2SerializationSurrogate ());
		ss.AddSurrogate (typeof(Character), new StreamingContext (StreamingContextStates.All), new CharacterSerializationSurrogate ());
		bf.SurrogateSelector = ss;
		if (File.Exists (fp)) {
			File.Delete (fp);
		}
		FileStream file = File.Create (fp);
		bf.Serialize (file, game);
		file.Close ();
		gameInfo [i] = game.info;
	}

	public Game Load (int i) {
		Game game = new Game ();
		if(File.Exists(filePaths [i])) {
			BinaryFormatter bf = new BinaryFormatter();
			SurrogateSelector ss = new SurrogateSelector ();
			ss.AddSurrogate (typeof(Vector2), new StreamingContext (StreamingContextStates.All), new Vector2SerializationSurrogate ());
			ss.AddSurrogate (typeof(Character), new StreamingContext (StreamingContextStates.All), new CharacterSerializationSurrogate ());
			bf.SurrogateSelector = ss;
			FileStream file = File.OpenRead(filePaths [i]);
			game = (Game)bf.Deserialize(file);
			file.Close();
		}
		return game;
	}
	//loads the latest save
	public Game Load () {
		return Load (filePaths.Count - 1);
	}

	public GameInfo GetGameInfo (int i) {
		return gameInfo [i];
	}
}
