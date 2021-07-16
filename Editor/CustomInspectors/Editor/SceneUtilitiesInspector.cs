using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Akitacore{
[CustomEditor(typeof(SceneUtilities))]
public class SceneUtilitiesInspector : Editor {

	public override void OnInspectorGUI(){
		SceneUtilities sceneUtilities = (SceneUtilities)target;

		if(GUILayout.Button("Precompile for Build")){
			sceneUtilities.Precompile ();
			Debug.Log ("Precompilation tasks have been executed.");
		}




		DrawDefaultInspector();
	}
}
}
