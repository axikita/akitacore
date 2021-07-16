using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Akitacore{
[CreateAssetMenu(fileName = "SpeakerLibrary", menuName = "ScriptableObjects/YarnAssets/SpeakerLibrary", order = 100)]
public class SpeakerLibrary : ScriptableObject
{
    public List<SpeakerData> speakers;
    
    [Tooltip("Depricated. Use SpeakerData.spriteSide instead.")]
    public List<string> playerCharacterNames;
    
    public void Propogate(){
        Resources.LoadAll(""); //loads entire resources folder to RAM so that it can be searched.
        if (speakers == null){
            speakers = new List<SpeakerData>();   
        } 
        speakers.Clear();
        foreach (var v in Resources.FindObjectsOfTypeAll (typeof(SpeakerData)) as SpeakerData[]) {
            speakers.Add(v);
        }
          
    }

}
}
