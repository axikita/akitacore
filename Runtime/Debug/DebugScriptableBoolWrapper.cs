//simple wrapper component for a bool variable.
//this script only exists to test the akitacore BoolVariable reference-type behavior in WebGL.
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Akitacore{
public class DebugScriptableBoolWrapper : MonoBehaviour
{
	public BoolVar myBool;

	public bool GetVarValue(){
		return myBool.val;
	}
}
}
