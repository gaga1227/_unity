using UnityEngine;
using System.Collections;

/// <summary>
/// main class
/// </summary>
public class CatController : MonoBehaviour {

	// anim clip event handler
	// removes gameobject from scene
	void GrantCatTheSweetReleaseOfDeath() {
		DestroyObject( gameObject );
	}

}
