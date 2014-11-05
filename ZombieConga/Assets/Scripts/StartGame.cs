using UnityEngine;
using System.Collections;

public class StartGame : MonoBehaviour {

	#region on start
	void Start () {
		//call loadlevel fn in 3 seconds
		Invoke("LoadLevel", 3f);
	}
	#endregion
	
	#region methods
	//method rto load another level
	void LoadLevel() {
		Application.LoadLevel("CongaScenePart2");
	}
	#endregion
}
