using UnityEngine;
using System.Collections;

public class MouseController : MonoBehaviour {

	#region private vars
	#endregion

	#region public vars
	//jetpack force
	public float jetpackForce = 75.0f;
	#endregion

	#region onStart
	void Start () {
		
	}
	#endregion

	#region onUpdate
	void Update () {
		
	}
	#endregion

	#region onFixedUpdate
	void FixedUpdate () {
		//enable jetpack on click
		bool jetpackActive = Input.GetButton("Fire1");
		//apply jetpack force if enabled
		if (jetpackActive) {
			rigidbody2D.AddForce(new Vector2(0, jetpackForce));
		}
	}
	#endregion
}
