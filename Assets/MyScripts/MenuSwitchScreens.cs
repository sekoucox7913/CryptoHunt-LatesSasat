using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuSwitchScreens : MonoBehaviour {

    public GameObject kontejner;

    public void OnSettingsButtonClick()
    {
        kontejner.transform.position = new Vector3(-67.5f, 0, 0);
    }

    public void OnSettingsButtonBack()
    {
       
        kontejner.transform.position = new Vector3(0, 0, 0);
    }
}
