using UnityEngine;
using System.Collections;

public class MoveScript : MonoBehaviour {

	#region vars
	// Object speed
	public Vector2 speed = new Vector2(10, 10);
	// Object moving direction
	public Vector2 direction = new Vector2(-1, 0);
	// Object movement
	private Vector2 movement;
	#endregion

	#region onStart
	void Start () {
		// Movement per direction
		movement = new Vector2(
			speed.x * direction.x,
			speed.y * direction.y);
	}
	#endregion

	#region onUpdate
	void Update () {
		
	}
	#endregion

	#region onFixedUpdate
	void FixedUpdate () {
		// Move the game object
		rigidbody2D.velocity = movement;
	}
	#endregion
}
