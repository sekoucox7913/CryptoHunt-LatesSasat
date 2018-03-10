using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CammeraController : MonoBehaviour {

    public Camera cam1;
    public Camera cam2;
    public Transform cam1Pos;
    public Transform cam2Pos;
    // Use this for initialization
    void Start () {

        cam1Pos = cam1.transform;
        cam2Pos = cam2.transform;
    }

    public void activateCam1()
    {
        cam1.enabled = true;
        cam2.enabled = false;
       // cam1.transform = cam1Pos;
    }
    public void activateCam2()
    {
        cam2.enabled = true;
        cam1.enabled = false;
       // cam2.transform = cam2Pos;
    }
}
