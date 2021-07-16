//Displays all Input Manager axes to the text component.
//Defines numbered joysticks in the input manager at start. (process is automated because there's a hell of a lot of them.)
//DefineAxes() must be called from button in custom inspector to write the desired joystick axes to the input manager before publishing a build.

#if (UNITY_EDITOR)

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.Text;
using System.Text.RegularExpressions;

namespace Akitacore{
public class InputDebugDisplay : MonoBehaviour {

	List<string> axisNames;

	public bool showDebuginfo;
	public Text text;

	[Tooltip("max 28")]
	public int numberOfSticks;
	[Tooltip("max 16")]
	public int axesPerStick;
	[Tooltip("Joy0 reports all joysticks.")]
	public bool defineJoyZero;
	public float gravity;
	public float dead;
	public float sensitivity=1;


	// Use this for initialization
	void Start () {

		DefineAxes (); 
		axisNames = new List<string> ();
		ReadAxes ();

	}
	
	// Update is called once per frame
	void Update () {
		if (showDebuginfo) {
			StringBuilder fullContent = new StringBuilder ("");
			StringBuilder line;

			foreach (var str in axisNames) {
				line = new StringBuilder ("");
				line.Append (str);
				line.Append (": ");
				line.Append (Input.GetAxis (str).ToString ());
				fullContent.Append (line);
				fullContent.Append ("\n");
			}

			text.text = fullContent.ToString();
		}
	}


	//this doesn't work yet.
	private void ClearAxes()
	{
		SerializedObject serializedObject = new SerializedObject (AssetDatabase.LoadAllAssetsAtPath ("ProjectSettings/InputManager.asset") [0]);
		SerializedProperty axesProperty = serializedObject.FindProperty ("m_Axes");

		axesProperty.Next (true);
		axesProperty.Next (true);
		while (axesProperty.Next (false)) {
	
			SerializedProperty axis = axesProperty.Copy ();
			axis.Next (true);


			if (Regex.IsMatch(axis.stringValue, "Joy\\d+Axis\\d+")) {
				axis.DeleteCommand ();
				serializedObject.ApplyModifiedProperties ();
			}

		//	Debug.Log(axesProperty.GetArrayElementAtIndex(i).stringValue);
		}

	}


	void ReadAxes(){
		var inputmanager = AssetDatabase.LoadAllAssetsAtPath ("ProjectSettings/InputManager.asset") [0];

		SerializedObject serializedObj = new SerializedObject (inputmanager);

		SerializedProperty axisArray = serializedObj.FindProperty ("m_Axes");

		if (axisArray.arraySize == 0) {
			//no defined axies
			Debug.Log("no defined axis");
			return;
		}
		for (int i = 0; i < axisArray.arraySize; i++) {
			var axis = axisArray.GetArrayElementAtIndex (i);

			axisNames.Add (axis.FindPropertyRelative("m_Name").stringValue);
		}
			
	}

	public void DefineAxes(){
		InputAxis tempAxis;

		if (defineJoyZero) {
			for (int j = 1; j <= axesPerStick; j++) {
				if (!AxisDefined ("Joy" + 0.ToString () + "Axis" + j.ToString ())) {
					tempAxis = new InputAxis ();
					tempAxis.name = "Joy" + 0.ToString () + "Axis" + j.ToString ();
					tempAxis.gravity = gravity;
					tempAxis.dead = dead;
					tempAxis.sensitivity = sensitivity;

					tempAxis.type = AxisType.JoystickAxis;
					tempAxis.axis = j;
					tempAxis.joyNum = 0;

					AddAxis (tempAxis);
				}

			}
		}

		for (int i = 1; i <= numberOfSticks && i<=28 ; i++) {  //unity only supports up to 28 axes
			for (int j = 1; j <= axesPerStick && j<=16; j++) { //unity only supports up to 16 axes
				//Debug.Log(i.ToString() + " " + j.ToString());
				if (!AxisDefined ("Joy" + i.ToString () + "Axis" + j.ToString ())) {

					tempAxis = new InputAxis ();
					tempAxis.name = "Joy" + i.ToString () + "Axis" + j.ToString ();
					tempAxis.gravity = gravity;
					tempAxis.dead = dead;
					tempAxis.sensitivity = sensitivity;

					tempAxis.type = AxisType.JoystickAxis;
					tempAxis.axis = j;
					tempAxis.joyNum = i;

					AddAxis (tempAxis);
				}
			}
		}

	}

	private static SerializedProperty GetChildProperty(SerializedProperty parent, string name){
		SerializedProperty child = parent.Copy ();
		child.Next (true);
		do {
			if (child.name == name)
				return child;
		} while (child.Next (false));
		return null;
	}

	private static bool AxisDefined(string axisName)
	{
		SerializedObject serializedObject = new SerializedObject (AssetDatabase.LoadAllAssetsAtPath ("ProjectSettings/InputManager.asset") [0]);
		SerializedProperty axesProperty = serializedObject.FindProperty ("m_Axes");

		axesProperty.Next (true);
		axesProperty.Next (true);
		while (axesProperty.Next (false)) {
			SerializedProperty axis = axesProperty.Copy ();
			axis.Next (true);
			if (axis.stringValue == axisName) {
				return true;
			}
		}
		return false;

	}

	//defining an axis

	public enum AxisType{
		KeyOrMouseButton = 0,
		MouseMovement = 1,
		JoystickAxis = 2
	}

	public class InputAxis{
		public string name;
		public string descriptiveName;
		public string descriptiveNegativeName;
		public string negativeButton;
		public string positiveButton;
		public string altNegativeButton;
		public string altPositiveButton;

		public float gravity;
		public float dead;
		public float sensitivity;

		public bool snap = false;
		public bool invert = false;

		public AxisType type;

		public int axis;
		public int joyNum;
	}

	public static void AddAxis(InputAxis axis){
		if (AxisDefined (axis.name)) {
			//Debug.Log ("axis is defined");
			return;
		}

		SerializedObject serializedObject = new SerializedObject (AssetDatabase.LoadAllAssetsAtPath ("ProjectSettings/InputManager.asset") [0]);
		SerializedProperty axesProperty = serializedObject.FindProperty ("m_Axes");

		axesProperty.arraySize++;
		serializedObject.ApplyModifiedProperties ();

		SerializedProperty axisProperty = axesProperty.GetArrayElementAtIndex (axesProperty.arraySize - 1);

	
		GetChildProperty (axisProperty, "m_Name").stringValue = axis.name;
		GetChildProperty (axisProperty, "descriptiveName").stringValue = axis.descriptiveName;
		GetChildProperty (axisProperty, "descriptiveNegativeName").stringValue = axis.descriptiveNegativeName;
		GetChildProperty (axisProperty, "negativeButton").stringValue = axis.negativeButton;
		GetChildProperty (axisProperty, "positiveButton").stringValue = axis.positiveButton;
		GetChildProperty (axisProperty, "altNegativeButton").stringValue = axis.altNegativeButton;
		GetChildProperty (axisProperty, "altPositiveButton").stringValue = axis.altPositiveButton;

		GetChildProperty (axisProperty, "gravity").floatValue = axis.gravity;
		GetChildProperty (axisProperty, "dead").floatValue = axis.dead;
		GetChildProperty (axisProperty, "sensitivity").floatValue = axis.sensitivity;

		GetChildProperty (axisProperty, "snap").boolValue = axis.snap;
		GetChildProperty (axisProperty, "invert").boolValue = axis.invert;

		GetChildProperty (axisProperty, "type").intValue = (int)axis.type;

		GetChildProperty (axisProperty, "axis").intValue = axis.axis-1;
		GetChildProperty (axisProperty, "joyNum").intValue = axis.joyNum;

		serializedObject.ApplyModifiedProperties ();
	}
}
}

#endif
