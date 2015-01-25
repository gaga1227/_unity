using UnityEngine;
using System.Collections;

// Launch projectile

public class WeaponScript : MonoBehaviour {
	#region vars
	// shot prefab ref
	public Vector3 shotOriginOffset = new Vector3(0, 0, 0);

	// shot prefab ref
	public Transform shotPrefab;

	// interval in seconds between two shots
	public float shootingRate = 0.25f;

	// mute flag
	public bool mute = false;

	// cooldown
	private float shootCooldown;
	// can attack flag
	public bool CanAttack {
		get {
			// returns flag value based on priavte var shootCooldown using getter
			// can attack when positive
			return shootCooldown <= 0f;
		}
	}
	#endregion
	
	#region onStart
	void Start () {
		// init cooldown with 0
		// makes CanAttack true
		shootCooldown = 0f;
	}
	#endregion
	
	#region onUpdate
	void Update () {
		// if is cooldown
		if (shootCooldown > 0) {
			shootCooldown -= Time.deltaTime;
		}
	}
	#endregion
	
	#region methods
	// Create a new projectile if CanAttack
	public void Attack(bool isEnemy) {
		if (CanAttack) {
			// reset shootCooldown to interval value
			// makes CanAttack false until shootCooldown is negative
			shootCooldown = shootingRate;

			// create a new shot instance from shotPrefab as Transform
			// and pass ref to shotTransform
			// using var here makes shotTransform implicitly typed, lets compiler determines the type
			// 'var' can only be used at method scope
			var shotTransform = Instantiate(shotPrefab) as Transform;
			
			// pass self position and rotation to shot instance
			// as starting position and rotation
			shotTransform.position = transform.position + shotOriginOffset;
			shotTransform.rotation = transform.rotation;

			// play SE
			if (!mute) {
				if (isEnemy) {
					SoundEffectsHelper.Instance.MakeEnemyShotSound();
				} else {
					SoundEffectsHelper.Instance.MakePlayerShotSound();
				}
			}
			
			// get shot instance's shot script comp
			// and pass isEnemy value to it
			ShotScript shot = shotTransform.gameObject.GetComponent<ShotScript>();
			if (shot != null) {
				shot.isEnemyShot = isEnemy;
			}

			// get shot instance's move script comp
			// and set direction value to this sprite's right
			MoveScript move = shotTransform.gameObject.GetComponent<MoveScript>();
			if (move != null) {
				move.direction = transform.right;
			}
		}
	}
	#endregion
}
