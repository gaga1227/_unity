﻿using UnityEngine;
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
	// touch input sensitivity
	private float touchSensitivityX;
	private float touchSensitivityY;
	// control transition smooth factor
	private float smooth = 30f;
	// finger size
	private float fingerSize = 0.2f;
	#endregion
	
	#region onStart
	void Start() {
		// assign comp refs
		health = transform.GetComponent<HealthScript>();
		animator = transform.GetComponent<Animator>();
		
		// calculate touch sensitivity
		touchSensitivityY = 0.05f;
		touchSensitivityX = 0.05f;
	}
	#endregion
	
	#region onUpdate
	void Update() {
		if (health != null) {
			// if player is alive
			if (health.hp > 0) {
				// Retrieve traditional input information
				
				// for Moving
				float inputX = Input.GetAxis("Horizontal");
				float inputY = Input.GetAxis("Vertical");
				
				// for Shooting
				// Retrieve input button information on either 'Ctrl', or 'Cmd' keys
				// '|=' equals 'shoot = shoot | Input.GetButtonDown("Fire3");'
				// 'GetButton' only fires on down state, not down action
				// 'GetButtonDown' only fires on down action, not down state
				bool shoot = Input.GetButton("Fire1");
				shoot |= Input.GetButtonDown("Fire3");

				// for accumulativeShoot
				bool accumulativeShoot = Input.GetButton("Fire2");
				
				// Update input info if has touch input
				if (UtilsHelper.Instance.isTouchInput) {
					if (Input.touchCount > 0) {
						// touch 1 - movement and regular shooting
						Touch touch1 = Input.GetTouch(0);
						if (touch1.phase == TouchPhase.Began) {
							shoot = true;
						}
						else if (touch1.phase == TouchPhase.Moved && touch1.phase != TouchPhase.Canceled) {
							float touchSpeedX = Mathf.Lerp(0, 1, Mathf.Abs(touch1.deltaPosition.x) * touchSensitivityX);
							float touchSpeedY = Mathf.Lerp(0, 1, Mathf.Abs(touch1.deltaPosition.y) * touchSensitivityY);
							inputX = (touch1.deltaPosition.x > 0 ? 1 : -1) * touchSpeedX;
							inputY = (touch1.deltaPosition.y > 0 ? 1 : -1) * touchSpeedY;
						}
						else if (touch1.phase == TouchPhase.Ended || touch1.phase == TouchPhase.Canceled) {
							inputX = 0;
							inputY = 0;
							shoot = false;
						}

						// touch 2 - accumulative shooting
						if (Input.touchCount > 1) {
							Touch touch2 = Input.GetTouch(1);
							if (touch2.phase == TouchPhase.Began) {
								accumulativeShoot = true;
							}
							else if (touch2.phase == TouchPhase.Ended || touch2.phase == TouchPhase.Canceled) {
								accumulativeShoot = false;
							}
						}
					}
				}
				
				// Apply input information
				
				// Movement per direction
				movement = new Vector2(
					speed.x * Mathf.Lerp(movement.x/speed.x, inputX, Time.deltaTime * smooth),
					speed.y * Mathf.Lerp(movement.y/speed.y, inputY, Time.deltaTime * smooth)
					);
				
				// Shooting
				// if is firing
				if (shoot) {
					// find weapon script comp
					WeaponScript weapon = GetComponent<WeaponScript>();
					if (weapon != null) {
						// accumulative shoot
						if (accumulativeShoot) {
							// start accumulating shot
							weapon.Accumulate(false);
						}
						// regular shooting
						else {
							// call Attack public method
							// with isEnemy flag (false for Player)
							weapon.Attack(false);
						}
					}
				}
				
				// keep player in camera bound
				keepObjInBounds(transform, Camera.main);
			}
			// if player is dead
			else {
				// make it fall
				movement = new Vector2(0, -10f);
				transform.Rotate(0, 0, 6f);
			}

			// update player animations
			updateAnimation();
		}
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
			Mathf.Clamp(
			transformObj.position.x,
			leftBorder + objExts.x + fingerSize,
			rightBorder - objExts.x - fingerSize
			),
			Mathf.Clamp(
			transformObj.position.y,
			topBorder + objExts.y,
			bottomBorder - objExts.y
			),
			transformObj.position.z);
	}
	
	// update animation based on states
	void updateAnimation() {
		if (health.isInvincible) {
			animator.Play("respawn");
		} else {
			animator.Play("default");
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