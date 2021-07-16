using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Akitacore{
[CreateAssetMenu(fileName = "IntVar", menuName = "ScriptableObjects/ScriptableData/IntVar", order = 50)]
public class IntVar : ScriptableData {

	//initial value
	public int init;

	//runtime value
	public int val;

	public override void Reset(){
		val = init;
	}
}
}