using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Text;
using System.Text.RegularExpressions; //used for formatting 
using System.Collections.Generic;
using Yarn.Unity;
using System;

using TMPro;


//This is the primary script for defining the implementation of Yarn dialogue in the unity scene.

namespace Akitacore{
public class CoreDialogueUI : Yarn.Unity.DialogueUIBehaviour {

	/// The object that contains the dialogue and the options. Enabled on conversation start, disabled at end.
	public GameObject dialogueContainer;

	/// The UI element that displays lines
	//public Text lineText;
	public TextMeshProUGUI lineText; //the api calls here are compatible with both unity Text and TextMeshPro, so just swap which is commented to change.

	/// A UI element that appears after lines have finished appearing
	public GameObject continuePrompt;

	public GameObject PauseMenu; //don't do input if pause is up

	/// A delegate (ie a function-stored-in-a-variable) that
	/// we call to tell the dialogue system about what option
	/// the user selected
	private Yarn.OptionChooser SetSelectedOption;


	/// How quickly to show the text, in seconds per character
	[Tooltip("How quickly to show the text, in seconds per character")]
	public float textSpeed = 0.025f;

	/// The buttons that let the user choose an option
	public List<Button> optionButtons;

	/// Make it possible to temporarily disable the controls when
	/// dialogue is active and to restore them when dialogue ends
	public RectTransform gameControlsContainer;

	public string speaker; //used to identify the speaker of a given line of text
	public List<string> emotes; //used to trigger portraits, animations, and other common mood indicators

	//library of recognizable emotes. Specify the emote string and the Regex.
	//IMPORTANT: first match will take hold. PUT LONGER EMOTES FIRST.
	//Emotes of the form :text: or more specifically the regex :\w: are always recognized and don't need to be specified
	public static List<Emote> emoteLib = new List<Emote>(){
		new Emote(">:|","\\>\\:\\|"),
		new Emote("D:<","D\\:\\<"),
		new Emote(">:3","\\>\\:3"),

		new Emote(">&|","\\>\\&\\|"),
		new Emote("D&<","D\\&\\<"),
		new Emote(">&3","\\>\\&3"),

		new Emote(":|","\\:\\|"),
		new Emote("D:","D\\:"),
		new Emote(":3","\\:3"), 



		new Emote("&|","\\&\\|"),
		new Emote("D&","D\\&"),
		new Emote("&3","\\&3"),



		new Emote("^^","\\^\\^"),
		new Emote(":V","\\:V"),
		new Emote(":<","\\:\\<")};
	

	///delegates for reporting the line count. Register with AddLineListener
	List<Action<int>> lineListeners;

	///displays current line of dialogue since start. Sent by lineListeners.
	public int lineNumber=-1;

	public int charIndex = 0;
	public bool nonWhiteSpace = false;
	public bool lineHasText = false;
	public bool lineRendered = false;

	public int optionCount;

	//----MONOBEHAVIOR--------------------------------------------------------------------------------------------------------------------------------------------

	void Awake ()
	{
		// Start by hiding the container, line and option buttons
		if (dialogueContainer != null)
			dialogueContainer.SetActive(false);

		lineText.gameObject.SetActive (false);


		foreach (var button in optionButtons) {
			button.gameObject.SetActive (false);
		}

		// Hide the continue prompt if it exists
		if (continuePrompt != null)
			continuePrompt.SetActive (false);

		lineListeners = new List<Action<int>> ();
	}

	//----External called functions--------------------------------------------------------------------------------------------------------------------------------

	

	/// Show a line of dialogue, gradually
	public override IEnumerator RunLine (Yarn.Line line)
	{
		//initialize data
		emotes.Clear ();
		speaker = "";
		string formattedLine = FormatLine (line.text);

		ReportToLineListeners ();
		lineNumber += 1;

		formattedLine = ManageSpeaker (formattedLine);
		formattedLine = ManageEmotes (formattedLine);
		formattedLine = RemoveLeadingSpaces (formattedLine);
		formattedLine = FixOctothorps (formattedLine);

		lineHasText = Regex.IsMatch (formattedLine, "(\\w+)");

		// Show the text
		lineText.gameObject.SetActive (true);

		//Do word wrapping based on color tags, with full markup support.
		if (textSpeed > 0.0f) {
			string start = "";
			string end = "";
			SuperString superLine = new SuperString(formattedLine);
			
			lineRendered = false;
			//render the entire chunk of text, with transparency tags moving along at the read rate.
			for (int i = 0; i < superLine.GetLength()+1; i++) {
				if (i > 5 && Input.anyKeyDown) {
					i = superLine.GetLength()-1;
				}

				start = superLine.SubstringBefore(i);
				end = "<color=#00000000>" + superLine.SubStringAfterNoTags(i) + "</color>";
				lineText.text = start + end;
				charIndex = i;
				if (i > 0) {
					if (formattedLine.Substring (i - 1)[0] ==' ') {
						nonWhiteSpace = false;
					} else {
						nonWhiteSpace = true;
					}
				}

				yield return new WaitForSeconds (textSpeed);
			}
			lineRendered = true;
		} else {
			// Display the line immediately if textSpeed == 0
			lineText.text = formattedLine;
		}

		// Show the 'press any key' prompt when done, if we have one
		if (continuePrompt != null)
			continuePrompt.SetActive (true);

		// Wait for any user input except pause functionality
		while (Input.anyKeyDown == false || Input.GetKeyDown(KeyCode.Escape)||(PauseMenu!=null && PauseMenu.activeInHierarchy)) {
			yield return null;
		}

		// Hide the text and prompt
		lineText.gameObject.SetActive (false);

		if (continuePrompt != null)
			continuePrompt.SetActive (false);
		
	}
	
	
	
	private class SuperString{
		//produced by regex "(^.*?(?=<))(<(.+)=?.*?>)(.*?(?!\\2))(<\\/(\\3)>)(.*)"
		//Group1: Text leading up to tag. Never contains a tag. [optional]
		//Group2: Opening Tag full text [save]
		//Group3: --regex utility. Contains opening tag up to an = sign, matches with closing tag.
		//Group4: Text contained in Tag [optional][search]
		//Group5: Closing Tag full text [save]
		//Group6: --regex utility. Contains closing tag that matches group 3.
		//Group7: Text after tag. [optional][search]
		string leadup;
		string openTag;
		SuperString contents;
		string closeTag;
		SuperString ending;
		
		///for a given line, only create one superstring. It will create nested superstrings as needed.
		public SuperString(string inputString){
			var match = Regex.Match(inputString, "(^.*?(?=<))(<(.+)=?.*?>)(.*?(?!\\2))(<\\/(\\3)>)(.*)");
			if(match.Success){
				leadup = match.Groups[1].ToString();
				openTag = match.Groups[2].ToString();
				contents = new SuperString(match.Groups[4].ToString());
				closeTag = match.Groups[5].ToString();
				ending = new SuperString(match.Groups[7].ToString());
			} else {
				leadup = inputString;
				openTag = "";
				contents = null;
				closeTag = "";
				ending = null;
			}
		}
		
		
		///Length includes plain text but not tags.
		public int GetLength(){
			int length = leadup.Length;
			if(contents !=null){
				length += contents.GetLength();
			}
			if(ending !=null){
				length += ending.GetLength();
			}
			return length;
		}
		
		
		public string FullString(){
			string returnString = leadup;
			returnString += openTag;
			if(contents!=null){
				returnString += contents.FullString();
			}
			returnString += closeTag;
			if(ending!=null){
				returnString += ending.FullString();
			}
			return returnString;
		}
		
		public string FullStringNoFormat(){
			string returnString = leadup;
			if(contents!=null){
				returnString += contents.FullStringNoFormat();
			}
			if(ending!=null){
				returnString += ending.FullStringNoFormat();
			}
			return returnString;
		}
		

		//[xxxxxi]xxxxxx
		///Includes formatting tags
		public string SubstringBefore(int i){
			string returnString = "";
			int gatheredCount = 0;
			
			if(i<leadup.Length){
				return leadup.Substring(0,i);
			} else {
				returnString = leadup;
				gatheredCount = leadup.Length;
				if(i==gatheredCount){
					return returnString;
				}
			}
			
			//i is greater than gatheredcount. i-gatheredcount left to find.
			if(i-gatheredCount >= contents.GetLength()){
				returnString += openTag;
				returnString += contents.FullString();
				returnString += closeTag;
				gatheredCount += contents.GetLength();
			} else {
				returnString += openTag;
				returnString += contents.SubstringBefore(i-gatheredCount);
				returnString += closeTag;
				return returnString;
			}
			
			//i is greater than leadup and contents. either part of ending left, or the full string.
			if(i >= gatheredCount+ending.GetLength()){
				returnString += ending.FullString();
				return returnString;
			} else {
				returnString += ending.SubstringBefore(i-gatheredCount);
			}
			
			return returnString;
		}
		
		//xxxxxi[xxxxxx]
		///Includes formatting tags
		public string SubStringAfter(int i){
			string returnString = "";
			int priorCount = 0;
			bool startFound = false;
			
			if(i<leadup.Length){
				returnString = leadup.Substring(i,leadup.Length);
				startFound = true;
			} else {
				priorCount = leadup.Length;
			}
			
			if(startFound){
				returnString+=openTag;
				returnString+=contents.FullString();
				returnString+=closeTag;
			} else if(i<priorCount+contents.GetLength()){
				returnString+=openTag;
				returnString+=contents.SubStringAfter(i-priorCount);
				returnString+=closeTag;
				startFound = true;
			} else {
				priorCount += contents.GetLength();
			}
			
			if(startFound){
				returnString+=ending.FullString();
			} else if(i<priorCount+ending.GetLength()){
				returnString += ending.SubStringAfter(i-priorCount);
			} else {
				returnString += ending.FullString();
			}
			
			return returnString;
		}
		
				///Does not include formatting tags
		public string SubStringBeforeNoTags(int i){
			string fullString = FullStringNoFormat();
			return fullString.Substring(0, i);
		}
		
		///Does not include formatting tags
		public string SubStringAfterNoTags(int i){
			string fullString = FullStringNoFormat();
			if(i<fullString.Length){
				return fullString.Substring(i);
			} else{
				return "";
			}
		}
	}
	
	
	
	
	
	
	
	
	
	
	

	/// Show a list of options, and wait for the player to make a selection.
	public override IEnumerator RunOptions (Yarn.Options optionsCollection, 
		Yarn.OptionChooser optionChooser)
	{
		// Do a little bit of safety checking
		if (optionsCollection.options.Count > optionButtons.Count) {
			Debug.LogWarning("There are more options to present than there are" +
				"buttons to present them in. This will cause problems.");
		}

		// Display each option in a button, and make it visible
		int i = 0;
		foreach (var optionString in optionsCollection.options) {
			optionButtons [i].gameObject.SetActive (true);
			optionButtons [i].GetComponentInChildren<Text> ().text = optionString;
			i++;
		}
		optionCount = i;

		// Record that we're using it
		SetSelectedOption = optionChooser;

		// Wait until the chooser has been used and then removed (see SetOption below)
		while (SetSelectedOption != null) {
			yield return null;
		}

		// Hide all the buttons
		foreach (var button in optionButtons) {
			button.gameObject.SetActive (false);
		}
	}

	/// Called by buttons to make a selection.
	public void SetOption (int selectedOption)
	{

		// Call the delegate to tell the dialogue system that we've
		// selected an option.
		SetSelectedOption (selectedOption);

		// Now remove the delegate so that the loop in RunOptions will exit
		SetSelectedOption = null; 
	}

	/// Run an internal command.
	public override IEnumerator RunCommand (Yarn.Command command)
	{
		
		// "Perform" the command
		//Debug.Log ("Command: " + command.text);
		YarnCommandLibrary.RunCommand(command.text);

		yield break;
	}

	/// Called when the dialogue system has started running.
	public override IEnumerator DialogueStarted ()
	{
		//Debug.Log ("Dialogue starting!");
		lineNumber = 0;

		// Enable the dialogue controls.
		if (dialogueContainer != null)
			dialogueContainer.SetActive(true);

		// Hide the game controls.
		if (gameControlsContainer != null) {
			gameControlsContainer.gameObject.SetActive(false);
		}

		yield break;
	}

	/// Called when the dialogue system has finished running.
	public override IEnumerator DialogueComplete ()
	{
		//Debug.Log ("Complete!");

		lineNumber = -1;
		ReportToLineListeners ();

		// Hide the dialogue interface.
		if (dialogueContainer != null)
			dialogueContainer.SetActive(false);

		// Show the game controls.
		if (gameControlsContainer != null) {
			gameControlsContainer.gameObject.SetActive(true);
		}

		yield break;
	}

	public void AddLineListener(Action<int> reportFunction){
		lineListeners.Add(reportFunction);
	}

	//----Private Utility functions----------------------------------------------------------------------------------------------------------------------------------

	void ReportToLineListeners(){
		foreach (Action<int> act in lineListeners) {
			act (lineNumber);
		}
	}

	//Trims the speaker off of a line and stores it in the speaker property
	string ManageSpeaker(string raw){
		string remainder = "";

		//this always returns 3 groups: the full text, the name (possibly empty) and the rest.
		var match = Regex.Match(raw,"^(?:([A-Za-z./%+-]+):\\s*)?(.+)$");

		speaker = match.Groups [1].ToString ();
		remainder = match.Groups [2].ToString ();

		return remainder;
	}

	//replace BBcode with unity text formatting, return corrected text.
	//DEPRICATED: this method is incompatible with the text-reveal implementation in RunLine().
	string FormatLine(string raw){

		raw = Regex.Replace(raw, "\\[b\\]", "<b>");

		raw = Regex.Replace (raw, "\\[/b\\]", "</b>");

		return raw;
	}

	string RemoveLeadingSpaces(string raw){
		if(Regex.IsMatch(raw, "^\\s(.*)")){
			var match = Regex.Match(raw, "^\\s(.*)");
			return match.Groups[1].ToString();
		}
		return raw;
			
	}
	
	//yarnspinner uses # to designate localization information, which means you can't use an octothorp anywhere or it'll break
	//YarnIntegrityEditor has an option to change all instances of "#" to "[octothorp]", this changes it back.
	//this is necessary for full color code support.
	string FixOctothorps(string raw){
		raw = Regex.Replace(raw, "_OCTOTHORPE", "#");
		return raw;
	}

	//Trims Emoticons off of a line and stores them in the emotes list.
	string ManageEmotes(string raw){
		//Debug.Log ("running ManageEmotes(" + raw + ")");

		//run thrice:
		//emotes at end of line
		//emotes at beginning of line
		//emotes with spaces on both sides
		
		//then, check for text emotes of the form :text:- escaped first, then plain.



		foreach (var entry in emoteLib) {
			if (Regex.IsMatch(raw, "( "+entry.regex+"$)")){//emotes at the end of a line 
				emotes.Add(entry.ascii);
				raw = Regex.Replace(raw,"( "+entry.regex+"$)",""); 
			}
		}

		foreach (var entry in emoteLib) {
			if (Regex.IsMatch(raw, "(^"+entry.regex+" )")){
				emotes.Add(entry.ascii);
				raw = Regex.Replace(raw,"(^"+entry.regex+" )",""); //emotes at the beginning of a line
			}
		}

		foreach (var entry in emoteLib) {
			if (Regex.IsMatch(raw, "( "+entry.regex+" )")){
				emotes.Add(entry.ascii);
				raw = Regex.Replace(raw,"( "+entry.regex+" )"," "); //emotes in mid line with spaces on both sides
			}
		}

		//then run again, with replacement for escaped emotes
		foreach (var entry in emoteLib) {
			if (Regex.IsMatch(raw, "( \\\\"+entry.regex+"$)")){//emotes at the end of a line 
				emotes.Add(entry.ascii);
				raw = Regex.Replace(raw,"( \\\\"+entry.regex+"$)"," "+entry.ascii); 
			}
		}

		foreach (var entry in emoteLib) {
			if (Regex.IsMatch(raw, "(^\\\\"+entry.regex+" )")){
				emotes.Add(entry.ascii);
				raw = Regex.Replace(raw,"(^\\\\"+entry.regex+" )",entry.ascii+" "); //emotes at the beginning of a line
			}
		}

		foreach (var entry in emoteLib) {
			if (Regex.IsMatch(raw, "( \\\\"+entry.regex+" )")){
				emotes.Add(entry.ascii);
				raw = Regex.Replace(raw,"( \\\\"+entry.regex+" )"," "+entry.ascii+" "); //emotes in mid line with spaces on both sides
			}
		}
		
		
		
		//--TEXT EMOTES---
		
		
		if (Regex.IsMatch(raw, "(?<!\\\\)(:(\\w+):)")){// non-escaped emotes of the form :\w*:
			var noEscapeMatch = Regex.Match(raw, "(?<!\\\\)(:(\\w+):)");  //group1: :text: group 2: text
			emotes.Add(noEscapeMatch.Groups[2].ToString());
			raw = Regex.Replace(raw,"("+noEscapeMatch.Groups[1].ToString()+")",""); 
		}
		
		if (Regex.IsMatch(raw, "\\\\(:(\\w+):)")){// escaped emotes of the form :\w*:
			var escapedMatch = Regex.Match(raw, "\\\\(:(\\w+):)");  //raw: \:text: group1: :text: group 2: text
			//emotes.Add(escapedMatch.Groups[2].ToString()); //don't actually add these
			raw = Regex.Replace(raw,"(\\\\"+escapedMatch.Groups[1].ToString()+")",escapedMatch.Groups[1].ToString()); 
		}
		

		
		
		/*
		foreach (var entry in emoteLib) {
			if (Regex.IsMatch(raw, "( "+entry.regex" )")){
				emotes.Add(entry.ascii);
				raw = Regex.Replace(raw,"( "+entry.regex" )"," "); //emotes in mid line with spaces on both sides
			}

			if (Regex.IsMatch(raw, entry.regexEscaped)){
				raw = Regex.Replace (raw, entry.regexEscaped, " "+entry.ascii);
			}
		}*/
			
			

		return raw;
	}


	//----Structures and Classes---------------------------------------------------------------------------------------------------------------------


	//Data for recognized emotes
	public struct Emote{
		public string ascii;
		public string regex;
		public string regexEscaped;

		///Initialize with the regex string for just the emote. Capture group will be added.
		public Emote(string Ascii, string Regex){
			ascii = Ascii;
			regex = Regex;
			regexEscaped = "( \\\\"+Regex+")";
		}
	}
}
}


//backup
/*public override IEnumerator RunLine (Yarn.Line line)
	{
		//initialize data
		emotes.Clear ();
		speaker = "";
		string formattedLine = FormatLine (line.text);

		ReportToLineListeners ();
		lineNumber += 1;

		formattedLine = ManageSpeaker (formattedLine);
		formattedLine = ManageEmotes (formattedLine);
		formattedLine = RemoveLeadingSpaces (formattedLine);

		lineHasText = Regex.IsMatch (formattedLine, "(\\w+)");

		// Show the text
		lineText.gameObject.SetActive (true);

		//Do word wrapping based on color tags, with full markup support.
		if (textSpeed > 0.0f) {
			string start = "";
			string end = "";
			
			lineRendered = false;
			//render the entire chunk of text, with transparency tags moving along at the read rate.
			for (int i = 0; i < formattedLine.Length+1; i++) {
				if (i > 5 && Input.anyKeyDown) {
					i = formattedLine.Length-1;
				}

				start = formattedLine.Substring (0, i);
				end = "<color=#00000000>" + formattedLine.Substring (i) + "</color>";
				lineText.text = start + end;
				charIndex = i;
				if (i > 0) {
					if (formattedLine.Substring (i - 1)[0] ==' ') {
						nonWhiteSpace = false;
					} else {
						nonWhiteSpace = true;
					}
				}

				yield return new WaitForSeconds (textSpeed);
			}
			lineRendered = true;
		} else {
			// Display the line immediately if textSpeed == 0
			lineText.text = formattedLine;
		}

		// Show the 'press any key' prompt when done, if we have one
		if (continuePrompt != null)
			continuePrompt.SetActive (true);

		// Wait for any user input except pause functionality
		while (Input.anyKeyDown == false || Input.GetKeyDown(KeyCode.Escape)||(PauseMenu!=null && PauseMenu.activeInHierarchy)) {
			yield return null;
		}

		// Hide the text and prompt
		lineText.gameObject.SetActive (false);

		if (continuePrompt != null)
			continuePrompt.SetActive (false);
		
	}*/


