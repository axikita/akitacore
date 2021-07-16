using System;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace Akitacore{
public class ForkNode
{

	public StateMachine.Fork.Type forkType;

	public int forkIndex;

	public Rect rect;
	public Rect labelRect;
	public bool isDragged;
	public bool isSelected;

	public GUIStyle style;
	public GUIStyle defaultNodeStyle;
	public GUIStyle selectedNodeStyle;

	public Action<StateNode> OnRemoveNode;

	public List<TransitionArrow> transitions;

	public ForkNode(Vector2 position, Vector2 size)
	{
		rect = new Rect(position.x, position.y, size.x, size.y);
		labelRect = new Rect (position.x + 5, position.y, size.x, size.y);

		style = new GUIStyle();
		style.normal.background = EditorGUIUtility.Load ("builtin skins/lightskin/images/node1.png") as Texture2D;
		style.border = new RectOffset (12, 12, 12, 12);

		defaultNodeStyle = new GUIStyle();
		defaultNodeStyle.normal.background = EditorGUIUtility.Load ("builtin skins/lightskin/images/node1.png") as Texture2D;
		defaultNodeStyle.border = new RectOffset (12, 12, 12, 12);

		selectedNodeStyle = new GUIStyle();
		selectedNodeStyle.normal.background = EditorGUIUtility.Load ("builtin skins/lightskin/images/node1 on.png") as Texture2D;
		selectedNodeStyle.border = new RectOffset (12, 12, 12, 12);

		transitions = new List<TransitionArrow> ();
	}

	public void Drag(Vector2 delta, StateMachineEditor SEditor)
	{
		rect.position += delta;
		labelRect.position += delta;
		SEditor.stateMachine.forks[forkIndex].editorPosition = rect.position;
	}

	public void Draw()
	{
		//rect = stateRect; //update this in the change position function instead.
		//title = stateName;
		GUI.Box(rect, "", style);
		GUI.Label (labelRect, forkType.ToString());

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
					sEditor.SetSelectedFork (forkIndex);
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
								sEditor.SetSelectedTransition (forkIndex, i);
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
					sEditor.SetSelectedFork (forkIndex);
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
