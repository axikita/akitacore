//This doesn't do anything, but you can copy it as a base format for writing new ability scripts.
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Akitacore{
public class AblWrapperTemplate : Ability {

	//----Variable Declarations-----------------------------------------------------------------
	
	public Ability wrappedAbl;

	//--Declared in base:
	//public GameObject rootObj;
	//public CollisionCheck2D collCheck2D;
	//public CollisionCheck3D collCheck3D;

	//public bool usesCollisionCheck2D = false; 
	//public bool usesCollisionCheck3D = false; 
	
	//public bool called; // latch utility
	//public bool latched; // latch utility


	//----Initialization and Verification--------------------------------------------------------------
	
	//Set up This object
	void Awake(){
		//usesCollisionCheck2D = true; //if uncommented, CollisionCheck2D will be assigned or created
		//usesCollisionCheck3D = true; //if uncommented, CollisionCheck3D will be assigned or created
		
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
		int expectedAxisCount =0; //change this if a certain axis count is requred. Remember to still check for null arrays in execute.
		if(axisInputs.Length < expectedAxisCount ){
			Debug.LogError("Expected at least "+expectedAxisCount.ToString());	
		}
		wrappedAbl.Verify(conditionsMet, axisInputs);
	}

	//--Execute---------------------------------------------------------------------
	
	//called by controller for each ability before Execute in the update step.
	//An ability that has PreUpdate called but not Execute should do nothing.
	override public void PreUpdate(){
		called = false;
		wrappedAbl.PreUpdate();
	}

	//this is called from controllers or wrapper abilities during Update. Input values are passed as arguments.
	//Behavior should only occur if conditionsmet == true. 
	override public void Execute(bool conditionsMet, float[] axisInputs){
		called = true;
		if(!latched){
			//do stuff for a latch ABL.
		}
		
		//this will very likely get changed depending on the wrapper
		wrappedAbl.Execute(conditionsMet,axisInputs);
	}
	
	//called by controller for each ability after Execute in the update step.
	override public void PostUpdate(){
		latched = called;
		wrappedAbl.PostUpdate();
	}

	//--Monobehavior Updates--------------------------------------------

	void Update () {

	}
	
	void FixedUpdate () {

	}

	//-----Utility Functions-----------------------------------------------------------------

}
}

