using UnityEngine;
using System.Collections;

// Creating instance of sounds from code with no effort

public class SoundEffectsHelper : MonoBehaviour {
	#region vars
	// game settings
	private GameObject GameSettingsObj;
	private SettingsScript gameSettings;

	// Use static keyword for Singleton ref: Instance
	public static SoundEffectsHelper Instance;

	// Sound assets refs
	public AudioClip explosionSound;
	public AudioClip playerShotSound;
	public AudioClip enemyShotSound;
	#endregion
	
	#region onAwake
	void Awake () {
		// multiple instances handling
		if (Instance != null) {
			Debug.LogError("Multiple instances of SoundEffectsHelper!");
		}
		// Register this as the singleton instance
		Instance = this;

		// find game settings ref
		GameSettingsObj = GameObject.Find("GameSettings");
		if (GameSettingsObj != null) {
			gameSettings = GameSettingsObj.GetComponent<SettingsScript>();
		}
	}
	#endregion
	
	#region methods
	// public methods for sounds
	public void MakeExplosionSound() {
		MakeSound(explosionSound, 0.5f);
	}
	public void MakePlayerShotSound() {
		MakeSound(playerShotSound, 0.25f);
	}
	public void MakeEnemyShotSound() {
		MakeSound(enemyShotSound, 0.9f);
	}

	// play a sound clip
	private void MakeSound(AudioClip originalClip, float vol) {
		// exit if sound setting is off
		if (gameSettings != null && !gameSettings.soundEnabled) return;
		// As it is not 3D audio clip, position doesn't matter
		AudioSource.PlayClipAtPoint(originalClip, transform.position, vol);
	}
	#endregion
}
