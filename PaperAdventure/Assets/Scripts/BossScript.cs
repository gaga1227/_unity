using UnityEngine;
using System.Collections;

// Boss generic behavior

public class BossScript : MonoBehaviour {
	#region vars
	// animator comp ref
	private Animator animator;
	// weapon comp ref
	private WeaponScript[] weapons;
	#endregion
	
	#region onAwake
	void Awake() {
		// find list of weapon script comps only once
		weapons = GetComponentsInChildren<WeaponScript>();
		// find animator comp
		animator = GetComponent<Animator>();
	}
	#endregion
	
	#region onStart
	void Start() {
	}
	#endregion
	
	#region onUpdate
	void Update () {
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

	#region handlers
	void OnTriggerEnter2D(Collider2D otherCollider2D) {
		// get shot script comp from collider
		ShotScript shot = otherCollider2D.gameObject.GetComponent<ShotScript>();
		if (shot != null) {
			if (!shot.isEnemyShot) {
				// trigger Hit animation
				animator.SetTrigger("Hit");
			}
		}
	}
	#endregion
}

