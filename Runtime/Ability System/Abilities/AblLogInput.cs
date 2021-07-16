using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Akitacore{
public class AblLogInput : Ability {
	//----Variable Declarations-----------------------------------------------------------------

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
		Debug.Log("AblLogInput present and awake");
		
	}
	

	//this is called from controllers or wrapper abilities during Update. Input values are passed as arguments.
	//Behavior should only occur if conditionsmet == true. 
	override public void Execute(bool conditionsMet, float[] axisInputs){
		Debug.Log("ConditionsMet = "+conditionsMet.ToString()+", axisInputs.Length = "+axisInputs.Length.ToString());
		for (int i=0; i<axisInputs.Length; i++){
			Debug.Log("AxisInput "+i.ToString()+": "+axisInputs[i].ToString());
		}
	}
	


	//-----Utility Functions-----------------------------------------------------------------
}
}
