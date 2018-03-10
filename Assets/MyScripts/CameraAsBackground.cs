using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraAsBackground : MonoBehaviour
{


    private RawImage image;
    private WebCamTexture cam;
    private AspectRatioFitter asf;


    public GameObject panel;

   // public Button btn;
  //  int counter = 0;
    // Use this for initialization
    void Start()
    {


      //  panel.SetActive(false);


        asf = GetComponent<AspectRatioFitter>();
        image = GetComponent<RawImage>();
        cam = new WebCamTexture(Screen.width, Screen.height);
		if (image != null) {
			image.texture = cam;
			cam.Play ();
		}
    }

    // Update is called once per frame
    void Update()
    {
        if (cam.width < 100)
        {
            return;
        }
        float cwNedeed = -cam.videoRotationAngle;
        if (cam.videoVerticallyMirrored)
        {
            cwNedeed += 100f;

        }
        image.rectTransform.localEulerAngles = new Vector3(0f, 0f, cwNedeed);
        float videoRatio = (float)cam.width / (float)cam.height;
        asf.aspectRatio = videoRatio;

        if (cam.videoVerticallyMirrored)

        {
            image.uvRect = new Rect(1, 0, -1, 1);
        }
        else
        {
            image.uvRect = new Rect(0, 0, 1, 1);
        }

    }

    //public void TaskOnClick()
    //{
    //    if (counter % 2 == 0)
    //    {
    //        image.enabled = false;
    //        if (!image.enabled)
    //            cam.Pause();
    //        panel.SetActive(true);
    //    }
    //    else if (counter % 2 == 1)
    //    {
    //        image.enabled = true;
    //        if (image.enabled)
    //            panel.SetActive(false);
    //            cam.Play();
    //    }
    //    counter++;
    //    if (counter == 12)
    //        counter = 0;
    //}


}
