//This is an editor script for verifying that the yarn files and the scene both have all the information they need.

//Critical tasks:
//verify that all speakers match a speaker in the scene.
//verify that all yarn variables match a variable in COMMnCore
//verify that all function targets match a target in the scene
//verify that all node titles and node references are properly formatted (no spaces)
//Verify that all yarn 

//Non critical tasks:
//verify that a portrait is assigned for every speaker/emote combo.
//verify that every yarn speaker has the same yarn script

//ToDo: I think the while>replace format scales

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Text.RegularExpressions;
using System.IO;
using System;

namespace Akitacore{
public class YarnIntegrityEditor : EditorWindow {

	public TextAsset targetScript;
	public TextAsset[] componentScripts;
	int componentScriptCount;

	string fullText;

	public ScriptableDataLibrary database;
	public SpeakerLibrary speakerLib;
	
	public ScriptableData assetInPath;
	




	[MenuItem("Window/Yarn Integrity Editor")]
	private static void OpenWindow()
	{
		YarnIntegrityEditor window = GetWindow<YarnIntegrityEditor>();
		window.titleContent = new GUIContent("Yarn Integrity Editor");
	}

	//--Monobehavior--------------------------------------------------------------

	//initialization
	void OnEnable(){
		componentScriptCount = 1;
		componentScripts = new TextAsset[componentScriptCount];
	}

	//main update loop
	void OnGUI(){

		//-Asset assignment

		GUIAssetAssignment ();
		GUIButtons ();
			

	}
		
	//-End Monobehavior-
	//--OnGUI subsets------------------
	void GUIAssetAssignment(){
		EditorGUILayout.LabelField ("Yarn Files must end in \".yarn.txt\" for compatibility.");
		targetScript = EditorGUILayout.ObjectField("Target Script (overwrite)", targetScript, typeof(TextAsset), false)as TextAsset;
		using (var horiz = new EditorGUILayout.HorizontalScope ()) {
			EditorGUILayout.LabelField ("# Component Scripts");
			componentScriptCount = EditorGUILayout.IntField (componentScriptCount);
		}
		if (componentScripts == null) {
			componentScripts = new TextAsset[componentScriptCount];
		}
		if (componentScripts.Length != componentScriptCount) {
			if (componentScriptCount < 1) {
				componentScriptCount = 1;
			}
			if (componentScriptCount > 100) {
				componentScriptCount = 100;
			}
			if (componentScripts.Length != componentScriptCount) {
				TextAsset[] temp = new TextAsset[componentScriptCount];
				for (int i = 0; i < temp.Length && i < componentScripts.Length; i++) {
					temp [i] = componentScripts [i];
				}
				componentScripts = temp;
			}
		}
		for (int i = 0; i < componentScripts.Length; i++) {
			componentScripts[i] = EditorGUILayout.ObjectField("Component Script "+i.ToString(), componentScripts[i], typeof(TextAsset), false)as TextAsset;
		}

		
		database = EditorGUILayout.ObjectField("ScriptableDataLibrary", database, typeof(ScriptableDataLibrary), false)as ScriptableDataLibrary;
		speakerLib = EditorGUILayout.ObjectField("Speaker Library", speakerLib, typeof(SpeakerLibrary), false)as SpeakerLibrary;
		
		assetInPath = EditorGUILayout.ObjectField("Variable in Target Folder", assetInPath, typeof(ScriptableData), false)as ScriptableData;
	}

	//----------

	void GUIButtons(){

		if(GUILayout.Button("Log Documentation")){
			LogDocumentation ();
		}

		if(GUILayout.Button("Combine and Load Components to Working Memory")){
			CombineComponents ();
		}
			
		if(GUILayout.Button("Run Diagnostics")){
			RunDiagnostics ();
		}

		//if(GUILayout.Button("Combine, Repair, and Check")){
		//	CombineComponents ();
		//	RepairTitles ();
		//	MatchTitles ();
		//	RunDiagnostics ();
		//}

		//-
		EditorGUILayout.LabelField ("Individual Diagnostics");
		//-

		if(GUILayout.Button("Check Speakers Exist")){
			CheckSpeakers ();
		}

		if(GUILayout.Button("Check Titles")){
			CheckTitles ();
		}
			
		if(GUILayout.Button("Check Scene Dialogue Launchers")){
			CheckDialogueLaunchers ();
		}

		if(GUILayout.Button("Check Command Syntax")){
			CheckCommands ();
		}

		if(GUILayout.Button("Check Variables")){
			CheckVariables ();
		}

		//-
		EditorGUILayout.LabelField ("Individual Repairs");
		//-

		if(GUILayout.Button("Repair Titles")){
			RepairTitles ();
		}

		if(GUILayout.Button("Match Titles")){
			MatchTitles ();
		}
		
		if(GUILayout.Button("Repair Octothorps")){
			RepairOctothorps ();
		}
		
		if(GUILayout.Button("Create Variables")){
			CreateVariables ();
		}
		
		if(GUILayout.Button("Initialize Variables")){
			InitializeVariables ();
		}

		EditorGUILayout.LabelField ("When finished");

		if(GUILayout.Button("Write to Target Script")){
			WriteToTarget ();
		}



	}

	//---------
	void RunDiagnostics(){
		if (fullText == null | fullText == "") {
			CombineComponents ();
		}
		CheckTitles ();//make
		CheckCommands ();

		CheckSpeakers (); 
		CheckVariables ();
		CheckDialogueLaunchers ();
	}


	//-Single Functions-------------------------------------------------------------------------------------------

	//----------

	void TestLog(){
		string textTest = targetScript.text;
		Debug.Log (textTest.Length);
	}

	void LogDocumentation(){
		string documentation = "---YARN INTEGRITY EDITOR COMMAND LINE DOCUMENTATION---\n";
		documentation = documentation+"This editor utility checks yarn scripts and ensures they don't contain errors, which would only show up at runtime.\n";
		documentation = documentation+"\n";
		documentation = documentation+"For basic use, assign a yarn script to Target Script.\n";
		documentation = documentation+"If you have multiple scripts you wish to combine, you can add them as Component Scripts.\n";
		documentation = documentation+"\n";
		documentation = documentation+"BUTTON: \"Combine and Load Components into Working Memory\"\n";
		documentation = documentation+"This will write all data from the components and target script into a working repository for debugging.\n";
		documentation = documentation+"The editor will not produce results until you load the scripts this way.\n";
		documentation = documentation+"\n";
		documentation = documentation+"BUTTON: \"Run Diagnostics\"\n";
		documentation = documentation+"Execute every Individial Diagnostic and print results.\n";
		documentation = documentation+"\n";
		documentation = documentation+"--INDIVIDUAL DIAGNOSTICS--\n";
		documentation = documentation+"\n";
		documentation = documentation+"BUTTON: \"Check Speakers Exist\"\n";
		documentation = documentation+"This will search the scene for every speaker in the yarn script and return a warning if a speaker cannot be found.\n";
		documentation = documentation+"A missing speaker will not cause an error as long as that dialogue is unreachable, eg, if part of the script is meant to be executed in a different scene.\n";
		documentation = documentation+"\n";
		documentation = documentation+"BUTTON: \"Check Titles\"\n";
		documentation = documentation+"Yarn node titles should not contain spaces and should be unique.\n";
		documentation = documentation+"Titles in links should not contain spaces and should exist.\n";
		documentation = documentation+"This will log an error if any of these conditions do not hold.\n";
		documentation = documentation+"\n";
		documentation = documentation+"BUTTON: \"Check Scene Dialogue Launchers\"\n";
		documentation = documentation+"Logs a warning if any dialogue launcher in the scene launches a different script.\n";
		documentation = documentation+"Logs a warning if the specified start node is not a node title in this script.\n";
		documentation = documentation+"\n";
		documentation = documentation+"BUTTON: \"Check Command Syntax\"\n";
		documentation = documentation+"Logs a warning if anything formatted as a yarn command does not match the syntax of a command in the yarn command library. ";
		documentation = documentation+"\n";
		documentation = documentation+"BUTTON: \"Check Variables\"\n";
		documentation = documentation+"Logs a warning if any variable referenced in the yarn script is not contained within the AllScriptableData database.\n";
		



		Debug.Log(documentation);
	}

	//----------

	void CombineComponents(){
		fullText = "";
		foreach (var component in componentScripts) {
			if (component != null) {
				fullText = fullText + component.text + "\n";
			}
		}
		if (fullText == "") {
			fullText = fullText + targetScript.text;
		}
		Debug.Log ("Component scripts written to working memory. Total length: " + fullText.Length.ToString());
	}

	//----------

	void CheckSpeakers(){
		if (fullText.Length > 0) {
			var matches = Regex.Matches (fullText, "\\n\\s*(\\w[\\w\\s]+)\\:"); //newline, then any number of spaces, then at least one word character, followed by whatever until :

			List<string> foundNames;
			foundNames = new List<string> ();


			var speakers = speakerLib.speakers;
			string name = "";

			Debug.Log ("##---------Checking Yarn Speaker names.");

			int errors = 0;

			for (int i = 0; i < matches.Count; i++) {
				name = matches [i].Groups [1].ToString();
				if (name != "title" && name != "tags" && name != "colorID" && name != "position") {
					bool speakerFound = false;
					foreach (var speaker in speakers) {
						if (speaker.speaker == name) {
							speakerFound = true;
						}
					}
					if (!speakerFound) {
						if (!foundNames.Contains (name)) {
							Debug.LogWarning ("\""+name + "\" does not match the name of an active Yarn Speaker in the scene.");
						}
						errors++;
					}
					if (!foundNames.Contains (name)) {
						foundNames.Add (name);
					}
				}
			}
			Debug.Log ("##---------Finished checking Yarn Speaker names. " + errors + " lines with an improperly configured speaker were found.");
		}
	}

	void CheckDialogueLaunchers(){
		if (fullText.Length > 0) {
			Debug.Log ("##---------Checking Dialogue launchers.");
			var Interact = GameObject.FindObjectsOfType<InteractDialogue> ();
			var Ability = GameObject.FindObjectsOfType<AblDialogue> ();
			var Enable = GameObject.FindObjectsOfType<DialogueOnEnable> ();

			var matches = Regex.Matches (fullText, "title: (\\w*)");
			var matchTitles = new List<string>();

			for (int i=0; i<matches.Count; i++){
				matchTitles.Add(matches[i].Groups[1].ToString());
			}

			foreach(var interact in Interact){
				if (interact.yarnScript.name != targetScript.name){
					Debug.LogWarning(interact.gameObject.name+"'s InteractDialogue.cs references yarn script: "+interact.yarnScript.name+", not "+targetScript.name);
				}
				if(interact.startNode == null){
					Debug.LogWarning(interact.gameObject.name+"'s InteractDialogue.cs has an unassigned startNode.");
				} else {
					if(!matchTitles.Contains(interact.startNode.val)){
						Debug.LogWarning(interact.gameObject.name+"'s InteractDialogue.cs references start node: "+interact.startNode.val+" which does not match the title of a node in "+targetScript.name);
					}
				}
			}

			foreach(var ability in Ability){
				if (ability.yarnScript.name != targetScript.name){
					Debug.LogWarning(ability.gameObject.name+"'s InteractDialogue.cs references yarn script: "+ability.yarnScript.name+", not "+targetScript.name);
				}
				if(ability.startNode == null){
					Debug.LogWarning(ability.gameObject.name+"'s InteractDialogue.cs has an unassigned startNode.");
				} else {
					if(!matchTitles.Contains(ability.startNode.val)){
						Debug.LogWarning(ability.gameObject.name+"'s InteractDialogue.cs references start node: "+ability.startNode.val+" which does not match the title of a node in "+targetScript.name);
					}
				}
			}

			foreach(var enable in Enable){
				if (enable.yarnScript.name != targetScript.name){
					Debug.LogWarning(enable.gameObject.name+"'s InteractDialogue.cs references yarn script: "+enable.yarnScript.name+", not "+targetScript.name);
				}
				if(enable.startNode == null){
					Debug.LogWarning(enable.gameObject.name+"'s InteractDialogue.cs has an unassigned startNode.");
				} else {
					if(!matchTitles.Contains(enable.startNode.val)){
						Debug.LogWarning(enable.gameObject.name+"'s InteractDialogue.cs references start node: "+enable.startNode.val+" which does not match the title of a node in "+targetScript.name);
					}
				}
			}
			Debug.Log ("##---------Finished checking Dialogue launchers.");
				
		}
	}

	void CheckTitles(){
		if (fullText.Length > 0) {
			var matches = Regex.Matches (fullText, "title: ([\\w\\s]*)\\n");
			List<string> foundTitles;
			foundTitles = new List<string> ();

			string title = "";

			Debug.Log ("##---------Checking node titles.");

			for (int i = 0; i < matches.Count; i++) {
				title = matches [i].Groups [1].ToString();
				if(!foundTitles.Contains(title)){
					if(Regex.IsMatch(title, "\\s+")){
						Debug.LogWarning("Title \""+title+"\" is an improperly formatted title: Contains spaces.");
					}
					foundTitles.Add(title);
				} else {
					Debug.LogWarning("Title \""+title+"\" exists as a title more than once.");
				}
			} 

			Debug.Log ("##---------finished checking node titles.");
			Debug.Log ("##---------Checking node links.");

			matches = Regex.Matches (fullText, "(?:(?:\\[\\[|\\[\\[.*\\|)([\\w\\s]+)\\]\\])");
			for (int i = 0; i < matches.Count; i++) {
				title = matches [i].Groups [1].ToString();

				if(!foundTitles.Contains(title)){
					Debug.LogWarning("Link \""+title+"\" refers to a node that does not exist.");
				}
				if(Regex.IsMatch(title, "\\s+")){
					Debug.LogWarning("Link \""+title+"\" is an improperly formatted link to another node: Contains spaces.");
				}
			} 
			Debug.Log ("##---------Finished checking node links.");
		
		}
	}

	void CheckCommands(){
		if (fullText.Length > 0) {
			int errors = 0;
			Debug.Log ("##---------Checking yarn commands.");
			var matches = Regex.Matches (fullText, "(<<(\\w*:?)(?:.*)>>)"); //matches command name
			for (int i = 0; i < matches.Count; i++) {
				if (!YarnCommandLibrary.HasCommand (matches [i].Groups [2].ToString())) {
					Debug.LogWarning (matches [i].Groups [1].ToString () + " is not a valid yarn command.");
					errors++;
				}
			}
			Debug.Log(errors.ToString()+" improperly formatted yarn commands were found.");
			Debug.Log ("##---------Finished checking yarn commands.");
		}
	}

	void CheckVariables(){
		if (fullText.Length > 0) {
			Debug.Log ("##---------Checking yarn variables.");
			var matches = Regex.Matches (fullText, " \\$([\\w]*)"); //matches $variable_name
			var foundVars = new List<string>();

			for(int i=0; i<matches.Count; i++){
				if(!foundVars.Contains(matches[i].Groups[1].ToString())){
					foundVars.Add(matches[i].Groups[1].ToString());
				}
			}
			

			var unmatchedVars = new List<string>();
			//unmatchedVars = foundVars;
			foreach(var v in foundVars){
				unmatchedVars.Add(v);	
			}

			foreach(var entry in foundVars){
				foreach(var v in database.boolVars){
					if (v.name == entry){
						unmatchedVars.Remove(v.name);
					}
				}
				foreach(var v in database.floatVars){
					if (v.name == entry){
						unmatchedVars.Remove(v.name);
					}
				}
				foreach(var v in database.intVars){
					if (v.name == entry){
						unmatchedVars.Remove(v.name);
					}
				}
				foreach(var v in database.stringVars){
					if (v.name == entry){
						unmatchedVars.Remove(v.name);
					}
				}
			}

			foreach(string unmatched in unmatchedVars){
				Debug.Log("No ScriptableData found for $"+unmatched);
			}
			



			/*
			COMMnCore commn = GameObject.FindObjectOfType<COMMnCore>();
			//commn.Startup(); //this should theoretically build the flag list. //wasn't working correctly, try running it during play mode.
			var commnVars = new List<string>();


			foreach(var flag in commn.commnLib.workingFlags){
				if(!commnVars.Contains(flag.Key)){
					commnVars.Add(flag.Key);
				}
			}


			foreach(string yarnVar in foundVars){
				if(!commnVars.Contains(yarnVar)){
					Debug.LogWarning("$"+yarnVar+" does not match the name of a variable in CommnCore. Check that the gameobject that intitializes the variable is enabled, or fix the variable.");
				}
			}*/
			Debug.Log ("##---------Finished checking yarn variables.");
			

		}
	}
		


	//this is running into scale problems and lagging indefinitely on larger data sets.
	void RepairTitles(){
		if (fullText.Length > 0) {


			//repair links to titles

			int i = 0;
			int iMax = 10;
		
			Debug.Log ("##---------Repairing node titles.------------------------------------------");
			Debug.Log ("##---Repairing links to nodes.");
			//remove leading spaces


			Debug.Log ("#Removing leading spaces.");
			i = Regex.Matches (fullText, "(?<=\\[\\[)( )(?=.*\\]\\])").Count;
			fullText = Regex.Replace (fullText, "(?<=\\[\\[)( )(?=.*\\]\\])", ""); //replace while with replace /w lookahead & lookbehind

			if (i > 0) {
			Debug.Log (i.ToString () + " leading spaces in node titles in direct links removed.");
			}
				
			i = Regex.Matches (fullText, "(?<=\\|)( )(?=.*\\]\\])").Count;
			fullText = Regex.Replace (fullText, "(?<=\\|)( )(?=.*\\]\\])", ""); //syntax only captures | words]], which isn't ideal, but lookbehind needs fixed width.

			if (i > 0) {
				Debug.Log (i.ToString () + " leading spaces in node titles in descriptive links were removed links removed.");
			}

			Debug.Log ("#Capitalizing first letters of links.");
			//repair first letter of title links

	
			fullText = Regex.Replace (fullText, "(?<=\\[\\[)([a-z])(?=.*\\]\\])", "$1".ToUpper ());


			fullText = Regex.Replace (fullText, "(?<=\\|)([a-z])(?=.*\\]\\])", "$1".ToUpper ());

			//repair additional letters of title links

			Debug.Log ("#Capitalizing additional letters of links.");



			//this is similar to the depricated while pattern, but regex.replace fixes every instance of the caps that matches, instead of just fixing one and running the whole while check again.
			//thus, this pattern does batches of "fix everything it can, then fix the next set."
			i = 0;
			//capitalize |Stuff [t]this]]
			while (Regex.IsMatch (fullText, "(?<=\\|)(.* )([a-z])(?=.*\\]\\])")&&i<iMax) { //this is similar to the depricated while pattern, but regex.
				fullText = Regex.Replace (fullText, "(?<=\\|)(.* )([a-z])(?=.*\\]\\])", "$1" + "$2".ToUpper ());
				i++;
				if (i == iMax) {
					Debug.Log ("iteration cap reached."); //!!!
				}
			}

			i = 0;
			//capitalize [[Stuff [t]his]]
			while (Regex.IsMatch (fullText, "(?<=\\[\\[)([\\w\\s]* )([a-z])(?=[\\w\\s]*\\]\\])")&&i<iMax) {
				fullText = Regex.Replace (fullText, "(?<=\\[\\[)([\\w\\s]* )([a-z])(?=[\\w\\s]*\\]\\])", "$1" + "$2".ToUpper ());
				i++;
				if (i == iMax) {
					Debug.Log ("iteration cap reached."); //!!!
				}
			}


			i = 0;
			//remove spaces [[word Word]] -> [[wordWord]] 
			while (Regex.IsMatch(fullText, "\\[\\[.*([ ]).+\\]\\]")&&i<iMax){
				fullText = Regex.Replace(fullText, "(\\[\\[.*)([ ])(.+\\]\\])", "$1$3");i++;
				i++;
				if (i == iMax) {
					Debug.Log ("iteration cap reached.");
				}
			}

			i = 0;
			//remove spaces [[some description|word Word]] -> [[some description|wordWord]] 
			while (Regex.IsMatch(fullText, "\\[\\[.*\\|.+([ ]).+\\]\\]")&&i<iMax){
				fullText = Regex.Replace(fullText, "(\\[\\[.*\\|.+)([ ])(.+\\]\\])", "$1$3");
				i++;
				if (i == iMax) {
					Debug.Log ("iteration cap reached.");
				}
			}




				
			Debug.Log ("##---Repairing titles of nodes.");

			i = 0;

			while (Regex.IsMatch(fullText, "((?:.|[\\n])*title: (?:.+))(?: )([a-z])((?:\\w+)(?:.|[\\n])*)")&&i<iMax){ //capitalize to camelcase
				fullText = Regex.Replace(fullText,"((?:.|[\\n])*title: (?:.+))(?: )([a-z])((?:\\w+)(?:.|[\\n])*)", "$1"+"$2".ToUpper()+"$3"); 
				//var match = Regex.Match(fullText, "((?:.|[\\n])*title: (?:.+))(?: )([a-z])((?:\\w+)(?:.|[\\n])*)");
				//fullText = match.Groups[1].ToString()+match.Groups[2].ToString().ToUpper()+match.Groups[3].ToString();
				i++;
				if (i == iMax) {
					Debug.Log ("iteration cap reached.");
				}
			}

			i = 0;

			//capitalize [[Stuff [t]his]]
			while (Regex.IsMatch(fullText, "((?:.|[\\n])*title: (?:.+))(?: )([a-z])((?:\\w+)(?:.|[\\n])*)")&&i<iMax){ //capitalize to camelcase
				fullText = Regex.Replace(fullText,"((?:.|[\\n])*title: (?:.+))(?: )([a-z])((?:\\w+)(?:.|[\\n])*)", "$1"+"$2".ToUpper()+"$3"); 
				//var match = Regex.Match(fullText, "((?:.|[\\n])*title: )([a-z])((?:\\w+)(?:.|[\\n])*)");
				//fullText = match.Groups[1].ToString()+match.Groups[2].ToString().ToUpper()+match.Groups[3].ToString();
				i++;
				if (i == iMax) {
					Debug.Log ("iteration cap reached."); //!!!
				}
			}

			i = 0;
			while (Regex.IsMatch(fullText, "title: (?:.+)( )(?:\\w+)")&&i<iMax){ //remove title spaces
				fullText = Regex.Replace(fullText, "(title: (?:.+))( )((?:\\w+))", "$1$3");
				i++;
				if (i == iMax) {
					Debug.Log ("iteration cap reached.");
				}
			}

		}
	}

	void MatchTitles(){
		if (fullText.Length > 0) {
			var matches = Regex.Matches (fullText, "title: ([\\w]+)");
			List<string> foundTitles;
			foundTitles = new List<string> ();

			string title = "";

			Debug.Log (matches.Count.ToString () + " matches found for \"title: ([\\\\w]+)\"");
			for (int i = 0; i < matches.Count; i++) {
				Debug.Log (matches [i].Groups [1].ToString ());
				title = matches [i].Groups [1].ToString ();
				if (!foundTitles.Contains (title)) {
					foundTitles.Add (title);
				}
			}
			Debug.Log (foundTitles.Count.ToString () + " titles found");

			//int errorTracker = 0;

			foreach (string thisTitle in foundTitles) {

				bool titlematch = false;
				foreach (string otherTitle in foundTitles) {
					if (otherTitle != thisTitle && String.Equals(thisTitle,otherTitle,StringComparison.OrdinalIgnoreCase)) {
						titlematch = true;
						Debug.LogWarning ("\"" + thisTitle + "\" matches \"" + otherTitle + "\". Manually check references to both, it can't be done automatically.");
					}
				}
				if (!titlematch) {
					fullText = Regex.Replace (fullText, "((?!\\[\\[))(?i)(" + thisTitle + ")(?-i)((?=\\]\\]))", thisTitle);
					fullText = Regex.Replace (fullText, "((?!\\[\\[.+\\|))(?i)(" + thisTitle + ")(?-i)((?=\\]\\]))", thisTitle);
				} 
					
			}
			//Debug.Log (errorTracker.ToString () + " title references were replaced by correctly capitalized titles.");
		}
		Debug.Log ("##---------finished Repairing node titles.------------------------------------------");
	}
	
	void RepairOctothorps(){
		
		Debug.Log("replacing instances of the pound sign \"#\" with \"[octothorp]\" which will not break yarnspinner and will be concerted back at runtime");
		if (fullText.Length > 0) {
			fullText = Regex.Replace(fullText, "(#(?!line))","[octothorp]");
		}
		Debug.Log("Finished replacing octothorps.");
	}
	
	void CreateVariables(){
		Debug.Log("Creating missing variable references.");
		
		if(assetInPath == null){
			Debug.LogWarning("Assign an asset in the target folder to \"Variable in Target Folder\" so that the created variables can be placed correctly.");
			Debug.Log("Aborting creation of missing variable references.");
			return;	
		}
		
		string path;
		path = AssetDatabase.GetAssetPath(assetInPath);
		path = path.Substring(0, path.Length-assetInPath.name.Length-6); //magic number 6 from length of ".asset"
		
		/*
		FloatVar testVar = new FloatVar();
		testVar.init = 5.0f;
		testVar.val = 5.0f;
		AssetDatabase.CreateAsset(testVar,path+"TestFloatVar"+".asset");*/
		
		if(fullText.Length>0){
			//check floats
			var floatMatches = Regex.Matches(fullText, "<<.*\\$(\\w*)\\s*==?\\s*-?\\d*\\.\\d*\\s*>>");
			//check ints
			var intMatches = Regex.Matches(fullText, "<<.*\\$(\\w*)\\s*==?\\s*-?\\d*\\s*>>");
			//check strings
			var stringMatches = Regex.Matches(fullText, "<<.*\\$(\\w*)\\s*==?\\s*\\\"");
			//check bools
			var boolMatches = Regex.Matches(fullText,"<<.*\\$(\\w*)\\s*==?\\s*[tfTF]");
			
			var createdVars = new List<string>();
			
			
			//compare lists to variables in database.
			bool varFound;
			for (int i=0; i<floatMatches.Count; i++){
				varFound = false;
				foreach(var datum in database.floatVars){
					if (datum.name == floatMatches[i].Groups[1].ToString()){
						varFound = true;
						break;	
					}	
				}
				if(!varFound){
					if(!createdVars.Contains(floatMatches[i].Groups[1].ToString())){
						FloatVar newVar = ScriptableObject.CreateInstance("FloatVar") as FloatVar;
						AssetDatabase.CreateAsset(newVar,path+floatMatches[i].Groups[1].ToString()+".asset");
						createdVars.Add(floatMatches[i].Groups[1].ToString());
					}
					
				}
			}
			
			createdVars.Clear();
			
			for (int i=0; i<intMatches.Count; i++){
				varFound = false;
				foreach(var datum in database.intVars){
					if (datum.name == intMatches[i].Groups[1].ToString()){
						varFound = true;
						break;	
					}	
				}
				if(!varFound){
					if(!createdVars.Contains(intMatches[i].Groups[1].ToString())){
						IntVar newVar = ScriptableObject.CreateInstance("IntVar")as IntVar;
						AssetDatabase.CreateAsset(newVar,path+intMatches[i].Groups[1].ToString()+".asset");
						createdVars.Add(intMatches[i].Groups[1].ToString());
					}
				}
			}
			
			createdVars.Clear();
			
			for (int i=0; i<stringMatches.Count; i++){
				varFound = false;
				foreach(var datum in database.stringVars){
					if (datum.name == stringMatches[i].Groups[1].ToString()){
						varFound = true;
						break;	
					}	
				}
				if(!varFound){
					if(!createdVars.Contains(stringMatches[i].Groups[1].ToString())){
						StringVar newVar = ScriptableObject.CreateInstance("StringVar") as StringVar;
						AssetDatabase.CreateAsset(newVar,path+stringMatches[i].Groups[1].ToString()+".asset");
						createdVars.Add(stringMatches[i].Groups[1].ToString());
					}
				}
			}
			
			createdVars.Clear();
			
			for (int i=0; i<boolMatches.Count; i++){
				varFound = false;
				foreach(var datum in database.boolVars){
					if (datum.name == boolMatches[i].Groups[1].ToString()){
						varFound = true;
						break;	
					}	
				}
				if(!varFound){
					if(!createdVars.Contains(boolMatches[i].Groups[1].ToString())){
						BoolVar newVar = ScriptableObject.CreateInstance("BoolVar") as BoolVar;
						AssetDatabase.CreateAsset(newVar,path+boolMatches[i].Groups[1].ToString()+".asset");
						createdVars.Add(boolMatches[i].Groups[1].ToString());
					}
				}
			}
			

		}
		database.Propogate();
	}
	
	void InitializeVariables(){
		Debug.Log("Setting variables to YARN_VARIABLE_INITIALIZATION values.");
		
		if(fullText.Length>0){
			
			var initBlockMatches = Regex.Matches(fullText, "title:\\s*YARN_VARIABLE_INITIALIZATION((?!---)[\\S\\s])*---\\n([\\S\\s]*?(?====))"); // captures body of Init block
			
			if(initBlockMatches.Count<1){
				Debug.Log("No node titled YARN_VARIABLE_INITIALIZATION found.");
				return;
			}
			
			string initBlock = "";
			initBlock = initBlockMatches[0].Groups[2].ToString();
			
			Debug.Log("YARN_VARIABLE_INITIALIZATION:\n"+initBlock);
			
			//check floats
			var floatMatches = Regex.Matches(initBlock, "<<.*\\$(\\w*)\\s*==?\\s*(-?\\d*\\.\\d*)\\s*>>");
			//check ints
			var intMatches = Regex.Matches(initBlock, "<<.*\\$(\\w*)\\s*==?\\s*(-?\\d*)\\s*>>");
			//check strings
			var stringMatches = Regex.Matches(initBlock, "<<.*\\$(\\w*)\\s*==?\\s*\"(.*)\">>");
			//check bools
			var boolMatches = Regex.Matches(initBlock,"<<.*\\$(\\w*)\\s*==?\\s*([tfTF])");
			
			bool varFound = false;
			
			foreach (var v in database.floatVars){
				varFound = false;
				for (int i=0; i<floatMatches.Count; i++){
					if (v.name == floatMatches[i].Groups[1].ToString()){
						v.init = float.Parse( floatMatches[i].Groups[2].ToString());
						v.val = v.init;
						varFound = true;
						break;
					}
				}
				if(!varFound){
					Debug.Log("No initialization found for $"+v.name);
				}
			}
			foreach (var v in database.intVars){
				varFound = false;
				for(int i=0; i<intMatches.Count; i++){
					if (v.name == intMatches[i].Groups[1].ToString()){
						v.init = int.Parse( intMatches[i].Groups[2].ToString());
						v.val = v.init;
						varFound = true;
						break;
					}
				}
				if(!varFound){
					Debug.Log("No initialization found for $"+v.name);
				}
			}
			foreach (var v in database.stringVars){
				varFound = false;
				for(int i=0; i<stringMatches.Count; i++){
					if (v.name == stringMatches[i].Groups[1].ToString()){
						v.init = stringMatches[i].Groups[2].ToString();
						v.val = v.init;
						varFound = true;
						break;
					}
				}
				if(!varFound){
					Debug.Log("No initialization found for $"+v.name);
				}
			}
			foreach (var v in database.boolVars){
				varFound = false;
				for(int i=0; i<boolMatches.Count; i++){
					if (v.name == boolMatches[i].Groups[1].ToString()){
						v.init = (boolMatches[i].Groups[2].ToString() == "T"||boolMatches[i].Groups[2].ToString() == "t");
						v.val = v.init;
						varFound = true;
						break;
					}
				}
				if(!varFound){
					Debug.Log("No initialization found for $"+v.name);
				}
			}
		}
		Debug.Log("Yarn variable initialization complete.");
	}


	void WriteToTarget(){
		if (fullText.Length > 0 && targetScript !=null) {
			string path = AssetDatabase.GetAssetPath (targetScript);
			Debug.Log (path);

			File.WriteAllText (path, fullText);
			/// Replace the file's data with the specified content. Will create a file if none exists.

		} else {
			if (fullText.Length < 1) {
				Debug.Log ("you need to write data to working memory first. Try Combine components.");
			}
			if (targetScript == null) {
				Debug.Log ("Target Script is null. Select a file to write to.");
			}
		}
	}

	//-Utilities-------------------------------------------

	//end
}
}
