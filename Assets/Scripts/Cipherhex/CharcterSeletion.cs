using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


public class CharcterSeletion : MonoBehaviour
{
	public static CharcterSeletion instance;
	public GameObject[] ObjCharacter;
	public GameObject CharacerSelectionPanel, MessagePopup, MessageSubPopup;
	public int index = 0;
	public GameObject CharCam;
	public InputField txtUserName;
	internal bool IsCharacterSelection;
	public GameObject MainCamera, OptionalCamera;


	void Awake ()
	{
		instance = this;
		CharacerSelectionPanel.SetActive (false);
		MessagePopup.SetActive (false);
		MessageSubPopup.SetActive (false);
		CharCam.SetActive (false);
		Constants.CharSelected = "Female";
		if (PlayerPrefs.HasKey ("SelectedChar")) {
			Constants.CharSelected = PlayerPrefs.GetString ("SelectedChar");
		}
		if (PlayerPrefs.HasKey ("Username")) {
			Constants.Username = PlayerPrefs.GetString ("Username");
		}
	}

	void HideSubPanel ()
	{
		MessageSubPopup.SetActive (false);
	}

	public void OnClosePopupButtonClick ()
	{
		Invoke ("HideSubPanel", 0.59f);
		SoundManagerScript.instance.OnPlaySlideSound ();
		AnimationScript.Inst.OnMoveUpAnimation (null, MessagePopup, MessageSubPopup, false, 0.359f);
	}

	public void OnSubmitButtonClick ()
	{
//		CharCam.SetActive (false);
//		CharacerSelectionPanel.SetActive (false);
//		PlayingScript.instance.PlayingMenuPanel.SetActive (true);
//		PlayingScript.instance.PlayingMenuPanel.transform.SetAsLastSibling ();

		if (txtUserName.text.Trim (' ').Length > 0) {
			PreloaderScript.instance.OnEnabledLoderLogin ();
			Constants.Username = txtUserName.text;
			PlayerPrefs.SetString ("Username", Constants.Username);
			if (index >= ObjCharacter.Length) {
				index = 0;
			}
			if (index == 0) {
				PlayerPrefs.SetString ("SelectedChar", "Female");
				Constants.CharSelected = "Female";
				Constants.gender = "F";
			} else {
				PlayerPrefs.SetString ("SelectedChar", "Male");
				Constants.CharSelected = "Male";
				Constants.gender = "M";
			}
			LoginScript.instance.OnAutoLogin ();
		

		} else {
			MessageSubPopup.SetActive (true);
			SoundManagerScript.instance.OnPlaySlideSound ();
			AnimationScript.Inst.OnMoveUpAnimation (null, MessagePopup, MessageSubPopup, true, 0.59f);
		}
	}

	internal void OnShowCharacterPanel ()
	{
		index = 0;
		PlayerPrefs.SetString ("SelectedChar", "Female");
		Constants.CharSelected = "Female";
		CharCam.SetActive (true);
		OnHideCharacter ();
		ObjCharacter [index].SetActive (true);
	}

	internal void OnLoginSucess ()
	{
		OnHideCharacter ();
	
		ObjCharacter [index].SetActive (true);
		MainGameController.instance.OnButtonClick ();
		CharracterSellection.instance.ConfirmSellection ();
		//			MainCamera.GetComponent<Camera> ().enabled = true;
		//			OptionalCamera.GetComponent<Camera> ().enabled = false;
		CharCam.SetActive (false);
		CharacerSelectionPanel.SetActive (false);
		PlayingScript.instance.PlayingMenuPanel.SetActive (true);
		PlayingScript.instance.PlayingMenuPanel.transform.SetAsLastSibling ();
		PlayingScript.instance.OnPlayingScreenDisplay ();
		HowToPlayScript.instance.OnHowTplayOpen ();
		UserSettingScript.instance.OnEnabledUIView ();
		SoundManagerScript.instance.OnPlayButtonClickSound ();
		SplashScreen.instance.SplashScreenPanel.SetActive (false);
		IsCharacterSelection = true;
	}

	internal void OnHideCharacter ()
	{
		foreach (GameObject obj in ObjCharacter) {
			obj.SetActive (false);
		}
	}

	public void OnNextButtonClick ()
	{
//		if (txtUserName.text.Trim (' ').Length > 0) {

//			Constants.Username = txtUserName.text;
//			PlayerPrefs.SetString("Username",txtUserName.text);
		SoundManagerScript.instance.OnPlayButtonClickSound ();
		OnHideCharacter ();
		index++;
		if (index >= ObjCharacter.Length) {
			index = 0;
		}
		if (index == 0) {
			PlayerPrefs.SetString ("SelectedChar", "Female");
			Constants.CharSelected = "Female";
		} else {
			PlayerPrefs.SetString ("SelectedChar", "Male");
			Constants.CharSelected = "Male";
		}
		ObjCharacter [index].SetActive (true);
//			MainGameController.instance.OnButtonClick ();
//			CharracterSellection.instance.ConfirmSellection ();
//		} else {
//			
//		}
	}

	public void OnPreviousButtonClick ()
	{
		SoundManagerScript.instance.OnPlayButtonClickSound ();
		OnHideCharacter ();
		index--;
		if (index < 0) {
			index = ObjCharacter.Length - 1;
		}
		ObjCharacter [index].SetActive (true);
		if (index == 0) {
			PlayerPrefs.SetString ("SelectedChar", "Female");
			Constants.CharSelected = "Female";
		} else {
			PlayerPrefs.SetString ("SelectedChar", "Male");
			Constants.CharSelected = "Male";
		}
	}
}
