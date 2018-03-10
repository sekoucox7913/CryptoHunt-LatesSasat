using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


public class HowToPlayScript : MonoBehaviour
{
	public static HowToPlayScript instance;
	public GameObject HowToPlayScreenPanel;

	void Awake ()
	{
		instance = this;
		HowToPlayScreenPanel.SetActive (false);
	}

	internal void OnHowTplayOpen ()
	{
		HowToPlayScreenPanel.SetActive (true);
		HowToPlayScreenPanel.transform.SetAsLastSibling ();
		SoundManagerScript.instance.OnPlayButtonClickSound ();
		SoundManagerScript.instance.InMenuClicked ();
	}

	public void OnCloseButtonClick ()
	{
		HowToPlayScreenPanel.SetActive (false);

		if (!CharcterSeletion.instance.IsCharacterSelection) {
			UserSettingScript.instance.ObjCharCamera.SetActive (true);
			SoundManagerScript.instance.OnPlayButtonClickSound (); 
			SoundManagerScript.instance.OnPlayButtonClickSound ();
		} else {
			UserSettingScript.instance.OnDisabledUIView ();
			SoundManagerScript.instance.OnPlayPlayingSound ();
			SoundManagerScript.instance.OnPlayButtonClickSound (); 
			RandomPlacement.instance.ChestInsertObject.SetActive (true);
		}
		CharcterSeletion.instance.IsCharacterSelection = false;
		 

	}
}
