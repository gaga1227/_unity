//imports
using UnityEngine;
using System.Collections;

//require comp
[RequireComponent (typeof(Animation))]

//class
public class Player : MonoBehaviour {

	//public delegate template fn OnPlayerAnimationFinished
	//of PlayerAnimationFinished delegate type
	public delegate void PlayerAnimationFinished (string animation);
	public PlayerAnimationFinished OnPlayerAnimationFinished = null;

	//private member var for shot pos
	//and its getter and setter
	private Vector3 _shotPosition;
	public Vector3 ShotPosition {
		get {
			return _shotPosition;
		}
		set {
			_shotPosition = value;
			//switch to different anim clip for pos transition
			//depending on the pos difference
			//so the player will walk to the new pos then start bouncing again
			if( Mathf.Abs( _shotPosition.x - _transform.position.x ) < 0.1f ) {
				State = PlayerStateEnum.BouncingBall;
			} else {
				State = PlayerStateEnum.Walking;
			}
		}
	}

	//define public enumeration type for player states
	//shortcuts to a collection of related constants
	public enum PlayerStateEnum {
		Idle,
		BouncingBall,
		PreparingToThrow,
		Throwing,
		Score,
		Miss,
		Walking
	};

	//private members
	//of player states
	private PlayerStateEnum _state = PlayerStateEnum.Idle;
	private float _elapsedStateTime = 0.0f;
	private bool _holdingBall = false;
	//of player(GameObject) properties
	private Transform _transform;
	private Transform _handTransform;
	private Animation _animation;
	private CapsuleCollider _collider;

	//public members

	//ball related members
	public Ball basketBall;
	//high force for ball rigibody to QUICKLY force it down to the ground
	public float bounceForce = 1000f;
	//ball throwing force upper limit
	public float maxThrowForce = 2000f;
	//ball throwing direction (fixed)
	public Vector3 throwDirection = new Vector3( 0.8f, 0.8f, 0.0f );

	//IsHoldingBall
	//public getter for '_holdingBall'
	public bool IsHoldingBall {
		get {
			return _holdingBall;
		}
	}

	//player moving speed
	public float walkSpeed = 5.0f;

	//PlayerStateEnum
	//public getter and setter for '_state'
	public PlayerStateEnum State {
		get {
			return _state;
		}
		set {
			//cancel OnAnimationFinished event firing
			//upon state change
			CancelInvoke("OnAnimationFinished");

			//assign value to private member
			_state = value;

			//reset _elapsedStateTime
			_elapsedStateTime = 0.0f;

			//switch on player state and anim clip
			switch(_state) {
				case PlayerStateEnum.Idle:
					SetCurrentAnimation( animIdle );
					break;
				case PlayerStateEnum.BouncingBall:
					//disable player collider when boucing ball
					_collider.enabled = false;
					//attach ball pos to hand pos
					AttachAndHoldBall();
					//set player anim to bounce up
					SetCurrentAnimation( animBounceUp );
					break;
				case PlayerStateEnum.PreparingToThrow:
					SetCurrentAnimation( animPrepareThrow );
					break;
				case PlayerStateEnum.Throwing:
					SetCurrentAnimation( animThrow );
					break;
				case PlayerStateEnum.Score:
					SetCurrentAnimation( animScore );
					break;
				case PlayerStateEnum.Miss:
					SetCurrentAnimation( animMiss );
					break;
				case PlayerStateEnum.Walking:
					//set walking anim clip depending on
					//diff from current pos to shot pos
					if( _shotPosition.x < _transform.position.x ) {
						SetCurrentAnimation( animWalkForward );
					} else{
						SetCurrentAnimation( animWalkBackward );
					}
					break;
			}
		}
	}
	
	//ElapsedStateTime
	//public getter for '_elapsedStateTime'
	public float ElapsedStateTime {
		get {
			return _elapsedStateTime;
		}
	}

	//animation clip placeholder
	private AnimationClip _currentAnimation = null;
	//CurrentAnimation
	public AnimationClip CurrentAnimation {
		get {
			return _currentAnimation;
		}
		//not used
		set {
			SetCurrentAnimation(value);
		}
	}

	//IsAnimating
	//get animation comp playing state
	public bool IsAnimating {
		get {
			return _animation.isPlaying;
		}
	}

	//animation clips
	public AnimationClip animIdle;
	public AnimationClip animBounceDown;
	public AnimationClip animBounceUp;
	public AnimationClip animWalkForward;
	public AnimationClip animWalkBackward;
	public AnimationClip animPrepareThrow;
	public AnimationClip animThrow;
	public AnimationClip animScore;
	public AnimationClip animMiss;

	//on awake
	void Awake ()
	{
		//on init, get attached comps from local GameObject
		//and assign/cache them to private vars for later use
		_transform = GetComponent<Transform>();
		_animation = GetComponent<Animation>();
		_collider = GetComponent<CapsuleCollider>();

		//assign hand transform info
		//traverse down from player's transform
		//following model skeleton structure
		_handTransform = _transform.Find("BPlayerSkeleton/Pelvis/Hip/Spine/Shoulder_R/UpperArm_R/LowerArm_R/Hand_R");

		//init _shotPosition with initial player pos
		_shotPosition = _transform.position;
	}

	//on start
	void Start ()
	{
		//reset anim comp playback state
		//and set anim clips' properties
		InitAnimations();
	}

	//on frame update
	void Update ()
	{
		//counting used time
		_elapsedStateTime += Time.deltaTime;

		//if player is holding ball (in bouncing anim clip)
		if( _holdingBall ){
			//continuously attach ball to hand
			AttachAndHoldBall();
		}

		//when player is in bouncing ball
		if( _state == PlayerStateEnum.BouncingBall ) {
			//if player holding ball
			if( _holdingBall ) {
				//if game state is play and get user touch down input
				if( GameController.SharedInstance.State == GameController.GameStateEnum.Play &&
				    GameController.SharedInstance.TouchDownCount == 1 )
				{
					//set player state to prepare to throw
					State = PlayerStateEnum.PreparingToThrow;
					return;
				}
			}

			//using update() to loop bounce up and down anim clips
			//with help from ball and hand collider

			//if current anim clip is bounce down
			if( _currentAnimation.name.Equals( animBounceDown.name ) ) {
				//if animation stopped and player holding ball
				if( !_animation.isPlaying && _holdingBall ) {
					//let go of ball, disable force attaching to hand
					_holdingBall = false;
					//throw ball down 
					basketBall.BallRigidbody.AddRelativeForce( Vector3.down * bounceForce );
				}
			}
			//else if current anim clip is bounce up
			//switched from animBounceDown upon ball collides with hand
			else if ( _currentAnimation.name.Equals( animBounceUp.name ) ) {
				//update player anim clip when current stops
				if( !_animation.isPlaying ) {
					SetCurrentAnimation( animBounceDown );
				}
			}
		}

		//when player is preparing to throw (bouncing ball while user touch down)
		//throw the ball when user release touch down by applying calculated motion
		if (_state == PlayerStateEnum.PreparingToThrow) {
			//if game state is play and user touch release all fingers
			if (GameController.SharedInstance.State == GameController.GameStateEnum.Play &&
			    GameController.SharedInstance.TouchCount == 0)
			{
				//set player state to throw
				State = PlayerStateEnum.Throwing;
				//let go of ball, disable bouncing ball anim clips loop
				_holdingBall = false;
				//apply throw motion to ball by apply fixed direction with throw force
				//ranging from 0 to maxThrowForce depending on animPrepareThrow's playback time: 0f ~ 1f
				//which is determined by user finger down duration
				basketBall.BallRigidbody.AddRelativeForce(
					throwDirection * 
					(maxThrowForce * Mathf.Clamp(_animation[animPrepareThrow.name].normalizedTime, 0.0F, 0.8F)));
			}
		}

		//when player has thrown the ball (user touch released)
		if (_state == PlayerStateEnum.Throwing ) {
			//turn on the player collider if it bounces back
			//it is disabled when player is in bouncing ball state
			//because we want to manually control the ball bouncing motion
			if( !_animation.isPlaying && !_collider.enabled ) {
				_collider.enabled = true;
			}
		}

		//when player is in walking state
		if (_state == PlayerStateEnum.Walking) {
			//new local var for current pos
			Vector3 pos = _transform.position;
			//calculate current pos for animating player
			//Vector3 Lerp(Vector3 from, Vector3 to, float t);
			pos = Vector3.Lerp(pos, _shotPosition, Time.deltaTime * walkSpeed);
			//assign calculated pos to player
			_transform.position = pos;
			//if player is almost reaching new target pos
			if ((pos - _shotPosition).sqrMagnitude < 1.0f) {
				//hard set player to new target pos
				//at the same time player will switch to bouncing anim
				pos = _shotPosition;
				//fire OnPlayerAnimationFinished delegate fn
				OnAnimationFinished();
//				if (OnPlayerAnimationFinished != null) {
//					OnPlayerAnimationFinished(_currentAnimation.name);
//				}
			}
		}
	}

	//private methods

	//InitAnimations
	private void InitAnimations()
	{
		//stop animation comp
		_animation.Stop();

		//set animation clip properties
		_animation[animIdle.name].wrapMode = WrapMode.Once;
		_animation[animBounceDown.name].wrapMode = WrapMode.Once;
		_animation[animBounceUp.name].wrapMode = WrapMode.Once;
		_animation[animWalkForward.name].wrapMode = WrapMode.Loop;
		_animation[animWalkBackward.name].wrapMode = WrapMode.Loop;
		_animation[animPrepareThrow.name].wrapMode = WrapMode.ClampForever; //Once
		_animation[animThrow.name].wrapMode = WrapMode.Once;
		_animation[animScore.name].wrapMode = WrapMode.Once;
		_animation[animMiss.name].wrapMode = WrapMode.Once;
		_animation[animBounceDown.name].speed = 2.0f;
		_animation[animBounceUp.name].speed = 2.0f;
	}

	//OnAnimationFinished
	private void OnAnimationFinished()
	{
		//check if OnPlayerAnimationFinished delegate template fn is empty
		//run template fn if not empty
		if (OnPlayerAnimationFinished != null) {
			OnPlayerAnimationFinished(_currentAnimation.name);
		}
	}

	//AttachAndHoldBall
	private void AttachAndHoldBall()
	{
		//update holding ball flag
		_holdingBall = true;

		//set local refs for public ball members
		Transform bTransform = basketBall.BallTransform;
		SphereCollider bCollider = basketBall.BallCollider;
		Rigidbody bRB = basketBall.BallRigidbody;

		//set ball Rigidbody with null velocity
		//to avoid movement
		bRB.velocity = Vector3.zero;

		//set ball rotation to perfectly aligned with the world or parent axes
		bTransform.rotation = Quaternion.identity;

		//define local var for ball pos
		Vector3 bPos;
		//assign with hand pos
		bPos = _handTransform.position;
		//offset with ball collider size / 2
		bPos.y -= bCollider.radius;
		//assign offset pos back to ball transform public member
		bTransform.position = bPos;
	}

	//OnTriggerEnter
	//fired when attached box collider (with trigger switch) collides
	public void OnTriggerEnter(Collider collider)
	{
		if (_state == PlayerStateEnum.BouncingBall) {
			//if not holding ball and ball collides with hand
			//(when ball bounce up from ground meeting the hand)
			if (!_holdingBall && collider.transform == basketBall.BallTransform) {
				//stick ball to hand
				AttachAndHoldBall();
				//switch anim clip to bounce up: hand moving up with ball attached
				SetCurrentAnimation(animBounceUp);
			}
		}
	}

	//public methods

	//SetCurrentAnimation
	public void SetCurrentAnimation (AnimationClip animationClip)
	{
		//assign supplied animationClip to current animation clip placeholder
		_currentAnimation = animationClip;
		//then reset(rewind) _currentAnimation's time
		_animation[_currentAnimation.name].time = 0.0f;
		//CrossFade/Play _currentAnimation in and others out for a period of 0.1f
		_animation.CrossFade(_currentAnimation.name, 0.1f);
		//if _currentAnimation is not looping,
		//manually invoke 'OnAnimationFinished' event
		//after animation clip playback time
		float playbackTime = _animation[_currentAnimation.name].length / _animation[_currentAnimation.name].speed;
		if (_currentAnimation.wrapMode != WrapMode.Loop) {
			Invoke("OnAnimationFinished", playbackTime);
		}
	}

}
