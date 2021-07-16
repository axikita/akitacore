using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Akitacore{
[CreateAssetMenu(fileName = "FloatVar", menuName = "ScriptableObjects/ScriptableData/FloatVar", order = 50)]
public class FloatVar : ScriptableData {

	//initial value
	public float init;

	//runtime value
	public float val;

	public override void Reset(){
		val = init;
	}
}
}
