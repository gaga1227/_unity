#pragma strict

var audioVictory : AudioClip;
var audioDefeat : AudioClip;
var volumeVictory : float = 2.0;
var volumeDefeat : float = 1.0;
 
function playSoundOfVictory(isVictory : boolean) {
    // Stop any current audio
    if (audio.isPlaying) audio.Stop();
    
    Debug.Log(audio);
 
    // Play either the sound of victory or defeat.
    // clip source is supplied here, no need to specify in editor
    audio.clip = isVictory ? audioVictory : audioDefeat;
    audio.volume = isVictory ? volumeVictory : volumeDefeat;
    audio.Play();
}
 
function resetGame() {
    // Reset to original state, stop any audio
    if (audio.isPlaying) audio.Stop();
}

//gives ref to audio object
@script RequireComponent(AudioSource)