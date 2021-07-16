using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Akitacore{
[CreateAssetMenu(fileName = "BoolVar", menuName = "ScriptableObjects/ScriptableData/BoolVar", order = 50)]
public class BoolVar : ScriptableData {

	//initial value
	public bool init;

	//runtime value
	public bool val;

	public override void Reset(){
		val = init;
	}
}
}
