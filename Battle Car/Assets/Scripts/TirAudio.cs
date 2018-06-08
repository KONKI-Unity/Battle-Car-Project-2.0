using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TirAudio : MonoBehaviour {

    public AudioClip SoundTir;

	// Update is called once per frame
	void Update () {
        if (Input.GetButton("Fire1") && Time.time > 0f)
        {
            GetComponent<AudioSource>().PlayOneShot(SoundTir);
        }
	}
}
