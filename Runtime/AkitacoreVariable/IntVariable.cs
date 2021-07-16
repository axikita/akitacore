using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using Object = UnityEngine.Object;

namespace Akitacore{
[System.Serializable]
public class IntVariable{
	public enum ExecutionType{simple, scriptable, reference}
	public ExecutionType executionType;
	
	public int baseVal;
	public IntVar intVar;
	public Object conditionObject;
	public string conditionMethod;
	
	public enum ArgumentType{None, Bool, Int, Float, String}
	public ArgumentType argType;
	public bool argBool;
	public int argInt;
	public float argFloat;
	public string argString;
	
	
	public object[] conditionArgs;

	public int val {
	get {return Get();}
	set {Set(value);}
	}
	
	public int Get(){
		if(executionType == ExecutionType.simple){
			return baseVal;
		} else if(executionType == ExecutionType.scriptable){
			if(intVar !=null){
				return intVar.val;
			} else {
				Debug.LogWarning("Missing ScriptableData, returning baseVal.");
				return baseVal;
			}
		} else { //reference
			try{
				if(argType == ArgumentType.None){
					if(conditionArgs==null || conditionArgs.Length !=0){
						conditionArgs = new object[0];
					}
					return (int)((object)conditionObject).GetType().GetMethod(conditionMethod).Invoke((object)conditionObject, conditionArgs);
					
				} else if(argType == ArgumentType.Bool){
					if(conditionArgs==null || conditionArgs.Length !=1){
						conditionArgs = new object[1];
					}
					conditionArgs[0] = argBool;
					return (int)((object)conditionObject).GetType().GetMethod(conditionMethod).Invoke((object)conditionObject, conditionArgs);
					
				} else if(argType == ArgumentType.Int){
					if(conditionArgs==null || conditionArgs.Length !=1){
						conditionArgs = new object[1];
					}
					conditionArgs[0] = argInt;
					return (int)((object)conditionObject).GetType().GetMethod(conditionMethod).Invoke((object)conditionObject, conditionArgs);
					
				} else if(argType == ArgumentType.Float){
					if(conditionArgs==null || conditionArgs.Length !=1){
						conditionArgs = new object[1];
					}
					conditionArgs[0] = argFloat;
					return (int)((object)conditionObject).GetType().GetMethod(conditionMethod).Invoke((object)conditionObject, conditionArgs);
					
				} else {
					if(conditionArgs==null || conditionArgs.Length !=1){
						conditionArgs = new object[1];
					}
					conditionArgs[0] = argString;
					return (int)((object)conditionObject).GetType().GetMethod(conditionMethod).Invoke((object)conditionObject, conditionArgs);
				}

			}
			catch{
			Debug.LogWarning("Improperly configured reference, returning baseVal");
			return baseVal;
			}
		}
	}
	
	
	public void Set(int newVal){
		if(executionType == ExecutionType.simple){
			baseVal = newVal;
		} else if(executionType == ExecutionType.scriptable){
			if(intVar !=null){
				intVar.val = newVal;
			} else {
				Debug.LogWarning("Missing ScriptableData, assigning to baseVal.");
				baseVal = newVal;
			}
		} else { //reference
			Debug.LogWarning("Cannot assign to a reference-type Akitacore Variable");
		}
	}
}
}

