using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Collections;

namespace Akitacore{
public class StateMachineEditor : EditorWindow
{
	//--Properties----------------------------------------------------------------------------

	//--State Machine Data-----
	public StateMachine stateMachine;
	public StateMachineGeneric controller;
	string selectedState; //this is a dictionary key, can be null.
	int selectedFork;
	int selectedTransition;
	string lastStateMachine = "";

	TransitionArrow workingTransition;

	enum SelectionType{noSelection, state, fork, stateTransition, forkTransition, parameters};
	SelectionType selectionType = SelectionType.noSelection;

	//--Display Elements----
	public Dictionary<string,StateNode> stateNodes;
	public List<ForkNode> forkNodes;
	//Dictionary<string, TransitionArrow> transitionArrows; this has been moved to a list in each stateNode and ForkNode

	Vector2 stateNodeSize;
	Vector2 forkNodeSize;


	//--Side Panel Data----
	public string sidePanelTitle = "No Statemachine Selected";
	private Rect sidePanel;
	private float sidePanelWidth = 0.3f;
	private Rect sidePanelResizer;

	private GUIStyle sidePanelStyle;
	private GUIStyle resizerStyle;


	//--Input tools----
    private Vector2 offset;
    private Vector2 drag;

	bool isResizing;

	string newParameterName = "New Parameter";

	//transition creation

	//--Primary Execution Loop-------------------------------------------------------------------------------

    [MenuItem("Window/State Machine Editor")]
    private static void OpenWindow()
    {
		StateMachineEditor window = GetWindow<StateMachineEditor>();
        window.titleContent = new GUIContent("State Machine Editor");
    }
		



    private void OnEnable()
    {
		stateNodes = new Dictionary<string, StateNode> ();
		forkNodes = new List<ForkNode> ();
		//transitionArrows = new Dictionary<string, TransitionArrow> ();

		sidePanelStyle = new GUIStyle (); //this gets deleted on play???
		sidePanelStyle.normal.background = new Texture2D (1, 1);
		sidePanelStyle.normal.background.SetPixel (0, 0, Color.gray);
		sidePanelStyle.normal.background.Apply (false);
		//sidePanelStyle.normal.background = EditorGUIUtility.Load("StateMachineEditor.Background") as Texture2D;

		resizerStyle = new GUIStyle ();
		resizerStyle.normal.background = EditorGUIUtility.Load ("icons/d_AvatarBlendBackground.png") as Texture2D;

		stateNodeSize = new Vector2 (200, 50);
		forkNodeSize = new Vector2 (50, 50);


    }



    private void OnGUI()
    {
		try{
	
			//Get state machine from selected controller
			if (Selection.activeGameObject != null) {
				
				if (Selection.activeGameObject.GetComponent<StateMachineGeneric> () != null && Selection.activeGameObject.GetComponent<StateMachineGeneric> ().stateMachine!=null) {
					controller =  Selection.activeGameObject.GetComponent<StateMachineGeneric> ();
					stateMachine = controller.stateMachine;
					sidePanelTitle = stateMachine.name;
	
					if(!string.Equals(lastStateMachine, stateMachine.name)){
						BuildUIFromMachine ();
						GUI.changed = true;
						lastStateMachine = stateMachine.name;
						selectionType = SelectionType.noSelection;
						selectedFork = 0;
						selectedTransition = 0;
						selectedState = "";
					}


				}
			}

			//if state machine exists but fork and state nodes become cleared, rebuild. This happens on play sometimes.
			if(stateMachine !=null && stateNodes !=null && forkNodes !=null){
				if(stateMachine.forks.Count != forkNodes.Count || stateMachine.states.Count != stateNodes.Count){
					BuildUIFromMachine();
				}
			}


			//Draw background grid
			DrawGrid(20, 0.2f, Color.gray);
			DrawGrid(100, 0.4f, Color.gray);

			//Draw state machine
			if (stateMachine != null) {

				DrawTransitions ();
				DrawStates ();
				DrawForks ();
			}




			//draw menu to the left
			DrawSidePanel ();

			DrawResizer ();
		
			ProcessNodeEvents(Event.current);
			Repaint ();
			ProcessEvents(Event.current);
			Repaint ();
	
			if (GUI.enabled) { //this runs as long as the gui window is visible.
				Repaint ();
			}

			//if (GUI.changed) { //this doesn't react immediately to a new selection
			//	Repaint ();
			//}
			if(stateMachine!=null){
				EditorUtility.SetDirty (stateMachine); //ensures that the state machine will be saved when the scene is saved.
			}
		}
		catch(UnityException e){
			Debug.LogWarning ("Error detected with state machine, attempting integrity checks");
			Debug.LogError (e);

			selectedFork = 0;
			selectedState = "";
			selectedTransition = 0;
			selectionType = SelectionType.noSelection;

			if(stateMachine != null) {
				stateMachine.VerifyIntegrity();
				BuildUIFromMachine ();
			}
		}

    }

	//--End of monobehavior functions--------------------------------------------------
		
	//--Execution Draw Event Functions----------------------------------------------------------------------------------------

    private void DrawGrid(float gridSpacing, float gridOpacity, Color gridColor)
    {
        int widthDivs = Mathf.CeilToInt(position.width / gridSpacing);
        int heightDivs = Mathf.CeilToInt(position.height / gridSpacing);

        Handles.BeginGUI();
        Handles.color = new Color(gridColor.r, gridColor.g, gridColor.b, gridOpacity);

        offset += drag * 0.5f;
        Vector3 newOffset = new Vector3(offset.x % gridSpacing, offset.y % gridSpacing, 0);

        for (int i = 0; i < widthDivs; i++)
        {
            Handles.DrawLine(new Vector3(gridSpacing * i, -gridSpacing, 0) + newOffset, new Vector3(gridSpacing * i, position.height, 0f) + newOffset);
        }

        for (int j = 0; j < heightDivs; j++)
        {
            Handles.DrawLine(new Vector3(-gridSpacing, gridSpacing * j, 0) + newOffset, new Vector3(position.width, gridSpacing * j, 0f) + newOffset);
        }

        Handles.color = Color.white;
        Handles.EndGUI();
    }



    private void DrawStates()
    {
		if (stateNodes != null) {
			foreach (var stateNode in stateNodes) {
				if (controller != null) {
					stateNode.Value.isActive = object.Equals (controller.currentState, stateMachine.states [stateNode.Key]);
				}

				stateNode.Value.Draw (stateNode.Key);
			}
		}
    }



	private void DrawForks()
	{
		if (forkNodes != null) {
			foreach (var fork in forkNodes) {
				fork.Draw ();
			}
		}
	}



    private void DrawTransitions()
    {
		if (stateNodes != null) {
			foreach (var node in stateNodes) {
				foreach (var transition in node.Value.transitions) {
					transition.Draw (this);
				}
			}
		}
		if (forkNodes != null) {
			foreach (var node in forkNodes) {
				foreach (var transition in node.transitions) {
					transition.Draw (this);
				}
			}
		}
		if (workingTransition != null) {
			workingTransition.Draw (this);
			Repaint ();
		}
    }



	private void DrawSidePanel(){

		//ensure GUIStyles exist

		GUIStyle rightAlignedButton = new GUIStyle ();
		rightAlignedButton.alignment = TextAnchor.MiddleRight;
		rightAlignedButton.normal.background = EditorGUIUtility.Load ("CollabDeleted Icon") as Texture2D;
		rightAlignedButton.fixedWidth = 24.0f;
		rightAlignedButton.fixedHeight = 24.0f;

		GUIStyle propertyNameField = new GUIStyle();
		propertyNameField.fixedWidth = 200.0f;

		if (sidePanelStyle.normal.background == null) {
			//sidePanelStyle = new GUIStyle (); //this gets deleted on play???
			sidePanelStyle.normal.background = new Texture2D (1, 1);
			sidePanelStyle.normal.background.SetPixel (0, 0, Color.gray);
			sidePanelStyle.normal.background.Apply (false);
			//Debug.Log ("side panel texture is being rebuilt.");
		}

		sidePanel = new Rect (0, 0, position.width * sidePanelWidth, position.height);
		using (var areaScope = new GUILayout.AreaScope (sidePanel,"",sidePanelStyle)){

			//panel title matches statemachine name
			GUILayout.Label (sidePanelTitle);

			//parameter button switches to parameter view
			if (GUILayout.Button ("Parameters")) {
				selectionType = SelectionType.parameters;
			}

			//display parameters for statemachine
			if (selectionType == SelectionType.parameters) {
				GUILayout.Label ("Parameters");

				if (stateMachine != null) {
					for (int i=0; i<stateMachine.parameters.Count; i++){
						
						using (var horizNameScope = new GUILayout.HorizontalScope ()) {
							GUILayout.Label ("  "+stateMachine.parameters [i].name, propertyNameField);
							GUILayout.Label (stateMachine.parameters [i].type.ToString());
							if(GUILayout.Button("  ", rightAlignedButton)){
								stateMachine.parameters.RemoveAt(i);
								stateMachine.VerifyIntegrity (); //make sure that no transitions are using the parameter.
							}
						}

					}
				}
					
				newParameterName = GUILayout.TextField (newParameterName);

				using (var horizScope = new GUILayout.HorizontalScope ()) {
					if (GUILayout.Button ("New Bool")) {
						//Debug.Log ("new bool "+newParameterName);
						if (stateMachine.GetParameter (newParameterName) == null) {
							stateMachine.parameters.Add (new StateMachine.Parameter (newParameterName, false));
							newParameterName = "New Parameter";
						}
					}
					if (GUILayout.Button ("New Float ")) {
						Debug.Log ("new float "+newParameterName);
						if (stateMachine.GetParameter (newParameterName) == null) {
							stateMachine.parameters.Add (new StateMachine.Parameter (newParameterName, 0.0f));
							newParameterName = "New Parameter";
						}
					}
					if (GUILayout.Button ("New Int")) {
						Debug.Log ("new int "+newParameterName);
						if (stateMachine.GetParameter (newParameterName) == null) {
							stateMachine.parameters.Add (new StateMachine.Parameter (newParameterName, 0));
							newParameterName = "New Parameter";
						}
					}

				}
			}

			//display state information for selected state
			if (selectionType == SelectionType.state) {
				GUILayout.Label ("State selected.");
				if(selectedState == StateMachine.initialState){
					GUILayout.Label ("Start");
				} else {
					GUILayout.Label (selectedState);
				}
				if (stateMachine != null && stateMachine.states != null && stateMachine.states.ContainsKey (selectedState)) {
					//GUILayout.Label (stateMachine.states [selectedState].editorPosition.ToString ());
					
					
					string editableName;
					editableName = selectedState;
					if(!(editableName == StateMachine.initialState)){
						editableName = GUILayout.TextField (editableName);
					}

					//change name- this messes up every reference, so they have to be fixed.
					if (!string.Equals (editableName, selectedState)) {
						stateNodes.Add (editableName, stateNodes [selectedState]);
						stateNodes.Remove (selectedState);
						stateMachine.states.Add (editableName, stateMachine.states [selectedState]);
						stateMachine.states.Remove (selectedState);

						//fix transitions
						foreach (var node in stateNodes) {
							foreach (var arrow in node.Value.transitions) {
								arrow.sourceState = node.Key;
								if (string.Equals (arrow.targetState, selectedState)) {
									arrow.targetState = editableName;
								}
							}
						}
						foreach (var node in forkNodes) {
							foreach (var arrow in node.transitions) {
								if (string.Equals (arrow.targetState, selectedState)) {
									arrow.targetState = editableName;
								}
							}
						}

						foreach (var state in stateMachine.states) {
							foreach (var trans in state.Value.transitions) {
								if (trans.target == null) {
									trans.target = stateMachine.states [editableName];
								}
							}
						}

						foreach (var fork in stateMachine.forks) {
							foreach (var trans in fork.transitions) {
								if (trans.target == null) {
									trans.target = stateMachine.states [editableName];
								}
							}
						}
				
						selectedState = editableName;
					}
					
					GUILayout.Label("State behavior is defined in the controller monobehavior.");
					/* Abilities were removed from
					GUILayout.Label ("Default Abilities");
				
					stateMachine.states [selectedState].OnEnter = EditorGUILayout.ObjectField ("On Enter", stateMachine.states [selectedState].OnEnter, typeof(Ability), false) as Ability;
					stateMachine.states [selectedState].OnStay = EditorGUILayout.ObjectField ("On Stay", stateMachine.states [selectedState].OnStay, typeof(Ability), false) as Ability;
					stateMachine.states [selectedState].OnExit = EditorGUILayout.ObjectField ("On Exit", stateMachine.states [selectedState].OnExit, typeof(Ability), false) as Ability;
					*/
				}

				//display fork information for selected fork
			} else if (selectionType == SelectionType.fork) {
				GUILayout.Label ("Fork selected.");
				GUILayout.Label (selectedFork.ToString ());
				if (stateMachine != null && stateMachine.forks != null && stateMachine.forks.Count > (selectedFork)) {

					StateMachine.Fork.Type displayType;
					displayType = stateMachine.forks [selectedFork].type;

					displayType = (StateMachine.Fork.Type)EditorGUILayout.EnumPopup (displayType);

					if (stateMachine.forks [selectedFork].type != displayType) {
						stateMachine.forks [selectedFork].type = displayType;
						forkNodes [selectedFork].forkType = displayType;
						Repaint ();
					}
						
				}

			//display transition info for selected transition
			} else if (selectionType == SelectionType.stateTransition) {
				GUILayout.Label ("Transition selected.");
				DrawTransitionPanelInfo (stateNodes [selectedState].transitions [selectedTransition]);

			} else if (selectionType == SelectionType.forkTransition) {
				GUILayout.Label ("Transition selected.");
				DrawTransitionPanelInfo (forkNodes [selectedFork].transitions [selectedTransition]);  //
			} else if(selectionType != SelectionType.parameters){
				GUILayout.Label ("No Selection.");
			}


			//add debug button to the menu
			if (GUILayout.Button ("Debug")) {
				DebugButtonClick ();
			}

		}

	}


	void DrawTransitionPanelInfo(TransitionArrow transitionArrow){

		StateMachine.TransitionCondition.Type transType;

		StateMachine.Transition transition;

		transition = new StateMachine.Transition(); //overwritten if a transition can be found.



		GUILayout.Label ("Transition " + transitionArrow.transitionIndex.ToString ());
	
		//Find transition in statemachine for easier reference
		if (transitionArrow.sourceIsState) {
			if(stateMachine.states [transitionArrow.sourceState].transitions [transitionArrow.transitionIndex]!=null){
				transition = stateMachine.states [transitionArrow.sourceState].transitions [transitionArrow.transitionIndex];
			} else {
				Debug.Log("transition can't be found");
			}
		} else {
			if(stateMachine.forks [transitionArrow.sourceFork].transitions [transitionArrow.transitionIndex] !=null){
				transition = stateMachine.forks [transitionArrow.sourceFork].transitions [transitionArrow.transitionIndex];
			} else {
				Debug.Log("transition can't be found");
			}
		}
			
		//Display transition data based on source

		if (!transitionArrow.sourceIsState) { //fork source
			if (forkNodes [transitionArrow.sourceFork].forkType == StateMachine.Fork.Type.Markov) {
				float weight;
				weight = transition.markovTransitionWeight;
				weight = EditorGUILayout.FloatField ("Markov transition weight:", weight);
				if (weight != transition.markovTransitionWeight) {
					transition.markovTransitionWeight = weight;
					Repaint ();
				}
			}  else if (forkNodes [transitionArrow.sourceFork].forkType == StateMachine.Fork.Type.Priority) {
				float priority = transition.transitionPriority;
				priority = EditorGUILayout.FloatField ("Priority:",priority);
				if (priority != transition.transitionPriority) {
					transition.transitionPriority = priority;
					Repaint ();
				}

				if (transition.transitionConditions.Count > 0) {
					for (int i = 0; i < transition.transitionConditions.Count; i++) {
						DrawTransitionCondition (transitionArrow, transition, i);
					}
				}

				if (GUILayout.Button("Add Condition")) {
					transition.transitionConditions.Add (new StateMachine.TransitionCondition ());
				}

			} else if (forkNodes [transitionArrow.sourceFork].forkType == StateMachine.Fork.Type.Chain) {

				if (transition.transitionConditions.Count > 0) {
					for (int i = 0; i < transition.transitionConditions.Count; i++) {
						DrawTransitionCondition (transitionArrow, transition, i);
					}
				}

				if (GUILayout.Button("Add Condition")) {
					transition.transitionConditions.Add (new StateMachine.TransitionCondition ());
				}

				/*
				//display transition type selector
				transType = transition.type;
				transType = (StateMachine.Transition.Type)EditorGUILayout.EnumPopup (transType);
				if (transition.type != transType) {
					transition.type = transType;
					transition.parameterName = "";
					Repaint ();
				}

				//display info for this transition type
				if (transition.type == StateMachine.Transition.Type.Boolean) { 
					DrawBoolTransitionInfo(transitionArrow, transition);
						
				} else if (transition.type == StateMachine.Transition.Type.Direct) {
					//nothing to display
				} else if (transition.type == StateMachine.Transition.Type.Float) { //------------------------------------TODO
					DrawFloatTransitionInfo(transitionArrow, transition);
				} else if (transition.type == StateMachine.Transition.Type.Int) { //------------------------------------TODO
					DrawIntTransitionInfo(transitionArrow, transition);
				} else if (transition.type == StateMachine.Transition.Type.Time) {
					transition.transitionTime = EditorGUILayout.FloatField ("Transition delay:", transition.transitionTime);
				}*/



			} else {
				Debug.LogWarning ("You need to define the UI display for " + forkNodes [transitionArrow.sourceFork].forkType.ToString ());
			}
		} else { //transition is from state  
			//display transition type selector

			if (transition.transitionConditions.Count > 0) {
				for (int i = 0; i < transition.transitionConditions.Count; i++) {
					DrawTransitionCondition (transitionArrow, transition, i);
				}
			}

			if (GUILayout.Button("Add Condition")) {
				transition.transitionConditions.Add (new StateMachine.TransitionCondition ());
			}

			/*
			transType = transition.type;
			transType = (StateMachine.Transition.Type)EditorGUILayout.EnumPopup (transType);
			if (transition.type != transType) {
				transition.type = transType;
				transition.parameterName = "";
				Repaint ();
			}
			//display info for this transition type

			if (transition.type == StateMachine.Transition.Type.Boolean) { 
				DrawBoolTransitionInfo (transitionArrow, transition);

			} else if (transition.type == StateMachine.Transition.Type.Direct) {
				//nothing to display
			} else if (transition.type == StateMachine.Transition.Type.Float) { //------------------------------------TODO
				DrawFloatTransitionInfo(transitionArrow, transition);
			} else if (transition.type == StateMachine.Transition.Type.Int) { 
				DrawIntTransitionInfo(transitionArrow, transition);
			} else if (transition.type == StateMachine.Transition.Type.Time) {
				transition.transitionTime = EditorGUILayout.FloatField ("Transition delay:", transition.transitionTime);
			}*/



		}
			

	}

	void DrawTransitionCondition(TransitionArrow transitionArrow, StateMachine.Transition transition, int conditionIndex){
		StateMachine.TransitionCondition.Type transType;

		GUIStyle rightAlignedButton = new GUIStyle ();
		rightAlignedButton.alignment = TextAnchor.MiddleRight;
		rightAlignedButton.normal.background = EditorGUIUtility.Load ("CollabDeleted Icon") as Texture2D;
		rightAlignedButton.fixedWidth = 24.0f;
		rightAlignedButton.fixedHeight = 24.0f;

		using (var horiz = new EditorGUILayout.HorizontalScope ()) {
			//display transition type selector
			transType = transition.transitionConditions [conditionIndex].type;
			transType = (StateMachine.TransitionCondition.Type)EditorGUILayout.EnumPopup (transType);
			if (transition.transitionConditions [conditionIndex].type != transType) {
				transition.transitionConditions [conditionIndex].type = transType;
				transition.transitionConditions [conditionIndex].parameterName = "";
				Repaint ();
			}
			//display 'delete' button
			if (GUILayout.Button ("  ", rightAlignedButton)) {
				transition.transitionConditions.RemoveAt (conditionIndex);
				return;
			}
		}

		//display info for this transition type
		if (transition.transitionConditions[conditionIndex].type == StateMachine.TransitionCondition.Type.Boolean) { 
			DrawBoolTransitionInfo(transitionArrow, transition, conditionIndex);
		} else if (transition.transitionConditions[conditionIndex].type == StateMachine.TransitionCondition.Type.Float) {
			DrawFloatTransitionInfo(transitionArrow, transition, conditionIndex);
		} else if (transition.transitionConditions[conditionIndex].type == StateMachine.TransitionCondition.Type.Int) {
			DrawIntTransitionInfo(transitionArrow, transition, conditionIndex);
		} else if (transition.transitionConditions[conditionIndex].type == StateMachine.TransitionCondition.Type.Time) {
			transition.transitionConditions[conditionIndex].transitionTime = EditorGUILayout.FloatField ("Transition delay:", transition.transitionConditions[conditionIndex].transitionTime);
		}

	}


	void DrawBoolTransitionInfo(TransitionArrow transitionArrow, StateMachine.Transition transition, int conditionIndex){
		string displayString = transition.transitionConditions[conditionIndex].parameterName;
		if (displayString == "") {
			displayString = "No parameter selected";
		}

		using(var horiz = new EditorGUILayout.HorizontalScope()){

			if (EditorGUILayout.DropdownButton (new GUIContent (displayString), FocusType.Passive)) {
				GenericMenu paramMenu = new GenericMenu ();
				foreach (var param in stateMachine.parameters) {
					if (param.type == StateMachine.Parameter.Type.Bool) {
						paramMenu.AddItem (new GUIContent (param.name), false, () => {transition.transitionConditions[conditionIndex].parameterName = param.name;});
					}
				}
				paramMenu.ShowAsContext ();
			}

			EditorGUILayout.LabelField ("Transition if false:");
			transition.transitionConditions[conditionIndex].invertToggle = EditorGUILayout.Toggle (transition.transitionConditions[conditionIndex].invertToggle);

		}
	}



	void DrawFloatTransitionInfo(TransitionArrow transitionArrow, StateMachine.Transition transition, int conditionIndex){
		string displayString = transition.transitionConditions[conditionIndex].parameterName;
		if (displayString == "") {
			displayString = "No parameter selected";
		}

		using (var horiz = new EditorGUILayout.HorizontalScope ()) {
			if (EditorGUILayout.DropdownButton (new GUIContent (displayString), FocusType.Passive)) {
				GenericMenu paramMenu = new GenericMenu ();
				foreach (var param in stateMachine.parameters) {
					if (param.type == StateMachine.Parameter.Type.Float) {
						paramMenu.AddItem (new GUIContent (param.name), false, () => {transition.transitionConditions[conditionIndex].parameterName = param.name;});
					}
				}
				paramMenu.ShowAsContext ();
			}

			transition.transitionConditions[conditionIndex].compareType = (StateMachine.TransitionCondition.CompareType) EditorGUILayout.EnumPopup (transition.transitionConditions[conditionIndex].compareType);

			transition.transitionConditions[conditionIndex].floatParamThreshold = EditorGUILayout.FloatField (transition.transitionConditions[conditionIndex].floatParamThreshold);
		}
	}



	void DrawIntTransitionInfo(TransitionArrow transitionArrow, StateMachine.Transition transition, int conditionIndex){
		string displayString = transition.transitionConditions[conditionIndex].parameterName;
		if (displayString == "") {
			displayString = "No parameter selected";
		}

		using (var horiz = new EditorGUILayout.HorizontalScope ()) {
			if (EditorGUILayout.DropdownButton (new GUIContent (displayString), FocusType.Passive)) {
				GenericMenu paramMenu = new GenericMenu ();
				foreach (var param in stateMachine.parameters) {
					if (param.type == StateMachine.Parameter.Type.Int) {
						paramMenu.AddItem (new GUIContent (param.name), false, () => {transition.transitionConditions[conditionIndex].parameterName = param.name;});
					}
				}
				paramMenu.ShowAsContext ();
			}

			transition.transitionConditions[conditionIndex].compareType = (StateMachine.TransitionCondition.CompareType) EditorGUILayout.EnumPopup (transition.transitionConditions[conditionIndex].compareType);

			transition.transitionConditions[conditionIndex].intParamThreshold = EditorGUILayout.IntField (transition.transitionConditions[conditionIndex].intParamThreshold);
		}
	}



	private void DrawResizer(){
		sidePanelResizer = new Rect ((position.width * sidePanelWidth) - 5f, 0, 10f, position.height);

		GUILayout.BeginArea (new Rect (sidePanelResizer.position + Vector2.right * 5f, new Vector2 (2, position.height)), resizerStyle);
		GUILayout.EndArea();

		EditorGUIUtility.AddCursorRect (sidePanelResizer, MouseCursor.ResizeHorizontal);
	}

	//--End of Draw Functions----------------------------------------------------

	//--Event Handling-------------------------------------------------------------------------------------

    private void ProcessEvents(Event e)
    {
        drag = Vector2.zero;

        switch (e.type)
        {
		case EventType.MouseDown:
			if (e.button == 0 && sidePanelResizer.Contains (e.mousePosition)) {
				isResizing = true;
				break;
			}
				
			if (e.button == 0) {
				if (stateMachine != null) {
					if (workingTransition != null) {
						foreach (var node in stateNodes) {
							if(node.Key!=StateMachine.initialState){
								if (node.Value.rect.Contains (e.mousePosition)) {
									OnClickFinalizeTransition (node.Key);
									break;
								}
							}
						}

						foreach (var node in forkNodes) {
							if (node.rect.Contains (e.mousePosition)) {
								OnClickFinalizeTransition (node.forkIndex);
								break;
							}
						}

					}
				}
			}

			if (e.button == 1) {
				if (stateMachine != null){
					bool nodeClick = false;
		
					foreach(var node in stateNodes){
						if (node.Value.rect.Contains (e.mousePosition) && node.Value.isSelected &!nodeClick) {
							ProcessStateNodeContextMenu (e.mousePosition, node.Key);
							nodeClick = true;
							break;
						}
							
						foreach (var trans in node.Value.transitions) {
							if (trans.arrowRect.Contains (e.mousePosition) && !nodeClick) {
								ProcessTransitionContextMenu (e.mousePosition, node.Key, trans.transitionIndex);
								nodeClick = true; 
								break;
							}
						}
					}

					foreach (var node in forkNodes) {
						if (node.rect.Contains(e.mousePosition) && node.isSelected &! nodeClick){
							ProcessForkNodeContextMenu(e.mousePosition, node.forkIndex);
							nodeClick = true;
							break;
						}

						foreach (var trans in node.transitions) {
							if (trans.arrowRect.Contains (e.mousePosition) && !nodeClick) {
								ProcessTransitionContextMenu (e.mousePosition, node.forkIndex, trans.transitionIndex);
								nodeClick = true; 
								break;
							}
						}
					}

					if (!nodeClick) {
						ProcessContextMenu (e.mousePosition);
					}
				}
					
            }
            break;

            case EventType.MouseDrag:
                if (e.button == 0)
                {
                    OnDrag(e.delta);
                }
            break;

			case EventType.MouseUp:
				isResizing = false;
			break;
        }


		Resize (e);
    }


	private void Resize(Event e){
		if (isResizing) {
			sidePanelWidth = e.mousePosition.x/ position.width;
			Repaint();
		}
	}
		
    private void ProcessContextMenu(Vector2 mousePosition)
    {
		GenericMenu genericMenu = new GenericMenu ();
		genericMenu.AddItem (new GUIContent ("Add State Node"), false, () => OnClickAddState(mousePosition));
		genericMenu.AddItem (new GUIContent ("Add Fork Node"), false, () => OnClickAddFork (mousePosition));
		genericMenu.ShowAsContext ();

    }

	private void ProcessStateNodeContextMenu(Vector2 mousePosition, string nodeKey){
		GenericMenu genericMenu = new GenericMenu ();
		if(nodeKey !=StateMachine.initialState){
			genericMenu.AddItem (new GUIContent ("Remove State"), false, () => OnClickRemoveState(nodeKey));
		}
		genericMenu.AddItem (new GUIContent ("Add Transition"), false, () => OnClickBeginTransition (nodeKey)); 
		genericMenu.ShowAsContext ();
	}
		
	private void ProcessForkNodeContextMenu(Vector2 mousePosition, int forkIndex){
		GenericMenu genericMenu = new GenericMenu ();
		genericMenu.AddItem (new GUIContent ("Remove Fork"), false, () => OnClickRemoveFork(forkIndex));
		genericMenu.AddItem (new GUIContent ("Add Transition"), false, () => OnClickBeginTransition (forkIndex)); 
		genericMenu.ShowAsContext ();
	}

	private void ProcessTransitionContextMenu(Vector2 mousePosition, string nodekey, int transitionIndex){
		GenericMenu genericMenu = new GenericMenu ();
		genericMenu.AddItem(new GUIContent ("Remove Transition"), false, () => OnClickRemoveTransition(nodekey, transitionIndex));
		genericMenu.ShowAsContext ();
	}

	private void ProcessTransitionContextMenu(Vector2 mousePosition, int forkIndex, int transitionIndex){
		GenericMenu genericMenu = new GenericMenu ();
		genericMenu.AddItem(new GUIContent ("Remove Transition"), false, () => OnClickRemoveTransition(forkIndex, transitionIndex));
		genericMenu.ShowAsContext ();
	}


	//executes when canvas is dragged
	void OnDrag(Vector2 mouseDelta){
		drag = mouseDelta;
		if (stateMachine != null) {
			foreach (var node in stateNodes) {
				node.Value.Drag (mouseDelta, this);

			}
			foreach (var fork in forkNodes) {
				fork.Drag (mouseDelta, this);
			}
		}

	}

	void OnClickAddState(Vector2 mousePosition){
		if (stateMachine != null) {
			StateMachine.State newState = new StateMachine.State ();
			newState.editorPosition = mousePosition;
			//find unique name for new state
			int highestMatch = -1;
			foreach (var state in stateMachine.states) {
				var match = Regex.Match (state.Key, "(newState)(\\d*)");
				if(match.Groups.Count>1){
					if(string.Equals(match.Groups[2].ToString(),"")){
						highestMatch=0;
					} else{
						if(int.Parse(match.Groups[2].ToString())>highestMatch){
							highestMatch = int.Parse(match.Groups[2].ToString());
						}
					}
				}
			}

			highestMatch += 1;
			stateMachine.states.Add ("newState" + highestMatch.ToString (), newState);
			StateNode newStateNode = new StateNode (mousePosition, stateNodeSize);
			stateNodes.Add ("newState" + highestMatch.ToString (), newStateNode);
		}

		Repaint ();
	}

	void OnClickRemoveState(string stateKey){
		stateNodes.Remove (stateKey);
		stateMachine.states.Remove (stateKey);

		for (int i = 0; i < forkNodes.Count; i++) {
			forkNodes [i].forkIndex = i;
		}

		CheckNullTransitions ();

		selectionType = SelectionType.noSelection;
		workingTransition = null;

		Repaint ();
	}

	void OnClickAddFork(Vector2 mousePosition){
		if (stateMachine != null) {
			StateMachine.Fork newFork = new StateMachine.Fork ();
			newFork.editorPosition = mousePosition;
			stateMachine.forks.Add (newFork);
			ForkNode newForkNode = new ForkNode (mousePosition, forkNodeSize);
			forkNodes.Add (newForkNode);
			forkNodes [forkNodes.Count - 1].forkIndex = forkNodes.Count - 1;
		}

		Repaint ();
	}

	void OnClickRemoveFork(int forkIndex){
		ClearTransitionsToFork (forkIndex);
		
		
		forkNodes.RemoveAt (forkIndex);
		stateMachine.forks.RemoveAt (forkIndex);
		for (int i = forkIndex; i < forkNodes.Count; i++) {
			forkNodes [i].forkIndex = i;
		}

		selectionType = SelectionType.noSelection;
		workingTransition = null;
		
		for(int i=0; i<forkNodes.Count; i++){
			foreach(var transition in forkNodes[i].transitions){
				transition.sourceFork =forkNodes[i].forkIndex;
			}
		}
		
		//stateMachine.VerifyIntegrity(); //deleting a fork that transitions to a fork was causing issues. This fixed it.

		Repaint ();
	}

	void OnClickBeginTransition(int forkIndex){
		workingTransition = new TransitionArrow (forkIndex, -1);
		Debug.Log ("transition begun from " + forkIndex.ToString ());
		Repaint ();
	}

	void OnClickBeginTransition(string stateID){
		workingTransition = new TransitionArrow (stateID, -1);
		Debug.Log ("transition begun from " + stateID);
		Repaint ();
	}

	void OnClickFinalizeTransition(int forkIndex){
		//check that transition is valid

		//block transition to self
		if (!workingTransition.sourceIsState && workingTransition.sourceFork == forkIndex) {
			workingTransition = null;
			return;
		}

		//block duplicate transitions
		if (workingTransition.sourceIsState) {
			foreach (var arrow in stateNodes[workingTransition.sourceState].transitions) {
				if (!arrow.targetIsState && arrow.targetFork == forkIndex) {
					workingTransition = null;
					return;
				}
			}
		} else {
			foreach (var arrow in forkNodes[workingTransition.sourceFork].transitions) {
				if (!arrow.targetIsState && arrow.targetFork == forkIndex) {
					workingTransition = null;
					return;
				}
			}
		}


		workingTransition.SetTarget (forkIndex);
		StateMachine.Transition newTrans = new StateMachine.Transition ();
		newTrans.target = stateMachine.forks [forkIndex];

		if (workingTransition.sourceIsState) {
			workingTransition.transitionIndex = stateNodes [workingTransition.sourceState].transitions.Count;
			stateNodes [workingTransition.sourceState].transitions.Add (workingTransition);
			stateMachine.states [workingTransition.sourceState].transitions.Add (newTrans);
		} else {
			workingTransition.transitionIndex = forkNodes [workingTransition.sourceFork].transitions.Count;
			forkNodes [workingTransition.sourceFork].transitions.Add (workingTransition);
			stateMachine.forks [workingTransition.sourceFork].transitions.Add (newTrans);
		}
			
		workingTransition = null;

		Debug.Log ("transition finalized.");
	}

	void OnClickFinalizeTransition(string stateID){

		//block transition to self
		if (workingTransition.sourceIsState && workingTransition.sourceState == stateID) {
			workingTransition = null;
			return;
		}

		//block duplicate transitions
		if (workingTransition.sourceIsState) {
			foreach (var arrow in stateNodes[workingTransition.sourceState].transitions) {
				if (arrow.targetIsState && string.Equals(arrow.targetState, stateID)) {
					workingTransition = null;
					return;
				}
			}
		} else {
			foreach (var arrow in forkNodes[workingTransition.sourceFork].transitions) {
				if (arrow.targetIsState && string.Equals(arrow.targetState, stateID)) {
					workingTransition = null;
					return;
				}
			}
		}

		workingTransition.SetTarget (stateID);
		StateMachine.Transition newTrans = new StateMachine.Transition ();
		newTrans.target = stateMachine.states [stateID];

		if (workingTransition.sourceIsState) {
			workingTransition.transitionIndex = stateNodes [workingTransition.sourceState].transitions.Count;
			stateNodes [workingTransition.sourceState].transitions.Add (workingTransition);
			stateMachine.states [workingTransition.sourceState].transitions.Add (newTrans);
		} else {
			workingTransition.transitionIndex = forkNodes [workingTransition.sourceFork].transitions.Count;
			forkNodes [workingTransition.sourceFork].transitions.Add (workingTransition);
			stateMachine.forks [workingTransition.sourceFork].transitions.Add (newTrans);
		}

		workingTransition = null;

		Debug.Log ("transition finalized.");
	}

	void OnClickRemoveTransition(string stateID, int transitionIndex){
		stateNodes [stateID].transitions.RemoveAt (transitionIndex);
		stateMachine.states [stateID].transitions.RemoveAt (transitionIndex);
		for (int i = 0; i < stateNodes [stateID].transitions.Count; i++) {
			stateNodes [stateID].transitions [i].transitionIndex = i;
		}
		selectionType = SelectionType.noSelection;
		workingTransition = null;
		Repaint ();
	}

	void OnClickRemoveTransition(int forkIndex, int transitionIndex){
		forkNodes [forkIndex].transitions.RemoveAt (transitionIndex);
		stateMachine.forks [forkIndex].transitions.RemoveAt (transitionIndex);
		for (int i = 0; i < forkNodes [forkIndex].transitions.Count; i++) {
			forkNodes [forkIndex].transitions [i].transitionIndex = i;
		}
		selectionType = SelectionType.noSelection;
		workingTransition = null;
		Repaint ();
	}


	void ProcessNodeEvents(Event e){
		if (stateMachine != null) {
			foreach (var node in stateNodes) {
				node.Value.ProcessEvents (e, this);
			}
			foreach (var node in forkNodes) {
				node.ProcessEvents (e, this);
			}
		}
		//Transition events are processed locally by the statenodes and forknodes, so this function processes states, nodes, and transitions.
	}

	void DebugButtonClick(){
		Debug.Log ("button pressed.");
		Debug.Log (selectionType);
		Debug.Log (stateMachine.name + " has "+ stateMachine.states.Count.ToString() + " states.");
		Debug.Log ("forknodes has " + forkNodes.Count.ToString () + " forks");
		Debug.Log ("statenodes has " + stateNodes.Count.ToString () + " states");

		sidePanelStyle = new GUIStyle (); //this gets deleted on play???
		sidePanelStyle.normal.background = new Texture2D (1, 1);
		sidePanelStyle.normal.background.SetPixel (0, 0, Color.gray);
		sidePanelStyle.normal.background.Apply (false);

		stateMachine.debugString = "set from editor button"; 

		foreach (var node in stateNodes) {
			if (node.Value.transitions.Count == 0) {
				Debug.Log ("State " + node.Key + " has no transitions.");
			}

			for (int i=0; i<node.Value.transitions.Count; i++){
				if (node.Value.transitions [i].targetIsState) {
					Debug.Log ("State " + node.Key + " transitions to " + node.Value.transitions [i].targetState);
				} else {
					Debug.Log ("State " + node.Key + " transitions to fork " + node.Value.transitions [i].targetFork.ToString());
				}
			}
		}
		foreach (var node in forkNodes) {
			if (node.transitions.Count == 0) {
				Debug.Log ("Fork " + node.forkIndex.ToString () + " has no transitions.");
			}

			for (int i=0; i<node.transitions.Count; i++){
				if (node.transitions [i].targetIsState) {
					Debug.Log ("Fork " + node.forkIndex.ToString () + " transitions to " + node.transitions[i].targetState);
				} else {
					Debug.Log ("Fork " + node.forkIndex.ToString () + " transitions to fork " + node.transitions[i].targetFork.ToString());
				}
			}
		}

	}


	//--External utilities------------------------
	public void SetSelectedState(string stateName){
		if (stateMachine.states.ContainsKey (stateName)) {
			selectedState = stateName;
			selectionType = SelectionType.state;
		} else {
			selectedState = "";
			selectionType = SelectionType.noSelection;
		}
	}

	public void SetSelectedFork(int forkIndex){
		if (stateMachine.forks.Count > forkIndex) {
			selectedFork = forkIndex;
			selectionType = SelectionType.fork;
		} else {
			selectedFork = -1;
			selectionType = SelectionType.noSelection;
		}
	}

	public void SetSelectedTransition(string stateName, int transIndex){
		if (stateMachine.states.ContainsKey (stateName)) {
			selectionType = SelectionType.stateTransition;
			selectedState = stateName;
			selectedTransition = transIndex;
			selectedFork = -1;
		} else {
			selectedState = "";
			selectedTransition = -1;
			selectedFork = -1;
			selectionType = SelectionType.noSelection;
		}
	}

	public void SetSelectedTransition(int forkIndex, int transIndex){
		if (stateMachine.forks.Count>forkIndex) {
			selectionType = SelectionType.forkTransition;
			selectedState = "";
			selectedTransition = transIndex;
			selectedFork = forkIndex;
		} else {
			selectedState = "";
			selectedTransition = -1;
			selectedFork = -1;
			selectionType = SelectionType.noSelection;
		}
	}



	//--Internal utilities-------------------------------------------------
	void BuildUIFromMachine(){

		stateMachine.VerifyIntegrity ();

		//Debug.Log ("Attempting to build state machine");
		StateNode stateNode;
		ForkNode forkNode;
		TransitionArrow transitionArrow;

		stateNodes.Clear ();
		forkNodes.Clear ();

		if (stateMachine.states != null) {
			foreach (var state in stateMachine.states) {
				stateNode = new StateNode (state.Value.editorPosition, stateNodeSize);
				stateNodes.Add (state.Key, stateNode);

				if (state.Value.transitions.Count > 0) {
					for (int i = 0; i < state.Value.transitions.Count; i++) {
						transitionArrow = new TransitionArrow (state.Key, i);
						//Debug.Log (state.Value.transitions [0].target.ToString ());
						if (typeof(StateMachine.State) == state.Value.transitions [i].target.GetType ()) {
							transitionArrow.SetTarget (KeyFromState ((StateMachine.State)state.Value.transitions [i].target));

						} else if (typeof(StateMachine.Fork) == state.Value.transitions [i].target.GetType ()) {
							transitionArrow.SetTarget (stateMachine.forks.IndexOf ((StateMachine.Fork)state.Value.transitions [i].target));
						}

						stateNodes [state.Key].transitions.Add (transitionArrow);
					}
				}

			}
		}

		if (stateMachine.forks != null) {
			for (int i = 0; i < stateMachine.forks.Count; i++) {
				forkNode = new ForkNode (stateMachine.forks [i].editorPosition, forkNodeSize);
				forkNode.forkType = stateMachine.forks [i].type;
				forkNodes.Add (forkNode);
				forkNodes [forkNodes.Count - 1].forkIndex = forkNodes.Count - 1;

				if (stateMachine.forks [i].transitions.Count > 0) {
					for (int j = 0; j < stateMachine.forks [i].transitions.Count; j++) {
						transitionArrow = new TransitionArrow (i, j);
						if (typeof(StateMachine.State) == stateMachine.forks [i].transitions [j].target.GetType ()) {
							transitionArrow.SetTarget (KeyFromState ((StateMachine.State)stateMachine.forks [i].transitions [j].target));
						} else if (typeof(StateMachine.Fork) == stateMachine.forks [i].transitions [j].target.GetType ()) {
							transitionArrow.SetTarget (stateMachine.forks.IndexOf ((StateMachine.Fork)stateMachine.forks [i].transitions [j].target));
						}

						forkNodes [i].transitions.Add (transitionArrow);
					}
				}

			}
		}

		CheckNullTransitions ();
	}

	string KeyFromState(StateMachine.State queryState){
		if (stateMachine != null) {
			if (stateMachine.states != null) {
				foreach(var stateData in stateMachine.states){
					if (Object.Equals (stateData.Value, queryState)) {
						return stateData.Key;
					}
				}
			}
		}
		return "";
	}

	///this will prevent null reference exceptions but cannot correctly resolve a transition to a fork now pointing to the wrong fork.
	void CheckNullTransitions(){
		//removing an element during iteration causes an error in a forEach loop. If an instance is removed, break the loop and recursively call function.
		bool removalfound = false;
		//state machine
		if (stateMachine != null) {
			foreach (var state in stateMachine.states) {
				foreach (var transition in state.Value.transitions) {
					if (transition.target == null) {
						state.Value.transitions.Remove (transition);
						removalfound = true;
						break;
					}
				}
			}

			for (int i = 0; i < stateMachine.forks.Count; i++) {
				for (int j = 0; j < stateMachine.forks [i].transitions.Count; j++) {
					if (stateMachine.forks [i].transitions [j].target == null) {
						stateMachine.forks [i].transitions.RemoveAt (j);
						removalfound = true;
						break;
					}
				}
			}
		}

		//editor
		foreach (var node in stateNodes) {
			foreach (var trans in node.Value.transitions) {
				if (trans.targetIsState) {
					if (!stateNodes.ContainsKey (trans.targetState)) {
						node.Value.transitions.Remove (trans);
						removalfound = true;
						break;
					}
				} else {
					if (trans.targetFork < 0 || trans.targetFork >= stateNodes.Count) {
						node.Value.transitions.Remove (trans);
						removalfound = true;
						break;
					}
				}
			}
		}

		foreach (var node in forkNodes) {
			foreach (var trans in node.transitions) {
				if (trans.targetIsState) {
					if (!stateNodes.ContainsKey (trans.targetState)) {
						node.transitions.Remove (trans);
						removalfound = true;
						break;
					}
				} else {
					if (trans.targetFork < 0 || trans.targetFork >= stateNodes.Count) {
						node.transitions.Remove (trans);
						removalfound = true;
						break;
					}
				}
			}
		}
		if (removalfound) {
			CheckNullTransitions ();
		}

	}

	void ClearTransitionsToFork(int forkIndex){
		//removing an element during iteration causes an error in a forEach loop. If an instance is removed, break the loop and recursively call function.
		bool removalFound = false;


		//stateMachine references
		if (stateMachine != null) {
			foreach (var state in stateMachine.states) {
				foreach (var transition in state.Value.transitions) {
					if (Object.Equals(transition.target, stateMachine.forks[forkIndex])) {
						state.Value.transitions.Remove (transition);
						removalFound = true;
						break;
					}
				}
			}

			for (int i = 0; i < stateMachine.forks.Count; i++) {
				for (int j = 0; j < stateMachine.forks [i].transitions.Count; j++) {
					if (Object.Equals(stateMachine.forks [i].transitions [j].target,stateMachine.forks [forkIndex])) {
						stateMachine.forks [i].transitions.RemoveAt (j);
						removalFound = true;
						break;
					}
				}
			}
		}

		//Node references
		foreach (var node in stateNodes) {
			foreach (var trans in node.Value.transitions) {
				if(trans.targetFork==forkIndex){
					node.Value.transitions.Remove (trans);
					removalFound = true;
					break;
				}
			}
		}

		foreach (var node in forkNodes) {
			foreach (var trans in node.transitions) {
				if (trans.targetFork == forkIndex) {
					node.transitions.Remove (trans);
					removalFound = true;
					break;
				}
			}
				
		}
		if (removalFound) {
			ClearTransitionsToFork (forkIndex);
		}

	}




}
}


//Base code adapted from http://gram.gs/gramlog/creating-node-based-editor-unity/