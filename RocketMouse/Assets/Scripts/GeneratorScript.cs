using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GeneratorScript : MonoBehaviour {
	
	#region private vars
	//cache view width
	private float screenWidthInPoints;
	#endregion
	
	#region public vars
	//array of available room prefabs
	public GameObject[] availableRooms;
	//list of rooms generated in scene
	public List<GameObject> currentRooms;
	#endregion
	
	#region onStart
	void Start () {
		//get view height from camera
		float height = 2.0f * Camera.main.orthographicSize;
		//calculate view width from height
		screenWidthInPoints = height * Camera.main.aspect;
	}
	#endregion
	
	#region onUpdate
	void Update () {
	}
	#endregion

	#region onFixedUpdate
	void FixedUpdate () {
		//check if room is required
		GenerateRoomIfRequired();
	}
	#endregion

	#region methods
	//adding new room to the scene
	//------------------------------------------------------------------------
	void AddRoom(float furthestRoomEndX) {
		//get random index from available rooms array as access key
		int randomRoomIndex = Random.Range(0, availableRooms.Length);
		//init new room prefab instance using random index key
		GameObject room = (GameObject)Instantiate(availableRooms[randomRoomIndex]);
		//get width from new room prefab instance (4.8 x 3 = 14.4)
		float roomWidth = room.transform.FindChild("floor").localScale.x;
		//calculate target insert position by
		//add half room's size to where the level ends
		//which is the rightmost point of the available scene
		float roomCenter = furthestRoomEndX + roomWidth / 2;
		//set new room's position
		room.transform.position = new Vector3(roomCenter, 0, 0);
		//add newly added room to the list for tracking
		currentRooms.Add(room);
	}

	//check if new room is required in the scene
	//------------------------------------------------------------------------
	void GenerateRoomIfRequired() {
		//create rooms list for removal
		List<GameObject> roomsToRemove = new List<GameObject>();
		//add room flag
		bool addRooms = true;
		//get self (player)'s x position
		float playerX = transform.position.x;
		//calculate x position to remove out-of-view room prefabs
		float removeRoomX = playerX - screenWidthInPoints;
		//calculate x position to add a new room prefab
		float addRoomX = playerX + screenWidthInPoints;

		//init furthestRoomEndX with 0
		float furthestRoomEndX = 0;

		//loop through current list of added rooms in scene
		foreach(var room in currentRooms) {
			//get this room's width and
			//calculate its left and right edges' positions
			float roomWidth = room.transform.FindChild("floor").localScale.x;
			float roomStartX = room.transform.position.x - (roomWidth * 0.5f);
			float roomEndX = roomStartX + roomWidth;

			//check if needed to add new room when
			//if this room's left edge positon is to the right of addRoom x position
			if (roomStartX > addRoomX)
				addRooms = false;

			//add this to remove when
			//if this room's right edge position is to the left of removeRoom x position
			if (roomEndX < removeRoomX)
				roomsToRemove.Add(room);
			
			//set end-of-scene x position to the roomEndX of the rightmost room instance
			furthestRoomEndX = Mathf.Max(furthestRoomEndX, roomEndX);
		}
		
		//loop through roomsToRemove
		foreach(var room in roomsToRemove) {
			//remove this room from scene room list
			currentRooms.Remove(room);
			//and remove(destroy) from the running scene
			Destroy(room);
		}

		//if adding new room is required
		//add new room with the x position of the rightmost room in scene
		if (addRooms)
			AddRoom(furthestRoomEndX);
	}
	#endregion
}
