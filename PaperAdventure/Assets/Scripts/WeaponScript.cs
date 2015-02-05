using UnityEngine;
using System.Collections;

// Launch projectile

public class WeaponScript : MonoBehaviour {
	#region vars
	// game settings
	private GameObject GameSettingsObj;
	private SettingsScript gameSettings;

	// shot prefab ref
	public Vector3 shotOriginOffset = new Vector3(0, 0, 0);

	// shot prefab ref
	public Transform shotPrefab;

	// interval in seconds between two shots
	public float shootingRate = 0.25f;

	// mute flag
	public bool mute = false;

	// accumulative shot release flag
	private bool shotIsReleased = true;
	private float accumulativeDamage = 1f;
	private float accumulativeScale = 1f;
	private float accumulativeAlpha = 0f;

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

	// accumulating shot object
	private Transform accumulatingShot;
	private SpriteRenderer accumulatingShotRenderer;
	private AudioSource accumulatingShotAudioSource;
	#endregion

	#region onAwake
	void Awake () {
		// find game settings ref
		GameSettingsObj = GameObject.Find("GameSettings");
		if (GameSettingsObj != null) {
			gameSettings = GameSettingsObj.GetComponent<SettingsScript>();
		}

		// find accumulatingShot and renderer
		// and set initial states
		accumulatingShot = transform.Find("accumulatingShot");
		if (accumulatingShot != null) {
			// renderer
			accumulatingShotRenderer = accumulatingShot.GetComponent<SpriteRenderer>();
			if (accumulatingShotRenderer != null) accumulatingShotRenderer.enabled = false;
			// audio source
			accumulatingShotAudioSource = accumulatingShot.GetComponent<AudioSource>();
			if (accumulatingShotAudioSource != null) accumulatingShotAudioSource.enabled = false;
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

			// if is player shot and accumulative shot not released
			if (!isEnemy && !shotIsReleased) {
				// apply accumulative scale and damage to first shot after accumulation
				shotTransform.localScale = new Vector3(accumulativeScale, accumulativeScale, 1);
				ShotScript shotScript = shotTransform.GetComponent<ShotScript>();
				if (shotScript != null) shotScript.damage = (int) accumulativeDamage;
				
				// set release flag, making next shot regular
				shotIsReleased = true;
//				Debug.Log ("Release shot!!!");
//				Debug.Log ("- Damage: " + (int) accumulativeDamage);
//				Debug.Log ("- Scale: " + accumulativeScale);
//				Debug.Log ("- Alpha: " + accumulativeAlpha);

				// reset accumulative scale and damage after release
				if (accumulativeScale != 1f) accumulativeScale = 1f;
				if (accumulativeDamage != 1f) accumulativeDamage = 1f;
				if (accumulativeAlpha != 0f) accumulativeAlpha = 0f;

				// if found accumulating shot renderer
				if (accumulatingShotRenderer != null) {
					// set accumulating shot renderer off
					// and reset accumulativeScale scale
					if (accumulatingShotRenderer.enabled) {
						accumulatingShotRenderer.enabled = false;
						accumulatingShot.localScale = new Vector3(accumulativeScale*3, accumulativeScale*3, 1);
					}
					// stop sfx
					if (accumulatingShotAudioSource != null) {
						if (accumulatingShotAudioSource.enabled) {
							accumulatingShotAudioSource.enabled = false;
						}
					}
				}
			}

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

	// Start accumulating shot
	public void Accumulate(bool isEnemy) {
		// exit on enemy shot
		if (isEnemy) return;

		// set release flag
		if (shotIsReleased) shotIsReleased = false;

		// accumulating scale and damage
		if (accumulativeDamage < 100f) accumulativeDamage += 0.25f;
		if (accumulativeScale < 4f) accumulativeScale += 0.01f;
		if (accumulativeAlpha < 1f) accumulativeAlpha += 0.025f;

		// if found accumulating shot renderer
		if (accumulatingShotRenderer != null) {
			// set accumulating shot renderer on
			if (!accumulatingShotRenderer.enabled) {
				accumulatingShotRenderer.enabled = true;
			}
			// play sfx
			if (gameSettings == null || (gameSettings != null && gameSettings.soundEnabled)) {
				if (accumulatingShotAudioSource != null) {
					if (!accumulatingShotAudioSource.enabled) {
						accumulatingShotAudioSource.enabled = true;
					}
				}
			}
			// match scale to accumulativeScale
			accumulatingShot.localScale = new Vector3(accumulativeScale*3, accumulativeScale*3, 1);
			// apply alpha transition
			accumulatingShotRenderer.color = new Color(255, 255, 255, accumulativeAlpha);
		}
	}
	#endregion
}
