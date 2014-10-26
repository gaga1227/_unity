using UnityEngine;
using System.Collections;

public class ScreenRelativePosition : MonoBehaviour {

	//------------------------------------------------------------------------
	//public vars

	//position parameters
	public enum ScreenEdge {LEFT, RIGHT, TOP, BOTTOM};
	public ScreenEdge screenEdge;
	public float yOffset;
	public float xOffset;

	//on start
	void Start () {
		//get current position (retains position.z value)
		Vector3 newPosition = transform.position;
		//get camera reference
		Camera camera = Camera.main;
		//get new position to edges
		switch(screenEdge) {
			//center of the camera view is 0,0, which is set in unity
			//'camera.orthographicSize' half of the view height,
			//which equals to the distance from center to each vertical edge
			//use 'camera.aspect' to calculate distance from center to each horizontal edge
			case ScreenEdge.LEFT:
				newPosition.x = -camera.aspect * camera.orthographicSize + xOffset;
				newPosition.y = yOffset;
				break;
			case ScreenEdge.RIGHT:
				newPosition.x = camera.aspect * camera.orthographicSize + xOffset;
				newPosition.y = yOffset;
				break;
			case ScreenEdge.TOP:
				newPosition.y = camera.orthographicSize + yOffset;
				newPosition.x = xOffset;
				break;
			case ScreenEdge.BOTTOM:
				newPosition.y = -camera.orthographicSize + yOffset;
				newPosition.x = xOffset;
				break;
		}
		//Update gameobject position
		transform.position = newPosition;
	}
	
	//on frame
	void Update () {
	
	}
}
