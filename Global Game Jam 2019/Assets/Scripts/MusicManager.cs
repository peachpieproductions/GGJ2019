using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour {

    public AudioClip[] songs;
    public AudioSource audioSource;

    float musicVol;
    bool musicOn = true;

    int currentSong;

    public void Start() {
        musicVol = audioSource.volume;
        StartCoroutine(Music());
    }

    public void ToggleMusicOnOff() {
        if (GameController.touch) GameController.touchTimer++;
        musicOn = !musicOn;
        if (musicOn) audioSource.volume = musicVol;
        else audioSource.volume = 0;
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
