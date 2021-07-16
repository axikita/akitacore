//basic top down motion with no smoothing or acceleration. Orthogonal rotation snapping. 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Akitacore{
public class AblSimpleMove : Ability {

	//----Variable Declarations-----------------------------------------------------------------
	//--Declared in base:
	//public GameObject rootObj;
	//public CollisionCheck2D collCheck2D;
	//public CollisionCheck3D collCheck3D;

	//public bool usesCollisionCheck2D = false; 
	//public bool usesCollisionCheck3D = false; 
	
	//public bool called; // latch utility
	//public bool latched; // latch utility
	
	public float maxSpeedX = 4.0f;
	public float maxSpeedY = 4.0f;
	public float inputDampThreshold = 0.1f;
	Vector3 frameMovement;
	public GameObject rotateRoot; //root object of colliders to rotate with sprite motion.


//----Initialization and Verification--------------------------------------------------------------
	
	//Set up This object
	void Awake(){
		//usesCollisionCheck2D = true; //if uncommented, CollisionCheck2D will be assigned or created
		//usesCollisionCheck3D = true; //if uncommented, CollisionCheck3D will be assigned or created
		
		base.FindReferences(); // finds rootObj and collCheck
		frameMovement = Vector3.zero;
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

	//--Execute---------------------------------------------------------------------
	override public void PreUpdate(){
		frameMovement.Set(0.0f,0.0f,0.0f);
	}
	
	override public void Execute(bool conditionsMet, float[] axisInputs){
		if(conditionsMet && axisInputs !=null && axisInputs.Length>1){
			frameMovement.Set (axisInputs[0] * maxSpeedX * Time.fixedDeltaTime, axisInputs[1] * maxSpeedY * Time.fixedDeltaTime, 0.0f);
		} 

	}
	
	void FixedUpdate() {
		if (frameMovement.magnitude / ((maxSpeedX+maxSpeedY*.05f) * Time.fixedDeltaTime) < inputDampThreshold) {
			return;
		}

		rootObj.transform.Translate (frameMovement,Space.World);

		if (frameMovement.x*frameMovement.x> frameMovement.y*frameMovement.y) {
			if (frameMovement.x >= 0.0f) {
				rotateRoot.transform.rotation = Quaternion.AngleAxis (0.0f, Vector3.forward);
			} else {
				rotateRoot.transform.rotation = Quaternion.AngleAxis (180.0f, Vector3.forward);
			}
		} else {
			if (frameMovement.y >= 0.0f) {
				rotateRoot.transform.rotation = Quaternion.AngleAxis (90.0f, Vector3.forward);
			} else {
				rotateRoot.transform.rotation = Quaternion.AngleAxis (270.0f, Vector3.forward);
			}
		}
	}

	//-----Utility Functions-----------------------------------------------------------------

}
}
