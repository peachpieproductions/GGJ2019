using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour {


    public static AudioSource audioSource;
    public AudioSource introAudioSource;
    public static List<AudioClip> clipsPlayedRecently = new List<AudioClip>();
    public static float volumeMult = 1f;

    private void Awake() {
        audioSource = GetComponent<AudioSource>();
    }

    private void Start() {
        StartCoroutine(ClearClipCache());
    }

    public void SetVolume(Slider slider) {
        volumeMult = slider.value;
        introAudioSource.volume = slider.value;
    }


    public static void PlayOneShot(AudioClip ac, float vol = 1f, bool randomPitch = true, float setPitch = 1f) {

        if (!clipsPlayedRecently.Contains(ac)) {
            clipsPlayedRecently.Add(ac);
            if (setPitch != 1) audioSource.pitch = setPitch;
            else if (randomPitch) audioSource.pitch = Random.Range(.8f, 1.2f);
            else audioSource.pitch = 1f;
            audioSource.PlayOneShot(ac, Mathf.Min(1,vol) * volumeMult);
        } 

    }

    IEnumerator ClearClipCache() {
        while (true) {
            if (clipsPlayedRecently.Count > 0) clipsPlayedRecently.Clear();
            yield return new WaitForSeconds(Random.Range(.04f,.07f));
        }
    }




}
