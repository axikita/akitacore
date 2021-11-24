//Interface for using a UnityEvent to set a flag value.
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Akitacore{
public class FlagSet : MonoBehaviour
{
	public BoolVariable boolFlag;
	public IntVariable intFlag;
	public FloatVariable floatFlag;
	public StringVariable stringFlag;
	
	public bool boolTargetValue;
	public int intTargetValue;
	public float floatTargetValue;
	public string stringTargetValue;
	
	public void SetFlags(){
		boolFlag.Set(boolTargetValue);
		intFlag.Set(intTargetValue);
		floatFlag.Set(floatTargetValue);
		stringFlag.Set(stringTargetValue);
	}
}
}
