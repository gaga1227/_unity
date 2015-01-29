using UnityEngine;
using System.Collections;

public class LoaderScript : MonoBehaviour {
	#region onStart
	void Start () {
		//this scene will kick off the settings gameobject
		//and kept in app across scenes
		
		//this scene is never revisited
		
		//load menu scene on start game
		Application.LoadLevel("Menu");
	}
	#endregion
}
