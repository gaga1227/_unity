#pragma strict

var projectile : Rigidbody;
var speed = 5;
var maxObstacles = 10;
var launchInterval : float = 1.0;
var target : Transform;
 
private var nextLaunch : float = 0.0;
private var numObstaclesLaunched = 0;
 
// define target on start, before Update()
function Start () {
    if (target == null) {
        // Find the player transform
        target = GameObject.FindGameObjectWithTag("Player").transform;
    }
}

// on frame
function Update () {
	//exit if game not running
	if (GameController != null && !GameController.gameRunning) return;
	
	//otherwise, keep lauching
    if ((numObstaclesLaunched < maxObstacles) && (Time.time > nextLaunch)) {
        // Set up the next launch time
        nextLaunch = Time.time + launchInterval;
 
        // Set up for launch direction
		var direction : Vector3 = new Vector3(0, 0, numObstaclesLaunched * -1);
 		
        // Instantiate the projectile
        var instantiatedProjectile : Rigidbody = Instantiate(projectile, transform.position, transform.rotation);
 
        // Simple block, try to get in front of the player
        instantiatedProjectile.velocity = target.TransformDirection(direction * speed);     
 
        // Increment the launch count
        numObstaclesLaunched++;   
    }
}

// public, on reset being called
function resetGame() {
    // Reset to original data
    numObstaclesLaunched = 0;
}