using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Options : MonoBehaviour {




    public Slider sndfxVolSlider;
    public Slider musicVolSlider;


    public void SaveSettings() {
        PlayerPrefs.SetFloat("sndfxVol", sndfxVolSlider.value);
        PlayerPrefs.SetFloat("musicVol", musicVolSlider.value);
    }

    public void LoadSettings() {
        sndfxVolSlider.value = PlayerPrefs.GetFloat("sndfxVol",1f);
        musicVolSlider.value = PlayerPrefs.GetFloat("musicVol",1f);
    }

}
