using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Akitacore{
[CreateAssetMenu(fileName = "Vector3Var", menuName = "ScriptableObjects/ScriptableData/Vector3Var", order = 50)]
public class Vector3Var : ScriptableData {

	//initial value
	public Vector3 init;

	//runtime value
	public Vector3 val;

	public override void Reset(){
		val = init;
	}
}
}