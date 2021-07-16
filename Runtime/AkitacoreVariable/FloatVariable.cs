using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using Object = UnityEngine.Object;

namespace Akitacore{
[System.Serializable]
public class FloatVariable{
	public enum ExecutionType{simple, scriptable, reference}
	public ExecutionType executionType;
	
	public float baseVal;
	public FloatVar floatVar;
	public Object conditionObject;
	public string conditionMethod;
	
	public enum ArgumentType{None, Bool, Int, Float, String}
	public ArgumentType argType;
	public bool argBool;
	public int argInt;
	public float argFloat;
	public string argString;
	
	
	public object[] conditionArgs;

	public float val {
	get {return Get();}
	set {Set(value);}
	}
	
	public float Get(){
		if(executionType == ExecutionType.simple){
			return baseVal;
		} else if(executionType == ExecutionType.scriptable){
			if(floatVar !=null){
				return floatVar.val;
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
					return (float)((object)conditionObject).GetType().GetMethod(conditionMethod).Invoke((object)conditionObject, conditionArgs);
					
				} else if(argType == ArgumentType.Bool){
					if(conditionArgs==null || conditionArgs.Length !=1){
						conditionArgs = new object[1];
					}
					conditionArgs[0] = argBool;
					return (float)((object)conditionObject).GetType().GetMethod(conditionMethod).Invoke((object)conditionObject, conditionArgs);
					
				} else if(argType == ArgumentType.Int){
					if(conditionArgs==null || conditionArgs.Length !=1){
						conditionArgs = new object[1];
					}
					conditionArgs[0] = argInt;
					return (float)((object)conditionObject).GetType().GetMethod(conditionMethod).Invoke((object)conditionObject, conditionArgs);
					
				} else if(argType == ArgumentType.Float){
					if(conditionArgs==null || conditionArgs.Length !=1){
						conditionArgs = new object[1];
					}
					conditionArgs[0] = argFloat;
					return (float)((object)conditionObject).GetType().GetMethod(conditionMethod).Invoke((object)conditionObject, conditionArgs);
					
				} else {
					if(conditionArgs==null || conditionArgs.Length !=1){
						conditionArgs = new object[1];
					}
					conditionArgs[0] = argString;
					return (float)((object)conditionObject).GetType().GetMethod(conditionMethod).Invoke((object)conditionObject, conditionArgs);
				}

			}
			catch{
			Debug.LogWarning("Improperly configured reference, returning baseVal");
			return baseVal;
			}
		}
	}
	
	
	public void Set(float newVal){
		if(executionType == ExecutionType.simple){
			baseVal = newVal;
		} else if(executionType == ExecutionType.scriptable){
			if(floatVar !=null){
				floatVar.val = newVal;
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
