﻿using UnityEngine;
using System.Collections;

// Player controller and behavior

public class PlayerScript : MonoBehaviour {
	#region vars
	// player speed
	public Vector2 speed = new Vector2(50, 50);
	// player movement
	private Vector2 movement;
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
	}
	#endregion

	#region onFixedUpdate
	void FixedUpdate() {
		// Move the game object
		rigidbody2D.velocity = movement;
	}
	#endregion

	#region handlers
	// collision handler
	void OnCollisionEnter2D(Collision2D collision) {
		// init player damage flag to false
		// won't hurt player unless collide with enemy
		bool damagePlayer = false;

		// find enemy script comp from collision gameobject
		EnemyScript enemy = collision.gameObject.GetComponent<EnemyScript>();
		// if found enemy script in collision gameobject
		if (enemy != null) {
			// find enemy's health script comp
			HealthScript enemyHealth = enemy.GetComponent<HealthScript>();
			// if found, call Damage method with enemy's full HP (kill it)
			if (enemyHealth != null) enemyHealth.Damage(enemyHealth.hp);
			// set player damage flag to true, makes this collision will hurt player
			damagePlayer = true;
		}
		
		// if player damage flag is true
		// only when collide with enemy
		if (damagePlayer) {
			// find player's health script comp
			HealthScript playerHealth = this.GetComponent<HealthScript>();
			// if found, call Damage method with 1 HP point
			if (playerHealth != null) playerHealth.Damage(1);
		}
	}
	#endregion
}