using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Akitacore{
[CustomEditor(typeof(BoolVar))]
public class BoolVarInspector : Editor {
	public override void OnInspectorGUI(){
		BoolVar scriptableData = (BoolVar)target;

		if (GUILayout.Button ("Reset")) {
			scriptableData.Reset ();
		}

		DrawDefaultInspector ();

	}

}
}
