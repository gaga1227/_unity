﻿using UnityEngine;
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
	//animator ref
	private Animator animator;
	//grounded flag
	private bool grounded;
	//mouse death flag
	private bool dead = false;
	//coins count (unsigned integer: can only store positive integers)
	private uint coins = 0;
	//GUI coin texture ref
	public Texture2D coinIconTexture;
	//GUI label style ref
	private GUIStyle labelStyle;
	//Coin audio, played directly by static AudioSource method
	public AudioClip coinCollectSound;
	//SE audio source refs
	public AudioSource jetpackAudio;
	public AudioSource footstepsAudio;
	//parallax bg ref 
	public ParallaxScroll parallax;
	#endregion

	#region onStart
	void Start () {
		//cache animator comp
		animator = GetComponent<Animator>();

		//create new GUI styles
		labelStyle = new GUIStyle();
		labelStyle.fontSize = 30;
		labelStyle.fontStyle = FontStyle.Bold;
		labelStyle.normal.textColor = Color.yellow;
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

		//pass this position as offset to parallax bg
		parallax.offset = transform.position.x;
	}
	#endregion

	#region onGUI
	void OnGUI() {
		//display coint count
		DisplayCoinsCount();

		//check and display restart button
		DisplayRestartButton();
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

	//updates jetpack particles and SE
	//------------------------------------------------------------------------
	void AdjustJetpack (bool jetpackActive) {
		//switch on/off depends on grounded
		jetpack.enableEmission = !grounded;
		//adjust emission rate depends on user action
		jetpack.emissionRate = jetpackActive ? 300.0f : 50.0f;
		//SE playback
		AdjustFootstepsAndJetpackSound(jetpackActive);
	}

	//hit by laser handler
	//------------------------------------------------------------------------
	void HitByLaser(Collider2D laserCollider) {
		//play SE if not dead already
		if (!dead) {
			laserCollider.gameObject.audio.Play();
		}
		//set death status
		dead = true;
		//and set animator params
		animator.SetBool("dead", true);
	}

	//hit by coin handler
	//------------------------------------------------------------------------
	void CollectCoin(Collider2D coinCollider) {
		//play SE clip on stage point
		//with a static method of the AudioSource class,
		//no AudioSource attached to gameobject
		AudioSource.PlayClipAtPoint(coinCollectSound, transform.position);
		//adds to coins count
		coins++;
		//and destroy coin gameobject
		Destroy(coinCollider.gameObject);
	}

	//footstep and jetpack SE playback
	//------------------------------------------------------------------------
	void AdjustFootstepsAndJetpackSound(bool jetpackActive) {
		//adjust jetpack volume depends on active flag
		jetpackAudio.volume = jetpackActive ? 1.0f : 0.5f;

		//enable footstep SE when not dead and not flying
		footstepsAudio.enabled = !dead && grounded;
		//enable jetpack SE when not dead and flying
		jetpackAudio.enabled = !dead && !grounded;
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

	#region GUI Methods
	//Display total coin count
	//------------------------------------------------------------------------
	void DisplayCoinsCount() {
		//create new rectangle drawing area for icon
		Rect coinIconRect = new Rect(10, 10, 32, 32);
		//draw GUI coin icon texture into coinIconRect
		GUI.DrawTexture(coinIconRect, coinIconTexture);

		//create coin count label next to coin count coinIconRect
		Rect labelRect = new Rect(coinIconRect.xMax, coinIconRect.y, 60, 32);
		//draw coins count value into label with styles
		GUI.Label(labelRect, coins.ToString(), labelStyle);
	}

	//Display restart button
	//------------------------------------------------------------------------
	void DisplayRestartButton() {
		//when mouse is dead and fall to ground
		if (dead && grounded) {
			//create restart button drawing rectangle
			//at stage center
			Rect buttonRect = new Rect(
				Screen.width * 0.33f, //x-pos
				Screen.height * 0.45f,//y-pos
				Screen.width * 0.33f, //w
				Screen.height * 0.1f);//h

			//Render GUI button with buttonRect and restartButtonStyle
			//and attached click handler
			if (GUI.Button(buttonRect, "Tap to restart!")) {
				//reload current level(scene)
				Application.LoadLevel (Application.loadedLevelName);
			};
		}
	}
	#endregion
}
