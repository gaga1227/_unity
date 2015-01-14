using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// Title screen script

public class MenuScript : MonoBehaviour {
	#region vars
	// main bg music ref
	public GameObject musicObj;
	private AudioSource music;
	// btn pause refs
	public GameObject btnPauseImage;
	public Sprite playSprite;
	public Sprite pauseSprite;
	private Image btnPauseImageComp;
	// paused flag
	public bool isPaused;
	#endregion

	#region	onAwake
	void Awake() {
		// find music audiosource ref
		if (musicObj != null) {
			music = musicObj.GetComponent<AudioSource>();
		}
		// find btn pause image comp
		if (btnPauseImage != null) {
			btnPauseImageComp = btnPauseImage.GetComponent<Image>();
		}
	}
	#endregion

	#region onStart
	void Start() {
		// init scene with isPaused = false
		isPaused = false;
		// init btn pause images
		toggleBtnPauseSprite(isPaused);
	}
	#endregion

	#region methods
	// play stage1
	public void Play() {
		Application.LoadLevel("Stage1");
	}

	// pause/resume method
	public void PauseResume() {
		// toggle pause flag
		isPaused = !isPaused;

		// toggle gameplay and music
		if (!isPaused) {
			// time
			Time.timeScale = 1;
			// bg music
			if (music != null) music.Play();
		} else {
			// time
			Time.timeScale = 0;
			// bg music
			if (music != null) music.Pause();
		}

		// toggle btn pause images
		toggleBtnPauseSprite(isPaused);
	}

	// update btn pause images
	private void toggleBtnPauseSprite(bool paused) {
		if (btnPauseImageComp != null) {
			btnPauseImageComp.sprite = paused ? playSprite : pauseSprite;
		}
	}
	#endregion
}
