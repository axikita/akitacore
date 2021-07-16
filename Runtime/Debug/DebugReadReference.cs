using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

namespace Akitacore{
    [System.Serializable]
    public class InspectorBool{
        public BoolVar boolVar;
        public UnityEngine.Component conditionObject;
        public string conditionMethod;
        object[] conditionParameters;
        
        public bool val {
            get {return Get();}
        }
        public bool Get(){
            if(boolVar!=null){
                return boolVar.val;
            }
            try{
                return (bool)((object)conditionObject).GetType().GetMethod(conditionMethod).Invoke((object)conditionObject, conditionParameters);
            }
            catch{
                Debug.LogWarning("No ScriptableData assigned, and the conditionMethod or conditionObjectbject are improperly specified.");
                return false;
            }
        
        }
    }
    
    [System.Serializable]
    public class InspectorInt{
        public IntVar intVar;
        public UnityEngine.Component conditionObject;
        public string conditionMethod;
        object[] conditionParameters;
        
        public int val {
            get {return Get();}
        }
        public int Get(){
            if(intVar!=null){
                return intVar.val;
            }
            try{
                return (int)((object)conditionObject).GetType().GetMethod(conditionMethod).Invoke((object)conditionObject, conditionParameters);
            }
            catch {
                Debug.LogWarning("No ScriptableData assigned, and the conditionMethod or conditionObjectbject are improperly specified.");
                return 0;
            }
        }
    }
    
    [System.Serializable]
    public class InspectorFloat{
        public FloatVar floatVar;
        public UnityEngine.Component conditionObject;
        public string conditionMethod;
        object[] conditionParameters;
        
        public float val {
            get {return Get();}
        }
        public float Get(){
            if(floatVar!=null){
                return floatVar.val;
            }
            try{
                return (float)((object)conditionObject).GetType().GetMethod(conditionMethod).Invoke((object)conditionObject, conditionParameters);
            }
            catch{
                Debug.LogWarning("No ScriptableData assigned, and the conditionMethod or conditionObjectbject are improperly specified.");
                return 0.0f;
            }
        }
    }
    
    [System.Serializable]
    public class InspectorString{
        public StringVar stringVar;
        public UnityEngine.Component conditionObject;
        public string conditionMethod;
        object[] conditionParameters;
        
        public string val {
            get {return Get();}
        }
        public string Get(){
            if(stringVar!=null){
                return stringVar.val;
            }
            try{
                return (string)((object)conditionObject).GetType().GetMethod(conditionMethod).Invoke((object)conditionObject, conditionParameters);
            }
            catch{
                Debug.LogWarning("No ScriptableData assigned, and the conditionMethod or conditionObjectbject are improperly specified.");
                return "";
            }
        }
    }

}

