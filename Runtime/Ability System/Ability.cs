//this is the template from which abilities are derived, which provides the standard interface.
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Akitacore{
public class Ability : MonoBehaviour {

	[Tooltip("Represents the entity doing the ability.")]
	public GameObject rootObj;
	[Tooltip("2D Collider information about the entity doing the ability.")]
	public CollisionCheck2D collCheck2D;
	[Tooltip("3D Collider information about the entity doing the ability.")]
	public CollisionCheck3D collCheck3D;
	

	[System.NonSerialized]
	public bool usesCollisionCheck2D = false;
	[System.NonSerialized]
	public bool usesCollisionCheck3D = false;
	
	[System.NonSerialized]
	public bool called;
	[System.NonSerialized]
	public bool latched;
	
	

	public virtual void FindReferences(){
		GetRootObj (gameObject.transform);
		GetCollisionCheck ();
	}
	
	//call after initializations to check everything is set up right.
	public virtual void Verify(bool conditionsMet, float[] axisInputs){
		return;
	}
	
	///Called by the controller in the update step before Execute. Use to reset input to avoid continued execution if not called.
	public virtual void PreUpdate(){
		return;
	}
	
	///Called during Update. Used to assign input to 
	public virtual void Execute(bool conditionsMet, float[] axisInputs){
		Debug.Log("unassigned Execute(bool buttonInput, float[] axisInputs) on " + gameObject.name);
	}
	
	///Called by the controller in the update step before Execute. Use to respond to a non-called execute, eg, releasing a latch.
	public virtual void PostUpdate(){
		return;
	}

	//fetches the closest parent with a rigidbody
	public void GetRootObj(Transform trans){
		if (rootObj != null) {
			return;
		}

		if (trans.gameObject.GetComponent<Rigidbody2D> () != null) {
			rootObj = trans.gameObject;
			return;
		}
		if (trans.gameObject.GetComponent<Rigidbody> () != null) {
			rootObj = trans.gameObject;
			return;
		}

		if (trans.parent != null) {
			GetRootObj (trans.parent);
		} else {
			Debug.Log ("No parent object on "+gameObject.name+" contains a Rigidbody or Rigidbody 2D, root object cannot be assigned.");
		}
	}
	
	public void GetCollisionCheck(){
		if (usesCollisionCheck2D && collCheck2D == null) {
			if (rootObj != null) {
				collCheck2D = CollisionCheck2D.GetAsSingleton (rootObj);
			} else {
				Debug.LogWarning ("root object and collission check are both null and so collission check cannot be initialized.");
			}
		}

		if (usesCollisionCheck3D && collCheck3D == null) {
			if (rootObj != null) {
				collCheck3D = CollisionCheck3D.GetAsSingleton (rootObj);
			} else {
				Debug.LogWarning ("root object and collission check are both null and so collission check cannot be initialized.");
			}
		}
	}
	
}
}
