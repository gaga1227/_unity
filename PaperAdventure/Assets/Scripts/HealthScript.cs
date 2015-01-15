using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

// Handle hitpoints and damages

public class HealthScript : MonoBehaviour {
	#region vars
	// enemy flag
	public bool isEnemy = true;

	// health points
	public int hp = 1;

	// list of hearts
	private List<Transform> hearts;
	public Transform canvas;
	public Transform heartPrefab;

	// invincible vars
	// damaged but not killed will trigger invincible time
	public bool isInvincible;
	private float invincibleTime = 3.0f;
	private float invincibleCooldown;
	#endregion
	
	#region onStart
	void Start () {
		// init isInvincible flag as false
		isInvincible = false;
		invincibleCooldown = 0.0f;

		// update hearts UI if is player
		if (!isEnemy) {
			updateHearts(hp);
		}
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

		// reduce HP by damage count
		hp -= damageCount;

		// update hearts UI if is player
		if (!isEnemy) {
			updateHearts(hp);
		}

		// destroy object if HP is negative (Dead!)
		if (hp <= 0) {
			// add explosion effects and SE at obj's position
			SpecialEffectsHelper.Instance.Explosion(transform.position);
			SoundEffectsHelper.Instance.MakeExplosionSound();

			// if enemy
			if (isEnemy) {
				// respawn
				SpawnScript spawn = transform.gameObject.GetComponent<SpawnScript>();
				if (spawn != null) {
					spawn.Respawn();
				}
			}
			// else player
			else {
				// Load menu scene
				// need to execute this line before gameobject is destroyed
				Invoke("LoadMenu", 2f);
				// destroy if player, after Invoke LoadMenu
				Destroy(gameObject, 3f);
			}
		}
		//damaged but alive, HP > 0
		else if (hp >= 0) {
			// set player invincible
			isInvincible = true;
			// reset invincible cooldown to invincible time
			invincibleCooldown = invincibleTime;
		}
	}

	// update UI hearts
	private void updateHearts(int newHp) {
		// init hearts tracking list if not created already
		if (hearts == null) {
			hearts = new List<Transform>();
		}

		// validate supplied count value
		if (newHp == hearts.Count) return;

		// position vars
		float startX = 20.0f;
		float posY = -20.0f;
		float width = 60.0f;
		float gapX = 5.0f;

		// get update difference
		int countDiff = newHp - hearts.Count;

		// HP increasing
		// add heart
		if (countDiff > 0) {
			// loop through countDiff and update heart instances and list
			for (int i = 0; i < Mathf.Abs(countDiff); i++) {
				// temp ref for heart instance
				var heart = Instantiate(heartPrefab) as Transform;
				// add heart as child of canvas
				heart.SetParent(canvas, false);
				// get RectTransform comp ref from heart
				RectTransform rectT = heart.GetComponent<RectTransform>();
				// position heart after the last one in list
				float posX = (width + gapX) * (hearts.Count + 0) + startX;
				rectT.anchoredPosition = new Vector2(posX, posY);
				// add heart to list
				hearts.Add(heart);
			}
		}
		// HP decreasing
		// remove heart
		else {
			// loop through countDiff and update heart instances and list
			for (int i = 0; i < Mathf.Abs(countDiff); i++) {
				// temp ref for the last heart instance in list
				Transform heart = hearts.LastOrDefault();
				// if heart exists
				if (heart != null) {
					// remove last heart from list
					hearts.Remove(heart);
					// remove last heart from scene
					Destroy(heart.gameObject);
				}
			}
		}
	}

	// load Menu scene
	private void LoadMenu() {
		Application.LoadLevel("Menu");
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
