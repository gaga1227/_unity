using UnityEngine;
using System.Collections;

// Boss generic behavior

public class BossScript : MonoBehaviour {
	#region vars
	// animator comp ref
	private Animator animator;
	// weapon script ref
	private WeaponScript[] weapons;
	// move script ref
	private MoveScript moveScript;
	#endregion
	
	#region onAwake
	void Awake() {
		// find animator comp
		animator = GetComponent<Animator>();
		// find list of weapon script comps only once
		weapons = GetComponentsInChildren<WeaponScript>();
		// find move script comp
		moveScript = GetComponent<MoveScript>();
	}
	#endregion
	
	#region onStart
	void Start() {

	}
	#endregion
	
	#region onUpdate
	void Update () {
		// stop other enemies
		stopOtherEnemies();

		// set boss stationary
		moveScript.direction = Vector2.zero;

		// fire weapons
		fireWeapons();
	}
	#endregion

	#region handlers
	void OnTriggerEnter2D(Collider2D otherCollider2D) {
		// get script comps from collider
		HealthScript health = otherCollider2D.gameObject.GetComponent<HealthScript>();
		ShotScript shot = otherCollider2D.gameObject.GetComponent<ShotScript>();
		// trigger Hit animation on player
		if (health != null && !health.isEnemy) {
			animator.SetTrigger("Hit");
		}
		// trigger Hit animation on player shot
		if (shot != null && !shot.isEnemyShot) {
			animator.SetTrigger("Hit");
		}
	}
	#endregion

	#region methods
	// stops other enemies from being enabled
	void stopOtherEnemies() {
		// Stop the player and main camera scrolling
		// this will also stop all respawn enemies
		foreach (ScrollingScript scrolling in FindObjectsOfType<ScrollingScript>()) {
			// if is scrolling script on Player
			// set speed to zero to stop player and camera
			if (scrolling.isLinkedToCamera) {
				scrolling.speed = Vector2.zero;
			}
		}
	}

	// fire attached weapons
	void fireWeapons() {
		// Auto-fire (no input required)
		// for all weapon comps in the list
		foreach (WeaponScript weapon in weapons) {
			// if weapon comp found and CanAttack
			// call Attack public method
			// with isEnemy flag (true for Enemy)
			if (weapon != null && weapon.CanAttack) {
				weapon.Attack(true);
			}
		}
	}
	#endregion
}

