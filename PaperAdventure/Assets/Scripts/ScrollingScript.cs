using UnityEngine;
using System.Collections.Generic;
using System.Linq;

// Parallax scrolling script that should be assigned to a layer

public class ScrollingScript : MonoBehaviour {
	#region vars
	// scrolling speed and direction vectors
	public Vector2 speed = new Vector2(2, 2);
	public Vector2 direction = new Vector2(-1, 0);

	// whether this scrolling object is locked to cam
	public bool isLinkedToCamera = false;

	// whether this scrolling object is looping 
	public bool isLooping = false;
	
	// list ref for background elements
	// list requires System.Collections.Generic directive
	private List<Transform> backgroundPart;
	#endregion
	
	#region onStart
	void Start () {
		// For looping background only
		if (isLooping) {
			// loop through all children in current object
			// and add valid child objects in the new backgroundPart list
			backgroundPart = new List<Transform>();
			for (int i = 0; i < transform.childCount; i++) {
				// temp ref for child
				Transform child = transform.GetChild(i);
				// add child to list if has renderer (visible)
				if (child.renderer != null) {
					backgroundPart.Add(child);
				}
			}
			
			// Sort list by position.x -> from left to right
			// 'OrderBy' is a c# sort method
			backgroundPart = backgroundPart.OrderBy(
				t => t.position.x
				).ToList();
		}
	}
	#endregion
	
	#region onUpdate
	void Update () {
		// Movement
		// create temp movement vector
		Vector3 movement = new Vector3(
			speed.x * direction.x,
			speed.y * direction.y,
			0);
		// apply movement with deltatime
		// to get movement amount for this frame
		movement *= Time.deltaTime;
		// apply movement to gameobject
		// use translate to apply incremental amount, not position
		transform.Translate(movement);
		
		// Move the camera by the same amount if
		// this object isLinkedToCamera
		if (isLinkedToCamera) {
			// find first MainCamera and apply movement
			Camera.main.transform.Translate(movement);
		}

		// Looping backgrounds
		if (isLooping) {
			// Get the first(leftmost) object from list
			// 'FirstOrDefault' is c# method
			Transform firstChild = backgroundPart.FirstOrDefault();

			// if found firstChild
			if (firstChild != null) {
				// Check if the child is already (partly) before the camera
				// We test the position first because the IsVisibleFrom
				// method is a bit heavier to execute
				if (firstChild.position.x < Camera.main.transform.position.x) {
					// If the child is already on the left of the camera,
					// we test if it's completely outside and needs to be recycled
					// 'IsVisibleFrom' is the renderer extension method
					if (firstChild.renderer.IsVisibleFrom(Camera.main) == false) {
						// Get the last(rightmost) object position and size
						Transform lastChild = backgroundPart.LastOrDefault();
						Vector3 lastPosition = lastChild.transform.position;
						Vector3 lastSize = (lastChild.renderer.bounds.max - lastChild.renderer.bounds.min);
						
						// Set the position of the recyled one to be AFTER the last child
						firstChild.position = new Vector3(
							lastPosition.x + lastSize.x,
							firstChild.position.y,
							firstChild.position.z);
						
						// Move the recycled child to the last position
						// of the backgroundPart list
						backgroundPart.Remove(firstChild);
						backgroundPart.Add(firstChild);
					}
				}
			}
		}
	}
	#endregion
}
