﻿using UnityEngine;
using System.Collections;

// Projectile behavior

public class ShotScript : MonoBehaviour {
	#region vars
	// designer vars, which can be adjusted in Unity GUI
	public int damage = 1;
	public float shotRotation = 6.0f;
	public bool isEnemyShot = false;

	// shot lifespan
	private float lifespan = 5.0f;

	// MenuControl
	private GameObject MenuControl;
	private MenuScript MenuControlScript;
	#endregion

	#region onAwake
	void Awake () {
		// assign menuControl ref
		MenuControl = GameObject.Find("MenuControl");
		if (MenuControl != null) {
			MenuControlScript = MenuControl.GetComponent<MenuScript>();
		}
	}
	#endregion

	#region onStart
	void Start () {
		// destroy obj after # secs to avoid any leak
		Destroy(gameObject, lifespan);
	}
	#endregion
	
	#region onUpdate
	void Update () {
		// rotate shot if is enemy shot
		if (isEnemyShot && MenuControlScript != null && !MenuControlScript.isPaused) {
			RotateShot();
		}
	}
	#endregion

	#region methods
	// rotate shot
	private void RotateShot() {
		if (shotRotation != 0.0f) {
			// rotate transform's rotation by shotRotation on Z axis
			transform.Rotate(0, 0, shotRotation);
		}
	}
	#endregion
}

