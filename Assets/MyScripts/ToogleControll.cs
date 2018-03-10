using UnityEngine;
using UnityEngine.UI;

public class ToogleControll : MonoBehaviour
{

    public Camera MainCamera;
    public Canvas image;
    public Toggle togl;

    

    CameraAsBackground camera1;
   

    public void ToogleChanged()
    {
        if (togl.isOn)
        {
            MainCamera.GetComponent<GyroCamera>().enabled = false;
   

           

        }
        else if (!togl.isOn)
        {
            MainCamera.GetComponent<GyroCamera>().enabled = true;

      

        }
       
    }

   
}
