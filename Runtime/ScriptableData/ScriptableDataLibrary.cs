using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Akitacore{
[CreateAssetMenu(fileName = "ScriptableDataLibrary", menuName = "ScriptableObjects/ScriptableDataContainers/ScriptableDataLibrary", order = 50)]
public class ScriptableDataLibrary : ScriptableDataContainer {

	public List<BoolVar> boolVars;
	public List<FloatVar> floatVars;
	public List<IntVar> intVars;
	public List<StringVar> stringVars;
	public List<Vector2Var> vector2Vars;
	public List<Vector3Var> vector3Vars;
	public List<Vector4Var> vector4Vars;
	

	// Use this for initialization
	public void Propogate () {
		
		Resources.LoadAll(""); //loads entire resources folder to RAM so that it can be searched.
		
		Debug.Log ("Preparing to propogate ScriptableData from Resources folder");
		if (stringVars == null) {
			stringVars = new List<StringVar> ();
		}
		if (floatVars == null) {
			floatVars = new List<FloatVar> ();
		}
		if (intVars == null) {
			intVars = new List<IntVar> ();
		}
		if (boolVars == null) {
			boolVars = new List<BoolVar> ();
		}
		if (vector2Vars == null) {
			vector2Vars = new List<Vector2Var> ();
		}
		if (vector3Vars == null) {
			vector3Vars = new List<Vector3Var> ();
		}
		if (vector4Vars == null) {
			vector4Vars = new List<Vector4Var> ();
		}

		stringVars.Clear ();
		floatVars.Clear ();
		intVars.Clear ();
		boolVars.Clear ();
		vector2Vars.Clear ();
		vector3Vars.Clear ();
		vector4Vars.Clear ();

		foreach (var v in Resources.FindObjectsOfTypeAll (typeof(StringVar)) as StringVar[]) {
			stringVars.Add (v);
		}
		foreach (var v in Resources.FindObjectsOfTypeAll (typeof(FloatVar)) as FloatVar[]) {
			floatVars.Add (v);
		}

		/*foreach (var v in Resources.LoadAll<FloatVar> ("") as FloatVar[]) {
			floatVars.Add (v);
		}*/
		foreach (var v in Resources.FindObjectsOfTypeAll (typeof(IntVar)) as IntVar[]) {
			intVars.Add (v);
		}
		foreach (var v in Resources.FindObjectsOfTypeAll (typeof(BoolVar)) as BoolVar[]) {
			boolVars.Add (v);
		}
		foreach (var v in Resources.FindObjectsOfTypeAll (typeof(Vector2Var)) as Vector2Var[]) {
			vector2Vars.Add (v);
		}
		foreach (var v in Resources.FindObjectsOfTypeAll (typeof(Vector3Var)) as Vector3Var[]) {
			vector3Vars.Add (v);
		}
		foreach (var v in Resources.FindObjectsOfTypeAll (typeof(Vector4Var)) as Vector4Var[]) {
			vector4Vars.Add (v);
		}
		Debug.Log ("Finished propogating ScriptableData from Resources folder");
	}

	public override void Reset() {
		Debug.Log ("Preparing to reset all contained ScriptableData");
		if (stringVars == null) {
			stringVars = new List<StringVar> ();
		}
		if (floatVars == null) {
			floatVars = new List<FloatVar> ();
		}
		if (intVars == null) {
			intVars = new List<IntVar> ();
		}
		if (boolVars == null) {
			boolVars = new List<BoolVar> ();
		}
		if (vector3Vars == null) {
			vector3Vars = new List<Vector3Var> ();
		}

		foreach (var v in stringVars) {
			v.Reset ();
		}
		foreach (var v in floatVars) {
			v.Reset ();
		}
		foreach (var v in intVars) {
			v.Reset ();
		}
		foreach (var v in boolVars) {
			v.Reset ();
		}
		foreach (var v in vector2Vars) {
			v.Reset ();
		}
		foreach (var v in vector3Vars) {
			v.Reset ();
		}
		foreach (var v in vector4Vars) {
			v.Reset ();
		}
		Debug.Log ("Finished resetting all contained ScriptableData");
	}
}
}
