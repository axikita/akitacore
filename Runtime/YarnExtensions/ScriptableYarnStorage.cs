//Responsible for translating ScriptableData into a Yarn VariableStorageBehavior.

//alternative Variable Storage Behavior to CommnCore.

//Rather than binding data, data that should be yarn editable is defined as a scriptableobject data type, EG, BoolVar rather than Bool.
//No library of editable data is maintained, rather, ScriptableYarnStorage is an interface for accessing any ScriptableData in the Resources Folder.

//Note that, unlike CommnCore, Saving and Loading is handled from a separate script- ScriptableSaveLoad.cs

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO; //used for saving and loading
using Yarn.Unity;

namespace Akitacore{
public class ScriptableYarnStorage : VariableStorageBehaviour {


	public ScriptableDataLibrary database;

	// Use this for initialization
	void Start () {
		List<StringVar> stringVars = new List<StringVar> ();
		List<FloatVar> floatVars = new List<FloatVar> ();
		List<IntVar> intVars = new List<IntVar> ();
		List<BoolVar> boolVars = new List<BoolVar> ();

		#if UNITY_EDITOR
			database.Propogate(); 
			//automatically updates the database within the editor. Within a build, no new data should be defined, or if it is, it should be added manually from the script that makes it.
			//the Propogate method is slow, so we don't want it unnecessarily bogging down load times in a build.

		#endif
	}


	//--VariableStorageBehavior Overrides-----------------------------
	//these functions are called by yarn scripts.

	public override void SetNumber(string variableName, float number){
		foreach (var v in database.floatVars) {
			if (v.name == variableName) {
				v.val = number;
				return;
			}
		}
		foreach (var v in database.intVars) {
			if (v.name == variableName) {
				v.val = Mathf.FloorToInt (number);
				return;
			}
		}
		Debug.LogWarning ("SetNumber failed: No number \"" + variableName + "\" found in " + database.name);
		return;
	}

	///Return yarn value as float. Used by Yarn integration.
	public override float GetNumber(string variableName){
		foreach (var v in database.floatVars) {
			if (v.name == variableName) {
				return v.val;
			}
		}
		foreach (var v in database.intVars) {
			if (v.name == variableName) {
				return (float)v.val;
			}
		}
		Debug.LogWarning ("GetNumber Failed: No number \"" + variableName + "\" found in " + database.name);
		return 0.0f;
	}

	public override Yarn.Value GetValue(string variableName){
		if(variableName[0] == '$'){
			variableName = variableName.Substring(1);
		}
		
		Yarn.Value yarnVal;
		foreach (var v in database.floatVars) {
			if (v.name == variableName) {
				yarnVal = new Yarn.Value ();

				yarnVal.SetType (Yarn.Value.Type.Number);
				yarnVal.SetValue (v.val);
				return yarnVal;
			}
		}
		foreach (var v in database.intVars) {
			if (v.name == variableName) {
				yarnVal = new Yarn.Value ();
				yarnVal.SetType (Yarn.Value.Type.Number);
				yarnVal.SetValue ((float)v.val);
				return yarnVal;
			}
		}
		foreach (var v in database.boolVars) {
			if (v.name == variableName) {
				yarnVal = new Yarn.Value ();
				yarnVal.SetType (Yarn.Value.Type.Bool);
				yarnVal.SetValue (v.val);
				return yarnVal;
			}
		}
		foreach (var v in database.stringVars) {
			if (v.name == variableName) {
				yarnVal = new Yarn.Value ();
				yarnVal.SetType (Yarn.Value.Type.String);
				yarnVal.SetValue (v.val);
				return yarnVal;
			}
		}
		Debug.LogWarning("Returning null yarn value, no ScriptableData \""+variableName+"\" found.");
		yarnVal = new Yarn.Value ();
		yarnVal.SetType (Yarn.Value.Type.Null);
		return yarnVal;
	}

	//Override a yarn value 
	public override void SetValue(string variableName, Yarn.Value value){
		variableName = variableName.Substring(1);
		if (value.type == Yarn.Value.Type.Number) {
			foreach (var v in database.floatVars) {
				if (v.name == variableName) {
					v.val = value.AsNumber;
					return;
				}
			}
			foreach (var v in database.intVars) {
				if (v.name == variableName) {
					v.val = Mathf.FloorToInt (value.AsNumber);
					return;
				}
			}
		} else if (value.type == Yarn.Value.Type.Bool) {
			foreach (var v in database.boolVars) {
				if (v.name == variableName) {
					v.val = value.AsBool;
					return;
				}
			}
		} else if (value.type == Yarn.Value.Type.String) {
			foreach (var v in database.stringVars) {
				if (v.name == variableName) {
					v.val = value.AsString;
					return;
				}
			}
		}
		Debug.LogWarning ("SetValue failed, \"" + variableName + " does not match the name of ScriptableData, or is null type.");
	}
		
	public override void Clear(){
		Debug.LogWarning ("Clear called from yarn. Don't use this.");
	}
	
	void OnApplicationQuit(){
		Debug.Log("Resetting scriptable data in "+database.name+"from var to init.");
		database.Reset();
	}

	//this function feels broken, in that it gets called automatically after initialization, at a time when I don't want defaults, I want the loaded file.
	//So the name does not match its function.
	///Depricated for unity usage. Initializes flag values but does not overwrite anything.
	public override void ResetToDefaults (){
		//database.Reset ();
	}
}
}
