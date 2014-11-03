using UnityEngine;
using System.Collections;

public class CatController : MonoBehaviour {
	#region vars
	//vars for cat carrier's tailing motion
	private Transform followTarget;
	private bool isZombie;
	//using same var names as zombie
	private float moveSpeed;
	private float turnSpeed;
	//placeholder for current stage target position
	private Vector3 targetPosition;
	#endregion

	#region on update
	void Update () {
		//moving cat carrier in motion if in zombie mode
		if(isZombie) {
			//get current cat carrier's pos
			Vector3 currentPosition = transform.position;
			//get position difference of this cat carrier and its target
			Vector3 moveDirection = targetPosition - currentPosition;

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
			//when target is ahead of this cat carrier (magnitude is positive)
			//when cat carrier has not catching up to target
			if (distanceToTarget > 0) {
				//set distance upper limit to movespeed
				//to limit cat carrier's moving speed to moveSpeed
				if ( distanceToTarget > moveSpeed ) {
					distanceToTarget = moveSpeed;
				}
				//normalize direction vector to a unit of 1
				moveDirection.Normalize();
				//calculate target position
				//use 'distanceToTarget' as speed to create an easing effect
				//'distanceToTarget' (speed) diminishes when reaching target
				Vector3 target = currentPosition + moveDirection * distanceToTarget;
				//start transitioning to target transform position
				transform.position = Vector3.Lerp(
					currentPosition,
					target,
					moveSpeed * Time.deltaTime );
			}
		}
	}
	#endregion

	#region methods
	//method to turn gameobject zombified
	public void JoinConga( Transform followTarget, float moveSpeed, float turnSpeed ) {
		//set following target and copy motion info to this cat carrier
		this.followTarget = followTarget; //ref
		this.moveSpeed = moveSpeed * 2.25f;
		this.turnSpeed = turnSpeed;
		//init target position to followTarget's position
		targetPosition = followTarget.position; //value
		//set cat carrier to zombie mode
		isZombie = true;
		//find 'cat' gameobject from 'cat carrier'
		Transform cat = transform.FindChild("cat");
		//switch off gameobject's collider once collided
		//stops firing more collision events
		//set animator parameter to trigger proper animation
		cat.collider2D.enabled = false;
		cat.GetComponent<Animator>().SetBool( "InConga", true );
	}
	#endregion

	#region handlers
	//anim clip event handler (from cat)
	//method to update current stage target position, from follow target
	public void UpdateTargetPosition() {
		targetPosition = followTarget.position;
	}

	//anim clip event handler (from cat)
	//removes gameobject from scene
	public void GrantCatTheSweetReleaseOfDeath() {
		//same as 'Destroy'
		//'gameObject' is a ref to self
		DestroyObject( gameObject );
		Debug.Log("Cat Disappeared! " + gameObject);
	}
	
	//instance out of view handler (from cat)
	//called when gameobject is out of ALL cameras views
	//called from 'cat'
	public void OnBecameInvisible() {
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
