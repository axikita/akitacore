using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Akitacore{
///Base class for deriving scriptable data types from./// 
public class ScriptableData : ScriptableObject {

	public virtual void Reset(){
		Debug.LogWarning ("no reset method specified for this data type. The reset function should set val = init");
	}

}
}
