using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoneButton : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void OnClick () {
		//end the player's turn if he/she is in combat
		int i = Game.current.BattleWithCharacter (Game.current.playerCharacter);
		if (i != -1) {
			Game.current.battles [i].EndTurn (Game.current.playerCharacter);
		}
	}
}
