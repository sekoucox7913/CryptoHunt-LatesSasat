using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class CheckSpeed : MonoBehaviour {

    public static float speedShow;

    

    private float speed;


    Transform posOrg;

    private float dis = 0;
    private float timeFactor = 1;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        SpeedCalculate();
	}

    void SpeedCalculate()
    {
        

        dis = Vector3.Distance(posOrg.position, transform.position);

        speed = dis / timeFactor;
        speedShow = speed;

    }
}
