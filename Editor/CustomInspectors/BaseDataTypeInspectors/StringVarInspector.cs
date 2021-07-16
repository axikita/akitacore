using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Akitacore{
[CustomEditor(typeof(StringVar))]
public class StringVarInspector : Editor {
	public override void OnInspectorGUI(){
		StringVar scriptableData = (StringVar)target;

		if (GUILayout.Button ("Reset")) {
			scriptableData.Reset ();
		}

		DrawDefaultInspector ();

	}

}
}