//This script performs collission checking and saves collission information, so that it is centralized and can easily be referred to by ability scripts.

//THIS NEEDS TO BE LATE IN THE SCRIPT EXECUTION ORDER in order to ensure proper reporting on collission entry.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Akitacore{
public class CollisionCheck3D : MonoBehaviour {

	public List<GameObject> colliders;
	public List<GameObject> triggers;
	List<GameObject> frameTriggers;

	public List<Vector3> collNormals;
	List<Vector3> frameNormals;

	public bool onGround;
	bool groundFound; //latch for loop behavior

	public bool rightHit;
	public bool topHit;
	public bool leftHit;
	public bool bottomHit;
	public bool frontHit;
	public bool backHit;
	bool rightFound; //latch for loop behavior
	bool topFound; //latch for loop behavior
	bool leftFound; //latch for loop behavior
	bool bottomFound; //latch for loop behavior
	bool frontFound;
	bool backFound;

	bool colliding; //latch for loop behavior
	bool triggering; //latch for loop behavior

	public float GroundAngleThreshold =1.0f;

	float angleToX;
	float angleToY;
	float angleToZ;
	


	//--Monobehavior Loop-----------------------------------------------------------------------------

	void Start(){
		//initialize lists
		colliders = new List<GameObject> ();
		triggers = new List<GameObject> ();
		frameTriggers = new List<GameObject> ();

		collNormals = new List<Vector3> ();
		frameNormals = new List<Vector3> ();
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
			groundFound = false;
			rightFound = false;
			topFound = false;
			leftFound = false;
			bottomFound = false;
			frontFound = false;
			backFound = false;
			collNormals.Clear();
			foreach(var norm in frameNormals){
				collNormals.Add(norm);	
			}
			frameNormals.Clear ();

		} else { //set reporters
			onGround = false;
			rightHit = false;
			topHit = false;
			leftHit = false;
			bottomHit = false;
			frontHit = false;
			backHit = false;
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
	void OnCollisionEnter(Collision coll){
		colliding = true;
		GroundCheck (coll);
		SurroundingsCheck (coll);
		if (!colliders.Contains (coll.collider.gameObject)) {
			colliders.Add (coll.collider.gameObject);
		}
	}
	void OnCollisionStay(Collision coll){
		colliding = true;
		GroundCheck (coll);
		SurroundingsCheck (coll);
		if (!colliders.Contains (coll.collider.gameObject)) {
			colliders.Add (coll.collider.gameObject);
		}
	}
	void OnCollisionExit(Collision coll){
		colliding = true;
		GroundCheck (coll);
		SurroundingsCheck (coll);
		if (!colliders.Contains (coll.collider.gameObject)) {
			colliders.Add (coll.collider.gameObject);
		}
	}




	//--Trigger Events-----------------------------------------------------------------------------------
	void OnTriggerEnter(Collider other){
		triggering = true;
		if (!frameTriggers.Contains (other.gameObject)) {
			frameTriggers.Add (other.gameObject);
		}
	}
	void OnTriggerStay(Collider other){
		triggering = true;
		if (!frameTriggers.Contains (other.gameObject)) {
			frameTriggers.Add (other.gameObject);
		}
	}
	void OnTriggerExit(Collider other){
		triggering = true;
		if (!frameTriggers.Contains (other.gameObject)) {
			frameTriggers.Add (other.gameObject);
		}
	}


	//--Utilities----------------------------------------------------------------------------------------

	//report ground contact
	void GroundCheck(Collision coll){
		//groundFound = false; //set in fixedupdate
		foreach (ContactPoint contact in coll.contacts) {
			if (Vector3.Angle (contact.normal, Vector3.up) < GroundAngleThreshold) {
				groundFound = true;
			}
		}
		onGround = groundFound;
	}

	//report sides hit.
	//TODO: make sure that this is in local coordinates. It might be necessary to multiply contact.normal with transform.rotation.
	void SurroundingsCheck(Collision coll){
		//float angleToX;
		//float angleToY;
		//float angleToZ;

		foreach (ContactPoint contact in coll.contacts) {

			Vector3 adjustedNormal = Quaternion.Inverse(transform.rotation) * contact.normal;

			//measure contact angles
			if (adjustedNormal.x > 0) {
				angleToX = Vector3.Angle (adjustedNormal, Vector3.right);
			} else {
				angleToX = Vector3.Angle (adjustedNormal, Vector3.left);
			}

			if (adjustedNormal.y > 0) {
				angleToY = Vector3.Angle (adjustedNormal, Vector3.up);
			} else {
				angleToY = Vector3.Angle (adjustedNormal, Vector3.down);
			}

			if (adjustedNormal.z > 0) {
				angleToZ = Vector3.Angle (adjustedNormal, Vector3.forward);
			} else {
				angleToZ = Vector3.Angle (adjustedNormal, Vector3.back);
			}

			//compare values
			if (adjustedNormal.x < 0 && angleToX < angleToY && angleToX <angleToZ) {
				rightFound = true;
			}
			if (adjustedNormal.x > 0 && angleToX < angleToY && angleToX <angleToZ) {
				leftFound = true;
			}

			if (adjustedNormal.y < 0 && angleToY < angleToX && angleToY <angleToZ) {
				topFound = true;
			}
			if (adjustedNormal.y > 0 && angleToY < angleToX && angleToY < angleToZ) {
				bottomFound = true;
			}

			if (adjustedNormal.z < 0 && angleToZ < angleToX && angleToZ <angleToY) {
				frontFound = true;
			}
			if (adjustedNormal.z > 0 && angleToZ < angleToX && angleToZ <angleToY) {
				backFound = true;
			}

			if (!frameNormals.Contains (adjustedNormal)) {
				frameNormals.Add (adjustedNormal);
			}
		}


		rightHit = rightFound;
		topHit = topFound;
		leftHit = leftFound;
		bottomHit = bottomFound;
		frontHit = frontFound;
		backHit = backFound;

		//collNormals = frameNormals; //moved to the start of fixedupdate
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
	
	//used for utilities like jumping to prevent doublejumps between detection passes.
	public void BreakGroundContact(){
		onGround = false;
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
	public bool GetFrontHit(){
		return frontHit;
	}
	public bool GetBackHit(){
		return backHit;
	}



	//--Static Members-----------------------------------------------------------------------------------
	public static CollisionCheck3D GetAsSingleton(GameObject obj){
		CollisionCheck3D colCheck;
		if ((colCheck = obj.GetComponent<CollisionCheck3D> ()) != null) {
			return colCheck;
		} else {
			colCheck = obj.AddComponent<CollisionCheck3D> ();
			return colCheck;
		}
	}
}
}
