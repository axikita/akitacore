//adds a button to the default inspector for permanently writing generated joystick axes to the input manager.
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Akitacore{
[CustomEditor(typeof(InputDebugDisplay))]
public class InputDebugDisplayInspector : Editor {

	public override void OnInspectorGUI(){
		InputDebugDisplay inputDebug = (InputDebugDisplay)target;

		if(GUILayout.Button("Assign Joysticks to Input")){
			inputDebug.DefineAxes();
			Debug.Log ("Joystick axes have been written to the Input manager.");
		}




		DrawDefaultInspector();
	}

}
}

