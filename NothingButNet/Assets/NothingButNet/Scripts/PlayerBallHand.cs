//imports
using UnityEngine;
using System.Collections;

//require comp
[RequireComponent (typeof(Collider))]

//class
public class PlayerBallHand : MonoBehaviour
{
	//player instance ref
	private Player _player = null;

	//on awake
	void Awake ()
	{
		
	}

	//on start
	void Start ()
	{
		//get Player comp from currently attached obj on start

		//transform parent placeholder var
		Transform parent = transform.parent;
		//as long as parent is valid and player is not assigned
		while (parent != null && _player == null) {
			//try request Player component
			Player parentPlayer = parent.GetComponent<Player>(); //get component by <type> of object
			//if found, assign to ref var
			if (parentPlayer != null) {
				_player = parentPlayer;
			}
			//otherwise keep traversing up
			else {
				parent = parent.parent;
			}
		}
	}

	//on frame update
	void Update ()
	{
		
	}

	//OnTriggerEnter event handler for player hand
	void OnTriggerEnter (Collider collider)
	{
		//manually trigger collision event on player
		//when hand collision event fires
		_player.OnTriggerEnter(collider); 
	}
}
