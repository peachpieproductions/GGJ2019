using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {


    public static AudioSource audioSource;
    public static List<AudioClip> clipsPlayedRecently = new List<AudioClip>();

    private void Awake() {
        audioSource = GetComponent<AudioSource>();
    }

    private void Start() {
        StartCoroutine(ClearClipCache());
    }


    public static void PlayOneShot(AudioClip ac, float vol = 1f, bool randomPitch = true, float setPitch = 1f) {

        if (!clipsPlayedRecently.Contains(ac)) {
            clipsPlayedRecently.Add(ac);
            if (setPitch != 1) audioSource.pitch = setPitch;
            else if (randomPitch) audioSource.pitch = Random.Range(.8f, 1.2f);
            else audioSource.pitch = 1f;
            audioSource.PlayOneShot(ac, Mathf.Min(1,vol));
        } 

    }

    IEnumerator ClearClipCache() {
        while (true) {
            if (clipsPlayedRecently.Count > 0) clipsPlayedRecently.Clear();
            yield return new WaitForSeconds(Random.Range(.04f,.07f));
        }
    }




}
