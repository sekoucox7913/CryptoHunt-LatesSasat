using System;
using System.Net;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TextController : MonoBehaviour {

    public Text ispis;
    

    //public Text ispis;

    public Text distance_ispis;

    public Text day;

    public Text hours;

    private void Start()
    {
        
    }

    private void Update()
    {
        SetNick();
        SetDistance();
        DayTime();
        HourTime();
    }

    private void SetNick()
    {
		if (Datas.nick != null && ispis != null) {
			ispis.text = Datas.nick;
		}
    }
    private void SetDistance()
    {
//        distance_ispis.text = CheckSpeed.speedShow.ToString();
    }
    
    private void DayTime()
    {
//        day.text = DateTime.Now.ToShortDateString();
    }

    private void HourTime()
    {
//        day.text = DateTime.Now.ToString();
    }
}
