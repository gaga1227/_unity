using UnityEngine;
using System.Collections;

// UtilsHelper

public class UtilsHelper : MonoBehaviour {
	#region vars
	// Use static keyword for Singleton ref: Instance
	public static UtilsHelper Instance;

	// touch input flag
	public bool isTouchInput {
		get {
			return Input.multiTouchEnabled;
		}
	}
	#endregion
	
	#region onAwake
	void Awake () {
		// multiple instances handling
		if (Instance != null) {
			Debug.LogError("Multiple instances of UtilsHelper!");
		}
		// Register this as the singleton instance
		Instance = this;
	}
	#endregion
	
	#region methods
	#endregion
}
