using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Akitacore{
[CreateAssetMenu(fileName = "InputStateAxis", menuName = "ScriptableObjects/InputState/InputStateAxis", order = 201)]
public class InputStateAxis : InputState{
    
    public string keyAxis;
    public string joyAxis;
	
	private float[] inputVal = new float[]{0.0f};
	
	
	///Returns array containing a single axis input.
	public override float[] GetAxes(){
		if(keyAxis != "" && joyAxis != ""){
			if(Mathf.Abs(Input.GetAxis(keyAxis))>Mathf.Abs(Input.GetAxis(joyAxis))){
				inputVal[0] = Input.GetAxis(keyAxis);
				return base.DampByThreshold( inputVal);
			}
				inputVal[0] = Input.GetAxis(joyAxis);
				return base.DampByThreshold(  inputVal );
		} else if(keyAxis != ""){
			return base.DampByThreshold(new float[]{Input.GetAxis(keyAxis)});
		} else if(joyAxis != ""){
			return base.DampByThreshold( new float[]{Input.GetAxis(joyAxis)});
		}
		Debug.LogWarning("No defined axis on "+this.name+", returning float[]{0.0f}");
		return new float[]{0.0f};
	}
	
	public override bool GetConditions(){
		return true;
	}
	
	public override int GetAxisCount(){
		return 1;
	}
}
}
