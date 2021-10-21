using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInput {
	public enum Bind {
		up,
		down,
		left,
		right,
		pause,
		interact,
		back,
		end_turn
	};

	public struct KeyBind {
		public Bind bind;
		public KeyCode key;
		public KeyCode altKey;
	};

	static List<KeyBind> binds = new List<KeyBind> ();

	public static void Init () {
		BindKey (Bind.up, KeyCode.W, KeyCode.UpArrow);
		BindKey (Bind.down, KeyCode.S, KeyCode.DownArrow);
		BindKey (Bind.left, KeyCode.A, KeyCode.LeftArrow);
		BindKey (Bind.right, KeyCode.D, KeyCode.RightArrow);
		BindKey (Bind.pause, KeyCode.Escape, KeyCode.None);
		BindKey (Bind.interact, KeyCode.Return, KeyCode.None);
		BindKey (Bind.back, KeyCode.RightShift, KeyCode.None);
		BindKey (Bind.end_turn, KeyCode.E, KeyCode.None);
		BindKey (Bind.interact, KeyCode.Space, KeyCode.Return);
	}

	public static bool GetKeyDown (Bind b) {
		for (int i = 0; i < binds.Count; i++) {
			if (binds [i].bind == b) {
				if (Input.GetKeyDown (binds [i].key))
					return true;
				if (Input.GetKeyDown (binds [i].altKey))
					return true;
				return false;
			}
		}
		return false;
	}

	public static bool GetKey (Bind b) {
		for (int i = 0; i < binds.Count; i++) {
			if (binds [i].bind == b) {
				if (Input.GetKey (binds [i].key))
					return true;
				if (Input.GetKey (binds [i].altKey))
					return true;
				return false;
			}
		}
		return false;
	}

	public static void BindKey (Bind bind, KeyCode key, KeyCode altKey) {
		//remove the existing keybind if this bind is already defined
		for (int i = 0; i < binds.Count; i++) {
			if (binds [i].bind == bind) {
				binds.RemoveAt (i);
				break;
			}
		}
		KeyBind b;
		b.bind = bind;
		b.key = key;
		b.altKey = altKey;
		binds.Add (b);
	}
}