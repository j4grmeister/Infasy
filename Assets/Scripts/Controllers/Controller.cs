using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour {
	protected Character character;

	Map.Direction walkDirection = Map.Direction.down;
	float walkTime = 0f;
	Vector2 lastPos;
	Vector2 targetPos = Vector2.zero;

	List<Map.Movement> moveQueue = new List<Map.Movement> ();

	public bool hasReachedTargetPosition {
		get {
			return (walkTime <= 0f);
		}
	}

	// Use this for initialization
	void Start () {
		character = gameObject.GetComponent<Character> ();
		ControllerStart ();
		if (character.newCharacter) {
			CharacterInit ();
			DailyUpdate ();
			character.newCharacter = false;
		}
		lastPos = character.position;
	}

	protected virtual void ControllerStart () {

	}

	public void Init () {
		
	}

	public virtual void CharacterInit () {

	}
	
	// Update is called once per frame
	void Update () {
		if (!GameTime.paused && !character.IsDead ()) {
			ControllerUpdate ();

			if (walkTime > 0f) {
				walkTime -= GameTime.deltaTime;
				if (walkTime < 0f && (targetPos - character.position).normalized == Map.Vector2FromDirection (walkDirection) * -1) {
					//Debug.Log (character.name + " " + character.position);
					//character.position = character.position + Map.Vector2FromDirection (walkDirection) * walkDistance;
					character.position = targetPos;
					walkTime = 0f;
				} else if (walkTime > 0f) {
					character.Walk (walkDirection);
					/*
					if (lastPos == character.position) {
						//the character must have collided with something
						walkTime = 0f;
					}
					*/
				}
			} else if (moveQueue.Count > 0) {
				Walk (moveQueue [0].direction, moveQueue [0].distance);
				moveQueue.RemoveAt (0);
			}
			lastPos = character.position;
		}
	}

	protected virtual void ControllerUpdate () {

	}

	public virtual void DailyUpdate () {

	}

	public virtual void OnPlayerInteract () {
		
	}

	public virtual Battle.Turn GetTurn () {
		Battle.Turn t;
		t.action = new List<Action> ();
		t.target = new List<Character> ();
		return t;
	}

	//edits the ability scores of a character of this type to match what they should be at the specified level
	public virtual void Level (int level, Character.Abilities a) {

	}

	//makes the character walk the given distance in the specified direction
	//returns amount of time (in seconds) that the movement will take
	protected float Walk (Map.Direction direction, float distance) {
		walkDirection = direction;
		walkTime = distance / character.speed;
		targetPos = character.position + Map.Vector2FromDirection (direction) * distance;
		return distance / character.speed;
	}

	//makes the character walk to the specified position
	//returns amount of time (in seconds) that the movement will take
	protected void WalkTo (Vector2 position) {
		moveQueue = PhysicsTD.GetPathBetween (character.position, position);
		walkTime = 0f;
	}
}
