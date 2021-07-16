//Responsible for drawing and assigning data for BoolVariable, IntVariable, FloatVariable, and StringVariable.
//Much of the type safety checking is done here in the editor script, rather than in the variable execution itself.
//this allows for faster execution times, but stuff isn't guaranteed safe if you bypass the editor script, or directly edit variables during runtime.

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Reflection;
using System.Linq;
using Object = UnityEngine.Object;
using Akitacore;


namespace Akitacore{
[CustomPropertyDrawer(typeof(BoolVariable))]
[CustomPropertyDrawer(typeof(IntVariable))]
[CustomPropertyDrawer(typeof(FloatVariable))]
[CustomPropertyDrawer(typeof(StringVariable))]
public class AkitacoreVariableDrawer : PropertyDrawer
{
	
	//because these enums are set by index, it's more readable to redefine them here.
	enum ExecutionType{simple, scriptable, reference}
	enum ArgumentType{None, Bool, Int, Float, String}

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label){
		
		//find the base type of the inspected property. BoolVariable, FloatVariable, IntVariable, or StringVariable.
		var serializedVariableType = property.FindPropertyRelative("baseVal").propertyType;
		Type variableType = null;
		if(serializedVariableType == SerializedPropertyType.Boolean){
			variableType = typeof(bool);
		} else if(serializedVariableType == SerializedPropertyType.Integer){
			variableType = typeof(int);
		}else if(serializedVariableType == SerializedPropertyType.Float){
			variableType = typeof(float);
		}else if(serializedVariableType == SerializedPropertyType.String){
			variableType = typeof(string);
		} else {
			Debug.LogError("unidentified variable type");
		}
		
		
		//Define some general parameters for the GUI display
		float baseHeight = 18;
		
		float dropdownWidth = 20;
		
		position  = new Rect(position.x, position.y, position.width, position.height);
		EditorGUI.BeginProperty(position, label, property);
		position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
		
		var indent = EditorGUI.indentLevel; //cache to set later
		EditorGUI.indentLevel = 0;
		
		var enumRect = new Rect(position.x-8, position.y, dropdownWidth, baseHeight);
		var baseValRect = new Rect(position.x+dropdownWidth-3, position.y+18*0, position.width-dropdownWidth, baseHeight);
		var scriptableRect = new Rect(position.x+dropdownWidth-3, position.y+18*0, position.width-dropdownWidth, baseHeight);
		var objectRect = new Rect(position.x+dropdownWidth-3, position.y+18*0, position.width-dropdownWidth, baseHeight);
		var methodRect = new Rect(position.x+dropdownWidth-3, position.y+18*1, position.width-dropdownWidth, baseHeight);
		var argumentRect = new Rect(position.x+dropdownWidth-3, position.y+18*2, position.width-dropdownWidth, baseHeight);
		
		var argumentLabelRect = new Rect(position.x+dropdownWidth-38, position.y+18*2, 35, baseHeight);
		
		
		



		
		SerializedProperty execTypeProp = property.FindPropertyRelative("executionType");
		var execType = (ExecutionType)execTypeProp.enumValueIndex; 
		
		GUIContent buttonLabel = new GUIContent("T");
		
		//Simple Value
		if(execType == ExecutionType.simple){
			if(EditorGUI.DropdownButton(enumRect, buttonLabel, FocusType.Keyboard)){
				EnumDropdown(property);
			}
			EditorGUI.PropertyField(baseValRect, property.FindPropertyRelative("baseVal"), GUIContent.none);
			
		//Scriptable Object
		} else if(execType == ExecutionType.scriptable){
			if(EditorGUI.DropdownButton(enumRect, buttonLabel, FocusType.Keyboard)){
				EnumDropdown(property);
			}
			
			if(variableType == typeof(bool)){
				EditorGUI.PropertyField(scriptableRect, property.FindPropertyRelative("boolVar"), GUIContent.none);
			} else if(variableType == typeof(int)){
				EditorGUI.PropertyField(scriptableRect, property.FindPropertyRelative("intVar"), GUIContent.none);
			} else if(variableType == typeof(float)){
				EditorGUI.PropertyField(scriptableRect, property.FindPropertyRelative("floatVar"), GUIContent.none);
			} else if(variableType == typeof(string)){
				EditorGUI.PropertyField(scriptableRect, property.FindPropertyRelative("stringVar"), GUIContent.none);
			} 
			
		//Inspector Callback Reference
		} else {
			if(EditorGUI.DropdownButton(enumRect, buttonLabel, FocusType.Keyboard)){
				EnumDropdown(property);
			}
			EditorGUI.PropertyField(objectRect, property.FindPropertyRelative("conditionObject"), GUIContent.none);
			//EditorGUI.PropertyField(methodRect, property.FindPropertyRelative("conditionMethod"), GUIContent.none);
			
			
			string argumentString = "";
			if(property.FindPropertyRelative("argType").enumValueIndex == 1){
				argumentString = "bool arg";
			}
			if(property.FindPropertyRelative("argType").enumValueIndex == 2){
				argumentString = "int arg";
			}
			if(property.FindPropertyRelative("argType").enumValueIndex == 3){
				argumentString = "float arg";
			}
			if(property.FindPropertyRelative("argType").enumValueIndex == 4){
				argumentString = "string arg";
			}
			
			
			GUIContent methodGUICont = new GUIContent(property.FindPropertyRelative("conditionMethod").stringValue + "("+argumentString+")");
			if(EditorGUI.DropdownButton(methodRect, methodGUICont, FocusType.Keyboard)){
				GUI.FocusControl("");
				MethodSelector(property, variableType);
			}
			
			if(property.FindPropertyRelative("argType").enumValueIndex == 0){
				
			}
			if(property.FindPropertyRelative("argType").enumValueIndex == 1){
				EditorGUI.LabelField(argumentLabelRect, "arg:");
				EditorGUI.PropertyField(argumentRect, property.FindPropertyRelative("argBool"), GUIContent.none);
			}
			if(property.FindPropertyRelative("argType").enumValueIndex == 2){
				EditorGUI.LabelField(argumentLabelRect, "arg:");
				EditorGUI.PropertyField(argumentRect, property.FindPropertyRelative("argInt"), GUIContent.none);
			}
			if(property.FindPropertyRelative("argType").enumValueIndex == 3){
				EditorGUI.LabelField(argumentLabelRect, "arg:");
				EditorGUI.PropertyField(argumentRect, property.FindPropertyRelative("argFloat"), GUIContent.none);
			}
			if(property.FindPropertyRelative("argType").enumValueIndex == 4){
				EditorGUI.LabelField(argumentLabelRect, "arg:");
				EditorGUI.PropertyField(argumentRect, property.FindPropertyRelative("argString"), GUIContent.none);
			}
			
		}
		
		
		// Set indent back to what it was
		EditorGUI.indentLevel = indent;
		
		EditorGUI.EndProperty();
	}
	
	
	public override float GetPropertyHeight(SerializedProperty property, GUIContent label){
		SerializedProperty execTypeProp = property.FindPropertyRelative("executionType");
		var execType = (ExecutionType)execTypeProp.enumValueIndex;
		float extraHeight;
	
		
		if(execType == ExecutionType.reference){
			extraHeight = 18;
		} else {
			extraHeight = 0;
		}
		
		if(property.FindPropertyRelative("argType").enumValueIndex > 0){
			extraHeight+=18;
		}
		
		return base.GetPropertyHeight(property, label) + extraHeight; //base is 18
		
	}

	

	void EnumDropdown(SerializedProperty property){
		
		SerializedProperty execTypeProp = property.FindPropertyRelative("executionType");
		var execType = (ExecutionType)execTypeProp.enumValueIndex;
		
		GUIContent enum0 = new GUIContent("Simple Value");
		GUIContent enum1 = new GUIContent("Scriptable Value");
		GUIContent enum2 = new GUIContent("Reference Value");
		
		var menu = new GenericMenu();
		menu.AddItem(enum0,false, () => {execTypeProp.intValue = 0; execTypeProp.serializedObject.ApplyModifiedProperties();});
		menu.AddItem(enum1,false, () => {execTypeProp.intValue = 1; execTypeProp.serializedObject.ApplyModifiedProperties();});
		menu.AddItem(enum2,false, () => {execTypeProp.intValue = 2; execTypeProp.serializedObject.ApplyModifiedProperties();});
		
		menu.ShowAsContext();
	}
	
	
	//Copied from SerialzableCallBackDrawer--------------------------------------------------------------------------------------------------
	//---------------------------------------------------------------------------------------------------------------------------------------
	//---------------------------------------------------------------------------------------------------------------------------------------
	//---------------------------------------------------------------------------------------------------------------------------------------
	//---------------------------------------------------------------------------------------------------------------------------------------
	
	//This whole chunk draws the dropdown menu for the method, and assigns the method to the appropriate string.
	
	private class MenuItem {
		public GenericMenu.MenuFunction action;
		public string path;
		public GUIContent label;

		public MenuItem(string path, string name, GenericMenu.MenuFunction action) {
			this.action = action;
			this.label = new GUIContent(path + '/' + name);
			this.path = path;
		}
	}
	
	void MethodSelector(SerializedProperty property, Type returnType) {
		
		// Return type constraint
		//Type returnType = typeof(bool); //EDITED Null -> typeof(bool)
		// Arg type constraint
		Type[] argTypes = new Type[0];

		// Get return type and argument constraints
		
		Type[] genericTypes = new Type[0];

		SerializedProperty targetProp = property.FindPropertyRelative("conditionObject");
		
		if(targetProp == null){
			return;
		}

		List<MenuItem> dynamicItems = new List<MenuItem>();
		List<MenuItem> staticItems = new List<MenuItem>();

		List<Object> targets = new List<Object>() { targetProp.objectReferenceValue };
		if (targets[0] is Component) {
			targets = (targets[0] as Component).gameObject.GetComponents<Component>().ToList<Object>();
			targets.Add((targetProp.objectReferenceValue as Component).gameObject);
		} else if (targets[0] is GameObject) {
			targets = (targets[0] as GameObject).GetComponents<Component>().ToList<Object>();
			targets.Add(targetProp.objectReferenceValue as GameObject);
		}
		for (int c = 0; c < targets.Count; c++) {
			Object t = targets[c];
			MethodInfo[] methods = targets[c].GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);

			for (int i = 0; i < methods.Length; i++) {
				MethodInfo method = methods[i];

				// Skip methods with wrong return type
				if (returnType != null && method.ReturnType != returnType) continue;
				// Skip methods with null return type
				// if (method.ReturnType == typeof(void)) continue;
				// Skip generic methods
				if (method.IsGenericMethod) continue;

				Type[] parms = method.GetParameters().Select(x => x.ParameterType).ToArray();

				// Skip methods with more than 4 args
				if (parms.Length > 1) continue; //EDITED: 4 -> 0 //edited again, 4 -> 1
				// Skip methods with unsupported args
				//if (parms.Any(x => !Arg.IsSupported(x))) continue;
				if (parms.Any(x => !ArgTypeSupported(x))) continue; //moved from serializablecallbackbase to below

				string methodPrettyName = PrettifyMethod(methods[i]);
				staticItems.Add(new MenuItem(targets[c].GetType().Name + "/" + methods[i].DeclaringType.Name, methodPrettyName, () => SetMethod(property, t, method, false)));

				// Skip methods with wrong constrained args
				if (argTypes.Length == 0 || !Enumerable.SequenceEqual(argTypes, parms)) continue;

				dynamicItems.Add(new MenuItem(targets[c].GetType().Name + "/" + methods[i].DeclaringType.Name, methods[i].Name, () => SetMethod(property, t, method, true)));
			}
		}

		// Construct and display context menu
		GenericMenu menu = new GenericMenu();
		if (dynamicItems.Count > 0) {
			string[] paths = dynamicItems.GroupBy(x => x.path).Select(x => x.First().path).ToArray();
			foreach (string path in paths) {
				menu.AddItem(new GUIContent(path + "/Dynamic " + PrettifyTypes(argTypes)), false, null);
			}
			for (int i = 0; i < dynamicItems.Count; i++) {
				menu.AddItem(dynamicItems[i].label, false, dynamicItems[i].action);
			}
			foreach (string path in paths) {
				menu.AddItem(new GUIContent(path + "/  "), false, null);
				menu.AddItem(new GUIContent(path + "/Static parameters"), false, null);
			}
		}
		for (int i = 0; i < staticItems.Count; i++) {
			menu.AddItem(staticItems[i].label, false, staticItems[i].action);
		}
		if (menu.GetItemCount() == 0) menu.AddDisabledItem(new GUIContent("No methods with return type '" + GetTypeName(returnType) + "'"));
		menu.ShowAsContext();
	}

	string PrettifyMethod(string methodName, Type[] parmTypes) {
		string parmnames = PrettifyTypes(parmTypes);
		return methodName + "(" + parmnames + ")";
	}

	string PrettifyMethod(MethodInfo methodInfo) {
		if (methodInfo == null) throw new ArgumentNullException("methodInfo");
		ParameterInfo[] parms = methodInfo.GetParameters();
		string parmnames = PrettifyTypes(parms.Select(x => x.ParameterType).ToArray());
		return GetTypeName(methodInfo.ReturnParameter.ParameterType) + " " + methodInfo.Name + "(" + parmnames + ")";
	}

	string PrettifyTypes(Type[] types) {
		if (types == null) throw new ArgumentNullException("types");
		return string.Join(", ", types.Select(x => GetTypeName(x)).ToArray());
	}

	MethodInfo GetMethod(object target, string methodName, Type[] types) {
		MethodInfo activeMethod = target.GetType().GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static, null, CallingConventions.Any, types, null);
		return activeMethod;
	}
	
	/*
	private Type[] GetArgTypes(SerializedProperty argProp) {
		Type[] types = new Type[argProp.arraySize];
		for (int i = 0; i < argProp.arraySize; i++) {
			types[i] = Arg.RealType((Arg.ArgType) argProp.FindPropertyRelative("Array.data[" + i + "].argType").enumValueIndex);
		}
		return types;
	}*/

	private void SetMethod(SerializedProperty property, UnityEngine.Object target, MethodInfo methodInfo, bool dynamic) {
		SerializedProperty targetProp = property.FindPropertyRelative("conditionObject");
		targetProp.objectReferenceValue = target;
		SerializedProperty methodProp = property.FindPropertyRelative("conditionMethod");
		methodProp.stringValue = methodInfo.Name;
		
		//Argument types
		SerializedProperty typeEnum = property.FindPropertyRelative("argType");
		ParameterInfo[] parameters = methodInfo.GetParameters();
	
		
		if(parameters.Length>0){
			if(parameters[0].ParameterType == typeof(bool)){
				typeEnum.enumValueIndex = (int)ArgumentType.Bool;
			} else if(parameters[0].ParameterType == typeof(int)){
				typeEnum.enumValueIndex = (int)ArgumentType.Int;
			}else if(parameters[0].ParameterType == typeof(float)){
				typeEnum.enumValueIndex = (int)ArgumentType.Float;
			}else if(parameters[0].ParameterType == typeof(string)){
				typeEnum.enumValueIndex = (int)ArgumentType.String;
			} else {
				Debug.LogError("Attempting to reference a function with unrecognized argument type.");
			}
			//property.FindPropertyRelative("conditionArgs").arraySize = 1;
		} else {
			typeEnum.enumValueIndex = (int)ArgumentType.None;
			//property.FindPropertyRelative("conditionArgs").arraySize = 0;
		}
		
		
		/*
		SerializedProperty dynamicProp = property.FindPropertyRelative("_dynamic");
		dynamicProp.boolValue = dynamic;
		SerializedProperty argProp = property.FindPropertyRelative("_args");
		ParameterInfo[] parameters = methodInfo.GetParameters();
		argProp.arraySize = parameters.Length;
		for (int i = 0; i < parameters.Length; i++) {
		argProp.FindPropertyRelative("Array.data[" + i + "].argType").enumValueIndex = (int) Arg.FromRealType(parameters[i].ParameterType);
		}
		property.FindPropertyRelative("dirty").boolValue = true;
		*/
		property.serializedObject.ApplyModifiedProperties();
		property.serializedObject.Update();
	}

	private static string GetTypeName(Type t) {
		if (t == typeof(int)) return "int";
		else if (t == typeof(float)) return "float";
		else if (t == typeof(string)) return "string";
		else if (t == typeof(bool)) return "bool";
		else if (t == typeof(void)) return "void";
		else return t.Name;
	}
	
	//End Copied from SerialzableCallBackDrawer--------------------------------------------------------------------------------------------------
	//---------------------------------------------------------------------------------------------------------------------------------------
	//---------------------------------------------------------------------------------------------------------------------------------------
	//---------------------------------------------------------------------------------------------------------------------------------------
	//---------------------------------------------------------------------------------------------------------------------------------------
	
	bool ArgTypeSupported(Type type){
		return (type == typeof(bool)||type == typeof(float)||type == typeof(int)||type == typeof(string));
	}

	
}	
}
