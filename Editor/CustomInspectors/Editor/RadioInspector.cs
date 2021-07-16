using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Akitacore{
[CustomEditor(typeof(Radio))]
public class RadioInspector : Editor {
	public override void OnInspectorGUI(){
		Radio radio = (Radio)target;

		if (GUILayout.Button ("Populate SFX Sources")) {
			radio.PopulateSFX ();
		}

		DrawDefaultInspector ();
	}
}
}