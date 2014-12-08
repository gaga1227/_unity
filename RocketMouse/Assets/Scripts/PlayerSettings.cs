using UnityEngine;
using System.Collections;
//for using new GUI
using UnityEngine.UI;

public class PlayerSettings : MonoBehaviour {

	#region vars
	//public setting vars to share across the game
	public bool audioEnabled;
	public float volume;
	#endregion

	#region onStart
	void Start () {
		//set init values
		audioEnabled = true;
		volume = 1.0f;

		//preserve this gameobject across scenes
		DontDestroyOnLoad(this);
	}
	#endregion

	#region methods
	//toggle audio
	//------------------------------------------------------------------------
	public void toggleAudio(bool state) {
		//pass UI control value to settings var
		audioEnabled = !state;
	}

	//set volume
	//------------------------------------------------------------------------
	public void setVolume(float vol) {
		//pass UI control value to settings var
		volume = vol;
	}
	#endregion

	#region handlers
	//on level loaded
	//------------------------------------------------------------------------
	void OnLevelWasLoaded(int level) {
		//if main camera exists
		GameObject mainCam = GameObject.Find("Main Camera");
		if (mainCam) {
			//set volumn on main camera to stored volume value
			mainCam.GetComponent<AudioSource>().volume = audioEnabled ? volume : 0.0f;
		}
		
		//if slider object exists
		GameObject sldObj = GameObject.Find("sdr_volume");
		if (sldObj) {
			//get slider UI and set its value to stored volume value
			Slider volumeSlider = sldObj.GetComponent<Slider>();
			volumeSlider.value = audioEnabled ? volume : 0.0f;
		}
		
		//if toggle object exists
		GameObject tglObj = GameObject.Find("tgl_sound");
		if (tglObj) {
			//get slider UI and set its value to stored volume value
			Toggle volumeToggle = tglObj.GetComponent<Toggle>();
			volumeToggle.isOn = !audioEnabled;
		}
	}
	#endregion
}
