using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Akitacore{
[CustomEditor(typeof(SpeakerLibrary))]
public class SpeakerLibraryInspector : Editor{
    public override void OnInspectorGUI(){
		SpeakerLibrary database = (SpeakerLibrary)target;

		DrawDefaultInspector ();

		if (GUILayout.Button ("Propogate")) {
			database.Propogate ();
		}

	}
}
}
