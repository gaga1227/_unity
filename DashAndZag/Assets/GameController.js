#pragma strict

//static var belongs to class not instances, only one available
static var gameRunning : boolean = false;
 
var gameTimeAllowed : float = 30.0;
var gameMessageFont : Font;

//define game objects array to reset
var gameObjectsToReset : GameObject [];

var intro : Transform;

//audio
var fanReactionScript : FanReaction;
 
private var gameMessageLabel = "";
private var gameMessageDisplay : Rect;

private var playButtonText = "Play";

private var timedOut : boolean = false;
private var missionCompleted : boolean = false;

private var gameTimeRemaining : float = gameTimeAllowed;
private var missionCompleteTime : float = gameTimeAllowed;
 
//draw msg display on start
function Awake() {
    gameMessageDisplay = Rect(10, 10, Screen.width - 20, 40);
}

//being called continuously, on gameobject with GUI layer
function OnGUI() { 
	//GUI settings
	GUI.skin.font = gameMessageFont;
    GUI.color = Color.yellow;
    GUI.backgroundColor = Color.black;
 	
 	//setup game msg
    var text : String = ""; 
    if (missionCompleted) {
        text = String.Format( "{0:00}:{1:00}", parseInt( missionCompleteTime / 60.0 ), parseInt( missionCompleteTime % 60.0 ) );
        gameMessageLabel = "Completed in: " + text;
    } else if (timedOut) {
        gameMessageLabel = "Time's up!!";
    } else {
        text = String.Format( "{0:00}:{1:00}", parseInt( gameTimeRemaining / 60.0 ), parseInt( gameTimeRemaining % 60.0 ) );
        gameMessageLabel = "Time left: " + text;
    }
    GUI.Box(gameMessageDisplay, gameMessageLabel);
    
    //The menu button
    if (!gameRunning) {
        var xPos = Screen.width / 2 - 100;
        var yPos = Screen.height / 2 + 100;
        var btn = GUI.Button( new Rect( xPos, yPos, 200, 50 ), playButtonText );
        //on click, button returns true, starts game
        if (btn) {
        	startGame();
        }
    }
}

//being called continuously
function Update() { 
    if (!gameRunning) return; 
 
    // Keep track of time and display a countdown
    gameTimeRemaining -= Time.deltaTime;
    if (gameTimeRemaining <= 0) {
        timedOut = true; 
        gameRunning = false;
        
        // Play the sound of defeat
        fanReactionScript.playSoundOfVictory(false);
    }
}

//public functions

//start game
function startGame() {
	if (gameRunning) return;
	
    // Reset if starting a new game
    gameTimeRemaining = gameTimeAllowed; 
    timedOut = false;
    missionCompleted = false;
 
    // Change button text after the initial run
    playButtonText = "Play Again";
    
    // Turn off the text items inside intro gameobject
    for (var child : Transform in intro ) {
        child.gameObject.renderer.enabled = false;
    }
    
    // Clean out any enemy objects
    var enemies = GameObject.FindGameObjectsWithTag("Enemy");
    for (var enemy : GameObject in enemies) {
        Destroy (enemy);
    }
    
    // Call game reset methods in all game objects in reset array
    for (var gameObjectReceiver : GameObject in gameObjectsToReset) {
        gameObjectReceiver.SendMessage("resetGame", null, SendMessageOptions.DontRequireReceiver);
    }
 
    // Kick off the game
    gameRunning = true;
}

//stop game
function MissionComplete() { 
    if (!gameRunning) return;
 	
 	//calculate completion time
 	missionCompleteTime = gameTimeAllowed - gameTimeRemaining;
    
    //set game status
    missionCompleted = true; 
    gameRunning = false;
    
    // Play the sound of victory
    fanReactionScript.playSoundOfVictory(true);
}