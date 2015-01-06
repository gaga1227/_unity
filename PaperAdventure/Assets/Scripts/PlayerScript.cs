using UnityEngine;
using System.Collections;

// Player controller and behavior

public class PlayerScript : MonoBehaviour {
	#region vars
	// player speed
	public Vector2 speed = new Vector2(50, 50);
	// player movement
	private Vector2 movement;
	// health script ref
	private HealthScript health;
	// animator comp ref
	private Animator animator;
	#endregion

	#region onStart
	void Start() {
		// assign comp refs
		health = transform.GetComponent<HealthScript>();
		animator = transform.GetComponent<Animator>();
	}
	#endregion

	#region onUpdate
	void Update() {
		// Moving
		// Retrieve input axis information
		float inputX = Input.GetAxis("Horizontal");
		float inputY = Input.GetAxis("Vertical");
		
		// Movement per direction
		movement = new Vector2(
			speed.x * inputX,
			speed.y * inputY);

		// Shooting
		// Retrieve input button information on either 'Ctrl', or 'Cmd' keys
		// '|=' equals 'shoot = shoot | Input.GetButtonDown("Fire3");'
		// 'GetButton' only fires on down state, not down action
		// 'GetButtonDown' only fires on down action, not down state
		bool shoot = Input.GetButton("Fire1");
		shoot |= Input.GetButtonDown("Fire3");

		// if fire button(Ctrl or Alt) is down
		if (shoot) {
			// find weapon script comp
			// and call Attack public method
			// with isEnemy flag (false for Player)
			WeaponScript weapon = GetComponent<WeaponScript>();
			if (weapon != null) {
				weapon.Attack(false);
			}
		}

		// keep player in camera bound
		keepObjInBounds(transform, Camera.main);

		// animations
		updateAnimation();
	}
	#endregion

	#region onFixedUpdate
	void FixedUpdate() {
		// Move the game object
		rigidbody2D.velocity = movement;
	}
	#endregion

	#region methods
	// keep object within camera bounds
	void keepObjInBounds(Transform transformObj, Camera cam) {
		// get object's extents, which is half of the size
		Vector3 objExts = transformObj.gameObject.renderer.bounds.extents;

		// calculate z distance from obj and cam
		var distZ = (transformObj.position - cam.transform.position).z;

		// convert viewport edges to world points for obj's application
		// calculation is happening on obj's depth-plane with distZ
		// but seems unnecessary, can just be 0.0f
		var leftBorder = cam.ViewportToWorldPoint(new Vector3(0,0,distZ)).x;
		var rightBorder = cam.ViewportToWorldPoint(new Vector3(1,0,distZ)).x;
		var topBorder = cam.ViewportToWorldPoint(new Vector3(0,0,distZ)).y;
		var bottomBorder = cam.ViewportToWorldPoint(new Vector3(0,1,distZ)).y;

		// apply obj's within-bounds positon
		transformObj.position = new Vector3(
			Mathf.Clamp(transformObj.position.x, leftBorder + objExts.x, rightBorder - objExts.x),
			Mathf.Clamp(transformObj.position.y, topBorder + objExts.y, bottomBorder - objExts.y),
			transformObj.position.z);
	}

	// update animation based on states
	void updateAnimation() {
		if (health != null) {
			if (health.isInvincible) {
				animator.Play("respawn");
			} else {
				animator.Play("default");
			}
		}
	}
	#endregion

	#region handlers
	// Cannot use collision handler for collision event
	// Rigibody2D will pick up collision velocity
	// and cause disruption on respawn movement
	// now use OnTriggerEnter2D instead in HealthScript
	#endregion
}