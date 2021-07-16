//Allows a single ability call to trigger multiple abilities.
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Akitacore{
public class AblWrapperComposite : Ability {

	//----Variable Declarations-----------------------------------------------------------------

	//--Declared in base:
	//public GameObject rootObj;
	//public CollisionCheck2D collCheck2D;
	//public CollisionCheck3D collCheck3D;

	//public bool usesCollisionCheck2D = false; 
	//public bool usesCollisionCheck3D = false; 
	
	//public bool called; // latch utility
	//public bool latched; // latch utility
	
	public List<Ability> abilities;
	
	//----Initialization and Verification--------------------------------------------------------------
	
	//Set up This object
	void Awake(){
		//usesCollisionCheck2D = true; //if uncommented, CollisionCheck2D will be assigned or created
		//usesCollisionCheck3D = true; //if uncommented, CollisionCheck3D will be assigned or created
		
		base.FindReferences(); // finds rootObj and collCheck
	}
	
	//use this to ensure everything is set up correctly. This should be called by the controller in Start.
	public override void Verify(bool conditionsMet, float[] axisInputs){
		foreach (var abl in abilities){
			abl.Verify(conditionsMet,axisInputs);
		}
		
	}

	//--Execute---------------------------------------------------------------------
	
	//called by controller for each ability before Execute in the update step.
	//An ability that has PreUpdate called but not Execute should do nothing.
	override public void PreUpdate(){
		foreach (var abl in abilities){
			abl.PreUpdate();
		}
	}

	//this is called from controllers or wrapper abilities during Update. Input values are passed as arguments.
	//Behavior should only occur if conditionsmet == true. 
	override public void Execute(bool conditionsMet, float[] axisInputs){
		foreach (var abl in abilities){
			abl.Execute(conditionsMet,axisInputs);
		}
	}
	
	override public void PostUpdate(){
		foreach (var abl in abilities){
			abl.PostUpdate();
		}
	}

	//-----Utility Functions-----------------------------------------------------------------


}
}
