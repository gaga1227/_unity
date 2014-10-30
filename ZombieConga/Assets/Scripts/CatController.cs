using UnityEngine;
using System.Collections;

public class CatController : MonoBehaviour {
	#region vars
	//vars for cat's tailing motion
	private Transform followTarget;
	private bool isZombie;
	//using same var names as zombie
	private float moveSpeed;
	private float turnSpeed;
	#endregion

	#region on update
	void Update () {
		//moving cat in motion if in zombie mode
		if(isZombie) {
			//get current cat's pos
			Vector3 currentPosition = transform.position;
			//get position difference of this cat and its target
			Vector3 moveDirection = followTarget.position - currentPosition;

			//calcaulte tailing direction angle from position difference
			float targetAngle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
			//start transitioning to target rotation
			transform.rotation = Quaternion.Slerp(
				transform.rotation,
				Quaternion.Euler(0, 0, targetAngle),
				turnSpeed * Time.deltaTime );

			//calcaulte tailing distance from position difference
			//direction vector's length
			float distanceToTarget = moveDirection.magnitude;
			//when target is ahead of this cat (magnitude is positive)
			//when cat has not catching up to target
			if (distanceToTarget > 0) {
				//set distance upper limit to movespeed
				//to limit cat's moving speed to moveSpeed
				if ( distanceToTarget > moveSpeed ) {
					distanceToTarget = moveSpeed;
				}
				//normalize direction vector to a unit of 1
				moveDirection.Normalize();
				//calculate target position
				Vector3 target = moveDirection * distanceToTarget + currentPosition;
				//start transitioning to target transform position
				transform.position = Vector3.Lerp(currentPosition, target, moveSpeed * Time.deltaTime);
			}
		}
	}
	#endregion

	#region methods
	//method to turn gameobject zombified
	public void JoinConga( Transform followTarget, float moveSpeed, float turnSpeed ) {
		//set following target and copy motion info to this cat
		this.followTarget = followTarget;
		this.moveSpeed = moveSpeed;
		this.turnSpeed = turnSpeed;
		//set cat to zombie mode
		isZombie = true;
		//switch off gameobject's collider once collided
		//stops firing more collision events
		collider2D.enabled = false;
		//set animator parameter to trigger proper animation
		GetComponent<Animator>().SetBool( "InConga", true );
	}
	#endregion

	#region handlers
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
		//if instance is not in zombie mode
		if ( !isZombie ) {
			//same as 'DestroyObject'
			//'gameObject' is a ref to self
			Destroy( gameObject );
			Debug.Log("Cat Removed! " + gameObject);
		}
	}
	#endregion
}
