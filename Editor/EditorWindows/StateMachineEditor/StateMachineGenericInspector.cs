using System.Collections;
using UnityEngine;
using UnityEditor;

namespace Akitacore{
[CustomEditor(typeof(StateMachineGeneric),true)]
public class StateMachineGenericInspector : Editor
{

	public override void OnInspectorGUI(){
		StateMachineGeneric stateMachine = (StateMachineGeneric)target;
		
		if (GUILayout.Button ("Initialize Parameters")) {
			stateMachine.InitializeParameterLists ();
		}
		if (GUILayout.Button ("Build Behavior Library")) {
			stateMachine.BuildBehaviorLibrary ();
		}


		DrawDefaultInspector();
	}

}
}
