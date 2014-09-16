#pragma strict

// public var to assign Game Object to, Game Object has GameController as its class
var gameControllerScript : GameController;

// on enter goal object collid event handler, event triggered by goal's collider
function OnTriggerEnter(target : Collider) {
    if (gameControllerScript.gameRunning && target.gameObject.tag == "Player") 
    { 
        gameControllerScript.MissionComplete();
        Debug.Log("You made it!!!");
    } 
}

// import dependency
@script RequireComponent(Collider)