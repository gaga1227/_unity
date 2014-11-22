using UnityEngine;
using System.Collections;

public class MouseController : MonoBehaviour {

	#region private vars
	#endregion

	#region public vars
	//jetpack force
	public float jetpackForce = 75.0f;
	//moving speed
	public float forwardMovementSpeed = 3.0f;
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

		//set moving speed to self via rigidbody2D
		//sets the velocity x-component, without affecting y-component
		Vector2 newVelocity = rigidbody2D.velocity;
		newVelocity.x = forwardMovementSpeed;
		rigidbody2D.velocity = newVelocity;
	}
	#endregion
}
