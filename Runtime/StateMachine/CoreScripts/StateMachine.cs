//defines state machine scriptable object data structure.
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;



namespace Akitacore{
[CreateAssetMenu(fileName = "StateMachine", menuName = "ScriptableObjects/StateMachine", order = 1)]
public class StateMachine : ScriptableObject,ISerializationCallbackReceiver {


	//--Properties------------------------------------
	public string debugString;

	//public string label;

	//used as a special dictionary key. Not displayed, not really supposed to be human readable. Just needs to be unique.
	//The state machine will 
	public const string initialState = "_AKITACORE_STATE_MACHINE_INITIAL_STATE_RESERVED_VALUE_30e6eucfqhavmc7l96c2"; 

	//public List<State> states;
	public List<Fork> forks;

	public Dictionary<string,State> states;

	public List<string> stateKeys;
	public List<State> stateValues;

	public List<Parameter> parameters;


	//--Methods-----------------------------------------------------
	public void OnAfterDeserialize(){

		//dictionaries don't serialize, so do it manually
		if (stateKeys != null && stateValues != null) {
			states = new Dictionary<string, State>();
			for (int i = 0; i < stateKeys.Count; i++) {
				states.Add (stateKeys [i], stateValues [i]);
			}
		}

		if (forks == null) {
			forks = new List<Fork> ();
		}
		if (states == null) {
			states = new Dictionary<string, State>();
		}
		if (parameters == null) {
			parameters = new List<Parameter> ();
		}


		if (states.Count > 0) {
			foreach (var state in states) {
				foreach (var trans in state.Value.transitions) {
					if (trans.targetForkIndex == -1) { 
						try {
							trans.target = states [trans.targetStateKey];
						} catch {
							Debug.LogWarning ("cannot find state " + trans.targetStateKey + "during deserialziation");
						}

					} else if (trans.targetForkIndex != -1 && trans.targetStateKey == "") {
						try {
							trans.target = forks [trans.targetForkIndex];
						} catch {
							Debug.LogWarning ("cannot find fork " + trans.targetForkIndex.ToString () + "during deserialziation");
						}
					}
				}
			}
		}

		if (forks.Count > 0) {
			foreach (var fork in forks) {
				foreach (var trans in fork.transitions) {
					if (trans.targetForkIndex == -1) { 
						try {
							trans.target = states [trans.targetStateKey];
						} catch {
							Debug.LogWarning ("cannot find state " + trans.targetStateKey + "during deserialziation");
						}

					} else if (trans.targetForkIndex != -1 && trans.targetStateKey == "") {
						try {
							trans.target = forks [trans.targetForkIndex];
						} catch {
							Debug.LogWarning ("cannot find fork " + trans.targetForkIndex.ToString () + "during deserialziation");
						}
					}
				}
			}
		}


	}

	//Ensure that the expected data structures are present and that all transitions are valid
	public void VerifyIntegrity(){
		bool removalFound = false;

		//check for null values
		if (states == null) {
			states = new Dictionary<string, State> ();
		}
		if (forks == null) {
			forks = new List<Fork> ();
		}
		if (parameters == null) {
			parameters = new List<Parameter> ();
		}
		
		//ensure start exists
		if(!states.ContainsKey(initialState)){
			var newState = new State();
			newState.editorPosition.Set(500,250);
			
			states.Add(initialState, newState);
		}
		
		//ensure all transitions have a target
		foreach(var state in states){
			foreach (var transition in state.Value.transitions) {
				if (transition.target == null) {
					state.Value.transitions.Remove (transition);
					Debug.LogWarning ("Orphan transition found and deleted on state " + state.Key);
					removalFound = true;
					break;
				}
			}
		}
		for (int i = 0; i < forks.Count; i++) {
			foreach (var transition in forks[i].transitions) {
				if (transition.target == null) {
					forks [i].transitions.Remove (transition);
					Debug.LogWarning ("Orphan transition found and deleted on node " + i.ToString());
					removalFound = true;
					break;
				}
			}
		}

		//check for repeat transitions
		foreach(var state in states){
			if (state.Value.transitions.Count > 0) {
				for (int i = 0; i < state.Value.transitions.Count; i++) {
					for (int j = 0; j < state.Value.transitions.Count; j++) { //iterate through same list for duplicates
						if (i != j && UnityEngine.Object.Equals (state.Value.transitions [i].target, state.Value.transitions [j].target)) {
							state.Value.transitions.RemoveAt (j);
							Debug.LogWarning ("duplicate transition found and deleted on state " + state.Key);
							removalFound = true;
							break;
						}
					}
					for (int k = 0; k < state.Value.transitions [i].transitionConditions.Count; k++) {
						if (GetParameter (state.Value.transitions [i].transitionConditions[k].parameterName) == null) {
							state.Value.transitions [i].transitionConditions[k].parameterName = "";
						}
					}
				}
			}
		}
			
		foreach (var fork in forks) {
			if (fork.transitions.Count > 0) {
				for (int i = 0; i < fork.transitions.Count; i++) {
					for (int j = 0; j < fork.transitions.Count; j++) {
						if (i != j && UnityEngine.Object.Equals (fork.transitions [i].target, fork.transitions [j].target)) {
							fork.transitions.RemoveAt (j);
							Debug.LogWarning ("duplicate transition found and deleted on fork " + i.ToString());
							removalFound = true;
							break;
						}
					}
					for (int k = 0; k < fork.transitions [i].transitionConditions.Count; k++) {
						if (GetParameter (fork.transitions [i].transitionConditions[k].parameterName) == null) {
							fork.transitions [i].transitionConditions[k].parameterName = "";
						}
					}
				}
			}
		}

		if (removalFound) {
			VerifyIntegrity ();
		}
	}
		
	public void OnBeforeSerialize(){
		VerifyIntegrity();
		
		if (states == null) {
			states = new Dictionary<string, State>();
		}
		if (forks == null) {
			forks = new List<Fork> ();
		}
		if (parameters == null) {
			parameters = new List<Parameter> ();
		}


		//dictionaries don't serialize, so do it manually
		if (states != null) {
			stateKeys = new List<string> ();
			stateValues = new List<State> ();
			foreach (var state in states) {
				stateKeys.Add (state.Key);
				stateValues.Add (state.Value);
			}
		}

		//transitions are not appropriately storing their targets, so set up the internal reference.
		if(states.Count>0){
		foreach (var state in states) {
			foreach (var trans in state.Value.transitions) {
				if (trans.target.IsState ()) {
					trans.targetStateKey = KeyFromState ((State)trans.target);
					trans.targetForkIndex = -1;
				} else {
					for (int i = 0; i < forks.Count; i++) {
						if(UnityEngine.Object.Equals(trans.target, forks[i])){
							trans.targetForkIndex = i;
						}
					}
					trans.targetStateKey = "";
				}
			}
		}
		}
		
		if(forks.Count>0){
		foreach (var fork in forks) {
			foreach (var trans in fork.transitions) {
				if (trans.target.IsState ()) {
					trans.targetStateKey = KeyFromState ((State)trans.target);
					trans.targetForkIndex = -1;
				} else {
					for (int i = 0; i < forks.Count; i++) {
						if(UnityEngine.Object.Equals(trans.target, forks[i])){
							trans.targetForkIndex = i;
						}
					}
					trans.targetStateKey = "";
				}
			}

		}
		}

	}




	//--Classes-------------------------------------------------------------------------------------------------------

	//----Abstracts and Interfaces--------------------------

	//A valid target to transition to. 
	[System.Serializable]
	public abstract class Transitionable:ITransition{
		public virtual State GetTarget(State source){
			return source;
		}
			
		public abstract bool IsState ();
	}

	///Interface for a walkable object in the state machine network. 
	public interface ITransition{
		State GetTarget(State source);
	}




	//--Instantiable Classes--------------------------------------------------------

	[System.Serializable]
	public class State:Transitionable{
		//public string label; //duplicate of dictionary key
		public float entryTime;
		public List<Transition> transitions;

		public Vector2 editorPosition;

		public override bool IsState(){return true;}

		//public Ability OnEnter;
		//public Ability OnStay;
		//public Ability OnExit;

		//Behaviors are defined in the StateMachineController, not the state itself.

		public State(){
			//label = "New State";
			entryTime = 0;
			transitions = new List<Transition>();
			editorPosition = Vector2.zero;
		}

		public State(Vector2 EditorPosition){
			//label = "New State";
			entryTime = 0;
			transitions = new List<Transition>();
			editorPosition = EditorPosition;
		}
			
		//this gets called when a state is reached.
		public override State GetTarget(State source){
			entryTime = Time.time;
			return this;
		}
			
	}

	//------------------------



	//A state with no behavior that transitions immediately to one of several states.
	//A fork needs to be a transitionable so it's a valid transition target, and needs to be an ITransition so we can invoke GetTarget;
	[System.Serializable]
	public class Fork:Transitionable{
		public Vector2 editorPosition;
		public enum Type {Chain, Markov, Priority};
		public Type type;
		public List<Transition> transitions;
		List<Transition> prioritizedTransitions; //used to sort for priority fork

		public override bool IsState(){return false;}

		public Fork(){
			editorPosition = Vector2.zero;
			type = Type.Chain;
			transitions = new List<Transition>();
			prioritizedTransitions = new List<Transition> ();
		}

		public Fork(Vector2 position){
			editorPosition = position;
			type = Type.Chain;
			transitions = new List<Transition>();
			prioritizedTransitions = new List<Transition> ();
		}
			
		public override State GetTarget(State source){
			switch (type) {
			case Type.Chain:
				foreach (var trans in transitions) {
					if (!UnityEngine.Object.Equals(trans.GetTarget(source), source)){
						return trans.GetTarget(source);
					}
				}
				return source;
			case Type.Markov:
				float totalWeight = 0.0f;
				foreach (var trans in transitions) {
					if (trans.markovTransitionWeight > 0.0f) {
						totalWeight += trans.markovTransitionWeight;
					}
				}
				float selectionThreshold = UnityEngine.Random.value * totalWeight;
				totalWeight = 0.0f;
				foreach (var trans in transitions) {
					if (trans.markovTransitionWeight > 0.0f) {
						totalWeight += trans.markovTransitionWeight;
						if (totalWeight >= selectionThreshold) {
							return trans.GetTarget (source);
						}
					}
				}
				return source;


			case Type.Priority:
				//transitions.Sort (); CAN'T DO THIS- breaks pairing with editor forknodes.
				prioritizedTransitions.Clear();
				foreach (var trans in transitions) {
					prioritizedTransitions.Add (trans);
				}
				prioritizedTransitions.Sort ();
				foreach (var trans in prioritizedTransitions) {

					if (!UnityEngine.Object.Equals (trans.GetTarget (source), source)) {
						return trans.GetTarget (source);
					}
				}
				return source;


			}


			return source; //TODO: make this branch based on fork type
		}
	}



	///A connection from a state to a single state or fork.
	[System.Serializable]
	public class Transition : ITransition, IComparable<Transition>{
		public Transitionable target;

		//transition parameters for fork transitions
		public float markovTransitionWeight =1.0f;
		public float transitionPriority = 1.0f;


		//Parameters for each transition condition

		public List<TransitionCondition> transitionConditions;

		///Used for serialization. Do not reference during runtime.
		public int targetForkIndex;
		///Used for serialization. Do not reference during runtime.
		public string targetStateKey;

		bool canTransition; //flag for iteration through conditions

		public Transition(){
			transitionConditions= new List<TransitionCondition>();
		}

		public virtual State GetTarget (State source){
			if (transitionConditions != null && transitionConditions.Count > 0) {
				canTransition = true;
				foreach (var cond in transitionConditions) {
					switch (cond.type) {
					case TransitionCondition.Type.Boolean:
						if (cond.parameterName == "" || cond.parameterName == null) {
							break;
						}
						if ((cond.boolParamValue && !cond.invertToggle) || (!cond.boolParamValue && cond.invertToggle)) {
							break;
						}
						canTransition = false;
						break;

					case TransitionCondition.Type.Float:
						if (cond.parameterName == "" || cond.parameterName == null) {
							break;
						}
						switch (cond.compareType) {
						case TransitionCondition.CompareType.equal:
							if (Mathf.Approximately (cond.floatParamValue, cond.floatParamThreshold)) {
								break;
							}
							canTransition = false;
							break;
						case TransitionCondition.CompareType.greater:
							if (cond.floatParamValue > cond.floatParamThreshold) {
								break;
							}
							canTransition = false;
							break;
						case TransitionCondition.CompareType.less:
							if (cond.floatParamValue < cond.floatParamThreshold) {
								break;
							}
							canTransition = false;
							break;
						case TransitionCondition.CompareType.notEqual:
							if (!Mathf.Approximately (cond.floatParamValue, cond.floatParamThreshold)) {
								break;
							}
							canTransition = false;
							break;
						}
						//Debug.LogWarning ("unidentified compare type");
						break;

					case TransitionCondition.Type.Int:
						if (cond.parameterName == "" || cond.parameterName == null) {
							break;
						}
						switch (cond.compareType) {
						case TransitionCondition.CompareType.equal:
							if (Mathf.Approximately (cond.intParamValue, cond.intParamThreshold)) {
								break;
							}
							canTransition = false;
							break;
						case TransitionCondition.CompareType.greater:
							if (cond.intParamValue > cond.intParamThreshold) {
								break;
							}
							canTransition = false;
							break;
						case TransitionCondition.CompareType.less:
							if (cond.intParamValue < cond.intParamThreshold) {
								break;
							}
							canTransition = false;
							break;
						case TransitionCondition.CompareType.notEqual:
							if (!Mathf.Approximately (cond.intParamValue, cond.intParamThreshold)) {
								break;
							}
							canTransition = false;
							break;
						}
						//Debug.LogWarning ("unidentified compare type");
						break;

					case TransitionCondition.Type.Time:
						if (Time.time > source.entryTime + cond.transitionTime) {
							break;
						} else {
							canTransition = false;
							break;
						}

					default:
						//Debug.LogWarning ("undefined condition type");
						break;
					}
				}

				if (canTransition) { //no conditions returned false;
					return target.GetTarget (source);
				}
				return source;

			}
			//no conditions defined, default to Direct transition.
			return target.GetTarget (source);
		}

		public int CompareTo(StateMachine.Transition trans){
			if (trans == null) {
				return -1;
			}
			if (trans.transitionPriority < this.transitionPriority) {
				return 1;
			}
			if (Mathf.Approximately (trans.transitionPriority, this.transitionPriority)) {
				return 0;
			}
			return -1;
		}
			
	}

	[System.Serializable]
	public class TransitionCondition{
		public string parameterName;

		public enum Type {Boolean, Float, Int, Time}
		public Type type;

		public enum CompareType{greater, less, equal, notEqual}
		public CompareType compareType;

		public bool boolParamValue;
		public float floatParamValue;
		public float floatParamThreshold;
		public int intParamValue;
		public int intParamThreshold;
		public bool invertToggle;

		public float transitionTime =1.0f;
	}

	//-------

	//A parameter is a value that can be used by the controller to affect transition logic.
	[System.Serializable]
	public class Parameter{
		///name parameter should not be set except during creation and serialization.
		public string name;
		public enum Type {Bool, Float, Int}
		public Type type;
		public bool boolParam;
		public float floatParam;
		public int intParam;

		public Parameter(string Name, bool newVal){
			name = Name;
			type = Type.Bool;
			boolParam = newVal;
			floatParam = 0.0f;
			intParam = 0;
		}

		public Parameter(string Name, float newVal){
			name = Name;
			type = Type.Float;
			boolParam = false;
			floatParam = newVal;
			intParam = 0;
		}

		public Parameter(string Name, int newVal){
			name = Name;
			type = Type.Int;
			boolParam = false;
			floatParam = 0.0f;
			intParam = newVal;
		}
	}

	//--State machine Utilities--------------------------------------------------------

	public string KeyFromState(StateMachine.State queryState){
		if (states != null) {
			foreach(var stateData in states){
				if (UnityEngine.Object.Equals (stateData.Value, queryState)) {
					return stateData.Key;
				}
			}
		}
		return "";
	}



	public Parameter GetParameter(string name){
		if (parameters != null) {
			foreach (var param in parameters) {
				if (string.Equals(param.name, name)){
					return param;
				}
			}
		}
		return null;
	}

}
}




//class heirarchy
// # = instantiable
/*
ITransition
ITransition->Transitionable
ITransition->Transitionable--->State #
ITransition->Transitionable--->Fork
ITransition->Transitionable--->Fork------------->ThresholdFork #
ITransition->Transitionable--->Fork------------->MarkovFork #

ITransition->Transition
ITransition->Transition------>DirectTransition #
ITransition->Transition------>BoolTransition #
ITransition->Transition------>FloatTransiton #

RichState #

*/
