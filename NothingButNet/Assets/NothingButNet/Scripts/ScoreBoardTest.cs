//imports
using UnityEngine;
using System.Collections;

//class def
public class ScoreBoardTest : MonoBehaviour
{
	//vars

	//ref to game obj
	public ScoreBoard scoreboard; 

	//on start
	public void Start()
	{
		scoreboard.SetTime( "60" );
		scoreboard.SetPoints( "100" );
	}
}