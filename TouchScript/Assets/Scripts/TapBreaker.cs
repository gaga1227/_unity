using System;
using UnityEngine;
using TouchScript.Gestures;

// import Random
using Random = UnityEngine.Random;

public class TapBreaker : MonoBehaviour
{
	#region vars
	// break force
	public float Power = 10.0f;

	// break direction vectors
	private Vector3[] directions = {
		new Vector3 (1, -1, 1),
		new Vector3 (-1, -1, 1),
		new Vector3 (-1, -1, -1),
		new Vector3 (1, -1, -1),
		new Vector3 (1, 1, 1),
		new Vector3 (-1, 1, 1),
		new Vector3 (-1, 1, -1),
		new Vector3 (1, 1, -1)
	};
	#endregion

	#region on enable
	private void OnEnable ()
	{
		// subscribe to gesture's Tapped event
		// for all touch gesture comps
		foreach (var tap in GetComponents<TapGesture>()) {
			tap.Tapped += tappedHandler;
		}
	}
	#endregion

	#region on disable
	private void OnDisable ()
	{
		// don't forget to unsubscribe
		// for all touch gesture comps
		foreach (var tap in GetComponents<TapGesture>()) {
			tap.Tapped -= tappedHandler;
		}
	}
	#endregion

	#region handlers
	private void tappedHandler (object sender, EventArgs e)
	{
//		var tap = sender as TapGesture;
//		switch (tap.NumberOfTapsRequired) {
//		case 1:
//			Debug.Log ("Single Tap");
//			break;
//		case 2:
//			// our double tap gesture
//			Debug.Log ("Double Tap");
//			break;
//		}

		// if we are not too small
		Debug.Log (transform.name);
		if (transform.name == "Cube" && transform.localScale.x > 0.05f) {
			Color color = new Color (Random.value, Random.value, Random.value);
			// break this cube into 8 parts
			for (int i = 0; i < 8; i++) {
				// clone self into pieces
				var obj = Instantiate (gameObject) as GameObject;
				var cube = obj.transform;
				cube.parent = transform.parent;
				cube.name = "Cube";
				cube.localScale = 0.5f * transform.localScale;
				cube.position = transform.TransformPoint (directions [i] / 4);
				cube.rigidbody.AddForce (Power * Random.insideUnitSphere, ForceMode.VelocityChange);
				cube.renderer.material.color = color;
			}
			// remove self
			Destroy (gameObject);
		}
	}
	#endregion
}
