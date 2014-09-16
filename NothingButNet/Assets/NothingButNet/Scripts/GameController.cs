//imports
using UnityEngine;
using System.Collections;

//class (implied Singleton)
public class GameController : MonoBehaviour {

	//private static ref to GameController itself
	private static GameController _instance = null;

	//public static GameController getter
	//static modifier declares a class member
	public static GameController SharedInstance {
		get {
			if (_instance == null) {
				//find GameObject of GameController type
				//from all GameObjects as GameController type
				//and assign to private var _instance
				_instance = GameObject.FindObjectOfType(typeof(GameController)) as GameController;
			}
			return _instance;
		}
	}

	//define enum type for game state
	public enum GameStateEnum
	{
		Undefined,
		Menu,
		Paused, 
		Play,
		GameOver
	};

	//refs to gameobjects
	public Player player;
	public ScoreBoard scoreboard;
	public Ball basketBall;
	//time for a single game session (in seconds)
	public float gameSessionTime = 30.0f;
	//radius the player will be positioned for each throw
	public float throwRadius = 5.0f;
	//position offset flag
	public bool positionOffsetEnabled = true;
	//state of the current game
	//controls how user interactions are interrupted and what is activivated and disabled
	private GameStateEnum _state = GameStateEnum.Undefined;
	//Points accumulated by the user for this game session
	private int _gamePoints = 0;
	//The time remaining for current game session
	private float _timeRemaining = 0.0f;
	//we only want to update the count down every second;
	//so we'll accumulate the time in this variable 
	//and update the remaining time after each second 	
	private float _timeUpdateElapsedTime = 0.0f; 
	//The original player position
	//each throw position will be offset based on this and a random value  
	//between throwRadius and throwRadius 
	private Vector3 _orgPlayerPosition = new Vector3(4.7f, 0.0f, 0.0f);
	//GUI menu controller comp ref
	private GameMenuController _menuController;
	//leaderboard controller comp ref
	private LeaderboardController _leaderboardController;
	//Alerter comp ref
	public Alerter alerter;

	//getters/setters

	//GameStateEnum
	public GameStateEnum State {
		get{
			return _state; 	
		}
		set{
			_state = value; 
			
			// MENU 
			if(_state == GameStateEnum.Menu) {
				player.State = Player.PlayerStateEnum.BouncingBall;
				//show GUI menu
				_menuController.Show();
			}
			// PAUSED 
			else if(_state == GameStateEnum.Paused) {
				//stop time
				Time.timeScale = 0.0f;
				//show GUI menu
				_menuController.Show();
			}
			// PLAY 
			else if(_state == GameStateEnum.Play) {
				//set time back to normal
				Time.timeScale = 1.0f;
				//hide GUI menu
				//_menuController.Hide();
				
				//notify user
				alerter.Show( "GAME ON", 0.2f, 2.0f );
			}
			// GAME OVER 
			else if(_state == GameStateEnum.GameOver) {
				//add score
				if( _gamePoints > 0 ) {
					_leaderboardController.AddPlayersScore( _gamePoints );
				}

				//reset time
				TimeRemaining = 0.0f;

				//stop time
				Time.timeScale = 0.0f;
				//show GUI menu
				_menuController.Show();
				
				//notify user
				alerter.Show( "GAME OVER", 0.2f, 2.0f );
			}
		}
	}

	//GamePoints
	public int GamePoints {
		get{
			return _gamePoints;
		}
		set{
			_gamePoints = value;
			//also set scoreboard display
			scoreboard.SetPoints(_gamePoints.ToString());
		}
	}

	//TimeRemaining
	public float TimeRemaining {
		get{
			return _timeRemaining;
		}
		set{
			_timeRemaining = value;
			_timeRemaining = (_timeRemaining < 0) ? 0.0f : _timeRemaining;
			//also set scoreboard display
			scoreboard.SetTime( _timeRemaining.ToString("00:00") );
			//reset the elapsed time 
			_timeUpdateElapsedTime = 0.0f;
		}
	}

	//on awake
	void Awake() {
		//assign self to private GameController ref
		_instance = this;
		//assign comps to local refs
		_menuController = GetComponent<GameMenuController>();
		_leaderboardController = GetComponent<LeaderboardController>();
	}

	//on start
	void Start () {
		//register HandleBasketBallOnNet to
		//OnNet event delegates template fn: OnNet
		basketBall.OnNet += HandleBasketBallOnNet;
		//register HandlePlayerOnPlayerAnimationFinished to
		//OnNet event delegates template fn: OnPlayerAnimationFinished
		player.OnPlayerAnimationFinished += HandlePlayerOnPlayerAnimationFinished;
	}

	//on frame update
	void Update () {
		//watch game states 
		if(_state == GameStateEnum.Undefined) {
			//if no state is set then we will switch to the menu state
			State = GameStateEnum.Menu;
		}
		else if(_state == GameStateEnum.Play) {
			UpdateStatePlay();
		}

		//if ball is thrown beyond boundary, marked as miss
		if (basketBall.transform.position.y < 0 && player.State == Player.PlayerStateEnum.Throwing) {
			Debug.Log("Ball is below the scene floor.");
			player.State = Player.PlayerStateEnum.Miss;
		}
	}

	//private methods

	//UpdateStatePlay
	private void UpdateStatePlay() {
		//update remain time
		_timeRemaining -= Time.deltaTime;

		//accumulate elapsed time 
		_timeUpdateElapsedTime += Time.deltaTime;

		//has a second past?
		//only update display remain time each second
		if(_timeUpdateElapsedTime >= 1.0f) {
			TimeRemaining = _timeRemaining;
		}

		//if session time is out and player still bouncing
		if (player.State == Player.PlayerStateEnum.BouncingBall && _timeRemaining <= 0.0f)
		{
			player.State = Player.PlayerStateEnum.Idle;
			State = GameStateEnum.GameOver;
		}

		//player is considered having finished a throw when 
		//he has been in either the Miss or Score state for 3 or more seconds
		if ((player.State == Player.PlayerStateEnum.Miss ||
		     player.State == Player.PlayerStateEnum.Score) &&
		     player.ElapsedStateTime >= 3.0f)
		{
			//reset the session if game is over
			if(_timeRemaining <= 0.0f) {
				State = GameStateEnum.GameOver;
			//otherwise reset the position
			} else {
				//define a new throw position
				Vector3 playersNextThrowPosition = _orgPlayerPosition;
				//offset player throw pos with random radius range on X
				if (positionOffsetEnabled) {
					playersNextThrowPosition.x += Random.Range(-throwRadius, throwRadius);
				}
				//assign new player pos
				player.ShotPosition = playersNextThrowPosition;
			}
		}
	}

	//public methods

	//StartNewGame
	public void StartNewGame() {
		GamePoints = 0;
		TimeRemaining = gameSessionTime;
		player.State = Player.PlayerStateEnum.BouncingBall;
		State = GameStateEnum.Play;
	}

	//PauseGame
	public void PauseGame() {
		State = GameStateEnum.Paused;
	}

	//ResumeGame
	public void ResumeGame() {
		if(_timeRemaining < 0) {
			StartNewGame();
		} else {
			State = GameStateEnum.Play;
		}
	}

	//OnBallCollisionEnter
	//process Ball collisions with other BoxColliders
	//called from Ball class
	public void OnBallCollisionEnter(Collision collision) {
		//Debug.Log ( "Game Controller: Ball collision occurred!" );
		if (!player.IsHoldingBall) {
			//if the ball gets collision on ground or court
			//while player is throwing the ball,
			//it's a miss!
			if ((collision.transform.name == "Ground" ||
			     collision.transform.name == "Court") &&
			     player.State == Player.PlayerStateEnum.Throwing)
			{
				player.State = Player.PlayerStateEnum.Miss;
			}
		}
	}

	//HandleBasketBallOnNet
	//registered with Ball.OnNet delegate template fn
	public void HandleBasketBallOnNet() {
		//adds points
		GamePoints += 3;
		//set player animation to score
		player.State = Player.PlayerStateEnum.Score;
	}

	//HandlePlayerOnPlayerAnimationFinished
	//registered with Player.OnPlayerAnimationFinished delegate template fn
	public void HandlePlayerOnPlayerAnimationFinished(string animationName) {
		//if player state is walking and animation ends,
		//switching state to BouncingBall
		if (player.State == Player.PlayerStateEnum.Walking) {
			player.State = Player.PlayerStateEnum.BouncingBall;
		}
	}

	//public helper members

	//IsMobile
	public bool IsMobile {
		get {
			return (Application.platform == RuntimePlatform.IPhonePlayer ||
			        Application.platform == RuntimePlatform.Android);
		}
	}

	//TouchCount
	public int TouchCount {
		get {
			if( IsMobile ) {
				//returns how many finger is touching
				return Input.touchCount;
			} else {
				//if its not consdered to be mobile
				//then query the left(0) mouse button click,
				//returning 1 if down or 0 if not
				if( Input.GetMouseButton(0) ){
					return 1; //mouse down
				} else {
					return 0; //not mouse down
				}
			}
		}
	}

	//TouchDownCount
	public int TouchDownCount {
		get {
			if( IsMobile ) {
				//returns how many finger is touching and phase.began
				int currentTouchDownCount = 0; 
				foreach( Touch touch in Input.touches ) {
					if( touch.phase == TouchPhase.Began
					   //exclude pause button area
					   && (touch.position.x < Screen.width - 140 * _menuController.ScaleOffset.x
					   || touch.position.y < Screen.height - 140 * _menuController.ScaleOffset.y)
					) {
						//Debug.Log ("valid input");
						currentTouchDownCount++;
					}
				}
				return currentTouchDownCount;
			} else {
				//if its not consdered to be mobile
				//then query the left(0) mouse button click,
				//returning 1 if down or 0 if not
				if( Input.GetMouseButtonDown(0)
				   //exclude pause button area
				   && (Input.mousePosition.x < Screen.width - 140 * _menuController.ScaleOffset.x
				   || Input.mousePosition.y < Screen.height - 140 * _menuController.ScaleOffset.y)
				) {
					//Debug.Log ("valid input");
					return 1; //mouse down
				} else {
					return 0; //not mouse down
				}
			}
		}
	}

}
