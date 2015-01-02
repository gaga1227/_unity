using UnityEngine;
using System.Collections;

// Enemy generic behavior

public class EnemyScript : MonoBehaviour {
	#region vars
	// weapon comp ref
	private WeaponScript[] weapons;
	#endregion
	
	#region onAwake
	void Awake() {
		// find list of weapon script comps only once
		weapons = GetComponentsInChildren<WeaponScript>();
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
}

