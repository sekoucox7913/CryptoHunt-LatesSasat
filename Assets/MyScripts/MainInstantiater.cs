using System.Collections.Generic;
using UnityEngine;
using GoMap;
using GoShared;

   public class MainInstantiater : MonoBehaviour
    {





    public GOMap map;
    public GameObject chest;

    public Coordinates[] coordinatesGPS;

   




    private void Awake()
    {
        if (map == null)
        {
            Debug.LogWarning("GOObject - Map property not set");
            return;
        }

        //register this class for location notifications
      
    }




    private void Start()
    {
        Instantiate(chest);
        for (int i = 0; i < coordinatesGPS.Length; i++)
        {

            map.dropPin(coordinatesGPS[i].latitude, coordinatesGPS[i].longitude, chest);

        }
    }
  }
        
