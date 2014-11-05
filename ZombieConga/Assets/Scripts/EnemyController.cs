using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour {
	#region vars
	//public vars
	//------------------------------------------------------------------------
	
	//moving speed
	public float speed = -1;
	
	//private vars
	//------------------------------------------------------------------------
	
	//spawn point's transform
	private Transform spawnPoint;

	//main camera ref
	private Camera mainCam;
	#endregion

	#region on start
	void Start () {
		//find spawn point game object and assign its transform to var
		spawnPoint = GameObject.Find("SpawnPoint").transform;
		//cache main camera ref
		mainCam = Camera.main;
	}
	#endregion

	#region on frame
	void Update () {
		
	}
	#endregion

	#region on fixed frame
	void FixedUpdate () {
		//assign speed to physics rigibody,
		//this makes gameobject move to left
		rigidbody2D.velocity = new Vector2(speed, 0);
	}
	#endregion

	#region handlers
	//out of view handler
	//called when gameobject is out of ALL cameras views
	//------------------------------------------------------------------------
	void OnBecameInvisible() {
		//causes error on trying to find main camera
		//when it doesn't exist anymore (e.g. stop scene)
		//Camera camera = Camera.main;

		//exit if no target camera
		if (mainCam == null) return;
		//get upper threshold of spawn point's y position
		//which is half of view height minus half of gameobject's height (rough estimate)
		float yMax = mainCam.orthographicSize - 0.8f;
		//update self position
		transform.position = new Vector3(
			//use spawn position's x pos
			spawnPoint.position.x,
			//get a random y pos within range
			Random.Range(-yMax, yMax),
			//keep current z pos
			transform.position.z );
	}
	#endregion
}
