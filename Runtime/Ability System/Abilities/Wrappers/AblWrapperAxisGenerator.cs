//Allows an axis ability to be called with a predefined axis input as a const ability.
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Akitacore{
public class AblWrapperAxisGenerator : Ability {

	//----Variable Declarations-----------------------------------------------------------------
	public Vector3 generatedAxis;
	public Ability wrappedAxisAbl;
	float[] sendAxis;

	//----Monobehavior Update Loop--------------------------------------------------------------

	//use start to initialize internal references to the thing with the ability.
	void Start () {
		sendAxis = new float[3];
		//latchAbl = true; //flag to call Execute once on button down
		//axisAbl = true; //flag to call each frame as an axis input.
		//twoAxisAbl = true; //will call execute(vector2)
		//threeAxisAbl = true; //will call execute(vector3)
		//usesCollissionCheck2D = true; //ensures that a collCheck2D will be assigned or created.
		//usesCollissionCheck3D = true; //ensures that a collCheck3D will be assigned or created.

	}

	override public void Execute(bool conditionsMet, float[] axisInputs){
		sendAxis[0]=generatedAxis.x;
		sendAxis[1]=generatedAxis.y;
		sendAxis[2]=generatedAxis.z;
		
		if(wrappedAxisAbl !=null){
			wrappedAxisAbl.Execute(conditionsMet, sendAxis);
		}
		if (conditionsMet) {
			if (wrappedAxisAbl != null) {
				wrappedAxisAbl.Execute(conditionsMet, sendAxis);
			}
		} else {
			if (wrappedAxisAbl != null) {
				wrappedAxisAbl.Execute(false, null);
			}
		}
	}

	//-----Utility Functions-----------------------------------------------------------------

}
}