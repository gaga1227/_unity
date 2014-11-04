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

	//invinsible flag and period
	private bool isInvincible = false;
	private float timeSpentInvincible;

	//player lives
	private int lives = 3;
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

		//invincible time period control and visual
		//------------------------------------------------------------------------
		if (isInvincible) {
			//vars to control the blinking animaiton
			float timePeriodTreshold = 3f;
			float frequencyFactor = 0.3f;
			float blinkSpanTreshold = 0.15f;
			//adds time period per frame to total time period
			timeSpentInvincible += Time.deltaTime;
			//if total time period is still within threshold
			//blinking gameobject via switching renderer
			if (timeSpentInvincible < timePeriodTreshold) {
				//remainder affects the frequency of the blink
				float remainder = timeSpentInvincible % frequencyFactor;
				//set renderer's flag directly depending on time period spent
				renderer.enabled = remainder > blinkSpanTreshold;
				//Debug.Log("Remainder: " + remainder);
			}
			//if reached treshold, switch off blinking
			//and turn off invincible
			else {
				renderer.enabled = true;
				isInvincible = false;
			}
		}

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
		//cat (used for animations)
		GameObject target = other.gameObject;
		//Cat Carrier (used for tailing motion)
		Transform targetCarrier = other.transform.parent;
		Debug.Log ("Hit " + targetCarrier + "'s " + target);
		//on colliding with others with 'cat' tag
		if(target.CompareTag("cat")) {
			//get proper target's transform as follow target
			Transform followTarget = (congaLine.Count == 0) ? transform : congaLine[congaLine.Count-1];
			//get cat gameobject's parent's script component and call its public method
			//to make targetCarrier start tailing followTarget, with zombie's speeds
			//use 'targetCarrier' for tailing manipulation because gameobject with animation(cat)
			//cannot be manipulated on 'transform', so does it on 'targetCarrier'
			targetCarrier.GetComponent<CatController>().JoinConga( followTarget, moveSpeed, turnSpeed );
			//finally push target's transform to list
			//only cat item gets added, cat carrier has no collider
			congaLine.Add( target.transform );
			//if got enough items, WIN the game
			if (congaLine.Count >= 5) {
				Application.LoadLevel("CongaScenePart2");
				Debug.Log("You won!");
			}
		}
		//on colliding with others with 'enemy' tag while NOT invincible
		else if (!isInvincible && target.CompareTag("enemy")) {
			//reduce player lives by one and check game state
			//if no lives left set game to LOSE
			if (--lives <= 0) {
				Application.LoadLevel("CongaScenePart2");
				Debug.Log("You lost!");
			}
			//if not losing yet, continue...
			else {
				//make self invincible (to ignore further collisions for a period)
				isInvincible = true;
				//resets invincible time period
				timeSpentInvincible = 0;
				//items to remove in conga line
				int itemsToRemove = 2;
				//remove cat carriers in conga line by 'itemsToRemove' times until the last one
				for( int i = 0; i < itemsToRemove && congaLine.Count > 0; i++ ) {
					//index for last item in conga list
					int lastIdx = congaLine.Count-1;
					//get last cat item's transform from list
					Transform cat = congaLine[ lastIdx ];
					//remove last item in conga list
					congaLine.RemoveAt(lastIdx);
					//get cat carrier's script comp and call its public method
					cat.parent.GetComponent<CatController>().ExitConga();
				}
			}
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
