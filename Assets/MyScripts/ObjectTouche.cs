using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectTouche : MonoBehaviour
{

    // public GameObject panel;

    public Animator animator;
    public ParticleSystem part;

    int i = 0;

    private void Start()
    {
        //   panel.SetActive(false);
        animator.enabled = false;
        part.enableEmission = false;
    }

    private void Update()
    {
        Tab();
    }

    void Tab()
    {
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
        {
            Vector3 mousePosFar = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.farClipPlane);
            Vector3 mousePosNear = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane);

            Vector3 mousePosF = Camera.main.ScreenToWorldPoint(mousePosFar);
            Vector3 mousePosN = Camera.main.ScreenToWorldPoint(mousePosNear);

            RaycastHit hit;

            if (Physics.Raycast(mousePosN, mousePosF - mousePosN, out hit))
            {


                if (hit.collider.tag.Equals("Sfera"))
                {
                    //Destroy(GameObject.FindWithTag("CloseChest"));
                    //Application.LoadLevel("Chest Animation");
                    if (i % 2 == 0)
                    {
                        animator.enabled = true;
                        part.enableEmission = true;


                    }



                    //}

                    //else if (i % 2 == 1)
                    //{
                    //    panel.SetActive(false);
                    //}
                    //i++;
                    //if (i == 12)
                    //{
                    //    i = 0;
                    //}
                }

            }
        }


    }


}