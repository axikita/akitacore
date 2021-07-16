using System;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace Akitacore{
public class StateNode
{
	public String stateName = "New State";

	public Rect rect;
	public Rect labelRect;
	public string title;
	public bool isDragged;
	public bool isSelected;
	public bool isActive;

	public GUIStyle style;
	public GUIStyle defaultNodeStyle;
	public GUIStyle startNodeStyle;
	public GUIStyle selectedNodeStyle;
	public GUIStyle activeNodeStyle;
	public GUIStyle whiteText;
	public GUIStyle blackText;

	public Action<StateNode> OnRemoveNode;

	public List<TransitionArrow> transitions;

	//public StateNode(Vector2 position, Vector2 size, Action<StateNode> OnAddTransition, Action<StateNode> OnClickRemoveNode)
	public StateNode(Vector2 position, Vector2 size)
	{
		rect = new Rect(position.x, position.y, size.x, size.y);
		labelRect = new Rect (position.x+15, position.y+18, size.x, size.y);

		style = new GUIStyle();
		style.normal.background = EditorGUIUtility.Load ("builtin skins/lightskin/images/node1.png") as Texture2D;
		style.border = new RectOffset (12, 12, 12, 12);

		defaultNodeStyle = new GUIStyle();
		defaultNodeStyle.normal.background = EditorGUIUtility.Load ("builtin skins/lightskin/images/node1.png") as Texture2D;
		defaultNodeStyle.border = new RectOffset (12, 12, 12, 12);
		
		startNodeStyle = new GUIStyle();
		startNodeStyle.normal.background = EditorGUIUtility.Load ("builtin skins/lightskin/images/node5.png") as Texture2D;
		startNodeStyle.border = new RectOffset (12, 12, 12, 12);

		selectedNodeStyle = new GUIStyle();
		selectedNodeStyle.normal.background = EditorGUIUtility.Load ("builtin skins/lightskin/images/node1 on.png") as Texture2D;
		selectedNodeStyle.border = new RectOffset (12, 12, 12, 12);

		activeNodeStyle = new GUIStyle ();
		activeNodeStyle.normal.background = EditorGUIUtility.Load ("builtin skins/darkskin/images/node1 on.png") as Texture2D;
		activeNodeStyle.border = new RectOffset (12, 12, 12, 12);

		whiteText = new GUIStyle ();
		whiteText.normal.textColor = Color.white;
		
		blackText = new GUIStyle ();
		blackText.normal.textColor = Color.black;

		transitions = new List<TransitionArrow> ();

		//OnRemoveNode = OnClickRemoveNode;
	}

	public void Drag(Vector2 delta, StateMachineEditor SEditor)
	{
		rect.position += delta;
		labelRect.position += delta;
		SEditor.stateMachine.states [title].editorPosition = rect.position;
		SEditor.stateMachine.debugString = "set From Drag Function";
	}

	public void Draw(string stateName)
	{
		title = stateName;
		if(stateName == StateMachine.initialState){
			stateName = "START";
			if (isActive) {
			GUI.Box(rect, "", activeNodeStyle);
			GUI.Label (labelRect, stateName, whiteText);
			} else {
			GUI.Box(rect, "", startNodeStyle);
			GUI.Label (labelRect, stateName, blackText);
			}
		} else {
			if (isActive) {
			GUI.Box(rect, "", activeNodeStyle);
			GUI.Label (labelRect, stateName, whiteText);
			} else {
			GUI.Box(rect, "", style);
			GUI.Label (labelRect, stateName, blackText);
		}
			
		}
		//rect = stateRect; //update this in the change position function instead.
		//inPoint.Draw();
		//outPoint.Draw();





	}

	//returns true if node has been altered
	public bool ProcessEvents(Event e, StateMachineEditor sEditor)
	{
		switch (e.type)
		{
		case EventType.MouseDown:
			if (e.button == 0) //select and drag
			{
				if (rect.Contains(e.mousePosition))
				{
					isDragged = true;
					GUI.changed = true;
					isSelected = true;
					sEditor.SetSelectedState (title);
					style = selectedNodeStyle;

					if (transitions.Count > 0) {
						foreach (var trans in transitions) {
							trans.isSelected = false;
						}
					}
				}
				else
				{
					GUI.changed = true;
					isSelected = false;
					style = defaultNodeStyle;

					if (transitions.Count > 0) {
						for (int i = 0; i < transitions.Count; i++) {
							if (transitions [i].arrowRect.Contains (e.mousePosition)) {
								sEditor.SetSelectedTransition (title, i);
								transitions [i].isSelected = true;
							} else {
								transitions [i].isSelected = false;
							}
						}
					}
						
				}
			}

			if (e.button == 1 ) //select, but do not drag
			{
				if (rect.Contains(e.mousePosition))
				{
					GUI.changed = true;
					isSelected = true;
					sEditor.SetSelectedState (title);
					style = selectedNodeStyle;
				}
				else
				{
					GUI.changed = true;
					isSelected = false;
					style = defaultNodeStyle;
				}
			}
			break;

		case EventType.MouseUp:
			isDragged = false;
			break;

		case EventType.MouseDrag:
			if (e.button == 0 && isDragged)
			{
				Drag(e.delta, sEditor);
				e.Use();
				return true;
			}
			break;
		}

		return false;
	}

	/*private void ProcessContextMenu()
	{
		GenericMenu genericMenu = new GenericMenu();
		genericMenu.AddItem(new GUIContent("Remove node"), false, OnClickRemoveNode);
		genericMenu.ShowAsContext();
	}*/

	/*
	private void OnClickRemoveNode(StateMachineEditor stEditor)
	{
		if (OnRemoveNode != null)
		{
			
			OnRemoveNode(this);
		}
	}*/
}
}

//Code from http://gram.gs/gramlog/creating-node-based-editor-unity/
