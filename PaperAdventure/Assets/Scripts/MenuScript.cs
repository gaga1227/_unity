using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// Title screen script
// Game UI script

public class MenuScript : MonoBehaviour {
	#region vars
	// game settings
	private GameObject GameSettingsObj;
	private SettingsScript gameSettings;
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
	// score UI ref
	public int Score;
	public GameObject scoreTextObj;
	private Text scoreText;
	// sound toggle button ref
	public GameObject btnSoundToggleLabel;
	private Text btnSoundToggleLabelText;
	#endregion

	#region	onAwake
	void Awake() {
		// find game settings ref
		GameSettingsObj = GameObject.Find("GameSettings");
		if (GameSettingsObj != null) {
			gameSettings = GameSettingsObj.GetComponent<SettingsScript>();
		}
		// find music audiosource ref
		if (musicObj != null) {
			music = musicObj.GetComponent<AudioSource>();
		}
		// find btn pause image comp
		if (btnPauseImage != null) {
			btnPauseImageComp = btnPauseImage.GetComponent<Image>();
		}
		// prep score and UI if is enemy
		if (scoreTextObj != null) {
			scoreText = scoreTextObj.GetComponent<Text>();
			Score = 0;
		}
		// find sound toggle button label text
		if (btnSoundToggleLabel != null) {
			btnSoundToggleLabelText = btnSoundToggleLabel.GetComponent<Text>();
		}
	}
	#endregion

	#region onStart
	void Start() {
		// init scene with isPaused = false
		isPaused = false;
		// init btn pause images
		toggleBtnPauseSprite(isPaused);
		// play music
		playMusic(true);
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
			playMusic(true);
		} else {
			// time
			Time.timeScale = 0;
			// bg music
			playMusic(false);
		}

		// toggle btn pause images
		toggleBtnPauseSprite(isPaused);
	}

	// play/pause music
	public void playMusic(bool play) {
		// exit if sound setting is off
		if (gameSettings != null && !gameSettings.soundEnabled) return;
		// play/pause music
		if (music != null) {
			if (play) {
				music.Play();
			} else {
				music.Pause();
			}
		}
	}

	// update btn pause images
	private void toggleBtnPauseSprite(bool paused) {
		if (btnPauseImageComp != null) {
			btnPauseImageComp.sprite = paused ? playSprite : pauseSprite;
		}
	}

	// update score
	public void updateScore(int amount) {
		Score += amount;
		scoreText.text = Score.ToString();
	}

	// update toggle sound setting
	public void toggleSound() {
		if (gameSettings != null) {
			// update game setting
			gameSettings.toggleSound();
			// update UI
			updateToggleSoundUI(gameSettings.soundEnabled);
		}
	}

	// update toggle sound UI
	public void updateToggleSoundUI(bool state) {
		if (btnSoundToggleLabelText != null) {
			btnSoundToggleLabelText.text = "Sound " + (state ? "On" : "Off");
		}
	}
	#endregion
}
