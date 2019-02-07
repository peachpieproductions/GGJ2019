using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MusicManager : MonoBehaviour {

    public AudioClip[] songs;
    public AudioSource audioSource;

    float musicVol;
    bool musicOn = true;

    int currentSong;

    public void Start() {
        musicVol = audioSource.volume;
        
    }

    private void OnEnable() {
        StartCoroutine(Music());
    }

    public void ToggleMusicOnOff() {
        if (GameController.touch) GameController.touchTimer++;
        musicOn = !musicOn;
        if (musicOn) audioSource.volume = musicVol;
        else audioSource.volume = 0;
    }

    public void SetVolume(Slider slider) {
        musicVol = slider.value;
        audioSource.volume = musicVol * .38f;
    }

    IEnumerator Music() {

        yield return null;

        while(!GameController.inst.gameStarted) {
            yield return null;
        }

        yield return new WaitForSeconds(2.85f);

        while (true) {

            audioSource.clip = songs[currentSong];
            audioSource.Play();

            yield return new WaitForSeconds(audioSource.clip.length + 10f);

            currentSong++;
            if (currentSong == songs.Length) currentSong = 0;

        }

    }


}
