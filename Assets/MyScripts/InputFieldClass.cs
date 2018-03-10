using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputFieldClass : MonoBehaviour {

    public InputField nickEntry;


    private void Start()
    {
//        nickEntry.text = Datas.nick;
    }

    private void Update()
    {
       SetText();
    }

    private void SetText()
    {
//        Datas.nick = nickEntry.text;
    }
}
