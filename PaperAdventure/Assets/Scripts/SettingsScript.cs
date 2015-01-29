using UnityEngine;
using System.Collections;
//for using new GUI
using UnityEngine.UI;

public class SettingsScript : MonoBehaviour {
	#region vars
	//public settings to share across the game
	public bool soundEnabled;
	#endregion
	
	#region onStart
	void Start () {
		//preserve this gameobject across scenes
		DontDestroyOnLoad(this);

		//set init values
		soundEnabled = true;
	}
	#endregion
	
	#region methods
	//toggle sound
	public void toggleSound() {
		//toggle sound state
		soundEnabled = !soundEnabled;
	}
	#endregion
	
	#region handlers
	//on level loaded
	void OnLevelWasLoaded(int level) {
		//refs
		GameObject menuControl = GameObject.Find("MenuControl");

		// update sound toggle UI with setting value
		//if menuControl exists
		if (menuControl != null) {
			// find menu script comp
			MenuScript menuControlScript = menuControl.GetComponent<MenuScript>();
			if (menuControlScript != null) {
				// call method to update UI
				menuControlScript.updateToggleSoundUI(soundEnabled);
			}
		}
	}
	#endregion
}
