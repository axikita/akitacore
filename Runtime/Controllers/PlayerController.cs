using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

namespace Akitacore{
public class PlayerController : MonoBehaviour {

	public ActionBinding[] actionBinds;

	public bool suspended; //disables input 


	void OnEnable(){

		//make sure that rigidbodies on player controllers do not sleep, or else collcheck will fail.
		if (GetComponent<Rigidbody> ()!=null) {
			GetComponent<Rigidbody> ().WakeUp ();
			GetComponent<Rigidbody> ().sleepThreshold = 0.0f;
		}

		if (GetComponent<Rigidbody2D> ()!=null) {
			GetComponent<Rigidbody2D> ().WakeUp ();
			GetComponent<Rigidbody2D> ().sleepMode = RigidbodySleepMode2D.NeverSleep;
		}
		
		foreach (ActionBinding act in actionBinds) {
			act.ability.Verify(true, act.inputState.GetAxes());
		}
		
	}
	
	// Update is called once per frame
	void Update () {
		
		foreach (ActionBinding actBnd in actionBinds) {
			actBnd.ability.PreUpdate();
		}


		//-Execute actions based on inputs and type. If suspended, execute with empty arguments to clear value.

		foreach (ActionBinding actBnd in actionBinds) {
			if(actBnd.inputState.GetConditions()){
				actBnd.ability.Execute(actBnd.inputState.GetConditions() && !suspended, actBnd.inputState.GetAxes());
			}
			
		}
		
		foreach (ActionBinding actBnd in actionBinds) {
			actBnd.ability.PostUpdate();
		}
	}


	[System.Serializable]
	public class ActionBinding{
		
		public InputState inputState;
		
		public Ability ability;
		bool latch = false;

		public bool GetInputDown(){
			if (latch == false && inputState.GetConditions()) {
				latch = true;
				return true;

			}
			if (latch == true && !inputState.GetConditions()) {
				latch = false;
			}
			return false;
		}
	}

	public void Suspend(bool setTo){
		suspended = setTo;
	}
		
		
}
}
