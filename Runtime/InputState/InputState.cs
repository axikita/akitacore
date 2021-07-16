//base template
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Akitacore{
public abstract class InputState : ScriptableObject{
	
	public bool useKeys;
	public bool usePad;
	public FloatVariable inputDampThreshold;
	
	///Returns array of inputs
	public abstract float[] GetAxes();
	
	///Returns a boolean value representing a keypress, button press, or other boolean condition.
	public abstract bool GetConditions();
	
	//returns the expected size of outArray for GetAxes()
	public abstract int GetAxisCount();
	
	public float[] DampByThreshold(float[] inputs){
		for(int i=0; i<inputs.Length; i++){
			if(inputs[i]*inputs[i] < inputDampThreshold.val*inputDampThreshold.val){
				inputs[i] = 0.0f;
			} else {
				inputs[i] = inputs[i]*(1.0f/(1.0f-inputDampThreshold.val));
			}
		}
		return inputs;
	}
	
}

}

/* for convienient copy pasting into derived classes: 
	public override float[] GetAxes(){
		return new float[0]; //default value.
	}
	public override bool GetConditions(){
		return false; //default value.
	}
	public override int GetAxisCount(){
		return 0; //default value.
	}
*/



