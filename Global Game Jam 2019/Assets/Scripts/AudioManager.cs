using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {


    public static AudioSource audioSource;

    private void Awake() {
        audioSource = GetComponent<AudioSource>();
    }



    public static void PlayOneShot(AudioClip ac, float vol = 1f, bool randomPitch = true, float setPitch = 1f) {

        if (setPitch != 1) audioSource.pitch = setPitch;
        else if (randomPitch) audioSource.pitch = Random.Range(.8f, 1.2f);
        else audioSource.pitch = 1f;
        audioSource.PlayOneShot(ac, vol);

    }




}
