using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Akitacore{
[CreateAssetMenu(fileName = "RadioLibrary", menuName = "ScriptableObjects/YarnAssets/RadioLibrary", order = 105)]
public class RadioLibrary : ScriptableObject{
    
    public AudioFile[] songs;
	public AudioFile[] foley;
    
	[System.Serializable]
	public class AudioFile{
		public string title;
		public AudioClip clip;
		public float volume = 1.0f;
	}

}
}
