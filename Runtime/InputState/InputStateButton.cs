using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Akitacore{
[CreateAssetMenu(fileName = "InputStateButton", menuName = "ScriptableObjects/InputState/InputStateButton", order = 200)]
public class InputStateButton : InputState{
	
	public KeyCode key;
	public string button;
	
	private float[] inputVal = new float[0];
	
	
	///Returns array containing a single axis input.
	public override float[] GetAxes(){
		return inputVal;
	}
	public override bool GetConditions(){
		if(button !=""){
			return (Input.GetKey(key) || Input.GetButton(button));
		} else {
			return 	Input.GetKey(key);
		}
	}
	public override int GetAxisCount(){
		return 0; //default value.
	}
}
}
