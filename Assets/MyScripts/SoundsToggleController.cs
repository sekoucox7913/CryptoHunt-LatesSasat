using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundsToggleController : MonoBehaviour {

    public Toggle musicTogl;

    public Toggle btnSoundTogl;

    //public AudioSource MainGameMusic;
    //public AudioSource CharSelectMusic;

    public GameObject Sounds;


    public AudioSource btnSound;

    public AudioSource slideSound;

    public void ToogleMusicChanged()
    {
        if (musicTogl.isOn)
        {
            //MainGameMusic.enabled = true;
            //CharSelectMusic.enabled = true;

            Sounds.SetActive(true);


        }
        else if (!musicTogl.isOn)
        {
            //MainGameMusic.enabled = false;
            //CharSelectMusic.enabled = false;

            Sounds.SetActive(false);
        }

    }

    public void ToogleBtnSoundChanged()
    {
        if (btnSoundTogl.isOn)
        {
            btnSound.enabled = true;
            slideSound.enabled = true;



        }
        else if (!btnSoundTogl.isOn)
        {
            btnSound.enabled = false;
            slideSound.enabled = false;


        }

    }
}
