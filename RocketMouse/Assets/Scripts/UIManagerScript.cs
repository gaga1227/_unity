using UnityEngine;
using System.Collections;

public class UIManagerScript : MonoBehaviour {

	#region vars
	//animator refs
	public Animator startButton;
	public Animator settingsButton;
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
	public void StartGame() {
		Application.LoadLevel("RocketMouse");
	}

	//start settings intro transition
	public void OpenSettings() {
		startButton.SetBool("isHidden", true);
		settingsButton.SetBool("isHidden", true);
	}
	#endregion

}
