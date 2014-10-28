using UnityEngine;
using System.Collections;

public class BackgroundRepeater : MonoBehaviour {

	#region vars
	//camera transform ref
	private Transform cameraTransform;
	//cache bg sprite image width
	private float spriteWidth;
	#endregion

	#region on start
	void Start () {
		//store camera transform ref to var
		cameraTransform = Camera.main.transform;
		//get ref to sprite renderer comp
		SpriteRenderer spriteRenderer = renderer as SpriteRenderer;
		//store sprite with to var
		spriteWidth = spriteRenderer.sprite.bounds.size.x;
	}
	#endregion

	#region on frame
	void Update () {
		//this works only when tile size is bigger than camera view size
		//you need two bg tile to make it repeat seamlessly
		//when second tile is fully in view and first tile is fully out of view
		//move first tile ahead to be the new second tile
		if((transform.position.x + spriteWidth) < cameraTransform.position.x) {
			//get current bg pos to var
			Vector3 newPos = transform.position;
			//calculate new pos on x axis
			//move out-of-view bg tile ahead of visible tile
			newPos.x += 2.0f * spriteWidth;
			//update back to bg pos
			transform.position = newPos;
		}
	}
	#endregion
}
