using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {
	#region vars
	//camera moving
	public float speed = 1f;
	private Vector3 newPosition;
	#endregion

	#region on start
	void Start () {
		//copy camera current position to new posiiton
		newPosition = transform.position;
	}
	#endregion
	
	#region on frame
	void Update () {
		//moving the camera view incrementally
		//using deltatime to maintain constant moving synced with time
		newPosition.x = transform.position.x + Time.deltaTime * speed;
		transform.position = newPosition;
	}
	#endregion
}
