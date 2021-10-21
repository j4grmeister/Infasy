using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaveInfo : MonoBehaviour {
	public Text characterName;
	public Text saveName;
	public Text playTime;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	//an index of -1 indicates that the current game's info should be displayed,
	//as if the "new game" save option is currently selected
	public void SetSaveIndex (int i) {
		characterName.text = Game.current.playerCharacter.name;
		if (i == -1) {
			GameSave.GameInfo info = Game.current.info;
			saveName.text = "New Save";
			playTime.text = "Play Time " + ((int)info.timePlayed.TotalHours).ToString () + ":" + info.timePlayed.Minutes.ToString () + ":" + info.timePlayed.Seconds.ToString ();
		} else {
			GameSave.GameInfo info = Game.currentGameSave.GetGameInfo (i);
			saveName.text = "Save " + (i + 1).ToString ();
			playTime.text = "Play Time " + ((int)info.timePlayed.TotalHours).ToString () + ":" + info.timePlayed.Minutes.ToString () + ":" + info.timePlayed.Seconds.ToString ();
		}
	}
}
