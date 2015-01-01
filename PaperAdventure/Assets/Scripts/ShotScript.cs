using UnityEngine;
using System.Collections;

// Projectile behavior

public class ShotScript : MonoBehaviour {
	#region vars
	// designer vars, which can be adjusted in Unity GUI
	public int damage = 1;
	public bool isEnemyShot = false;
	// shot lifespan
	private float lifespan = 5.0f;
	#endregion

	#region onStart
	void Start () {
		// destroy obj after # secs to avoid any leak
		Destroy(gameObject, lifespan);
	}
	#endregion
	
	#region onUpdate
	void Update () {
		
	}
	#endregion
}

