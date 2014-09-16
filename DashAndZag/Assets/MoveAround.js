#pragma strict

// speed factors
var speed : float = 6.0;
var rotateSpeed : float = 1.0;

// joysticks
var moveJoystick : Joystick;
var rotateJoystick : Joystick;

// original transform states
var originalPosition : Vector3;
var originalRotation : Quaternion;

// save to original transform states on start
function Awake() {
    originalPosition = transform.position;
    originalRotation = transform.rotation;
}

// public, reset player's transform states to original on reset
function resetGame() {
    // Reset to original position
    transform.position = originalPosition;
    transform.rotation = originalRotation;
}

// on frame
function Update () {
	//exit if game not running
	if (GameController != null && !GameController.gameRunning) return;

	// create charactor controller instance
	var controller : CharacterController = GetComponent(CharacterController);

    // get input info: default input or joystick
    var movePos = Input.GetAxis ("Vertical") ? Input.GetAxis ("Vertical") : joyStickInput(moveJoystick);
	var rotatePos = Input.GetAxis ("Horizontal") ? Input.GetAxis ("Horizontal") : joyStickInput(rotateJoystick);

	// Rotate around y - axis
    // apply horizontal input (-1, 1) with speed factor on Cube's transform.rotateY
    transform.Rotate(0, rotatePos * rotateSpeed, 0);
    
	// Move forward / backward
    // converts local vector to global, serves as moving direction
    var forward : Vector3 = transform.TransformDirection(Vector3.forward);
    // apply vertical input (-1, 1) with speed factor, serves as moving amount
    var curSpeed : float = movePos * speed;
    // utilise SimpleMove from charactor controller to translate/move itself
    controller.SimpleMove(forward * curSpeed);
}

// common function to convert joystick touch position to input info
function joyStickInput (joystick : Joystick) {
    // get abs touch position offset from joystick, amount only, no direction 
    var absJoyPos = Vector2(Mathf.Abs(joystick.position.x), Mathf.Abs(joystick.position.y));
    // get axis direction from joystick position
    var xDirection = (joystick.position.x > 0) ? 1 : -1;
    var yDirection = (joystick.position.y > 0) ? 1 : -1;
    // return direction x amount, but only with dominant offset axis
    return ((absJoyPos.x > absJoyPos.y) ? absJoyPos.x * xDirection : absJoyPos.y * yDirection);
}

//can only be attached to a GameObject that has a Character Controller component
@script RequireComponent(CharacterController)