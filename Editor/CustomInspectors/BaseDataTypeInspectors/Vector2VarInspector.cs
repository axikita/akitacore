using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Akitacore{
[CustomEditor(typeof(Vector2Var))]
public class Vector2VarInspector : Editor {
	public override void OnInspectorGUI(){
		Vector2Var scriptableData = (Vector2Var)target;

		if (GUILayout.Button ("Reset")) {
			scriptableData.Reset ();
		}

		DrawDefaultInspector ();

	}

}
}