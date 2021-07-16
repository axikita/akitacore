using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Akitacore{
[CreateAssetMenu(fileName = "Vector2Var", menuName = "ScriptableObjects/ScriptableData/Vector2Var", order = 50)]
public class Vector2Var : ScriptableData {

	//initial value
	public Vector2 init;

	//runtime value
	public Vector2 val;

	public override void Reset(){
		val = init;
	}
}
}