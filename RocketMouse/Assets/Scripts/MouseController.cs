using UnityEngine;
using System.Collections;

public class MouseController : MonoBehaviour {
	#region vars
	//jetpack force
	public float jetpackForce = 75.0f;
	//moving speed
	public float forwardMovementSpeed = 3.0f;
	//jetpack particle comp ref
	public ParticleSystem jetpack;
	//ground check gameobject ref
	public Transform groundCheckTransform;
	//ground check layermask ref
	//layermask works as a filter, returns any object tagged with a certain layer
	public LayerMask groundCheckLayerMask;
	//grounded flag
	private bool grounded;
	//animator ref
	Animator animator;
	//mouse death flag
	private bool dead = false;
	//coins count (unsigned integer: can only store positive integers)
	private uint coins = 0;
	#endregion

	#region onStart
	void Start () {
		//cache animator comp
		animator = GetComponent<Animator>();
	}
	#endregion

	#region onUpdate
	void Update () {
		
	}
	#endregion

	#region onFixedUpdate
	void FixedUpdate () {
		//enable jetpack on click
		bool jetpackActive = Input.GetButton("Fire1");

		//jetpack can only be on when NOT dead
		jetpackActive = jetpackActive && !dead;

		//apply jetpack force if enabled
		if (jetpackActive) {
			rigidbody2D.AddForce(new Vector2(0, jetpackForce));
		}

		//apply moving force if NOT dead
		if (!dead) {
			//set moving speed to self via rigidbody2D
			//sets the velocity x-component, without affecting y-component
			Vector2 newVelocity = rigidbody2D.velocity;
			newVelocity.x = forwardMovementSpeed;
			rigidbody2D.velocity = newVelocity;
		}

		//check grounded status
		UpdateGroundedStatus();

		//updates jetpack particles
		AdjustJetpack(jetpackActive);
	}
	#endregion

	#region Methods
	//check grounded status
	//------------------------------------------------------------------------
	void UpdateGroundedStatus() {
		//local grounded flag
		grounded = Physics2D.OverlapCircle(
			//use position of the ground check object as center
			groundCheckTransform.position,
			//do an overlap check with a radius of 0.1f
			0.1f,
			//with any object returns true with the layermask filter
			groundCheckLayerMask);
		//pass grounded flag result to animator parameter
		//to switch animations
		animator.SetBool("grounded", grounded);
	}

	//updates jetpack particles
	//------------------------------------------------------------------------
	void AdjustJetpack (bool jetpackActive) {
		//switch on/off depends on grounded
		jetpack.enableEmission = !grounded;
		//adjust emission rate depends on user action
		jetpack.emissionRate = jetpackActive ? 300.0f : 50.0f; 
	}

	//hit by laser handler
	//------------------------------------------------------------------------
	void HitByLaser(Collider2D laserCollider) {
		//set death status
		dead = true;
		//and set animator params
		animator.SetBool("dead", true);
	}

	//hit by coin handler
	//------------------------------------------------------------------------
	void CollectCoin(Collider2D coinCollider) {
		//adds to coins count
		coins++;
		//and destroy coin gameobject
		Destroy(coinCollider.gameObject);
	}
	#endregion

	#region Handlers
	//collision handlers
	//------------------------------------------------------------------------
	void OnTriggerEnter2D(Collider2D collider) {
		//check collider target's tag
		if ( collider.gameObject.CompareTag("Coins") ) {
			//trigger hit by coin
			CollectCoin(collider);
		} else {
			//trigger hit by laser
			HitByLaser(collider);
		}
	}
	#endregion
}
