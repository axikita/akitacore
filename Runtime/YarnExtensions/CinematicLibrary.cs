using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Akitacore{
[CreateAssetMenu(fileName = "CinematicLibrary", menuName = "ScriptableObjects/YarnAssets/CinematicLibrary", order = 101)]
public class CinematicLibrary : ScriptableObject
{
    public CinematicImage[] cinematicImages;
    
	[System.Serializable]
    public class CinematicImage{
		public string name;
		public float scale;
		public float framerate;
		public Sprite[] sprite;

		public CinematicImage(){
			name = "";
			sprite = new Sprite[0];
			scale = 1.0f;
			framerate = 12.0f;
		}
	}
}
}
