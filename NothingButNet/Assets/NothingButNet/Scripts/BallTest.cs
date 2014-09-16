//imports
using UnityEngine;

public class BallTest : MonoBehaviour {
	//public var for Ball instance
	public Ball ball; 

	//on start
	void Start () {
		//register handler to delegate
		ball.OnNet += Handle_OnNet;
	}

	//private fn
	protected void Handle_OnNet(){
		Debug.Log ( "[TEST]: Handle_OnNet" );
	}
}
