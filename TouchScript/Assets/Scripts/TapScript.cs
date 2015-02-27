using System;
using TouchScript.Gestures;
using UnityEngine;

public class TapScript : MonoBehaviour
{

	// attach tap handler
	//------------------------------------------------------------------------
	private void OnEnable ()
	{
		foreach (var tap in GetComponents<TapGesture>()) {
			tap.Tapped += tappedHandler;
		}
		foreach (var press in GetComponents<LongPressGesture>()) {
			press.LongPressed += longPressedHandler;
		}
	}

	// detach tap handler
	//------------------------------------------------------------------------
	private void OnDisable ()
	{
		foreach (var tap in GetComponents<TapGesture>()) {
			tap.Tapped -= tappedHandler;
		}
		foreach (var press in GetComponents<LongPressGesture>()) {
			press.LongPressed -= longPressedHandler;
		}
	}

	#region handlers
	// tap handler
	//------------------------------------------------------------------------
	private void tappedHandler (object sender, EventArgs eventArgs)
	{
		var tap = sender as TapGesture;
		switch (tap.NumberOfTapsRequired) {
		case 1:
			Debug.Log ("Single Tap");
			break;
		case 2:
			Debug.Log ("Double Tap");
			break;
		}
	}
	// long press handler
	//------------------------------------------------------------------------
	private void longPressedHandler (object sender, EventArgs eventArgs)
	{
//		var press = sender as LongPressGesture;
		Debug.Log ("Long Pressed");
	}
	#endregion
}
