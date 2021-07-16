//creates stuff when an object is disabled. Useful for destruction effects.
//spawn on destroy creates issues in the editor, so if you want to do that, use this script, and call disable and then destroy.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Akitacore{
public class SpawnOnDisable : MonoBehaviour {

	public GameObject[] replacements;
	
	// Update is called once per frame
	void OnDisable () {
		foreach(var replacement in replacements){
			GameObject spawned = GameObject.Instantiate (replacement);
			spawned.transform.position = gameObject.transform.position;
		}
	}
}
}
