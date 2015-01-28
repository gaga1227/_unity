using UnityEngine;
using System.Collections;

// Boss generic behavior

public class BossScript : MonoBehaviour {
	#region vars
	// spawn script ref
	private SpawnScript spawnScript;
	// health script ref
	private HealthScript healthScript;
	// animator comp ref
	private Animator animator;
	// renderer comp ref
	private SpriteRenderer spriteRenderer;
	// weapon script ref
	private WeaponScript[] weapons;
	// move script ref
	//private MoveScript moveScript;

	// default boss HP
	private int defaultHP;

	// boss attack flag
	public bool willAttack;

	// sprites
	public Sprite damaged0;
	public Sprite damaged1;
	public Sprite damaged2;
	public Sprite damaged3;
	#endregion
	
	#region onAwake
	void Awake() {
		// find spawn script comp
		spawnScript = GetComponent<SpawnScript>();
		// find health script comp
		healthScript = GetComponent<HealthScript>();
		// find animator comp
		animator = GetComponent<Animator>();
		// find renderer comp
		spriteRenderer = GetComponent<SpriteRenderer>();
		// find list of weapon script comps only once
		weapons = GetComponentsInChildren<WeaponScript>();
		// find move script comp
		//moveScript = GetComponent<MoveScript>();
	}
	#endregion
	
	#region onStart
	void Start() {
		// store default HP
		if (healthScript != null) defaultHP = healthScript.hp;

		// boss won't attack at start
		willAttack = false;
	}
	#endregion
	
	#region onUpdate
	void Update () {
		// update boss status
		updateBossStatus();

		// fire weapons
		if (willAttack) fireWeapons();
	}
	#endregion

	#region handlers
	void OnTriggerEnter2D(Collider2D otherCollider2D) {
		// get script comps from collider
		HealthScript colliderHealth = otherCollider2D.gameObject.GetComponent<HealthScript>();
		ShotScript colliderShot = otherCollider2D.gameObject.GetComponent<ShotScript>();
		// trigger Hit handler on player
		if (colliderHealth != null && !colliderHealth.isEnemy) {
			onHit();
		}
		// trigger Hit handler on player shot
		if (colliderShot != null && !colliderShot.isEnemyShot) {
			onHit();
		}
	}

	// on hit by enemy
	void onHit() {
		// switch to hit animation
		if (animator != null) animator.SetTrigger("Hit");
		// reduce size
		transform.localScale -= new Vector3(0.007f, 0.007f, 0);
	}
	#endregion

	#region methods
	// update boss status
	void updateBossStatus() {
		// update damaged level
		// if all comps are valid
		if (spawnScript != null && healthScript != null && spriteRenderer != null) {
			// if boss is spawned in scene
			if (spawnScript.hasSpawn) {
				// no damage
				if (healthScript.hp > (defaultHP * 0.8)) {
					spriteRenderer.sprite = damaged0;
				}
				// damage level 1
				else if (healthScript.hp > (defaultHP * 0.5)) {
					spriteRenderer.sprite = damaged1;
				}
				// damage level 2
				else if (healthScript.hp > (defaultHP * 0.2)) {
					spriteRenderer.sprite = damaged2;
				}
				// damage level 3
				else {
					spriteRenderer.sprite = damaged3;
				}

				// update willAttack flag
				willAttack = (healthScript.hp > (defaultHP * 0.3)) ? false : true;
			}
		}
		// update attack animation if valid
		if (animator != null) {
			animator.SetBool("Attack", willAttack);
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

