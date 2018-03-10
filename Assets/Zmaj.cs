using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Zmaj : MonoBehaviour {

    // Use this for initialization

    

    public Animation anim;
    public Animation anim2;
    public int brojac = 0;
    private void Start()
    {
        anim = GetComponent<Animation>();
        anim2 = GetComponent<Animation>();

    }

    public void OnBat()
    {

        if (brojac % 2 == 0)
        {
            if (anim2["New AnimationHHH"])
                anim.Play("New AnimationHHH");
        
        }
        else if (brojac % 2 == 1)
        {
//            anim2s.Play();
        }

        brojac++;

        if (brojac == 256)
        {
            brojac = 0;
        }
       
        }



}
