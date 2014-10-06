using UnityEngine;
using System.Collections;

public class ZombieAnimator : MonoBehaviour {

	//------------------------------------------------------------------------
	//public vars

	//sprite sequence array
	public Sprite[] sprites;
	//sprite frames played in a second
	public float spriteFramesPerSecond;
	//move speed (per frame)
	public float moveSpeed;
	//turn speed (per frame)
	public float turnSpeed;

	//------------------------------------------------------------------------
	//private vars

	//ref to renderer
	private SpriteRenderer spriteRenderer;
	//move direction (with init value)
	private Vector3 moveDirection = Vector3.right;

	//on start
	void Start () {

		//cast renderer component as SpriteRenderer type and assign to ref
		spriteRenderer = renderer as SpriteRenderer;

	}
	
	//on frame
	void Update () {

		//------------------------------------------------------------------------
		//use script to animate a sprite sequence

		//get elapsed time (Time.timeSinceLevelLoad) returns time in seconds
		//then get current frame index by multiply with fps
		//and finally converts to int
		int index = (int)(Time.timeSinceLevelLoad * spriteFramesPerSecond);
		//Debug.Log ("timeSinceLevelLoad: " + Time.timeSinceLevelLoad);
		//Debug.Log ("spriteFramesPerSecond: " + spriteFramesPerSecond);

		//divide frame index by sprites sequence count
		//and use remainder as current sprite index
		index = index % sprites.Length;

		//update component renderer with calculated sprite index
		spriteRenderer.sprite = sprites[index];

		//------------------------------------------------------------------------
		//animating game object towards direction

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

		//------------------------------------------------------------------------
		//animating game object with rotation

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

	}
}
