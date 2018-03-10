using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuContScript : MonoBehaviour {

    public GameObject MenuCont;
    public AudioSource zvukGumba;

    


        public void OnButtonClickCameraSwitch() {

        MenuCont.transform.Translate(-1800f, 0, 0);
//        zvukGumba.Play();
    }
   

}
