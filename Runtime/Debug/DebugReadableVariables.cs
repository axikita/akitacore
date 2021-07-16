using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Akitacore;

namespace Akitacore{
public class DebugReadableVariables : MonoBehaviour
{
    
    public bool myBool;
    public int myInt;
    public float myFloat;
    public string myString;
    
    
    public bool GetBool(){
        return myBool;
    }
    
    public int GetInt(){
        return myInt;   
    }
    
    public float GetFloat(){
        return myFloat;
    }
    public string GetString(){
        return myString;
    }
    
	public bool GetBoolWithArgBool(bool test){
		Debug.Log("Argument: "+test.ToString());
        return (myBool||test);
    }
	public bool GetBoolWithArgInt(int test){
		Debug.Log("Argument: "+test.ToString());
        return (test>0);
    }
	public bool GetBoolWithArgFloat(float test){
		Debug.Log("Argument: "+test.ToString());
        return (test>0);
    }
	public bool GetBoolWithArgString(string test){
		Debug.Log("Argument: "+test.ToString());
        return (test.Length>1);
    }
	public int GetIntWithArg(int test){
		Debug.Log("Argument: "+test.ToString());
        return (myInt+test);
    }
	public float GetFloatWithArg(float test){
		Debug.Log("Argument: "+test.ToString());
        return (myFloat+test);
    }
	public string GetStringWithArg(string test){
		Debug.Log("Argument: "+test.ToString());
        return (myString+test);
    }
	public bool GetBoolWithBadArg(Ability test){
		Debug.Log("Argument: "+test.ToString());
        return (myBool);
    }
	public bool GetBoolTooManyArgs(bool test1, bool test2){
		Debug.Log("Argument: "+test1.ToString());
		return test1&&test2;
	}

}
}
