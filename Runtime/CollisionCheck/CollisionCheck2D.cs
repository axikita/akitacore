//This script performs collission checking and saves collission information, so that it is centralized and can easily be referred to by ability scripts.

//THIS NEEDS TO BE LATE IN THE SCRIPT EXECUTION ORDER in order to ensure proper reporting on collission entry.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Akitacore{
public class CollisionCheck2D : MonoBehaviour {

	public List<GameObject> colliders;
	public List<GameObject> triggers;
	List<GameObject> frameTriggers;

	public List<Vector2> collNormals;
	public List<Vector2> frameNormals;

	public bool onGround;
	bool groundFound; //latch for loop behavior

	public bool rightHit;
	public bool topHit;
	public bool leftHit;
	public bool bottomHit;
	bool rightFound; //latch for loop behavior
	bool topFound; //latch for loop behavior
	bool leftFound; //latch for loop behavior
	bool bottomFound; //latch for loop behavior

	bool colliding; //latch for loop behavior
	bool triggering; //latch for loop behavior

	public float GroundAngleThreshold =1.0f;
	



	//--Monobehavior Loop-----------------------------------------------------------------------------

	void Start(){
		//initialize lists
		colliders = new List<GameObject> ();
		triggers = new List<GameObject> ();
		frameTriggers = new List<GameObject> ();

		collNormals = new List<Vector2> ();
		frameNormals = new List<Vector2> ();
		
	}

	void OnDisable(){
		//clear lists, so that they don't report false contact upon reenable.
		if (colliders != null) {
			colliders.Clear ();
		}
		if (triggers != null) {
			triggers.Clear ();
		}
		if (collNormals != null) {
			collNormals.Clear ();
		}

	}

	void FixedUpdate(){
		//FixedUpdate is used to reset latch values before the collision round.
		//Reporter values will be changed iff there was no collision on the previous frame.
		//CollissionCheck2D should be late in the script execution order, to prevent unreliable reporting on collision entry.
		//specifically, collission flags need to be set before any actions access the flags in their own FixedUpdate step.

		//Collision--------------
		if (colliding) { //set flags
			Debug.Log("Colliding Reset on "+gameObject.name);
			groundFound = false;
			rightFound = false;
			topFound = false;
			leftFound = false;
			bottomFound = false;
			collNormals.Clear();
			foreach(var norm in frameNormals){
				collNormals.Add(norm);	
			}
			frameNormals.Clear ();

		} else { //set reporters
			Debug.Log("Non-Colliding Reset on "+gameObject.name);
			onGround = false;
			rightHit = false;
			topHit = false;
			leftHit = false;
			bottomHit = false;
			frameNormals.Clear ();
			collNormals.Clear ();
			colliders.Clear ();
				
		}
		colliding = false;

		//Trigger----------------
		if (triggering) {
			triggers = frameTriggers;
			frameTriggers.Clear ();

		} else {
			triggers.Clear ();
			frameTriggers.Clear ();
		}
		triggering = false;



	}

	//--Collision Events------------------------------------------------------------------------------
	void OnCollisionEnter2D(Collision2D coll){
		Debug.Log("collcheckOnEnter");
		colliding = true;
		GroundCheck (coll);
		SurroundingsCheck (coll);
		if (!colliders.Contains (coll.collider.gameObject)) {
			colliders.Add (coll.collider.gameObject);
		}

	}
	void OnCollisionStay2D(Collision2D coll){
		Debug.Log("collcheckOnStay------------------------------------------------****");
		colliding = true;
		GroundCheck (coll);
		SurroundingsCheck (coll);
		if (!colliders.Contains (coll.collider.gameObject)) {
			colliders.Add (coll.collider.gameObject);
		}

	}
	void OnCollisionExit2D(Collision2D coll){
		Debug.Log("collcheckOnexit");
		colliding = true;
		GroundCheck (coll);
		SurroundingsCheck (coll);
		if (!colliders.Contains (coll.collider.gameObject)) {
			colliders.Add (coll.collider.gameObject);
		}

	}




	//--Trigger Events-----------------------------------------------------------------------------------
	void OnTriggerEnter2D(Collider2D other){
		triggering = true;
		if (!frameTriggers.Contains (other.gameObject)) {
			frameTriggers.Add (other.gameObject);
		}
	}
	void OnTriggerStay2D(Collider2D other){
		triggering = true;
		if (!frameTriggers.Contains (other.gameObject)) {
			frameTriggers.Add (other.gameObject);
		}
	}
	void OnTriggerExit2D(Collider2D other){
		triggering = true;
		if (!frameTriggers.Contains (other.gameObject)) {
			frameTriggers.Add (other.gameObject);
		}
	}


	//--Utilities----------------------------------------------------------------------------------------

	//report ground contact
	void GroundCheck(Collision2D coll){
		//groundFound = false; //set in fixedupdate
		foreach (ContactPoint2D contact in coll.contacts) {
			if (Vector2.Angle (contact.normal, Vector2.up) < GroundAngleThreshold) {
				groundFound = true;
			}
		}
		onGround = groundFound;
	}

	//report sides hit
	void SurroundingsCheck(Collision2D coll){
		

		foreach (ContactPoint2D contact in coll.contacts) {
			if (Vector2.Angle (contact.normal, Vector2.left) < 45.0f) {
				rightFound = true;
			}
			if (Vector2.Angle (contact.normal, Vector2.down) < 45.0f) {
				topFound = true;
			}
			if (Vector2.Angle (contact.normal, Vector2.right) < 45.0f) {
				leftFound = true;
			}
			if (Vector2.Angle (contact.normal, Vector2.up) < 45.0f) {
				bottomFound = true;
			}

			if (!frameNormals.Contains (contact.normal)) {
				frameNormals.Add (contact.normal);
			}
		}
			

		rightHit = rightFound;
		topHit = topFound;
		leftHit = leftFound;
		bottomHit = bottomFound;

		//collNormals = frameNormals;
	}



	//--Public Utilities---------------------------------------------------------------------------------
	public bool CompareTriggerTag(string tag){
		foreach (GameObject obj in triggers) {
			if (gameObject.CompareTag (tag)) {
				return true;
			}
		}
		return false;
	}

	//--Accessors----------------------------------------------------------------------------------------
	//these public values can be accessed directly, but functions are provided so they can be accessed as a  SerializableCallback<bool>

	public bool GetOnGround(){
		return onGround;
	}
	public bool GetRightHit(){
		return rightHit;
	}
	public bool GetTopHit(){
		return topHit;
	}
	public bool GetLeftHit(){
		return leftHit;
	}
	public bool GetBottomHit(){
		return bottomHit;
	}

		

	//--Static Members-----------------------------------------------------------------------------------
	public static CollisionCheck2D GetAsSingleton(GameObject obj){
		CollisionCheck2D colCheck;
		if ((colCheck = obj.GetComponent<CollisionCheck2D> ()) != null) {
			return colCheck;
		} else {
			colCheck = obj.AddComponent<CollisionCheck2D> ();
			return colCheck;
		}
	}
}
}
