using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour {

    public AudioClip[] songs;
    public AudioSource audioSource;

    int currentSong;

    public void Start() {
        StartCoroutine(Music());
    }

    IEnumerator Music() {

        yield return new WaitForSeconds(2.8f);

        while (true) {

            audioSource.clip = songs[currentSong];
            audioSource.Play();

            yield return new WaitForSeconds(audioSource.clip.length + 10f);

            currentSong++;
            if (currentSong == songs.Length) currentSong = 0;

        }

    }


}
