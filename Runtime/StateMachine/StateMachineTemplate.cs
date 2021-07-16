//Template for defining a state machine. Copy this code as a starting point for making a state machine with custom behavior.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Akitacore{
public class StateMachineTemplate : StateMachineGeneric
{
	//--DECLARED IN BASE---------------------------
	//public StateMachine stateMachine;
	//----> Contains public Dictionary<string,State> states;
	
	//public StateMachine.State currentState;
	// search stateMachine for a state matching currentState, use the dictionary key to determine current behavior.
	// Alternatively, just use base.GetcurrentState() to get the string Key fro the current state.
	
	
	
	
	//Called during start. Perform any behavior initialization here.
	public override void BehaviorInitialization(){
		return;
	}
	
	//called after a new state is found and PerformExit() is called. Load new behavior for Perform functions.
	public override void FindCurrentStateBehavior(){
		return;
	}
	
	//called once on the first frame the machine is in a new state
	public override void PerformEnter(){
		return;
	}

	//called every frame the machone is in a state. This will be called on the same state as PerformEnter and PerformExit events.
	//PerformEnter will preceed PerformStay for a given state.
	//PerformExit will be called after the last call to PerformStay for a given state.
	public override void PerformStay(bool buttonState){
		return;
	}
	
	//called once on the last frame a machine is in a new state.
	public override void PerformExit(){
		return;
	} 
	
	//called by an inspector Button. Use this to build a data structure for behavior, which contains an entry corresponding to each state.
	//I reccomend checking for state name == Statemachine.initialState and not defining behavior for the Start node.
	public override void BuildBehaviorLibrary(){
		return;
	}
}
}
