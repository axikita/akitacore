using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Akitacore{
[CustomEditor(typeof(FloatVar))]
public class FloatVarInspector : Editor {
	public override void OnInspectorGUI(){
		FloatVar scriptableData = (FloatVar)target;

		if (GUILayout.Button ("Reset")) {
			scriptableData.Reset ();
		}

		DrawDefaultInspector ();

	}

}
}