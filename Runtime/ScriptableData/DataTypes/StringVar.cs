using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Akitacore{
[CreateAssetMenu(fileName = "StringVar", menuName = "ScriptableObjects/ScriptableData/StringVar", order = 50)]
public class StringVar : ScriptableData {

	//initial value
	public string init;

	//runtime value
	public string val;

	public override void Reset(){
		val = init;
	}
}
}