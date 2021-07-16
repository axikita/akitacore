using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Akitacore{
[CustomEditor(typeof(ScriptableDataLibrary))]
public class ScriptableDataLibraryInspector : Editor {
	public override void OnInspectorGUI(){
		ScriptableDataLibrary database = (ScriptableDataLibrary)target;

		DrawDefaultInspector ();

		if (GUILayout.Button ("Propogate")) {
			database.Propogate ();
			EditorUtility.SetDirty(database);
		}

		if (GUILayout.Button ("Reset")) {
			database.Reset ();
			EditorUtility.SetDirty(database);
		}



	}
}
}