using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Akitacore{
public class AblWrapperAxisToCamCoord : Ability {

	//----Variable Declarations-----------------------------------------------------------------
	
	public Ability wrappedABL;
	public Transform targetCamera;
	
	public enum TargetType {groundTarget, screenTarget, globalTarget};
	public TargetType targetType;
	
	float[] passedInputs;
	Vector3 workingInputs;

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
		passedInputs = new float[]{0.0f,0.0f};
		workingInputs = new Vector3(0.0f,0.0f,0.0f);
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
		wrappedABL.Verify(conditionsMet, passedInputs);
	}

	//--Execute---------------------------------------------------------------------
	
	//called by controller for each ability before Execute in the update step.
	//An ability that has PreUpdate called but not Execute should do nothing.
	override public void PreUpdate(){
		called = false;
		wrappedABL.PreUpdate();
	}

	//this is called from controllers or wrapper abilities during Update. Input values are passed as arguments.
	//Behavior should only occur if conditionsmet == true. 
	override public void Execute(bool conditionsMet, float[] axisInputs){
		if (targetType == TargetType.groundTarget){
			//controller x-y becomes world XZ. Rotate around Y axis so input X is aligned with camera X. 
			//assumes both camera X and global X are in the X-Z plane
			workingInputs.Set(axisInputs[0], 0.0f, axisInputs[1]);
			workingInputs = Quaternion.AngleAxis(Vector3.AngleBetween(targetCamera.right, Vector3.right),Vector3.up)*workingInputs;
			passedInputs[0] = workingInputs.x;
			passedInputs[1] = workingInputs.z;
			
		} else if (targetType == TargetType.screenTarget){
			workingInputs.Set(axisInputs[0],axisInputs[1], 0.0f);
			workingInputs = targetCamera.rotation*workingInputs;
			passedInputs[0] = workingInputs.x;
			passedInputs[1] = workingInputs.y;
		} else{
			passedInputs= axisInputs;
		}
		
		wrappedABL.Execute(conditionsMet, passedInputs);

	}
	
	//called by controller for each ability after Execute in the update step.
	override public void PostUpdate(){
		latched = called;
		wrappedABL.PostUpdate();
	}

	//--Monobehavior Updates--------------------------------------------

	void Update () {

	}
	
	void FixedUpdate () {

	}

	//-----Utility Functions-----------------------------------------------------------------

}
}
