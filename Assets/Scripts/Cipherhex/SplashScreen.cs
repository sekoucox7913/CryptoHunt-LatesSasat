using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplashScreen : MonoBehaviour
{

	public static SplashScreen instance;
	public GameObject SplashScreenPanel;

	void Awake ()
	{
		instance = this;
		SplashScreenPanel.SetActive (true);
		SplashScreenPanel.transform.SetAsLastSibling ();
		Invoke ("OnNextButtonClick", 0.01f);
//		OnNextButtonClick();

	}

	void Start ()
	{
		if (PlayerPrefs.HasKey ("IsRegister")) {
			//			PlayerPrefs.DeleteKey ("IsRegister");
			if (PlayerPrefs.GetString ("IsRegister") == "true") {
				//				PlayingScript.instance.OnPlayingScreenDisplay ();
				LoginScript.instance.OnAutoLogin ();
			} else {
				SplashScreenPanel.SetActive (false);
				RegisterScript.instance.RegisterPanel.SetActive (true);
				RegisterScript.instance.RegisterPanel.transform.SetAsLastSibling ();
			} 
		} else { 
			SplashScreenPanel.SetActive (false);
			RegisterScript.instance.RegisterPanel.SetActive (true);
			RegisterScript.instance.RegisterPanel.transform.SetAsLastSibling ();
		}
	}

	public void OnNextButtonClick ()
	{
		
		if (PlayerPrefs.HasKey ("IsRegister")) {
//			PlayerPrefs.DeleteKey ("IsRegister");
			if (PlayerPrefs.GetString ("IsRegister") == "true") {
//				PlayingScript.instance.OnPlayingScreenDisplay ();
				LoginScript.instance.OnAutoLogin ();
			} else {
				SplashScreenPanel.SetActive (false);
				RegisterScript.instance.RegisterPanel.SetActive (true);
				RegisterScript.instance.RegisterPanel.transform.SetAsLastSibling ();
			} 
		} else { 
			SplashScreenPanel.SetActive (false);
			RegisterScript.instance.RegisterPanel.SetActive (true);
			RegisterScript.instance.RegisterPanel.transform.SetAsLastSibling ();
		}
	}
}
