#pragma strict

// check touch device
private var isTouchDevice : boolean = false;

function Awake() {
    isTouchDevice = (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.Android) 
	    ? true
	    : false;
}

var speed : float = 3.0;
var rotateSpeed : float = 10.0;
		
function Update() {
    // Detect click
    var clickDetected : boolean = isTouchDevice
	    ? (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)	    
	    : Input.GetMouseButtonDown(0);
    
    // Get calculate touch position
    var inputPos : Vector3;
    if (clickDetected) {
    	if (isTouchDevice) {
    		inputPos = Input.GetTouch(0).position;
    	} else {
    		inputPos = Input.mousePosition;
    	}
	    Debug.Log(inputPos);
    }
    
	// Detect mouse left clicks
    if (clickDetected) {
        // Check if the GameObject is clicked by casting a
        // Ray from the main camera to the touched position.
        var ray : Ray = Camera.main.ScreenPointToRay(inputPos);
        var hit : RaycastHit;
        // Cast a ray of distance 100, and check if this
        // collider is hit.
        if (collider.Raycast(ray, hit, 100.0)) {
            // Log a debug message
            Debug.Log("Moving the target");
            // Move the target forward
            transform.Translate(Vector3.forward * speed);       
            // Rotate the target along the y-axis
            transform.Rotate(Vector3.up * rotateSpeed);
        } else {
            // Clear the debug message
            Debug.Log("");
        }
    }
}