using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Akitacore{
public class InvokablePosition : MonoBehaviour
{
	public PositionEntry[] targetPositions;
	
	public void SetPosition(int targetIndex){
		if(targetPositions.Length >targetIndex){
			transform.position = targetPositions[targetIndex].targetPosition;
		} else {
			Debug.LogWarning("TargetIndex supplied to InvokablePosition on "+gameObject.name+" is greater than the size of the position array. Cannot assign position");
		}
	}
	
	//called by the custom inspector, so you can just drag the object to its desired positions and hit a button to assign them.
	public void AddPositionToTargets(){
		PositionEntry[] tempArray;
		if(targetPositions == null){
			tempArray = new PositionEntry[1];
		} else{
			tempArray = new PositionEntry[targetPositions.Length+1];
		}
		if(targetPositions!=null){
			for (int i=0; i<targetPositions.Length; i++){
				tempArray[i]=targetPositions[i];
			}
		}
		for (int i=0; i<tempArray.Length; i++){
			if(tempArray[i] == null){
				tempArray[i] = new PositionEntry();
			}
		}
		
		tempArray[tempArray.Length-1].targetPosition=transform.position;
		targetPositions = tempArray;
	}
	
	[System.Serializable]
	public class PositionEntry{
		public string ReadonlyDescription;
		public Vector3 targetPosition;
	}
}
}
