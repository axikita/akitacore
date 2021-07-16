using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Akitacore{
public class EventOnDisable : MonoBehaviour {

	public UnityEvent ExecuteEvent;

	void OnDisable(){
		ExecuteEvent.Invoke ();
	}
}
}