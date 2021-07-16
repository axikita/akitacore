using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Akitacore{
public class AblSimpleMouseRotate3D : Ability {

	//----Variable Declarations-----------------------------------------------------------------

	//--Declared in base:
	//public GameObject rootObj;
	//public CollisionCheck2D collCheck2D;
	//public CollisionCheck3D collCheck3D;

	//public bool latchAbl = false; 
	//public bool constAbl = false; 
	//public bool axisAbl = false; 
	//public bool usesCollisionCheck2D = false;
	//public bool usesCollisionCheck3D = false;

	public float rotationSpeed = 10;
	float frameInput;
	Vector3 frameRotation;
	public bool invert;

	//----Monobehavior Update Loop--------------------------------------------------------------
	//use awake to inmitialize any values that depend on game state, things that should be initialized on reactivation, or references to other objects.
	
	//----Initialization and Verification--------------------------------------------------------------
	
	//Set up This object
	void Awake(){
		//usesCollisionCheck2D = true; //if uncommented, CollisionCheck2D will be assigned or created
		//usesCollisionCheck3D = true; //if uncommented, CollisionCheck3D will be assigned or created
		
		base.FindReferences(); // finds rootObj and collCheck
		frameRotation = Vector3.zero;
	}
	
	//set up communication with other objects
	void Start(){
		
	}
	
	//reset anything that needs resetting on reactivation
	void OnEnable(){
		
	}
	
	
	//use this to ensure everything is set up correctly. This should be called by the controller in Start.
	public override void Verify(bool conditionsMet, float[] axisInputs){
		int expectedAxisCount =1; //change this if a certain axis count is requred. Remember to still check for null arrays in execute.
		if(axisInputs.Length < expectedAxisCount ){
			Debug.LogError("Expected at least "+expectedAxisCount.ToString());	
		}
		
	}
	


	//--Execute functions---------------------------------------------------------------------


	override public void Execute(bool conditionsMet, float[] axisInputs){
		if(conditionsMet){
			if (invert) {
				frameInput = axisInputs[0] * -1;
			} else {
				frameInput = axisInputs[0];
			}
		} else {
			frameInput = 0.0f;
		}
	}


	void FixedUpdate () {
		frameRotation.Set (0.0f, frameInput * rotationSpeed * Time.fixedDeltaTime, 0.0f);
		rootObj.transform.Rotate (frameRotation);
	}

	//-----Utility Functions-----------------------------------------------------------------

}
}
