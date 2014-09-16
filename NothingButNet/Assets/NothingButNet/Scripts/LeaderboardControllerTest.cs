using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class LeaderboardControllerTest : MonoBehaviour {

	//LeaderboardController instance ref
	public LeaderboardController leaderboard;
	
	void Start () {
		//adding scores (test)
		//leaderboard.AddPlayersScore( 10020 );

		//register handler to leaderboard controler delegate fn (handle async result)
		leaderboard.OnScoresLoaded += Handle_OnScoresLoaded;
		//start fetch updates (init async call)
		leaderboard.FetchScores();
	}
	
	void Update () {
	}
	
	//async score fetch call hanlder, list fetched scores in log
	//'List<ScoreData>' type is a list of ScoreData objects
	public void Handle_OnScoresLoaded( List<ScoreData> scores ){
		foreach( ScoreData score in scores ){
			Debug.Log ( score.points );
		}
	}

}