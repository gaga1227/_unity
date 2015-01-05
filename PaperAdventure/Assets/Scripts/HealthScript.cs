using UnityEngine;
using System.Collections;

// Handle hitpoints and damages

public class HealthScript : MonoBehaviour {
	#region vars
	// enemy flag
	public bool isEnemy = true;

	// health points
	public int hp = 1;

	// invincible vars
	// damaged but not killed will trigger invincible time
	public bool isInvincible;
	private float invincibleTime = 5.0f;
	private float invincibleCooldown;
	#endregion
	
	#region onStart
	void Start () {
		// init isInvincible flag as false
		isInvincible = false;
		invincibleCooldown = 0.0f;
	}
	#endregion
	
	#region onUpdate
	void Update () {
		// if is in invincibleCooldown
		if (invincibleCooldown > 0) {
			invincibleCooldown -= Time.deltaTime;
		}
		// otherwise isInvincible set to false
		else {
			isInvincible = false;
		}
	}
	#endregion

	#region methods
	// called when hit by projectile
	// Inflicts damage and check if the object should be destroyed
	public void Damage(int damageCount) {
		// no damage when player is invincible
		if (isInvincible) return;

		//reduce HP by damage count
		hp -= damageCount;
		//destroy object if HP is negative (Dead!)
		if (hp <= 0) {
			if (isEnemy) {
				// respawn if enemy
				SpawnScript spawn = transform.gameObject.GetComponent<SpawnScript>();
				if (spawn != null) {
					spawn.Respawn();
				}
			} else {
				// destroy if player
				Destroy(gameObject);
			}
		}
		//damaged but alive, HP > 0
		else {
			// set player invincible
			isInvincible = true;
			// reset invincible cooldown to invincible time
			invincibleCooldown = invincibleTime;
		}
	}
	#endregion

	#region handlers
	// collision trigger handler
	void OnTriggerEnter2D(Collider2D otherCollider) {
		// get script comp ref from collider -> gameobject -> script comp
		ShotScript shot = otherCollider.gameObject.GetComponent<ShotScript>();
		HealthScript health = otherCollider.gameObject.GetComponent<HealthScript>();

		// if is hit by a shot instance
		if (shot != null) {
			// if shot projectile is from other party (avoids friendly fire)
			if (shot.isEnemyShot != isEnemy) {
				// call damage method
				Damage(shot.damage);
				// destroy shot object immediately, before its self destruction
				// targets the gameobject of shot, not the script comp itself
				Destroy(shot.gameObject);
			}
		}

		// if is hit by player instance
		if (health != null) {
			// if self and collider is NOT same type
			if (health.isEnemy != isEnemy) {
				// call damage methods
				health.Damage(1);
				Damage(1);
			}
		}
	}
	#endregion
}
