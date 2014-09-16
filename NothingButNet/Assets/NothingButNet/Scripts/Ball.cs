//imports
using UnityEngine;
using System.Collections;

//require comps
[RequireComponent (typeof(SphereCollider))]
[RequireComponent (typeof(Rigidbody))]

//class
public class Ball : MonoBehaviour {

	//vars - ball's private property
	private Transform _transform;
	private Rigidbody _rigidbody;
	private SphereCollider _sphereCollider; 

	//var - ref to GameController (a singleton)
	private GameController _gameController;

	//declare public delegate: Net()
	//and public member var: OnNet of type: Net as template fn
	//OnNet has default value of null
	public delegate void Net();
	public Net OnNet;

	//on awake
	void Awake ()
	{
		//on init, get attached comps from local GameObject
		//and assign/cache them to private vars for later use
		_transform = GetComponent<Transform>();
		_rigidbody = GetComponent<Rigidbody>();
		_sphereCollider = GetComponent<SphereCollider>();
	}

	//on start
	void Start ()
	{
		//access GameController via SharedInstance static property
		//and assign/cache to private var
		_gameController = GameController.SharedInstance;
	}

	//on frame update
	void Update ()
	{
		
	}

	//public getters
	public Transform BallTransform {
		get {
			return _transform; 
		}
	}
	public Rigidbody BallRigidbody {
		get {
			return _rigidbody;
		}
	}
	public SphereCollider BallCollider {
		get {
			return _sphereCollider;
		}
	}
	
	//OnCollisionEnter
	//event handler when Rigidbody collides
	//with BoxCollider from others, e.g. walls, ground, etc.
	public void OnCollisionEnter(Collision collision)
	{
		//pass on the collison info to GameController
		_gameController.OnBallCollisionEnter(collision);
	}

	//OnTriggerEnter
	//event handler when Rigidbody collides(passing through)
	//with BoxCollider with trigger, e.g. LeftHoop_001
	public void OnTriggerEnter(Collider collider)
	{
		//if trigger collider is LeftHoop_001
		if (collider.name.Equals("LeftHoop_001")) {
			//call OnNet as a function after checking
			//if there's fn added to delegate fn template
			if (OnNet != null) {
				OnNet();
			}
		}
	}
}
