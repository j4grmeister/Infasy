using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightScreen : MonoBehaviour {
	SpriteRenderer srender;

	// Use this for initialization
	void Start () {
		srender = gameObject.GetComponent<SpriteRenderer> ();
	}
	
	// Update is called once per frame
	void Update () {
		gameObject.transform.position = Game.current.playerCharacter.gameObject.transform.position + Vector3.back * 5;
		srender.color = new Color (0f, 0f, 0f, 1f - Game.current.map.currentMap.light);
	}
}
