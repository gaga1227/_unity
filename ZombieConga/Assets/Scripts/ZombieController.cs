using UnityEngine;
using System.Collections;
//import generic package to use 'list' type
using System.Collections.Generic;

public class ZombieController : MonoBehaviour {
	#region vars
	//public vars
	//------------------------------------------------------------------------
	
	//move speed (per frame)
	public float moveSpeed;
	//turn speed (per frame)
	public float turnSpeed;
	
	//private vars
	//------------------------------------------------------------------------
	
	//move direction (with init value)
	private Vector3 moveDirection = Vector3.right;
	//declare a list of transform for cats conga line
	private List<Transform> congaLine = new List<Transform>();
	#endregion

	#region on start
	void Start () {
		
	}
	#endregion

	#region on frame
	void Update () {
		
		//animating game object towards direction
		//------------------------------------------------------------------------
		
		//get self position
		Vector3 currentPosition = transform.position;
		
		//on input
		if(Input.GetButton("Fire1")) {
			//get target position
			//using camera to convert input pos to world coordinates
			Vector3 moveToward = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			//update current direction vector
			moveDirection = moveToward - currentPosition;
			//reset z pos to 0 for 2D
			moveDirection.z = 0;
			//ensures moveDirection is 'unit length' (has a length of 1)
			//Unit length vectors are convenient because you can multiply them 
			//by scalar values e.g. speed
			moveDirection.Normalize();
			//Debug.Log ("moveDirection: " + moveDirection);
		}
		
		//calculate target position
		//based on current position, plus per frame increment
		//towards move direction
		Vector3 targetPosition = moveDirection * moveSpeed + currentPosition;
		//apply interpolated position to self
		//use deltaTime makes updates based on time instead of frames
		transform.position = Vector3.Lerp(currentPosition, targetPosition, Time.deltaTime);
		//Debug.Log ("Time.deltaTime: " + Time.deltaTime);
		
		//animating game object with rotation
		//------------------------------------------------------------------------
		
		//get target angle from current move direction
		//Atan2 returns in radians, then convert to degree
		float targetAngle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
		//update rotation with interpolation
		//rotation takes Quaternion angles and Slerp is recommended
		//rotation should apply to Z axis in 2D mode
		transform.rotation = Quaternion.Slerp(
			transform.rotation, 
			Quaternion.Euler(0,0,targetAngle),
			turnSpeed * Time.deltaTime);

		//check and keep self within screen bounds
		//------------------------------------------------------------------------
		EnforceBounds();
	}
	#endregion

	#region physics colliders
	//physics colliders
	//------------------------------------------------------------------------
	
	//this makes private var accessible in unity inspector
	[SerializeField]
	//declare single array of any size, of type 'PolygonCollider2D'
	private PolygonCollider2D[] colliders;
	//current collider index, default is 0, which is matched in unity editor
	private int currentColliderIndex = 0;
	
	//anim clip event handler
	//updates current collider from sprite frames
	public void SetColliderForSprite( int spriteNum ) {
		colliders[currentColliderIndex].enabled = false;
		currentColliderIndex = spriteNum;
		colliders[currentColliderIndex].enabled = true;
	}
	
	//collision handler
	//------------------------------------------------------------------------
	void OnTriggerEnter2D( Collider2D other ) {
		GameObject target = other.gameObject;
		Debug.Log ("Hit " + target);
		if(target.CompareTag("cat")) {
			//get proper target's transform as follow target
			Transform followTarget = (congaLine.Count == 0) ? transform : congaLine[congaLine.Count-1];
			//get cat gameobject's script component and call its public method
			//to make target start tailing followTarget, with zombie's speeds
			target.GetComponent<CatController>().JoinConga( followTarget, moveSpeed, turnSpeed );
			//finally push target's transform to list
			congaLine.Add( target.transform );
		}
		else if (target.CompareTag("enemy")) {
			Debug.Log ("Pardon me, ma'am.");
		}
	}
	#endregion

	#region keeping self in view bounds
	private void EnforceBounds() {
		//get current position, retains z position
		Vector3 newPosition = transform.position; 
		//get camera ref
		Camera mainCamera = Camera.main;
		//get camera position: (0,0)
		Vector3 cameraPosition = mainCamera.transform.position;

		//get distances from view center to edges
		//given camera is positioned at (0,0)
		float xOffset = 0.7f;
		float yOffset = 0.7f;
		float xDist = mainCamera.aspect * mainCamera.orthographicSize - xOffset;
		float yDist = mainCamera.orthographicSize - yOffset;

		//calculate positions for all edges
		float xMin = cameraPosition.x - xDist;
		float yMin = cameraPosition.y - yDist;
		float xMax = cameraPosition.x + xDist;
		float yMax = cameraPosition.y + yDist;
		
		//when out of horizontal bounds
		if ( newPosition.x < xMin || newPosition.x > xMax ) {
			//clamp overlimit position to edge position
			//either left and right edge position
			newPosition.x = Mathf.Clamp( newPosition.x, xMin, xMax );
			//revert move direction
			moveDirection.x = -moveDirection.x;
		}
		//when out of vertical bounds
		if ( newPosition.y < yMin || newPosition.y > yMax ) {
			//clamp overlimit position to edge position
			//either top and bottom edge position
			newPosition.y = Mathf.Clamp( newPosition.y, yMin, yMax );
			//revert move direction
			moveDirection.y = -moveDirection.y;
		}

		//apply new position to self
		transform.position = newPosition;
	}
	#endregion
}
