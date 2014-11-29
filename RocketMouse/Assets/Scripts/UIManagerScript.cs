using UnityEngine;
using System.Collections;

public class UIManagerScript : MonoBehaviour {

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
	#endregion

}
