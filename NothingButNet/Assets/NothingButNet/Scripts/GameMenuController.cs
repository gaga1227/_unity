//imports
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

//require comp from gameobject
[RequireComponent (typeof (LeaderboardController))]

//class def
public class GameMenuController : MonoBehaviour {
	
	//vars

	//textures
	public Texture2D backgroundTex;
	public Texture2D playButtonTex;
	public Texture2D pauseButtonTex;
	public Texture2D resumeButtonTex;
	public Texture2D restartButtonTex;
	public Texture2D titleTex;
	public Texture2D leaderboardBgTex;
	public Texture2D loginCopyTex;
	public Texture2D fbButtonTex;
	public Texture2D instructionsTex;

	//skins (to have different font sizes for different screen resolutions)
	public GUISkin gameMenuGUISkinForSmall;
	public GUISkin gameMenuGUISkinForNormal;

	//comps
	private GameController _gameController;
	private LeaderboardController _leaderboardController;

	//local var for scores
	private List<ScoreData> _scores = null;
	private int _leaderboardLimit = 10;

	//default screen dimensions from GUI design
	//used as a base referencing point
	public const float kDesignWidth = 960f;
	public const float kDesignHeight = 640f;

	//GUI scaling factor and offset placeholder for 2D dimensions
	private float _scale = 1.0f;
	private Vector2 _scaleOffset = Vector2.one;
	public float Scale {
		get {
			return _scale;
		}
	}
	public Vector2 ScaleOffset {
		get {
			return _scaleOffset;
		}
	}

	//GUI setting and game statictics
	private bool _showInstructions = false;
	private bool _showTitle = false;
	private bool _showLeaderboard = false;
	private int _gamesPlayedThisSession = 0;

	//init
	void Awake(){
		//get attached comps, assign to local vars
		_gameController = GetComponent<GameController>();
		_leaderboardController = GetComponent<LeaderboardController>();
	}

	//on start
	void Start(){
		//scaling factors for screen dimensions
		//calculate both dimensions to cater different ARs
		_scaleOffset.x = Screen.width / kDesignWidth;
		_scaleOffset.y = Screen.height / kDesignHeight;
		//pick bigger scale factor from 2D dimensions as unified GUI scale factor
		_scale = Mathf.Max( _scaleOffset.x, _scaleOffset.y );

		//register handler to leaderboard controler delegate fn (handle async result)
		_leaderboardController.OnScoresLoaded += HandleLeaderboardControllerOnScoresLoaded;
		//start fetch updates (init async call)
		_leaderboardController.FetchScores();
	}

	//leaderboard controller OnScoresLoaded handler
	public void HandleLeaderboardControllerOnScoresLoaded( List<ScoreData> scores ){
		//assign results to local var
		_scores = scores;
	}

	//on GUI update (run each frame)
	void OnGUI () {
		//use different skin files depending on calculated screen scale
		if( _scale < 1 ){
			GUI.skin = gameMenuGUISkinForSmall;
		} else {
			GUI.skin = gameMenuGUISkinForNormal;
		}

		//drawing GUI elements

		//overlay mask bg, fill entire screen
		if(_gameController.State != GameController.GameStateEnum.Play) {
			GUI.DrawTexture( new Rect( 0, 0, Screen.width, Screen.height ), backgroundTex );
		}

		//buttons depends on game controller states
		//when in-game paused, draw resume button
		if(_gameController.State == GameController.GameStateEnum.Paused){
			//on resume button click
			if (GUI.Button(
				new Rect(
					//button location (multiply with dimensional offset scales)
					77 * _scaleOffset.x, 400 * _scaleOffset.y,
					//button size (multiply with unified scale)
					130 * _scale, 130 * _scale ),
				//texture image
				resumeButtonTex,
				//style
				GUIStyle.none))
			{
				//call game controller resume game
				_gameController.ResumeGame();
			}
			//disable title, instructions and leaderboard
			_showTitle = false;
			_showInstructions = false;
			_showLeaderboard = false;
		}
		//when game over, draw start new button
		else if(_gameController.State == GameController.GameStateEnum.GameOver){
			//on new game button click
			if (GUI.Button(
				new Rect(
				92 * _scaleOffset.x, 415 * _scaleOffset.y,
				100 * _scale, 100 * _scale ),
				restartButtonTex,
				GUIStyle.none))
			{
				//call game controller start new game
				_gameController.StartNewGame();
			}
			//disable title, instructions and enable leaderboard
			_showTitle = false;
			_showInstructions = false;
			_showLeaderboard = true;
		}
		//when game playing, draw pause button
		else if(_gameController.State == GameController.GameStateEnum.Play){
			//on pause button click
			if (GUI.Button(
				new Rect(
				Screen.width - 100 * _scaleOffset.x, 40 * _scaleOffset.y,
				60 * _scale, 60 * _scale ),
				pauseButtonTex,
				GUIStyle.none))
			{
				//call game controller pause game
				_gameController.PauseGame();
			}
			//disable title, instructions and leaderboard
			_showTitle = false;
			_showInstructions = false;
			_showLeaderboard = false;
		}
		//when before/after game play, draw play button
		else {
			//on play button click
			if (GUI.Button(
				new Rect(
					77 * _scaleOffset.x, 400 * _scaleOffset.y,
					130 * _scale, 130 * _scale ),
				playButtonTex,
				GUIStyle.none))
			{
				//if instruction is showing or had at least 1 game session
				//no show instructions and starts game
				//also incrementing game sessions played count
				//this is second and further button click
				if( _showInstructions || _gamesPlayedThisSession > 0 ){
					_showInstructions = false;
					_gamesPlayedThisSession++;
					_gameController.StartNewGame();
				}
				//show instructions before any gameplay session
				//this is first button click
				else {
					_showInstructions = true;
				}
			}
			//enable title
			_showTitle = true;
			//disable leaderboard
			_showLeaderboard = false;
		}

		//draw Instructions
		if( _showInstructions ) {
			GUI.DrawTexture(
				new Rect(
					67*_scaleOffset.x, 80*_scaleOffset.y,
					510*_scale, 309*_scale),
				instructionsTex);
		}

		//draw Title
		if( _showTitle && !_showInstructions ) {
			GUI.DrawTexture(
				new Rect(
					67*_scaleOffset.x, 188 * _scaleOffset.y,
					447 * _scale, 113 * _scale),
				titleTex);
		}

		//draw leaderboard
		if( _showLeaderboard ) {
			//start group 1
			GUI.BeginGroup(
				new Rect(
				Screen.width - 215 * _scale, 0,
				215 * _scale, 603 * _scale ));
			
			//leaderboard bg
			GUI.DrawTexture(
				new Rect(
				0, 0, 215 * _scale, 603 * _scale ),
				leaderboardBgTex );
			
			//define leaderboard table
			Rect leaderboardTable = new Rect(10 * _scaleOffset.x, 70 * _scaleOffset.y, 190 * _scale, 534 * _scale);
			
			//if has facebook and not logged in
			if(_leaderboardController.IsFacebookAvailable && !_leaderboardController.IsLoggedIn) {
				//update leaderboard to new position
				leaderboardTable = new Rect(10 * _scaleOffset.x, 70 * _scaleOffset.y, 190 * _scale, 410 * _scale);
				
				//also draw login text
				GUI.DrawTexture(
					new Rect(29 * _scaleOffset.x, 490 * _scaleOffset.y, 156 * _scale, 42 * _scale),
					loginCopyTex);
				//also draw FB login button and on click
				if (GUI.Button(
					new Rect(41 * _scaleOffset.x, 560 * _scaleOffset.y, 135 * _scale, 50 * _scale),
					fbButtonTex, GUIStyle.none))
				{
					//LoginToFacebook (fake)
					_leaderboardController.LoginToFacebook();
				}
			}
			
			//start group 2 with leaderboardTable
			GUI.BeginGroup( leaderboardTable );
			//if having scores
			if( _scores != null ){
				//loop through scores list
				for( int i=0; i < Mathf.Min(_scores.Count, _leaderboardLimit); i++ ){
					//prep entry fields
					Rect nameRect = new Rect(5 * _scaleOffset.x, (10 * _scaleOffset.y) + i * 35 * _scale, 90 * _scale, 35 * _scale);
					Rect scoreRect = new Rect(100 * _scaleOffset.x, (10 * _scaleOffset.y) + i * 35 * _scale, 90 * _scale, 35 * _scale);
					//populate entry fields with score object vals
					GUI.skin.label.alignment = TextAnchor.UpperLeft;
					GUI.Label(nameRect, _scores[i].name);
					GUI.skin.label.alignment = TextAnchor.UpperRight;
					GUI.Label(scoreRect, _scores[i].points.ToString());
				}
			}
			//end group 2
			GUI.EndGroup();
			//end group 1
			GUI.EndGroup();
		}
	}//end of OnGUI

	//public GUI switch API methods
	public void Show(){
		//ignore if you are already enabled
		if( this.enabled ){ return; }
		//get scores (async)
		_leaderboardController.FetchScores();
		//switch this comp(script) on, OnGUI, Update, etc starts running
		//all GUI drawing calls start, GUI start fading in...
		this.enabled = true;
	}
	public void Hide(){
		//switch this comp(script) off, OnGUI, Update, etc stops running
		//all GUI drawing calls stop
		this.enabled = false;
	}

}