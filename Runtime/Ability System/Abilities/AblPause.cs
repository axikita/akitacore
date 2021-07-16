//launches pause menu. Does not actually pause the game, that should be handled elsewhere as a response to the pausemenu becoming active.
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Akitacore{
public class AblPause : Ability {
	//----Variable Declarations-----------------------------------------------------------------
	public GameObject pauseMenu;


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
		if(!latched){
			if(conditionsMet){
				pauseMenu.SetActive (true);
			}
		}
	}
	
	override public void PostUpdate(){
		latched = called;
	}
	
	
	
}
}
