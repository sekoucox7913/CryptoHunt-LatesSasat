using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextInputfield : MonoBehaviour {

    private TextMeshProUGUI ispis_nicka;

    // Use this for initialization
    void Start()
    {
        ispis_nicka = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        ispis_nicka.text = Datas.nick;
    }
}
