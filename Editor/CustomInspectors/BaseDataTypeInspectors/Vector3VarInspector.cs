using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Akitacore{
[CustomEditor(typeof(Vector3Var))]
public class Vector3VarInspector : Editor {
	public override void OnInspectorGUI(){
		Vector3Var scriptableData = (Vector3Var)target;

		if (GUILayout.Button ("Reset")) {
			scriptableData.Reset ();
		}

		DrawDefaultInspector ();

	}

}
}