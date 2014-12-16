using UnityEngine;
using System.Collections;

public class PlayerScript : MonoBehaviour {
	#region vars
	// player speed
	public Vector2 speed = new Vector2(50, 50);
	// player movement
	private Vector2 movement;
	#endregion
	
	#region onUpdate
	void Update() {
		// Retrieve input axis information
		float inputX = Input.GetAxis("Horizontal");
		float inputY = Input.GetAxis("Vertical");
		
		// Movement per direction
		movement = new Vector2(
			speed.x * inputX,
			speed.y * inputY);
	}
	#endregion

	#region onFixedUpdate
	void FixedUpdate() {
		// Move the game object
		rigidbody2D.velocity = movement;
	}
	#endregion
}