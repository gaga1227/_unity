using UnityEngine;
using System.Collections;

// Handle hitpoints and damages

public class HealthScript : MonoBehaviour {
	#region vars
	// designer vars
	public int hp = 1;
	public bool isEnemy = true;
	#endregion
	
	#region onStart
	void Start () {

	}
	#endregion
	
	#region onUpdate
	void Update () {
		
	}
	#endregion

	#region methods
	// called when hit by projectile
	// Inflicts damage and check if the object should be destroyed
	public void Damage(int damageCount) {
		//reduce HP by damage count
		hp -= damageCount;
		//deatory object if HP is negative (Dead!)
		if (hp <= 0) {
			Destroy(gameObject);
		}
	}
	#endregion

	#region handlers
	// collision trigger handler
	void OnTriggerEnter2D(Collider2D otherCollider) {
		// get script comp ref from collider -> gameobject -> script comp
		ShotScript shot = otherCollider.gameObject.GetComponent<ShotScript>();
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
	}
	#endregion
}
