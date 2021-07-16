using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;
using System.Reflection;
using Yarn.Unity;
using Akitacore;


namespace Akitacore{
public class StateMachineGeneric : MonoBehaviour{

	//string entryState;

	public StateMachine stateMachine;

	[System.NonSerialized]
	public StateMachine.State currentState;
	StateMachine.State nextState;


	public List<BoolParameter> boolParameters;
	public List<FloatParameter> floatParameters;
	public List<IntParameter> intParameters;

	public bool suspended = false;


	void Start () {
		
		//find initial state
		foreach (var state in stateMachine.states) {
			if(state.Key == StateMachine.initialState){
				
				currentState = state.Value;
				currentState.entryTime = Time.time;
				FindCurrentStateBehavior ();
				break;
			}
		}
		
		BehaviorInitialization();
		
	}

	//Determine the state for the next frame, and execute OnXXX events. 
	//Any state that is reached will persist for at least one frame, executing each of it's OnXXX behaviors.
	void Update () {

		if (!suspended) {
			
			//perform OnStay behavior for the current state
			PerformStay (true);

			//update the transition parameters from the scene for each transition in the state machine.
			//if (!webGLBuild) {
			ParameterUpdate ();
			//}

			//evaluate state conditions, return state for next frame
			nextState = FindNextState ();

			//if next state is different, execute current.OnExit and next.OnEnter(), and set the new state to currentstate.
			if (!object.Equals (currentState, nextState)) {
				PerformExit (); 
				currentState = nextState;
				FindCurrentStateBehavior ();
				PerformEnter ();
			}
		} else {
			PerformStay (false);
		}
			

	}

	//--Update utilities------------------------------------------------------

	///Return first transition that results in a different state, or return current state.
	StateMachine.State FindNextState(){
		StateMachine.State stateFromTransition;
		if (currentState.transitions != null) {
			foreach (var transition in currentState.transitions) {
				stateFromTransition = transition.GetTarget (currentState); //Walk the transition diagram
				if (!object.Equals (currentState, stateFromTransition)) {
					return stateFromTransition;
				}
			}
		}
		return currentState;
	}

	//update the transition parameters in every transition.
	void ParameterUpdate(){
		StateMachine.Parameter workingParam;

		foreach (var state in stateMachine.states) {
			foreach (var trans in state.Value.transitions) {
				foreach (var cond in trans.transitionConditions) {

					workingParam = stateMachine.GetParameter (cond.parameterName);
					if (workingParam != null && workingParam.type == StateMachine.Parameter.Type.Bool) {
						for (int i = 0; i < boolParameters.Count; i++) {
							if (boolParameters [i].parameterName == workingParam.name) {
								
								cond.boolParamValue = boolParameters [i].GetBool ();

								break;
							}
						}
					}
					if (workingParam != null && workingParam.type == StateMachine.Parameter.Type.Float) {
						for (int i = 0; i < floatParameters.Count; i++) {
							if (floatParameters [i].parameterName == workingParam.name) {

								cond.floatParamValue = floatParameters [i].GetFloat ();

								break;
							}
						}
					}
					if (workingParam != null && workingParam.type == StateMachine.Parameter.Type.Int) {
						for (int i = 0; i < intParameters.Count; i++) {
							if (intParameters [i].parameterName == workingParam.name) {

								cond.intParamValue = intParameters [i].GetInt ();
								
								break;
							}
						}
					}
				}
			}
		}


		foreach (var fork in stateMachine.forks) {
			foreach (var trans in fork.transitions) {
				foreach (var cond in trans.transitionConditions) {
					workingParam = stateMachine.GetParameter (cond.parameterName);
					if (workingParam != null && workingParam.type == StateMachine.Parameter.Type.Bool) {
						for (int i = 0; i < boolParameters.Count; i++) {
							if (boolParameters [i].parameterName == workingParam.name) {

								cond.boolParamValue = boolParameters [i].GetBool ();
								
								break;
							}
						}
					}
					if (workingParam != null && workingParam.type == StateMachine.Parameter.Type.Float) {
						for (int i = 0; i < floatParameters.Count; i++) {
							if (floatParameters [i].parameterName == workingParam.name) {

								cond.floatParamValue = floatParameters [i].GetFloat ();
								
								break;
							}
						}
					}
					if (workingParam != null && workingParam.type == StateMachine.Parameter.Type.Int) {
						for (int i = 0; i < intParameters.Count; i++) {
							if (intParameters [i].parameterName == workingParam.name) {

								cond.intParamValue = intParameters [i].GetInt ();
								
								break;
							}
						}
					}
				}
			}
		}
	}

	
	//--Classes and structures------------------------------------------------


		
	//--Inspector Utilities-----------------------------------------------
	
	public void InitializeParameterLists(){
		boolParameters = new List<BoolParameter> ();
		floatParameters = new List<FloatParameter> ();
		intParameters = new List<IntParameter> ();

		if (stateMachine != null) {
			for (int i=0; i<stateMachine.parameters.Count; i++){
				switch (stateMachine.parameters [i].type) {
				case StateMachine.Parameter.Type.Bool:
					boolParameters.Add (new BoolParameter ());
					boolParameters [boolParameters.Count - 1].parameterName = stateMachine.parameters [i].name;
					//condition left undefined, define in inspector.
					break;
				case StateMachine.Parameter.Type.Float:
					floatParameters.Add (new FloatParameter ());
					floatParameters [floatParameters.Count - 1].parameterName = stateMachine.parameters [i].name;
					//condition left undefined, define in inspector.
					break;
				
				case StateMachine.Parameter.Type.Int:
					intParameters.Add (new IntParameter ());
					intParameters [intParameters.Count - 1].parameterName = stateMachine.parameters [i].name;
					//condition left undefined, define in inspector.
					break;
				}
			}
		}
	}

	//--Parameter Classes-------------------------------------------------------------------------------------------------

	//serailizable wrapper class that pairs a string as a parameter lookup with the condition
	[System.Serializable]
	public class BoolParameter{
		public string parameterName;
		public BoolVariable parameter;
		
		public bool GetBool(){
			return parameter.Get();
		}
	}


	[System.Serializable]
	public class FloatParameter{
		public string parameterName;
		public FloatVariable parameter;
		
		
		public float GetFloat(){
			return parameter.Get();
		}
	}

	[System.Serializable]
	public class IntParameter{
		public string parameterName;
		public IntVariable parameter;
		
		
		public int GetInt(){
			return parameter.Get();
		}
	}

	//--Public Accessor Functions----------------------------------------------------------------------
	
	///return the name of the current stateMachine state. Returns "" if state not found. 
	public string GetCurrentState(){
		foreach(var state in stateMachine.states){
			if (state.Value == currentState){
				return state.Key;
			}
		}
		return "";
	}
	
	public bool CompareState(string stateName){
		return (GetCurrentState()==stateName);
	}
	
	
	//--Overridable state behavior functions-----------------------------------------------
	//--To subclass, copy StateMachine Template.cs-----------------------------------------
	
	public virtual void PreUpdate(){
		return;
	}
	
	public virtual void PostUpdate(){
		return;
	}
	
	public virtual void BehaviorInitialization(){
		return;
	}
	
	public virtual void FindCurrentStateBehavior(){
		return;
	}
		
	public virtual void PerformEnter(){
		return;
	}

	public virtual void PerformStay(bool buttonState){
		return;
	}

	public virtual void PerformExit(){
		return;
	} 
	
	public virtual void BuildBehaviorLibrary(){
		Debug.Log("No buildable behavior data structure is defined for this state machine.");
		return;
	}
	
}
}

