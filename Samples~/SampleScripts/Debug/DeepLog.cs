//Deeplog provides logging of a target variable for most steps of the Monobehavior Update loop.
//this is used for debugging variables who's value is critical at various stages in a loop, and which may change in several places.
//this script will need to be edited to be used, see line 19.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using Object = UnityEngine.Object;


namespace Akitacore{
public class DeepLog : MonoBehaviour
{

	
	string variableNameInLog = "trigger_Count";
	
	public bool collisionsAre3D =true;
	
	//--TO USE THIS SCRIPT:-----------------------------------------------------------------------------------------------------------
	//define public references:
	public CollisionCheck2D collCheck;
	
	//define a method here that returns a string:
	string GetValueAsString(){
		return collCheck.triggers.Count.ToString();
	}
	
	
	
	//END EDITABLE PORTION------------------------------------------------------------------------------------------------------------
	//Thats it. Let the rest run.
	
	void Awake(){
		LogTarget("Awake");
	}
	void OnEnable(){
		LogTarget("OnEnable");
	}
	void Reset(){
		LogTarget("Reset");
	}
	void Start()
	{
		CreateColliders(); //-------------------------------
		LogTarget("Start");
	}
	
	void FixedUpdate(){
		Debug.Log("##---------------------------FIXEDUPDATE");
		MatchColliderPositions(); //-------------------------
		LogTarget("FixedUpdate");
	}
	
	void OnTriggerEnter(){
		LogTarget("OnTriggerEnter");
	}
	void OnTriggerStay(){
		LogTarget("OnTriggerStay");
	}
	void OnTriggerExit(){
		LogTarget("OnTriggerExit");
	}
	
	void OnTriggerEnter2D(){
		LogTarget("OnTriggerEnter2D");
	}
	void OnTriggerStay2D(){
		LogTarget("OnTriggerStay2D");
	}
	void OnTriggerExit2D(){
		LogTarget("OnTriggerExit2D");
	}
	
	void OnCollisionEnter(){
		LogTarget("OnCollisionEnter");
	}
	void OnCollisionStay(){
		LogTarget("OnCollisionStay");
	}
	void OnCollisionExit(){
		LogTarget("OnCollisionExit");
	}
	
	void OnCollisionEnter2D(){
		LogTarget("OnCollisionEnter2D");
	}
	void OnCollisionStay2D(){
		LogTarget("OnCollisionStay2D");
	}
	void OnCollisionExit2D(){
		LogTarget("OnCollisionExit2D");
	}
	
	void Update()
	{
		Debug.Log("##---------------------------UPDATE");
		LogTarget("Update");
	}
	void LateUpdate(){
		LogTarget("LateUpdate");
	}
	
	void OnDrawGizmos(){
		//LogTarget("OnDrawGizmos");
	}
	
	void OnGUI(){
		LogTarget("OnGUI");
	}
	
	void OnDisable(){
		LogTarget("OnDisable");
	}
	void OnDestroy(){
		LogTarget("OnDestroy");
	}
	
	
	
	void LogTarget(string phase){
		string value;
		
		value = GetValueAsString();
		
		Debug.Log(string.Format("##--DEEPLOG: In {0} {1} is {2}", phase, variableNameInLog, value));
	}
	
	
	
	private GameObject colliderObj;
	private GameObject triggerObj;
	
	private Vector3 farAway;
	
	void CreateColliders(){
		
		farAway = new Vector3(1000000.0f,1000000.0f,1000000.0f);
		
		colliderObj = new GameObject();
		colliderObj.name = "Deeplogger Collider";
		triggerObj = new GameObject();
		triggerObj.name = "Deeplogger Trigger";
		
		if(collisionsAre3D){
			gameObject.AddComponent<BoxCollider>();
			var rb = gameObject.AddComponent<Rigidbody>();
			rb.isKinematic = false;
			rb.useGravity = false;
			rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
		
			var otherTrigger = triggerObj.AddComponent<BoxCollider>();
			var otherCollider = colliderObj.AddComponent<BoxCollider>();
			otherTrigger.isTrigger = true;

		} else {
			gameObject.AddComponent<BoxCollider2D>();
			var rb = gameObject.AddComponent<Rigidbody2D>();
			rb.isKinematic = false;
			rb.gravityScale = 0.0f;
			rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
		
			var otherTrigger = triggerObj.AddComponent<BoxCollider2D>();
			var otherCollider = colliderObj.AddComponent<BoxCollider2D>();
			otherTrigger.isTrigger = true;
		}
		

	}
	

	
	void MatchColliderPositions(){
		if(gameObject.GetComponent<Rigidbody>()!=null){
			gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
		}
		if(gameObject.GetComponent<Rigidbody2D>()!=null){
			gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
		}
		gameObject.transform.position = farAway;
		colliderObj.transform.position = farAway;
		triggerObj.transform.position = farAway;
	}
}
}
