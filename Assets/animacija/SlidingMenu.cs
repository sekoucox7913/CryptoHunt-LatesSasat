using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlidingMenu : MonoBehaviour {

    // Use this for initialization
    public Button bnt1;
    public Button bnt2;
    public 


    int brojac = 0;
    // Use this for initialization

    private void Start()
    {

    }

    public void ButtonClick()
    {

        if (brojac % 2 == 0)
        {
            bnt1.GetComponent<Animation>().Play("prvidesno");
            bnt2.GetComponent<Animation>().Play("drugidesno");
        }
        else if (brojac % 2 == 1)
        {
            bnt1.GetComponent<Animation>().Play("prvilevo");
            bnt2.GetComponent<Animation>().Play("drugilevo");
        }

        brojac++;
        if (brojac == 256)
        {
            brojac = 0;
        }
    }

}
