using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionSchedule {
	//used scheduling actions through the use of lambdas
	static List<System.Action> scheduledActions = new List<System.Action> ();
	static List<float> scheduledTimes = new List<float> ();
	static List<System.Func<bool>> repeatedActions = new List<System.Func<bool>> ();

	public static void Update () {
		//reduce time on scheduled actions
		//perform any actions that are due to be performed and then remove them from the schedule
		for (int i = 0; i < scheduledTimes.Count; i++) {
			float t = scheduledTimes [i];
			scheduledTimes.RemoveAt (i);
			t -= GameTime.deltaWorldTime;
			if (t <= 0) {
				scheduledActions [i] ();
				scheduledActions.RemoveAt (i);
			} else {
				scheduledTimes.Insert (i, t);
			}
		}

		for (int i = 0; i < repeatedActions.Count; i++) {
			//Debug.Log ("repeat loop");
			bool b = repeatedActions [i] ();
			if (b) {
				repeatedActions.RemoveAt (i);
			}
		}
	}

	//schedules actions through the use of lambdas
	//schedules the given action to take place in t seconds
	public static void ScheduleAction (System.Action action, float t) {
		scheduledActions.Add (action);
		scheduledTimes.Add (t);
	}

	//performs repeat every frame until it returns true
	public static void AddRepeatedAction (System.Func<bool> repeat) {
		repeatedActions.Add (repeat);
	}
}
