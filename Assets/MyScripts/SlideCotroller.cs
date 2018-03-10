using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlideCotroller : MonoBehaviour {

    public GameObject slider;
    

    
   

    int lblCounter = 0;



    public void OnButtonSliderClick()
    {
        if(lblCounter % 2 == 0)
        {

            slider.GetComponent<Animation>().Play("Left");
        }

            else if (lblCounter % 2 == 1)
        {
            slider.GetComponent<Animation>().Play("Right");
        }
        lblCounter++;
        if (lblCounter == 256)
        {
            lblCounter = 0;
        }

    }

   
}
