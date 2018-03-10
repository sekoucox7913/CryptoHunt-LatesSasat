using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


public class PreloaderScript : MonoBehaviour
{

	public static PreloaderScript instance;
	public Image ImgLoderImage, ImgLoderImage2;
	public GameObject PreloaderPanel;


	void Awake ()
	{
		instance = this;
	}

	void Update ()
	{
		ImgLoderImage2.transform.Rotate (0, 0, -15);
		ImgLoderImage.transform.Rotate (0, 0, -15);
	}

	internal void OnEnabledLoder ()
	{
		PreloaderPanel.SetActive (true);
		ImgLoderImage.enabled = true;
		ImgLoderImage2.enabled = false;
		PreloaderPanel.transform.SetAsLastSibling ();
		Invoke ("OnDisabledLoder", 15f);
	}

	internal void OnEnabledLoderLogin ()
	{
		PreloaderPanel.SetActive (true);
		ImgLoderImage.enabled = false;
		ImgLoderImage2.enabled = true;
		PreloaderPanel.transform.SetAsLastSibling ();
		Invoke ("OnDisabledLoder", 15f);
	}

	internal void OnDisabledLoder ()
	{
		PreloaderPanel.SetActive (false);
		CancelInvoke ();
	}
	 
}
