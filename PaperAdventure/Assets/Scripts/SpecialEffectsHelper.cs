using UnityEngine;
using System.Collections;

// SpecialEffectsHelper

public class SpecialEffectsHelper : MonoBehaviour {
	#region vars
	// Use static keyword for Singleton ref: Instance
	public static SpecialEffectsHelper Instance;

	// particle systems
	public ParticleSystem smokeEffect;
	public ParticleSystem fireEffect;
	#endregion

	#region onAwake
	void Awake () {
		// multiple instances handling
		if (Instance != null) {
			Debug.LogError("Multiple instances of SpecialEffectsHelper!");
		}
		// Register this as the singleton instance
		Instance = this;
	}
	#endregion

	#region methods
	// Create an explosion at the given location
	public void Explosion(Vector3 position) {
		// call instantiate prefabs util at position
		instantiate(smokeEffect, position);
		instantiate(fireEffect, position);
	}
	#endregion

	#region utils
	// Instantiate a Particle system from prefab
	private ParticleSystem instantiate(ParticleSystem prefab, Vector3 position) {
		ParticleSystem newParticleSystem = Instantiate(
			prefab,
			position,
			Quaternion.identity // rotation of the object: 'identity' is no rotation
			) as ParticleSystem; // use 'as' to typecast for anything can be a null value

		// Make sure it will be destroyed in time
		Destroy(
			newParticleSystem.gameObject,
			newParticleSystem.startLifetime
			);

		// return instantiated ParticleSystem
		return newParticleSystem;
	}
	#endregion
}
