using UnityEngine;
using System.Collections;

public class CatController : MonoBehaviour {
	
	//anim clip event handler
	//removes gameobject from scene
	void GrantCatTheSweetReleaseOfDeath() {
		//same as 'Destroy'
		//'gameObject' is a ref to self
		DestroyObject( gameObject );
		Debug.Log("Cat Disappeared! " + gameObject);
	}

	//instance out of view handler
	//called when gameobject is out of ALL cameras views
	void OnBecameInvisible() {
		//same as 'DestroyObject'
		//'gameObject' is a ref to self
		Destroy( gameObject );
		Debug.Log("Cat Removed! " + gameObject);
	}
}
