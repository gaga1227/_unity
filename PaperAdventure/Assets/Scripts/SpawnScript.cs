using UnityEngine;
using System.Collections;

// Spawn behavior

public class SpawnScript : MonoBehaviour {
	#region vars
	// if instance is spawn
	public bool hasSpawn;
	// respawn offset
	public Vector2 spawnOffset;
	// ref for move script comp
	private MoveScript moveScript;
	// ref for health script comp
	private HealthScript healthScript;
	// weapon comp ref
	private WeaponScript[] weapons;
	// ref for main cam
	private Camera mainCam;
	#endregion
	
	#region onAwake
	void Awake() {
		// find obj's move script comp
		moveScript = GetComponent<MoveScript>();
		// find obj's health script comp
		healthScript = GetComponent<HealthScript>();
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
		}
		// if instance is spawn
		else {
			// Destroy the instance when out of the camera
			// 'IsVisibleFrom' is renderer's extension method
			if (renderer.IsVisibleFrom(mainCam) == false) {
				Respawn();
				//Destroy(gameObject);
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
		Vector3 newPos = new Vector3(
			// base pos + random x-pos + instance size shift
			mainCam.transform.position.x + camExtW * Random.Range(1.0f, 3.0f) + objExts.x + spawnOffset.x,
			// from negative (half view - half instance), to positive
			(camExtH - objExts.y - spawnOffset.y) * Random.Range(-1.0f, 1.0f),
			transform.position.z);
		
		// apply new pos to instance
		transform.position = newPos;
	}
	
	// respawn enemy instance
	public void Respawn() {
		// reset instance as disabled
		Spawn(false);
		// reposition
		Reposition();
		// disable invincible on respawn
		if (healthScript != null) {
			healthScript.hp = 1;
			healthScript.isInvincible = false;
		}
	}
	#endregion
}

