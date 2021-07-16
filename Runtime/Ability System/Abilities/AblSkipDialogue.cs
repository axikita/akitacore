using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Akitacore{
public class AblSkipDialogue : Ability {

	//----Variable Declarations-----------------------------------------------------------------
	//--Declared in base:
	//public GameObject rootObj;
	//public CollisionCheck2D collCheck2D;
	//public CollisionCheck3D collCheck3D;

	//public bool usesCollisionCheck2D = false; 
	//public bool usesCollisionCheck3D = false; 
	
	//public bool called; // latch utility
	//public bool latched; // latch utility
	
	
	bool inputThisFrame;
	CoreDialogueUI dialogue;
	float textSpeedSave;


	//----Initialization and Verification--------------------------------------------------------------
	
	//Set up This object
	void Awake(){
		//usesCollisionCheck2D = true; //if uncommented, CollisionCheck2D will be assigned or created
		//usesCollisionCheck3D = true; //if uncommented, CollisionCheck3D will be assigned or created
		
		base.FindReferences(); // finds rootObj and collCheck
		dialogue = GameObject.FindObjectOfType<CoreDialogueUI> ();
		
	}
	
	//set up communication with other objects
	void Start(){
		textSpeedSave = dialogue.textSpeed;
	}
	
	//reset anything that needs resetting on reactivation
	void OnEnable(){
		
	}
	
	
	//use this to ensure everything is set up correctly. This should be called by the controller in Start.
	public override void Verify(bool conditionsMet, float[] axisInputs){
		int expectedAxisCount =0; //change this if a certain axis count is requred. Remember to still check for null arrays in execute.
		if(axisInputs.Length < expectedAxisCount ){
			Debug.LogError("Expected at least "+expectedAxisCount.ToString());	
		}
		
	}

	//--Execute---------------------------------------------------------------------
	
	//called by controller for each ability before Execute in the update step.
	//An ability that has PreUpdate called but not Execute should do nothing.
	override public void PreUpdate(){
		called = false;
		inputThisFrame = false;
	}

	//this is called from controllers or wrapper abilities during Update. Input values are passed as arguments.
	//Behavior should only occur if conditionsmet == true. 
	override public void Execute(bool conditionsMet, float[] axisInputs){
		called = true;
		inputThisFrame = conditionsMet;
	}
	
	//--Monobehavior Updates--------------------------------------------
	
	void Update () {
		if (inputThisFrame) {
			if (dialogue.textSpeed > 0) {
				textSpeedSave = dialogue.textSpeed;
			}
			dialogue.textSpeed = -1.0f;
		} else {
			dialogue.textSpeed = textSpeedSave;
		}
		inputThisFrame = false;
	}

	//-----Utility Functions-----------------------------------------------------------------

}
}

