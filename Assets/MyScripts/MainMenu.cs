using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {

    public Button btn;
    public GameObject image;

    // Use this for initialization
    void Start()
    {
        image.SetActive(false);
        btn.onClick.AddListener(TaskOnClick);
    }

    void TaskOnClick()
    {
        image.SetActive(true);
    }

    private void Update()
    {
        TaskOnClick();
    }
}
