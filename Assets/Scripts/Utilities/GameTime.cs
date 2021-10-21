using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTime {
	static System.DateTime sessionStart;
	public static System.TimeSpan playTime {
		get {
			return System.DateTime.Now.Subtract (sessionStart);
		}
	}

	public static float timeScale = 1;
	public static float freezeTime = 0;

	public static bool paused = false;
	//static bool frozen = false;
	public static bool frozen {
		get {
			return (freezeTime > 0);
		}
	}

	public static float deltaTime {
		get {
			//if (paused || frozen)
			if (paused || freezeTime > 0)
				return 0;
			return Time.deltaTime * timeScale;
		}
	}

	public static float deltaWorldTime {
		get {
			if (paused)
				return 0;
			return Time.deltaTime * timeScale;
		}
	}

	public static void Init () {
		timeScale = 1;
		freezeTime = 0;
		paused = false;
	}

	public static void ResetPlayTime () {
		sessionStart = System.DateTime.Now;
	}

	//this must be called exactly once (AND ONLY ONCE) each frame from a monoscript
	public static void Update () {
		//reduce freeze time if necessary
		if (freezeTime > 0) {
			freezeTime -= deltaWorldTime;
			if (freezeTime < 0) {
				freezeTime = 0;
			}
		}
	}

	//freezes the game for t seconds
	//this is different than pausing the game
	//freezing is used primarily for animation purposes that must occur between operations
	public static void Freeze (float t) {
		freezeTime = Mathf.Max (freezeTime, t);
	}

	public static void TogglePause () {
		paused = !paused;
	}

	/*
	public static void ToggleFrozen () {
		frozen = !frozen;
	}
	*/
}
