using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public  class ObjectCopy : MonoBehaviour {

    public GameObject chest;
    

     void Start()
    {
        Instantiate(chest,transform.position, transform.rotation);
    }
}
