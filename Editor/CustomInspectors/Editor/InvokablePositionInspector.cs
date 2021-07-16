using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Akitacore{
[CustomEditor(typeof(InvokablePosition))]
public class InvokablePositionInspector : Editor
{
	public override void OnInspectorGUI(){
		InvokablePosition invokable = (InvokablePosition)target;
		if(GUILayout.Button("Add Current Position to Targets")){
			invokable.AddPositionToTargets();
		}
		
		DrawDefaultInspector();
	}
}
}
