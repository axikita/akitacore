using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Akitacore{
[CreateAssetMenu(fileName = "Vector4Var", menuName = "ScriptableObjects/ScriptableData/Vector4Var", order = 50)]
public class Vector4Var : ScriptableData {

	//initial value
	public Vector4 init;

	//runtime value
	public Vector4 val;

	public override void Reset(){
		val = init;
	}
}
}