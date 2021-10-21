using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Character : MonoBehaviour {
	[System.Serializable]
	public struct Position {
		//ID of the map that this position is for
		public ushort map;
		public Vector2 position;
	}

	public enum Race {
		man,
		woman,
		skeleton
	}

	public enum Alignment {
		good,
		neutral,
		evil
	}

	[System.Serializable]
	public class Abilities {
		public static int baseAbilityScore = 7;
		public static int pointsPerLevel = 2;
		public static int baseTotal {
			get {
				return baseAbilityScore * 7;
			}
		}

		public int constitution;
		public int intelligence;
		public int wisdom;
		public int strength;
		public int dexterity;
		public int charisma;
		public int total {
			get {
				return constitution + intelligence + wisdom + strength + dexterity + charisma;
			}
		}

		public Abilities () {
			constitution = baseAbilityScore;
			intelligence = baseAbilityScore;
			wisdom = baseAbilityScore;
			strength = baseAbilityScore;
			dexterity = baseAbilityScore;
			charisma = baseAbilityScore;
		}

		//calculates the level of a character with the given ability scores
		public static int LevelOf (Abilities a) {
			int dif = a.total - baseAbilityScore;
			return dif / pointsPerLevel;
		}
	}

	public static float damageFlashTime = .5f;


	public System.Type controllerType;

	public Map.Direction faceDirection = Map.Direction.down;
	public Map.Direction lastFaceDirection = Map.Direction.down;
	bool moving = false;

	//public static int combatDistance = 10;

	//each character has a unique ID
	//an ID of 0 indicates that an ID has not yet been assigned to this character
	public ushort id;
	//public float radius;
	//public Vector2 center;
	public float speed = 3f;
	public bool newCharacter = true;

	public Library<object> characterInfo = new Library<object> ();
	public List<Quest> quests = new List<Quest> ();
	public List<Quest> completedQuests = new List<Quest> (); //only used by the player character
	public ushort currentQuestID = 0;
	public Quest currentQuest {
		get {
			for (int i = 0; i < quests.Count; i++) {
				if (quests [i].id == currentQuestID)
					return quests [i];
			}
			return null;
		}
	}

	//public string characterName;
	public Race race;
	public Alignment alignment;
	public Controller controller; //don't serialize
	public List<Position> mapPositions = new List<Position> ();
	public ushort mapID = Game.current.map.id; //the id of the map that this character is located in; world map by default
	public Vector2 position {
		//returns the position on the current map
		get {
			for (int i = 0; i < mapPositions.Count; i++) {
				if (mapPositions [i].map == mapID) {
					return mapPositions [i].position;
				}
			}
			return Vector2.zero;
		}
		//sets the position for the current map
		set {
			int listIndex = -1;
			for (int i = 0; i < mapPositions.Count; i++) {
				if (mapPositions [i].map == mapID) {
					listIndex = i;
					break;
				}
			}
			Position p;
			p.map = mapID;
			p.position = value;
			if (listIndex == -1) {
				mapPositions.Add (p);
				listIndex = mapPositions.Count - 1;
			} else {
				mapPositions [listIndex] = p;
			}
		}
	}
	//public List<Location> location = new List<Location> (); //history of locations for purpose of backtracking from alternate areas back to the world map
	public float sightDistance;
	public float sightWidth; //the width of sight at sightDistance away from the character
	//public Vector2 faceDirection; //the direction this character is facing

	public Inventory inventory = new Inventory ();

	//public List<Action> skills = new List<Action> ();
	public List<string> skills = new List<string> ();
	//public Action readiedAction;

	//list of the characters that this character is currently engaged in battle with or can engage in battle with
	//character is not in combat if this is either empty or each character is currently non-hostile
	public List<ushort> engagedTargetIDs = new List<ushort> ();
	public List<Character> engagedTargets {
		get {
			List<Character> l = new List<Character> ();
			for (int i = 0; i < engagedTargetIDs.Count; i++) {
				l.Add (Game.current.GetCharacterByID (engagedTargetIDs [i]));
			}
			return l;
		}
	}
	Animator animator;
	SpriteRenderer srender;

	public int armor;

	public int level;
	public int hp;
	public int maxhp;
	public int ap;
	public int fp;
	public int maxfp;
	public int exp;
	public Abilities abilities;
	//used for old art
	//List<Animator> equipmentAnimators = new List<Animator> ();

	//CircleCollider2D collider;
	//the position in the target block where this portal leads
	void Start () {
		gameObject.AddComponent<SpriteRenderer> ();
		gameObject.AddComponent<Animator> ();

		animator = gameObject.GetComponent<Animator> ();
		//gameObject.AddComponent (controllerType);
		//controller = gameObject.GetComponent<Controller> ();
		srender = gameObject.GetComponent<SpriteRenderer> ();

		//faceDirection = new Vector2 (0, -1);
		//faceDirection = Map.Direction.down;

		controllerType = controller.GetType ();

		gameObject.GetComponent<SpriteRenderer> ().sprite = GetCharacterSprite (race);
		gameObject.GetComponent<Animator> ().runtimeAnimatorController = GetCharacterAnimatorController (race);
	}

	static int nOfFramesForStop = 5;
	//Vector2 lastMoveDirection;
	//Vector2 moveDirection;
	int stoppedFrames = 0;
	Vector2 lastPos = new Vector2 ();

	void Update () {
		Animate (); //animate the character
		if (!GameTime.paused)
			CharacterUpdate ();
	}

	void CharacterUpdate() {
		//set the position of the character's gameobject
		if (moving) {
			Vector2 movement = Map.Vector2FromDirection (faceDirection) * speed * GameTime.deltaTime;
			position = PhysicsTD.MoveObject (position, movement);
		}
		moving = false;
		gameObject.transform.localPosition = Game.GameToUnity (position);
		//move the character object back if this character is dead. that way it isn't rendered over living characters when it is walked on top of
		if (IsDead ()) {
			gameObject.transform.Translate (Vector3.forward * .01f);
		}

		//check the tile the character is on and perform any necessary actions
		/*
		//get the tile character is on
		Tile t = new Tile ();
		t = Game.current.map.GetTile ((int)position.x, (int)position.y);
		if (t.mods.Contains (Tile.ModType.door_wood)) {
			t.boolAttributes.Set ("closed", false);
		}
		*/

		//reset the move direction to zero, otherwise the character will keep showing the walking
		//animation as long asnit isn't receiving constant input to the Walk method
		//moving = false;
		/*
		if (moveDirection != Vector2.zero) {
			faceDirection = moveDirection.normalized;
			moveDirection = Vector2.zero;
		}
		*/
	}

	public void Init () {
		gameObject.AddComponent (controllerType);
		controller = gameObject.GetComponent<Controller> ();
		//controller.Init ();
		abilities = new Abilities ();

		//maxhp = GetMaxHP (race, level);
		maxhp = GameBalance.GetMaxHP (level, abilities.constitution);
		hp = maxhp;
		ap = GetMaxAP (race, level);
		maxfp = GetMaxFP (race, level);
		fp = maxfp;
		//strength = GetStandardAttack (race, level);
		//strength = GameBalance.GetStandardStrength (level);
		armor = GameBalance.GetStandardArmor (level);
	}

	void Animate () {
		//choose walk animation state
		//if (moveDirection != lastMoveDirection) {
		if (faceDirection != lastFaceDirection) {
			//character animation
			/*
			if (moveDirection.x != 0 && moveDirection.y != 0) {
				//a little work around to prevent the character's animation from switching directions
				//between every frame when it travels diagonally
				moveDirection = lastMoveDirection;
			}
			*/
			Vector2 moveDirection = Map.Vector2FromDirection (faceDirection);
			animator.SetFloat ("Horizontal Direction", moveDirection.x);
			animator.SetFloat ("Vertical Direction", moveDirection.y);
			//equipment animation
			//used for old art
			/*
			for (int i = 0; i < equipmentAnimators.Count; i++) {
				equipmentAnimators [i].SetFloat ("Horizontal Direction", moveDirection.x);
				equipmentAnimators [i].SetFloat ("Vertical Direction", moveDirection.y);
			}
			*/
			stoppedFrames = 0;
		}// else if (!moving) {
			//stoppedFrames++;
		//}
		if (stoppedFrames == nOfFramesForStop) {
			//character animation
			animator.SetTrigger ("Stop");
			//equipment animation
			//used for old art
			/*
			for (int i = 0; i < equipmentAnimators.Count; i++) {
				equipmentAnimators [i].SetTrigger ("Stop");
			}
			*/
			moving = false;
		}
		lastFaceDirection = faceDirection;
		animator.SetBool ("Moving", moving);
	}

	public void Say (string dialogue) {
		string[] a = dialogue.Split (new char[] { '\n' });
		for (int i = 0; i < a.Length; i++) {
			Game.dialogueMenu.AddDialogue (this, a [i]);
		}
	}

	public void Prompt (string prompt, string [] response, System.Action[] onSelect) {
		string[] a = prompt.Split (new char[] { '\n' });
		for (int i = 0; i < a.Length - 1; i++) {
			Game.dialogueMenu.AddDialogue (this, a [i]);
		}
		Game.dialogueMenu.AddPrompt (this, a [a.Length - 1], response, onSelect);
	}

	public void Walk (Map.Direction direction) {
		//only complete the task if there are action points available
		//if (ap > 0) {
		//moveDirection = direction.normalized * ((GameTime.paused || GameTime.frozen) ? 0 : 1);
		//Vector2 movement = PhysicsTD.AdjustMovementForCollisions (transform.position + (Vector3)collider.offset, Game.GameToUnity (direction.normalized * speed * GameTime.deltaTime), collider.radius);
		//Vector2 movement = Map.Vector2FromDirection (direction) * speed * GameTime.deltaTime;
		faceDirection = direction;
		//adjust movement for available action points
		//float d = Mathf.Min (movement.magnitude, ap);
		//Vector2 m = movement.normalized * movement.magnitude;
		//position = PhysicsTD.MoveObject (position, m);
		//moveDirection = m.normalized;
		moving = true;
		//deplete action points for movement
		//can't move in combat anymore, haven't decided yet whether to permanently remove
		//ap -= d;
		//}
	}

	public List<Action> AllActions () {
		List<Action> actions = new List<Action> ();
		actions.Add (inventory.equippedWeapon.action);
		for (int i = 0; i < skills.Count; i++) {
			actions.Add (Skill.skills.Get (skills [i]));
		}
		return actions;
	}

	//only returns the actions which this character can currently perform
	//makes AI programming easier
	public List<Action> AvailableActions () {
		List<Action> actions = new List<Action> ();
		if (inventory.equippedWeapon.action.CanDo (this)) {
			actions.Add (inventory.equippedWeapon.action);
		}
		for (int i = 0; i < skills.Count; i++) {
			if (Skill.skills.Get (skills [i]).CanDo (this)) {
				actions.Add (Skill.skills.Get (skills [i]));
			}
		}
		return actions;
	}

	//returns the amount of time (in seconds) this action will take to perform
	public float DoAction (Action action, Character target) {
		return action.DoAction (this, target);
		/*
		if (action.hostile) {
			Game.current.Engage (this, target);
		}
		*/
	}

	public void EndTurn () {
		if (InCombat ()) {
			Game.current.battles [Game.current.BattleWithCharacter (this)].EndTurn (this);
		}
	}

	public Battle GetBattle () {
		return Game.current.battles [Game.current.BattleWithCharacter (this)];
	}

	//public void LearnSkill (Action skill) {
	public void LearnSkill (string skill) {
		skills.Add (skill);
		//oldfp.Add (skill.name, skill.maxfp);
	}

	public bool HasQuest (Quest q) {
		for (int i = 0; i < quests.Count; i++) {
			if (quests [i].id == q.id)
				return true;
		}
		return false;
	}

	public List<Quest> GetActiveQuestsOfType (System.Type t) {
		List<Quest> l = new List<Quest> ();
		for (int i = 0; i < quests.Count; i++) {
			if (quests [i].GetType () == t)
				l.Add (quests [i]);
		}
		return l;
	}

	public bool InCombat () {
		return (engagedTargetIDs.Count > 0);
	}

	//engages the specified character
	public void Engage (Character target) {
		if (!engagedTargetIDs.Contains (target.id)) {
			engagedTargetIDs.Add (target.id);
		}
	}

	//disengages the specified character
	public void Disengage (Character target) {
		if (engagedTargetIDs.Contains (target.id)) {
			engagedTargetIDs.Remove (target.id);
		}
	}

	//disengages all targets
	public void Disengage () {
		engagedTargetIDs.Clear ();
	}

	public bool EngagedWith (Character target) {
		return engagedTargetIDs.Contains (target.id);
	}

	public int HP () {
		return hp;
	}

	public int MaxHP () {
		return maxhp;
	}

	public int AP () {
		return (int)ap;
	}

	/*
	public int FP (string actionName) {
		return oldfp.Get (actionName);
	}
	*/

	public void Heal (int hp) {
		this.hp += hp;
		if (this.hp >= maxhp)
			this.hp = maxhp;
	}

	//source should be 0 if it did not come from another character
	public void Damage (int dmg, ushort source) {
		hp -= dmg;
		if (hp <= 0) {
			if (source != 0) {
				Game.current.GetCharacterByID (source).GainExp (ExpGivenOnDeath ());
			}
			Die ();
		}

		//display the damage text
		Game.codeCanvas.AddFadingTextToWorld (dmg.ToString (), position, 2);
		//flash the character
		float runTime = 0f;
		Color c = Color.white;
		System.Func<bool> flashCharacter = () => {
			runTime += GameTime.deltaWorldTime;
			float q = damageFlashTime / 4;
			float f = 0f;
			if (runTime <= q) {
				f = runTime / q;
			} else if (runTime <= q * 2) {
				f = 1 - runTime / (q * 2);
			} else if (runTime <= q * 3) {
				f = runTime / (q * 3);
			} else {
				f = 1 - runTime / (q * 4);
			}
			//this flashes the character black. ideally it would flash white. fix later cuz i'm a lazy bastard
			srender.color = new Color ((c.r - f), (c.g - f), (c.b - f));
			if (runTime >= damageFlashTime) {
				srender.color = c;
				return true;
			}
			return false;
		};
		ActionSchedule.AddRepeatedAction (flashCharacter);
		GameTime.Freeze (damageFlashTime);
	}

	//damages the character without a source for the damage
	public void Damage (int dmg) {
		Damage (dmg, 0);
	}

	public void Die () {
		hp = 0;
		animator.SetBool ("Dead", true);
		Disengage ();
	}

	public void UseAP (int i) {
		ap -= i;
	}
				
	public void UseFP (string actionName, int i) {
		//oldfp.Set (actionName, oldfp.Get (actionName) - i);
		fp -= i;
	}

	public void ReplenishAP () {
	}

	public bool IsDead () {
		return (hp <= 0);
	}

	public void GainExp (int e) {
		//if this is the player character and it is in a battle, then add the exp to the battle instead so that the gaining of exp can be managed by the battle when it finishes
		if (id == Game.current.playerCharacterID && InCombat ()) {
			GetBattle ().playerExpReward += e;
		} else {
			exp += e;
			if (e >= ExpForLevel (level))
				LevelUp ();
		}
	}

	public void LevelUp () {
		level++;
		maxhp = GameBalance.GetMaxHP (level, abilities.constitution);
		hp = maxhp;
		ap = GameBalance.GetMaxAP (abilities.dexterity);
		abilities.strength = GetStandardAttack (race, level);
		armor = GetStandardDefense (race, level);
		//display dialogue if this is the player character
		if (id == Game.current.playerCharacterID) {
			Game.dialogueMenu.AddDialogue (null, "you leveled up!");
		}
	}

	public void Freeze () {
		controller.enabled = false;
	}

	public void Unfreeze () {
		controller.enabled = true;
	}

	public List<Character> NearbyCharacters () {
		List<Character> nearby = new List<Character> ();
		//iterate through the character list and add any that are nearby
		for (int i = 0; i < Game.current.characters.Count; i++) {
			float distance = Vector2.Distance (position, Game.current.characters [i].position);
			if (distance < 5) {
				//skip this character if it is the same as this one
				if (Game.current.characters [i] != this) {
					nearby.Add (Game.current.characters [i]);
				}
			}
		}
		return nearby;
	}

	public List<Character> EngagedCharacters () {
		return engagedTargets;
	}

	public int ExpGivenOnDeath () {
		return Mathf.RoundToInt (GameBalance.ExpGained (exp));
	}

	//returns true if character c1 is hostile towards character c0
	public static bool AreHostile (Character c0, Character c1) {
		/*
		if (c0.alignment == Alignment.good && c1.alignment == Alignment.evil)
			return true;
		return false;
		*/
		if (c0.EngagedWith (c1))
			return true;
		return false;
	}

	public static Character CreateCharacter (Race r, Alignment a, System.Type controllerType) {
		Character c = Game.CreateObject<Character> ();
		//initialize data
		c.race = r;
		c.alignment = a;
		c.controllerType = controllerType;
		//c.speed = 3;
		c.level = 1;
		c.exp = 0;
		c.sightDistance = 5;
		c.sightWidth = 3;
		//initialize game object
		//GameObject textObj = Game.CreateObject<SpriteRenderer> (c.gameObject).gameObject;
		//textObj.transform.localPosition = new Vector3 (0, 0, -5);
		c.Init ();


		if (r == Race.skeleton) {
			Sword sword = new Sword ();
			c.inventory.Add (sword);
			c.inventory.Equip (sword);
		}
		return c;
	}

	public static Character CreateCharacter () {
		Character c = Game.CreateObject<Character> ();
		c.Init ();
		return c;
	}

	public static Sprite GetCharacterSprite (Race r) {
		//return Resources.Load<Sprite> ("Sprites/Characters/" + r.ToString ());
		return GameResources.GetCharacterSprite (r);
	}

	public static RuntimeAnimatorController GetCharacterAnimatorController (Race r) {
		return Resources.Load<RuntimeAnimatorController> ("Controllers/Animator Controllers/Characters/" + r.ToString());
	}

	public static int GetMaxHP (Race r, int level) {
		//return 8 * level;
		return GameBalance.GetStandardHP (level);
	}

	public static int GetMaxAP (Race r, int level) {
		return 5 * level;
	}

	public static int GetMaxFP (Race r, int level) {
		return 10 * level;
	}

	public static int GetStandardAttack (Race r, int level) {
		return 5 + 3 * level;
	}

	public static int GetStandardDefense (Race r, int level) {
		return 3 + 2 * level;
	}
		
	public static int ExpForLevel (int level) {
		return (int)GameBalance.ExpForLevel (level);
	}

	public static string GenerateName (Race r) {
		string path = "Data/Random/Names/" + r.ToString ();
		TextAsset file = Resources.Load<TextAsset> (path);
		string data = file.text;
		string first = "";
		string last = "";

		//find beginning index of first names
		int i = 0;
		for (; true; i++) {
			if (data [i] == '<') {
				string match = data.Substring (i + 1, 5);
				if (match == "first") {
					i += 8;
					break;
				}
			}
		}
		//create a list of first names
		List<string> fnames = new List<string> ();
		for (int l = 0; true; l++) {
			if (data [i] == '<') {
				string match = data.Substring (i + 1, 6);
				if (match == "/first") {
					break;
				}
			}
			if (data [i + l] == '\n') {
				fnames.Add (data.Substring (i, l));
				i += l + 1;
				l = 0;
			}
		}

		//find beginning index of last names
		i = 0;
		for (; true; i++) {
			if (data [i] == '<') {
				string match = data.Substring (i + 1, 4);
				if (match == "last") {
					break;
				}
			}
		}
		//create a list of first names
		List<string> lnames = new List<string> ();
		for (int l = 0; true; l++) {
			if (data [i] == '<') {
				string match = data.Substring (i + 1, 5);
				if (match == "/last") {
					break;
				}
			}
			if (data [i + l] == '\n') {
				lnames.Add (data.Substring (i, l));
				i += l + 1;
				l = 0;
			}
		}

		//generate and return a random name
		first = fnames [Random.Range(0, fnames.Count)];
		last = lnames [Random.Range(0, lnames.Count)];
		return (first + " " + last).ToLower ();
	}

	public static System.Type GetDefaultControllerType (Race r) {
		switch (r) {
		case Race.man:
		case Race.woman:
			return typeof(TownsPersonAI);
		case Race.skeleton:
			return typeof(SkeletonAI);
		default:
			return typeof(Controller);
		}
	}
}