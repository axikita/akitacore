using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Akitacore;

namespace Akitacore{
public class ConditionDebug : MonoBehaviour
{
	public BoolVariable myBoolVariable;
	public FloatVariable myFloatVariable;
	public IntVariable myIntVariable;
	public StringVariable myStringVariable;

	
	void Update(){
		Debug.Log("Bool Variable: "+myBoolVariable.Get().ToString());
		Debug.Log("Float Variable: "+myFloatVariable.Get().ToString());
		Debug.Log("Int Variable: "+myIntVariable.Get().ToString());
		Debug.Log("String Variable: "+myStringVariable.Get().ToString());
	}
}
}
