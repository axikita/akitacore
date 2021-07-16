//Used to configure scene based on scriptabledata flags
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Akitacore{
public class SceneInitializer : MonoBehaviour
{
	public BoolEvent[] boolEvents;
	public FloatEvent[] floatEvents;
	public IntEvent[] intEvents;
	public StringEvent[] stringEvents;
	
    // Start is called before the first frame update
    void Start()
    {
		foreach (var entry in boolEvents){
			entry.TryInvoke();
		}
		foreach (var entry in floatEvents){
			entry.TryInvoke();
		}
		foreach (var entry in intEvents){
			entry.TryInvoke();
		}
		foreach (var entry in stringEvents){
			entry.TryInvoke();
		}
    }
	
	[System.Serializable]
	public class BoolEvent{
		public BoolVar flag;
		public bool condition;
		public UnityEvent responseEvent;
		
		public void TryInvoke(){
			if(flag.val==condition){
				responseEvent.Invoke();
			}
		}
		
	}
	
	[System.Serializable]
	public class FloatEvent{
		public FloatVar flag;
		public enum executionType{lessThanCond, greaterThanCond, equalToCond};
		public executionType executeIfFlag;
		public float condition;
		public UnityEvent responseEvent;
		
		public void TryInvoke(){
			if(Compare(flag.val,condition,executeIfFlag)){
				responseEvent.Invoke();
			}
		}
		
		bool Compare(float thisVar, float thisCond, executionType thisType){
			if(thisType == executionType.lessThanCond){
				return thisVar<thisCond;
			}
			if(thisType == executionType.greaterThanCond){
				return thisVar>thisCond;
			}
			if(thisType == executionType.equalToCond){
				return Mathf.Approximately(thisVar, thisCond);
			}
			
			Debug.LogError("unexpected value for execution type");
			return false;
		}
		
	}
	
	[System.Serializable]
	public class IntEvent{
		public IntVar flag;
		public enum executionType{lessThanCond, greaterThanCond, equalToCond};
		public executionType executeIfFlag;
		public int condition;
		public UnityEvent responseEvent;
		
		public void TryInvoke(){
			if(Compare(flag.val,condition,executeIfFlag)){
				responseEvent.Invoke();
			}
		}
		
		bool Compare(int thisVar, int thisCond, executionType thisType){
			if(thisType == executionType.lessThanCond){
				return thisVar<thisCond;
			}
			if(thisType == executionType.greaterThanCond){
				return thisVar>thisCond;
			}
			if(thisType == executionType.equalToCond){
				return thisVar==thisCond;
			}
			
			Debug.LogError("unexpected value for execution type");
			return false;
		}
		
	}
	
	[System.Serializable]
	public class StringEvent{
		public StringVar flag;
		[Tooltip("Event executes if strings are equivalent.")]
		public string condition;
		public UnityEvent responseEvent;
		
		public void TryInvoke(){
			if(string.Equals(flag.val, condition)){
				responseEvent.Invoke();
			}
		}
	}

}
}
