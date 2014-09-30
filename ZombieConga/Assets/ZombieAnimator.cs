using UnityEngine;
using System.Collections;

public class ZombieAnimator : MonoBehaviour {

	//------------------------------------------------------------------------
	//public vars

	//sprite sequence array
	public Sprite[] sprites;
	//fps (placeholder for time in seconds)
	public float framesPerSecond;
	//move speed
	public float moveSpeed;
	//turn speed
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
		int index = (int)(Time.timeSinceLevelLoad * framesPerSecond);
		Debug.Log (Time.timeSinceLevelLoad);

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
			Vector3 moveToward = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			//update current direction vector
			moveDirection = moveToward - currentPosition;
			//reset z pos to 0 for 2D
			moveDirection.z = 0;
			//ensures moveDirection is 'unit length' (has a length of 1)
			//Unit length vectors are convenient because you can multiply them 
			//by scalar values e.g. speed
			moveDirection.Normalize();
		}

		//calculate updated target position
		Vector3 target = moveDirection * moveSpeed + currentPosition;
		//apply interpolated position to self
		transform.position = Vector3.Lerp(currentPosition, target, Time.deltaTime);
		Debug.Log (Time.deltaTime);

		//------------------------------------------------------------------------
		//animating game object with rotation

		//get target angle from current move direction
		//Atan2 returns in radians
		float targetAngle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
		//update rotation with interpolation
		//rotation takes Quaternion angles and Slerp is recommended
		transform.rotation = Quaternion.Slerp(
			transform.rotation, 
			Quaternion.Euler(0,0,targetAngle),
			turnSpeed * Time.deltaTime);

	}
}
