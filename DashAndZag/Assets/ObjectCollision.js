#pragma strict

var impact : AudioClip;

function OnCollisionEnter () {
    audio.PlayOneShot(impact);
}

// gives audio obj ref
@script RequireComponent(AudioSource)