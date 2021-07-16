using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Akitacore{
[CreateAssetMenu(fileName = "InputState2Axis", menuName = "ScriptableObjects/InputState/InputState2Axis", order = 202)]
public class InputState2Axis : InputState{
    
    public string keyAxisX;
	public string keyAxisY;
    public string joyAxisX;
	public string joyAxisY;

	float[] inputVal = new float[]{0.0f,0.0f};
	
	public override float[] GetAxes(){
		inputVal[0] = GreaterAxis(keyAxisX, joyAxisX);
		inputVal[1] = GreaterAxis(keyAxisY,joyAxisY);
		return base.DampByThreshold(inputVal);
	}
	public override bool GetConditions(){
		return true; 
	}
	public override int GetAxisCount(){
		return 2; 
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
}
}
