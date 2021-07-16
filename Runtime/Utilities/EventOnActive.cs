using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Akitacore{
public class EventOnActive : MonoBehaviour {

	public UnityEvent ExecuteEvent;

	void OnEnable(){
		ExecuteEvent.Invoke ();
	}
}
}
