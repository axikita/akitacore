using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Akitacore{
public class StateMachineController : StateMachineGeneric
{
	
	public List<StateActionSet> stateActionSets;
	StateActionSet currentStateActionSet;
	
	public override void BehaviorInitialization(){
		return;
	}
	
	public override void FindCurrentStateBehavior(){
		foreach (var stateActionSet in stateActionSets) {
			if (string.Equals (stateActionSet.stateName, stateMachine.KeyFromState(currentState))) {
				currentStateActionSet = stateActionSet;
				return;
			}
		}
		currentStateActionSet = null; //statemachine is in a state without defined actions.
	}
	
	public override void PreUpdate(){
		foreach(var actionSet in stateActionSets){
			if(actionSet.OnEnter !=null){
				actionSet.OnEnter.PreUpdate();
			}
			if(actionSet.OnStay !=null){
				actionSet.OnStay.PreUpdate();
			}
			if(actionSet.OnExit !=null){
				actionSet.OnExit.PreUpdate();
			}
		}
	}
	
	//--Safe Execution Functions------------------------------------------------------------
	public override void PerformEnter(){
		if (currentStateActionSet != null) {
			if (currentStateActionSet.OnEnter != null) {
				currentStateActionSet.OnEnter.Execute (true, null);
				
			}
		}
	}

	public override void PerformStay(bool buttonState){
		if (currentStateActionSet != null) {
			if (currentStateActionSet.OnStay != null) {
				if (currentStateActionSet.OnStay != null) {
					currentStateActionSet.OnStay.Execute (buttonState, null);
				}
			}
		}
	}

	public override void PerformExit(){
		if (currentStateActionSet != null) {
			if (currentStateActionSet.OnExit != null) {
				if (currentStateActionSet.OnExit != null) {
					currentStateActionSet.OnExit.Execute (true, null);
				}
			}
		}
	} 
	
	public override void PostUpdate(){
		foreach(var actionSet in stateActionSets){
			if(actionSet.OnEnter !=null){
				actionSet.OnEnter.PostUpdate();
			}
			if(actionSet.OnStay !=null){
				actionSet.OnStay.PostUpdate();
			}
			if(actionSet.OnExit !=null){
				actionSet.OnExit.PostUpdate();
			}
		}
	}
	
	//-Data structures--------------------------------------
	
	[System.Serializable]
	public class StateActionSet {
		public string stateName;
		[Tooltip("Only Latch abilities. If you want to trigger something else, use a latch wrapper.")]
		public Ability OnEnter;
		[Tooltip("Only Latch or Const abilities. If you want to trigger something else, use a wrapper.")]
		public Ability OnStay;
		[Tooltip("Only Latch abilities. If you want to trigger something else, use a latch wrapper.")]
		public Ability OnExit;
	}
	
	
	//-Inspector Utilities -----------------------------------
	
	public override void BuildBehaviorLibrary(){
		if(stateMachine == null){
			stateActionSets = new List<StateActionSet> ();
			return;
		}
		
		StateActionSet temp;
		bool setFound;
		bool removalFound = false;
		if (stateActionSets == null) {
			stateActionSets = new List<StateActionSet> ();
		}
		//remove entries that do not correspond to a state
		foreach (var set in stateActionSets) {
			if(set.stateName == StateMachine.initialState){
				stateActionSets.Remove (set);
				removalFound = true;
				break;
			}
			if (!stateMachine.states.ContainsKey (set.stateName)) {
				stateActionSets.Remove (set);
				removalFound = true;
				break;
			}
		}
		//add unlisted entries
		foreach (var state in stateMachine.states) {
			if(state.Key == StateMachine.initialState){
				continue;
			}
			setFound = false;
			foreach (var set in stateActionSets) {
				if (set.stateName == state.Key) {
					setFound = true;
				}
			}
			if (!setFound) {
				temp = new StateActionSet();
				temp.stateName = state.Key;
				stateActionSets.Add(temp);
			}
		}
		if(removalFound){
			BuildBehaviorLibrary();
		}
	}
}
}
