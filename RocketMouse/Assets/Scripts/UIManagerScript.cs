using UnityEngine;
using System.Collections;

public class UIManagerScript : MonoBehaviour {

	#region vars
	//animator refs
	public Animator startButton;
	public Animator settingsButton;
	public Animator dialog;
	#endregion

	#region onStart
	void Start () {

	}
	#endregion
	
	#region onUpdate
	void Update () {
		
	}
	#endregion

	#region methods
	//load game level
	//------------------------------------------------------------------------
	public void StartGame() {
		Application.LoadLevel("RocketMouse");
	}

	//open settings menu
	//------------------------------------------------------------------------
	public void OpenSettings() {
		//trigger out anims for main menu buttons
		startButton.SetBool("isHidden", true);
		settingsButton.SetBool("isHidden", true);

		//enable dialog animator and trigger in anim
		//for settings panel
		dialog.enabled = true;
		dialog.SetBool("isHidden", false);
	}

	//close settings menu
	//------------------------------------------------------------------------
	public void CloseSettings() {
		//trigger in anims for main menu buttons
		startButton.SetBool("isHidden", false);
		settingsButton.SetBool("isHidden", false);

		//trigger out anim for settings panel
		dialog.SetBool("isHidden", true);
	}
	#endregion

}
