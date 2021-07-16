//Various functions that can be called by UnityEvent.Invoke

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventLibrary : MonoBehaviour
{

	public void TeleoprtTo(Vector3 targetLocation){
		transform.position = targetLocation;
	}
}
