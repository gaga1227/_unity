using UnityEngine;
using System.Collections;

public class CatUpdater : MonoBehaviour {

	#region vars
	//'CatController' ref
	private CatController catController;
	#endregion

	#region on start
	void Start () {
		//get ref of script comp of cat carrier(parent)
		catController = transform.parent.GetComponent<CatController>();
	}
	#endregion

	#region methods
	//receivers to anim clips events
	//proxy wrappers to cat carrier's public methods on gameobject
	//only 'cat carriers' are gameobjects
	void UpdateTargetPosition() {
		catController.UpdateTargetPosition();
	}
	void GrantCatTheSweetReleaseOfDeath() {
		catController.GrantCatTheSweetReleaseOfDeath();
		//Destroy( transform.parent.gameObject );
	}
	void OnBecameInvisible() {
		catController.OnBecameInvisible();
	}
	#endregion
}