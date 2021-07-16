using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Akitacore{
public class AblMove : Ability{

	//----Variable Declarations-----------------------------------------------------------------
	public float maxSpeedX = 4.0f;
	public float maxSpeedY = 4.0f;
	public float inputDampThreshold = 0.1f;
	Vector3 frameMovement;
	public GameObject flipRoot; //root object of colliders to rotate with sprite motion.

	//----Initialization and Verification--------------------------------------------------------------
	
	//Set up This object
	void Awake(){
		//usesCollisionCheck2D = true; //if uncommented, CollisionCheck2D will be assigned or created
		//usesCollisionCheck3D = true; //if uncommented, CollisionCheck3D will be assigned or created
		
		base.FindReferences(); // finds rootObj and collCheck
		
		frameMovement = Vector3.zero;

		
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
		if(conditionsMet){
			frameMovement.Set (axisInputs[0] * maxSpeedX * Time.fixedDeltaTime, axisInputs[1] * maxSpeedY * Time.fixedDeltaTime, 0.0f);
		} 
	}
	
	override public void PostUpdate(){
		latched = called;
		if(!called){
			frameMovement = Vector3.zero;
		}
	}

	//--Monobehavior Updates--------------------------------------------

	void FixedUpdate() {
		if (frameMovement.magnitude / ((maxSpeedX+maxSpeedY*.05f) * Time.fixedDeltaTime) < inputDampThreshold) {
			return;
		}

		rootObj.transform.Translate (frameMovement,Space.World);

		if (frameMovement.x*frameMovement.x >0.001f) {
			if (frameMovement.x >= 0.0f) {
				//Debug.Log ("pos");
				flipRoot.transform.localScale.Set (1.0f, 1.0f, 1.0f);
				flipRoot.transform.localScale= new Vector3 (1.0f, 1.0f, 1.0f);
			} else {
				flipRoot.transform.localScale.Set (-1.0f, 1.0f, 1.0f);
				flipRoot.transform.localScale= new Vector3 (-1.0f, 1.0f, 1.0f);
				//Debug.Log ("neg");
			}
		} 
	}

	//-----Utility Functions-----------------------------------------------------------------

}
}
