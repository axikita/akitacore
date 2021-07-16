using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Akitacore{
[CustomEditor(typeof(IntVar))]
public class IntVarInspector : Editor {
	public override void OnInspectorGUI(){
		IntVar scriptableData = (IntVar)target;

		if (GUILayout.Button ("Reset")) {
			scriptableData.Reset ();
		}

		DrawDefaultInspector ();

	}

}
}