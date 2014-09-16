//imports
using UnityEngine;
using System.Collections;

//class/object definition
public class ScoreBoard : MonoBehaviour
{
	//public vars
	public TextMesh pointsTextMesh;
	public TextMesh timeRemainingTextMesh;

	//event hooks
	void Awake ()
	{
	}
	
	void Start ()
	{ 
	}
	
	void Update ()
	{
	}

	//public methods
	public void SetTime (string timeRemaining)
	{
		timeRemainingTextMesh.text = timeRemaining;
	}
	
	public void SetPoints (string points)
	{
		pointsTextMesh.text = points;
	}
}
