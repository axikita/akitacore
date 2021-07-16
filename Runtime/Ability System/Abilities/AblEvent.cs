//Run a unity event as a latch ability.
//unity event = any public void function.
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Akitacore{
public class AblEvent : Ability {

	//----Variable Declarations-----------------------------------------------------------------

	public UnityEvent ExecuteEvent;

	//----Monobehavior Update Loop--------------------------------------------------------------
	//use awake to inmitialize any values that depend on game state, things that should be initialized on reactivation, or references to opther objects.
	void Awake(){
		base.FindReferences(); // finds rootObj and collCheck
	}
	
	//--Execute---------------------------------------------------------------------
	
	//called by controller for each ability before Execute in the update step.
	//An ability that has PreUpdate called but not Execute should do nothing.
	override public void PreUpdate(){
		called = false;
	}

	//this is called from controllers or wrapper abilities during Update. Input values are passed as arguments.
	//Behavior should only occur if conditionsmet == true. 
	override public void Execute(bool conditionsMet, float[] axisInputs){
		called = true;
		if(!latched){
			if(conditionsMet){
				ExecuteEvent.Invoke ();
			}
		}
	}
	
	override public void PostUpdate(){
		latched = called;
	}

	//-----Utility Functions-----------------------------------------------------------------

}
}