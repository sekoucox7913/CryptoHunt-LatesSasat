using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Touch : MonoBehaviour
{
    public GameObject _container;
    public GameObject other;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Tab();
    }

    void Tab()
    {
        // if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Click");
            Vector3 mousePosFar = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.farClipPlane);
            Vector3 mousePosNear = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane);

            Vector3 mousePosF = Camera.main.ScreenToWorldPoint(mousePosFar);
            Vector3 mousePosN = Camera.main.ScreenToWorldPoint(mousePosNear);

            RaycastHit hit;

            if (Physics.Raycast(mousePosN, mousePosF - mousePosN, out hit))
            {
                Debug.Log("Hit");

                if (hit.collider.tag.Equals("CloseChest"))
                {
                    // tu ide neka akcija


                    int random = Random.Range(1, 6);
                    Vector3 upRnd = Random.onUnitSphere;
                    // upRnd.y = Mathf.Abs(upRnd.y);

                    Transform rndChild = _container.transform.GetChild(random);
                    Transform instance = Instantiate(rndChild, transform.position + Vector3.up, Quaternion.identity);
                    instance.GetComponent<Rigidbody>().AddForce(Vector3.up + Vector3.forward);
                    instance.transform.localScale = other.transform.localScale;
                }
            }
        }
    }
}
