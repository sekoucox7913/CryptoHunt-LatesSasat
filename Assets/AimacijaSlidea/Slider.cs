using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slider : MonoBehaviour {


    public AudioSource slideEffect;

    int brojac = 0;

   
    // Use this for initialization

    private void Start()
    {

    }

    public void ButtonClick()
    {

        if (brojac % 2 == 0)
        {
            this.GetComponent<Animation>().Play("Right");
        }
        else if (brojac % 2 == 1)
        {
            this.GetComponent<Animation>().Play("Left");
        }

        brojac++;
        if (brojac == 256)
        {
            brojac = 0;
        }
    }
}
