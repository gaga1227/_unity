using UnityEngine;
using System.Collections;

// Spawn behavior

public class SpawnScript : MonoBehaviour {
	#region vars
	// if instance is spawn
	public bool hasSpawn;
	// respawn offset
	public Vector2 spawnOffset;

	// respawn cooldown
	public float respawnThreshold;
	private float respawnCooldown;

	// ref for move script comp
	private MoveScript moveScript;
	// ref for health script comp
	private HealthScript healthScript;
	// ref for animator comp
	private Animator animator;

	// weapon comp ref
	private WeaponScript[] weapons;

	// ref for main cam
	private Camera mainCam;

	// player scrolling script ref
	private ScrollingScript scrollingScript;
	#endregion
	
	#region onAwake
	void Awake() {
		// find obj's move script comp
		moveScript = GetComponent<MoveScript>();
		// find obj's health script comp
		healthScript = GetComponent<HealthScript>();
		// find obj's animator comp
		animator = GetComponent<Animator>();
		// find list of weapon script comps only once
		weapons = GetComponentsInChildren<WeaponScript>();
		// find main camera
		mainCam = Camera.main;
	}
	#endregion
	
	#region onStart
	void Start() {
		// init enemy instance as disabled
		// enable when in cam view
		Spawn(false);

		// set cooldown to threshold
		respawnCooldown = respawnThreshold;
		
		// find scrolling script comp
		foreach (ScrollingScript tempScroll in FindObjectsOfType<ScrollingScript>()) {
			if (tempScroll.isLinkedToCamera) {
				scrollingScript = tempScroll;
			}
		}
	}
	#endregion
	
	#region onUpdate
	void Update () {
		// if instance is disabled
		if (hasSpawn == false) {
			// enable instance when in view
			// 'IsVisibleFrom' is renderer's extension method
			if (renderer.IsVisibleFrom(mainCam)) {
				Spawn(true);
			}

			// if instance is still in cooldown
			if (respawnCooldown > 0) {
				// start cooldown
				respawnCooldown -= Time.deltaTime;
				// keep moving along with player and cam
				if (scrollingScript != null) {
					transform.Translate(scrollingScript.movement);
				}
			}
		}
		// if instance is spawn
		else {
			// Destroy the instance when out of the camera
			// 'IsVisibleFrom' is renderer's extension method
			if (renderer.IsVisibleFrom(mainCam) == false) {
				Respawn();
				//Destroy(gameObject);
			}

			// reset cooldown to threshold if not already
			if (respawnCooldown != respawnThreshold) {
				respawnCooldown = respawnThreshold;
			}
		}
	}
	#endregion
	
	#region methods
	// spawn enemy instance (on/off)
	private void Spawn(bool isEnabled) {
		// set spawn as true
		hasSpawn = isEnabled;
		
		// Enable everything
		// -- Collider
		if (collider2D != null) collider2D.enabled = isEnabled;
		// -- Moving
		if (moveScript != null) moveScript.enabled = isEnabled;
		// -- Animation
		if (animator != null) animator.enabled = isEnabled;
		// -- Shooting
		if (weapons != null && weapons.Length > 0) {
			foreach (WeaponScript weapon in weapons) {
				weapon.enabled = isEnabled;
			}
		}
	}
	
	// reposition enemy instance for respawn
	private void Reposition() {
		// get instance and main cam extents
		Vector3 objExts = transform.gameObject.renderer.bounds.extents;
		float camExtH = mainCam.orthographicSize;
		float camExtW = mainCam.aspect * camExtH;

		// calculate new position

		// base pos + random x-pos + instance size + shift
		float newPosX = mainCam.transform.position.x + camExtW * Random.Range(1.0f, 3.0f) + objExts.x + spawnOffset.x;
		// from negative (half view - half instance), to positive
		float newPosY = (camExtH - objExts.y - spawnOffset.y) * Random.Range(-1.0f, 1.0f);
		// use existing z pos
		float newPosZ = transform.position.z;
		// overwrite posX calculation if is boss
		if (healthScript != null && healthScript.isBoss) {
			// base pos + x-pos (make sure boss is respawned off stage) + instance size + shift
			newPosX = mainCam.transform.position.x + camExtW * 3.0f + objExts.x + spawnOffset.x;
		}
		
		// apply new position/rotation to instance
		transform.position = new Vector3(newPosX, newPosY, newPosZ);
		transform.rotation = Quaternion.identity;
	}
	
	// respawn enemy instance
	public void Respawn() {
		// reset instance as disabled
		Spawn(false);
		// reposition
		Reposition();
		// reset instance properties
		if (healthScript != null) {
			// disable invincible
			healthScript.isInvincible = false;
			// reset health
			healthScript.hp = 1;

			// if is boss
			if (healthScript.isBoss) {
				// reser health
				healthScript.hp = 100;
				// reset scale
				transform.localScale = new Vector3(1, 1, 1);
			}
		}
	}
	#endregion
}

