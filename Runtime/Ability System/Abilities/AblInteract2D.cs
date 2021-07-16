//trigger an Interactable on button press.
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Akitacore{
public class AblInteract2D : Ability {
	//----Variable Declarations-----------------------------------------------------------------


	//----Monobehavior Update Loop--------------------------------------------------------------
	void Awake(){
		usesCollisionCheck2D = true; //if uncommented, CollisionCheck2D will be assigned or created
		//usesCollisionCheck3D = true; //if uncommented, CollisionCheck3D will be assigned or created
		
		base.FindReferences(); // finds rootObj and collCheck
		
	}
	
	
	//--Execute---------------------------------------------------------------------
	
	//called by controller for each ability before Execute in the update step.
	//An ability that has PreUpdate called but not Execute should do nothing.
	override public void PreUpdate(){
		called = false;
	}

	//this is called from controllers or wrapper abilities during Update. Input values are passed as arguments.
	//Behavior should only occur if conditionsmet == true. 
	override public void Execute(bool conditionsMet, float[] axisInputs){
		called = true;
		//Debug.Log ("called");
		if(!latched){
			//Debug.Log("past latch");
			if(conditionsMet){
				//Debug.Log ("running");
				Interactable[] interactables;
				GameObject[] colliding = new GameObject[collCheck2D.triggers.Count];

				for (int i = 0; i < collCheck2D.triggers.Count; i++) {
					colliding [i] = collCheck2D.triggers [i];
				}

				//sort the array by root distance so that the closest interactable is prioritized.
				//this sorts in 3d, prolly should change to a 2d sort, but if z~=y it shouldn't be a huge issue
				Array.Sort (colliding, delegate(GameObject obj1, GameObject obj2) {
					return (rootObj.transform.position - obj1.transform.position).magnitude.CompareTo ((rootObj.transform.position - obj2.transform.position).magnitude);
				});

				 //log distances
				
				foreach (GameObject obj in colliding) {
					//Debug.Log (gameObject.transform.position.ToString () + " " + obj.transform.position.ToString ());
					//Debug.Log (obj.name + " "+(Mathf.Sqrt( Mathf.Pow((obj.transform.position.x - rootObj.transform.position.x),2) + Mathf.Pow((obj.transform.position.y - rootObj.transform.position.y),2)).ToString()));
				}
			

				//Debug.Log (collCheck2D.triggers.Count);
				foreach (GameObject obj in colliding) {

					//Debug.Log ("testing " + obj.name);
					if ((interactables = obj.GetComponents<Interactable> ()) != null) {
						foreach (var interactable in interactables) {
							interactable.Interact ();
						}
						//return;
					}
				}
			}
		}
	}
	
	override public void PostUpdate(){
		latched = called;
	}




}
}
