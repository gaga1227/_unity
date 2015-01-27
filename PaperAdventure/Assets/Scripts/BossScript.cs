using UnityEngine;
using System.Collections;

// Boss generic behavior

public class BossScript : MonoBehaviour {
	#region vars
	// spawn script ref
	private SpawnScript spawnScript;
	// animator comp ref
	private Animator animator;
	// weapon script ref
	private WeaponScript[] weapons;
	// move script ref
	//private MoveScript moveScript;
	#endregion
	
	#region onAwake
	void Awake() {
		// find spawn script comp
		spawnScript = GetComponent<SpawnScript>();
		// find animator comp
		animator = GetComponent<Animator>();
		// find list of weapon script comps only once
		weapons = GetComponentsInChildren<WeaponScript>();
		// find move script comp
		//moveScript = GetComponent<MoveScript>();
	}
	#endregion
	
	#region onStart
	void Start() {
	}
	#endregion
	
	#region onUpdate
	void Update () {
		// fire weapons
		//fireWeapons();
	}
	#endregion

	#region handlers
	void OnTriggerEnter2D(Collider2D otherCollider2D) {
		// get script comps from collider
		HealthScript health = otherCollider2D.gameObject.GetComponent<HealthScript>();
		ShotScript shot = otherCollider2D.gameObject.GetComponent<ShotScript>();
		// trigger Hit handler on player
		if (health != null && !health.isEnemy) {
			onHit();
		}
		// trigger Hit handler on player shot
		if (shot != null && !shot.isEnemyShot) {
			onHit();
		}
	}

	// on hit by enemy
	void onHit() {
		// switch to hit animation
		animator.SetTrigger("Hit");
		// reduce size
		transform.localScale -= new Vector3(0.007f, 0.007f, 0);
	}
	#endregion

	#region methods
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

