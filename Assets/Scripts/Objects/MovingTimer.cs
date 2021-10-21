using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingTimer : MonoBehaviour {

	public float time; //time (in seconds) to destroy this object in
	public Vector2 velocity;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		gameObject.transform.Translate (Game.GameToUnity (new Vector3 (velocity.x, velocity.y, 0) * GameTime.deltaWorldTime));
		time -= GameTime.deltaWorldTime;
		if (time <= 0) {
			GameObject.Destroy (gameObject);
		}
	}
}
