using UnityEngine;
using System.Collections;

// Title screen script

public class MenuScript : MonoBehaviour {
	#region vars
	// main bg music ref
	public GameObject musicObj;
	private AudioSource music;
	// paused flag
	public bool isPaused;
	#endregion

	#region	onAwake
	void Awake() {
		// init scene with play
		isPaused = false;
		// assign music ref
		music = musicObj.GetComponent<AudioSource>();
	}
	#endregion

	#region methods
	// play stage1
	public void Play() {
		Application.LoadLevel("Stage1");
	}

	// pause/resume method
	public void PauseResume() {
		// toggle gameplay and music
		if (isPaused) {
			Time.timeScale = 1;
			if (music != null) music.Play();
		} else {
			Time.timeScale = 0;
			if (music != null) music.Pause();
		}
		// toggle pause flag
		isPaused = !isPaused;
	}
	#endregion
}
