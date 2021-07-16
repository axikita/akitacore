//simple 3d motion controller. uses wasd navigation relative to the root orientation.
//intended for use with a locked camera parented behind the character.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Akitacore{
public class AblSimpleMove3D : Ability {

	//----Variable Declarations-----------------------------------------------------------------

	//--Declared in base:
	//public GameObject rootObj;
	//public CollisionCheck2D collCheck2D;
	//public CollisionCheck3D collCheck3D;

	//public bool usesCollisionCheck2D = false; 
	//public bool usesCollisionCheck3D = false; 
	
	//public bool called; // latch utility
	//public bool latched; // latch utility

	Vector3 inputDirection;
	public float speed = 1.0f;


	//----Initialization and Verification--------------------------------------------------------------
	
	//Set up This object
	void Awake(){
		//usesCollisionCheck2D = true; //if uncommented, CollisionCheck2D will be assigned or created
		usesCollisionCheck3D = true; //if uncommented, CollisionCheck3D will be assigned or created
		
		base.FindReferences(); // finds rootObj and collCheck
		
	}
	
	//set up communication with other objects
	void Start(){
		
	}
	
	//reset anything that needs resetting on reactivation
	void OnEnable(){
		
	}
	

	
	//use this to ensure everything is set up correctly. This should be called by the controller in Start.
	public override void Verify(bool conditionsMet, float[] axisInputs){
		int expectedAxisCount =2; //change this if a certain axis count is requred. Remember to still check for null arrays in execute.
		if(axisInputs.Length < expectedAxisCount ){
			Debug.LogError("Expected at least "+expectedAxisCount.ToString());	
		}
		
	}
	


	//--Execute functions---------------------------------------------------------------------

	override public void PreUpdate(){
		inputDirection = Vector3.zero;
	}

	override public void Execute(bool conditionsMet, float[] axisInputs){
		if(conditionsMet && axisInputs !=null && axisInputs.Length>1){
			inputDirection.Set (axisInputs[0], 0.0f, axisInputs[1]);
		}
	}

	void FixedUpdate () {
		if (inputDirection.magnitude > 0.05f){
			rootObj.transform.Translate (inputDirection * speed * Time.fixedDeltaTime);
		}
	}

	//-----Utility Functions-----------------------------------------------------------------

}
}