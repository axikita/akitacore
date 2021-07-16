using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Akitacore{
[CustomEditor(typeof(Vector4Var))]
public class Vector4VarInspector : Editor {
	public override void OnInspectorGUI(){
		Vector4Var scriptableData = (Vector4Var)target;

		if (GUILayout.Button ("Reset")) {
			scriptableData.Reset ();
		}

		DrawDefaultInspector ();

	}

}
}