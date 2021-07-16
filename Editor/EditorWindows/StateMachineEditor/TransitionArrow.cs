using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Akitacore{
public class TransitionArrow {

	//lookups for source
	public string sourceState;
	public int sourceFork;
	public bool sourceIsState;

	//lookups for target
	public string targetState;
	public int targetFork;
	public bool targetIsState;
	public bool targetSet;

	public bool isSelected;

	//self lookup
	public int transitionIndex;

	public Rect rect;
	public Rect arrowRect;
	float arrowRectSize =24;

	string arrowIcon = "forward@2x";
	string arrowIconSelected = "d_PlayButton On";

	GUIStyle transitionStyle;
	GUIStyle transitionArrowStyle;
	GUIStyle transitionArrowSelectedStyle;


	public TransitionArrow(string stateID, int transIndex){
		sourceState = stateID;
		sourceFork = -1;
		sourceIsState = true;
		transitionIndex = transIndex;

		targetSet = false;

		transitionStyle = new GUIStyle ();
		transitionStyle.normal.background = new Texture2D (1, 1);
		transitionStyle.normal.background.SetPixel (0, 0, Color.white);
		transitionStyle.normal.background.Apply (false);

		transitionArrowStyle = new GUIStyle ();
		transitionArrowStyle.normal.background = EditorGUIUtility.Load ("icons/"+arrowIcon+".png") as Texture2D;

		transitionArrowSelectedStyle = new GUIStyle ();
		transitionArrowSelectedStyle.normal.background = EditorGUIUtility.Load("icons/"+arrowIconSelected+".png") as Texture2D;
	}

	public TransitionArrow(int forkIndex, int transIndex){
		sourceState = "";
		sourceFork = forkIndex;
		sourceIsState = false;
		transitionIndex = transIndex;

		targetSet = false;

		transitionStyle = new GUIStyle ();
		transitionStyle.normal.background = new Texture2D (1, 1);
		transitionStyle.normal.background.SetPixel (0, 0, Color.white);
		transitionStyle.normal.background.Apply (false);

		transitionArrowStyle = new GUIStyle ();
		transitionArrowStyle.normal.background = EditorGUIUtility.Load ("icons/"+arrowIcon+".png") as Texture2D;

		transitionArrowSelectedStyle = new GUIStyle ();
		transitionArrowSelectedStyle.normal.background = EditorGUIUtility.Load("icons/"+arrowIconSelected+".png") as Texture2D;
	}

	public void SetTarget(string stateID){
		targetState = stateID;
		targetFork = -1;
		targetIsState = true;
		targetSet = true;
	}

	public void SetTarget(int forkIndex){
		targetState = "";
		targetFork = forkIndex;
		targetIsState = false;
		targetSet = true;
	}

	public void Draw(StateMachineEditor sEditor){
		if (transitionStyle !=null && transitionStyle.normal.background == null) {
			transitionStyle = new GUIStyle ();
			transitionStyle.normal.background = new Texture2D (1, 1);
			transitionStyle.normal.background.SetPixel (0, 0, Color.white);
			transitionStyle.normal.background.Apply (false);
		}

		if (transitionArrowStyle !=null && transitionArrowStyle.normal.background == null) {
			transitionArrowStyle = new GUIStyle ();
			transitionArrowStyle.normal.background = EditorGUIUtility.Load ("icons/"+arrowIcon+".png") as Texture2D;
		}

		if (transitionArrowSelectedStyle!=null && transitionArrowSelectedStyle.normal.background == null) {
			transitionArrowSelectedStyle = new GUIStyle ();
			transitionArrowSelectedStyle.normal.background = EditorGUIUtility.Load ("icons/"+arrowIconSelected+".png") as Texture2D;
		}


		//find reference points for drawing
		Vector2 startPoint;
		if (sourceIsState) {
			startPoint = sEditor.stateNodes [sourceState].rect.position + (sEditor.stateNodes [sourceState].rect.size * 0.5f);
		} else {
			startPoint = sEditor.forkNodes [sourceFork].rect.position + (sEditor.forkNodes [sourceFork].rect.size * 0.5f);
		}

		Vector2 endPoint;

		if (targetSet) {
			if (targetIsState) {
				endPoint = sEditor.stateNodes [targetState].rect.position + (sEditor.stateNodes [targetState].rect.size * 0.5f);
			} else {

				endPoint = sEditor.forkNodes [targetFork].rect.position + (sEditor.forkNodes [targetFork].rect.size * 0.5f);
			}
		} else {
			endPoint = Event.current.mousePosition;
		}

		//this would be used for drawing a box-based line, but drawing a box and then rotating it causes the box to get cut off by the sides of the canvas.
		rect = new Rect (startPoint+Vector2.up*4.0f, new Vector2((endPoint - startPoint).magnitude, 6.0f));


		Vector3 offset3 = Quaternion.AngleAxis (Vector2.SignedAngle (Vector2.right, endPoint - startPoint), Vector3.forward) * Vector3.down;
		Vector2 offset = new Vector2 (offset3.x*8, offset3.y*8);

		startPoint = startPoint + offset;
		endPoint = endPoint + offset;

		arrowRect = new Rect(startPoint+(endPoint-startPoint)*0.6f+new Vector2(arrowRectSize*-0.5f, arrowRectSize*-0.5f), new Vector2(arrowRectSize, arrowRectSize));

		//the actual rendering

			/*
		GUIUtility.RotateAroundPivot (Vector2.SignedAngle (Vector2.right, endPoint - startPoint), startPoint);

		GUI.Box (rect, "", transitionStyle);

		GUI.matrix = Matrix4x4.identity; //reset the gui matrix*/

		Handles.DrawLine (startPoint, endPoint);
		Handles.DrawLine (startPoint+Vector2.right, endPoint+Vector2.right);
		Handles.DrawLine (startPoint+Vector2.up, endPoint+Vector2.up);
		Handles.DrawLine (startPoint+Vector2.left, endPoint+Vector2.left);
		Handles.DrawLine (startPoint+Vector2.down, endPoint+Vector2.down);

		//GUI.Box (new Rect (400, 400, 24, 24), "", transitionArrowStyle); //test arrow sprite

		GUIUtility.RotateAroundPivot (Vector2.SignedAngle (Vector2.right, endPoint - startPoint), startPoint + (endPoint - startPoint) * 0.6f);

		if (isSelected) {
			GUI.Box (arrowRect, "", transitionArrowSelectedStyle);
		} else {
			GUI.Box (arrowRect, "", transitionArrowStyle);
		}
		GUI.matrix = Matrix4x4.identity; //reset the gui matrix






	}



}
}
