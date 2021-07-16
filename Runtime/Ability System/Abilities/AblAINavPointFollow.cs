//const ability that follows a series of navigation points.
//each navigation path should have it's own instance of AblAINavPointFollow.cs

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Akitacore{
public class AblAINavPointFollow : Ability {



	//----Variable Declarations-----------------------------------------------------------------

	public Ability moveAbl; //this should be a twoAxisAbl or a threeAxisAbl
	public float destinationOffset = 0.2f;

	public List<Vector3> navPoints;
	public int navIndex = 0;

	bool destinationReached = false;

	bool calledThis = false;

	//--Declared in base:
	//public GameObject rootObj;
	//public CollisionCheck2D collCheck2D;
	//public CollisionCheck3D collCheck3D;

	//public bool latchAbl = false; 
	//public bool constAbl = false; 
	//public bool axisAbl = false; 
	//public bool usesCollisionCheck2D = false;
	//public bool usesCollisionCheck3D = false;


	//----Monobehavior Update Loop--------------------------------------------------------------



	//--Execute functions---------------------------------------------------------------------

	override public void PreUpdate(){
		calledThis = false;
	}

	//called every frame with button value if constAbl == true.
	override public void Execute(bool conditionsMet, float[] axisInputs){
		if (conditionsMet) {
			if (moveAbl != null && axisInputs.Length<3) {
				if (navIndex < navPoints.Count) {
					if (Vector2.Distance (rootObj.transform.position.V2XY (), navPoints [navIndex].V2XY ()) < destinationOffset) {
						navIndex++;
						return;
					} else {
						moveAbl.Execute (conditionsMet, axisInputs);
					}

				} else { //target destination reached
					destinationReached = true;
					return;
				}
			} else if (moveAbl != null) {
				if (navIndex < navPoints.Count) {
					if (Vector3.Distance (rootObj.transform.position, navPoints [navIndex]) < destinationOffset) {
						navIndex++;
						return;
					} else {
						moveAbl.Execute (conditionsMet, axisInputs); //this sets input direction, not speed.
					}

				} else { //target destination reached
					destinationReached = true;
					return;
				}
			}
		} 
		calledThis = true;
	}

	override public void PostUpdate () {
		if (!calledThis) {
			if (moveAbl != null) {
				moveAbl.Execute (false, null);
			}
		}

	}

	//-----Utility Functions-----------------------------------------------------------------

	//used by editor script.
	public void AddNavPoint(){
		if (navPoints != null) {
			navPoints.Add (gameObject.transform.position);
		}
	}
	public void SetToStart(){
		if (navPoints.Count > 0) {
			rootObj.transform.position = navPoints [0];
			gameObject.transform.position = navPoints [0];
		} else {
			gameObject.transform.position = rootObj.transform.position;
		}
	}

	public void SetToEnd(){
		if (navPoints.Count > 0) {
			rootObj.transform.position = navPoints [navPoints.Count - 1];
			gameObject.transform.position = navPoints [navPoints.Count - 1];
		} else {
			gameObject.transform.position = rootObj.transform.position;
		}
	}

	public bool GetDestinationReached(){
		return destinationReached;
	}
}
}
