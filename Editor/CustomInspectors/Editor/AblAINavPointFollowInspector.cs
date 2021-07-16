using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Akitacore{
[CustomEditor(typeof(AblAINavPointFollow))]
public class AblAINavPointFollowInspector : Editor {
	public override void OnInspectorGUI(){
		AblAINavPointFollow ablNav = (AblAINavPointFollow)target;

		if(GUILayout.Button("Store position as nav point")){
			ablNav.AddNavPoint ();
			Debug.Log ("Navigation point added.");
		}

		if(GUILayout.Button("SetToStart")){
			ablNav.SetToStart ();
			Debug.Log ("Navigation agent returned to starting position..");
		}

		if(GUILayout.Button("SetToEnd")){
			ablNav.SetToEnd ();
			Debug.Log ("Navigation agent returned to final position..");
		}

		DrawDefaultInspector ();
	}
}
}
