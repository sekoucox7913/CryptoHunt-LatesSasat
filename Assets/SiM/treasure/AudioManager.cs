using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {


    public AudioSource muzika;
    public AudioSource gumbClick;

    int brojac = 0;
    int brojac2 = 0;
	// Use this for initialization
	

    public void OnMusicButtonClick()
    {
        if (brojac % 2 == 0)
        {
            muzika.Stop();
        }
        else if (brojac % 2 == 1 )
        {
//            muzika.Play();
        }
        brojac++;
        if (brojac == 11)
        {
            brojac = 0;
        }
    }
    public void OnBtnSoundsButtonClick()
    {
        if (brojac2 % 2 == 0)
        {
            gumbClick.enabled = false;
        }
        else if (brojac2 % 2 == 1)
        {
            gumbClick.enabled = true;
        }
        brojac2++;
        if (brojac2 == 11)
        {
            brojac2 = 0;
        }
    }
}
