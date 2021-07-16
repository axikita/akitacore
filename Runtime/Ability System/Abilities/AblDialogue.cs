//this does not execute COMMnCore.Bind, so the Start script cannot be changed by COMMn.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

namespace Akitacore{
public class AblDialogue : Ability {

	//----Variable Declarations-----------------------------------------------------------------
	public StringVar startNode;
	public TextAsset yarnScript;
	//--Declared in base:
	//public GameObject rootObj;
	//public CollisionCheck2D collCheck2D;
	//public CollisionCheck3D collCheck3D;

	//public bool latchAbl = false; 
	//public bool constAbl = false; 
	//public bool axisAbl = false; 
	//public bool usesCollisionCheck2D = false;
	//public bool usesCollisionCheck3D = false;
	//public bool usesRoot = true;


	//----Monobehavior Update Loop--------------------------------------------------------------
	//use awake to inmitialize any values that depend on game state, things that should be initialized on reactivation, or references to other objects.
	void Awake(){
		//usesCollisionCheck2D = true; //if uncommented, CollisionCheck2D will be assigned or created
		//usesCollisionCheck3D = true; //if uncommented, CollisionCheck3D will be assigned or created
		
		base.FindReferences(); // finds rootObj and collCheck
		
	}

	//--Execute functions---------------------------------------------------------------------
	
	override public void PreUpdate(){
		called = false;
	}

	//this is called from controllers or wrapper abilities during Update. Input values are passed as arguments.
	//Behavior should only occur if conditionsmet == true. 
	override public void Execute(bool conditionsMet, float[] axisInputs){
		called = true;
			if(!latched){
					if(conditionsMet){
				DialogueRunner runner = FindObjectOfType<DialogueRunner> ();
				if (runner.isDialogueRunning || yarnScript == null) {
					return;
				}
				runner.Clear ();
				runner.AddScript (yarnScript);
				runner.StartDialogue (startNode.val);
			}
		}
	}
	
	override public void PostUpdate(){
		latched = called;
	}



	//-----Utility Functions-----------------------------------------------------------------

}
}
