using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Akitacore{
[CreateAssetMenu(fileName = "InputStateNAxis", menuName = "ScriptableObjects/InputState/InputStateNAxis", order = 203)]
public class InputStateNAxis : InputState{
    
    public InputPair[] axisInputs;
	
	float[] inputVal = new float[0];
	
	
	public override float[] GetAxes(){
		if(inputVal.Length != axisInputs.Length){
			inputVal = new float[axisInputs.Length];
		}
		for (int i=0; i<axisInputs.Length; i++){
			inputVal[i] = GreaterAxis(axisInputs[i].keyAxis,axisInputs[i].joyAxis);
		}
		return base.DampByThreshold( inputVal);
	}
	public override bool GetConditions(){
		return false; //default value.
	}
	
	public override int GetAxisCount(){
		if(inputVal !=null){
			return axisInputs.Length;
		} else {
			return 0;
		}
	}
	
	float GreaterAxis(string key, string joy){
		if(key != "" && joy != ""){
			if(Mathf.Abs(Input.GetAxis(key))>Mathf.Abs(Input.GetAxis(joy))){
				return Input.GetAxis(key);
			}
			return Input.GetAxis(joy);
		}else if(key!=""){
			return Input.GetAxis(key);
		}else if(joy!=""){
			return Input.GetAxis(joy);
		}
		Debug.LogWarning("Missing axis definition on "+this.name+". Returning zero value.");
		return 0.0f;
	}
	
	[System.Serializable]
	public struct InputPair{
		public string keyAxis;
		public string joyAxis;
	}
}
}
