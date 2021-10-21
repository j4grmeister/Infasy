using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Library<E> {
	List<E> data = new List<E>();
	List<string> name = new List<string>();

	public void Add(string s, E d) {
		name.Add (s);
		data.Add (d);
	}

	public int Size() {
		return data.Count;
	}

	public E Get (string s) {
		return data [name.IndexOf (s)];
	}
	public E Get (int i) {
		return data [i];
	}
	public string GetName (int i) {
		return name [i];
	}

	public int IndexOfName (string s) {
		return name.IndexOf (s);
	}

	public void Set(string s, E d) {
		int i = name.IndexOf (s);
		if (i == -1) {
			Add (s, d);
		} else {
			data [i] = d;
		}
	}
	public void Set(int i, E d) {
		data [i] = d;
	}

	/*
	public E this[int i] {
		get {

		}
		set {

		}
	}
	*/
}