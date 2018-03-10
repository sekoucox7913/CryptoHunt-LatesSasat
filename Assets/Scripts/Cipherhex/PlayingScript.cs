using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


public class PlayingScript : MonoBehaviour
{
	public static PlayingScript instance;
	public GameObject PlayingMenuPanel;
	public GameObject[] BtnMenu;
	internal Vector3[] BtnMenuPosition;
	float AnimationTime = 0.359f;
	bool IsMenuOpen;
	public GameObject MaleCharacter, FemaleCharacter;
	public Text txtUserName, txtCHCoin;
	public string strButtonClickName;
	//public Image compasNeedle;
	void Awake ()
	{
		instance = this;
		PlayingMenuPanel.SetActive (false);
		Invoke ("OnGetMenuPosition", 0.1f);
	}

	internal void OnPlayingScreenDisplay ()
	{
		PlayingMenuPanel.SetActive (true);
		PlayingMenuPanel.transform.SetAsLastSibling ();
		SplashScreen.instance.SplashScreenPanel.SetActive (false);
		if (Constants.CharSelected == "Male") {
			MaleCharacter.SetActive (true);
			FemaleCharacter.SetActive (false);
		} else {
			MaleCharacter.SetActive (false);
			FemaleCharacter.SetActive (true);
		}
		txtUserName.text = Constants.Username;
		UserSettingScript.instance.OnDisabledUIView ();
		SoundManagerScript.instance.OnPlayPlayingSound ();
		RandomPlacement.instance.ChestInsertObject.SetActive (true);
	}

	void OnGetMenuPosition ()
	{
		BtnMenuPosition = new Vector3[BtnMenu.Length];
		for (int i = 0; i < BtnMenu.Length; i++) {
			BtnMenuPosition [i] = BtnMenu [i].transform.localPosition;
		}
		for (int i = 1; i < BtnMenu.Length; i++) {
			BtnMenu [i].transform.localPosition = BtnMenu [0].transform.localPosition;
			BtnMenu [i].SetActive (false);
		}
	}

	IEnumerator HideMenuItem (GameObject BtnObj)
	{
		yield return new WaitForSeconds (AnimationTime + 0.1f);
		BtnObj.SetActive (false);

	}

	void Update ()
	{
		//compasNeedle.transform.localRotation = Quaternion.Euler(-0, 0, Input.compass.magneticHeading);
	}

	public void OnMenuButtonClick ()
	{
		if (IsMenuOpen) {
			IsMenuOpen = false;
			for (int i = 1; i < BtnMenu.Length; i++) {
				iTween.MoveTo (BtnMenu [i], iTween.Hash ("x", BtnMenuPosition [0].x, "time", AnimationTime, "islocal", true, "easetype", iTween.EaseType.linear));
				StartCoroutine (HideMenuItem (BtnMenu [i]));
			}
		} else {
			IsMenuOpen = true;
			for (int i = 1; i < BtnMenu.Length; i++) {
				StopAllCoroutines ();
				iTween.MoveTo (BtnMenu [i], iTween.Hash ("x", BtnMenuPosition [i].x, "time", AnimationTime, "islocal", true, "easetype", iTween.EaseType.linear));
//				BtnMenu [i].transform.localPosition = BtnMenu [0].transform.localPosition;
				BtnMenu [i].SetActive (true);
			}
		}
		SoundManagerScript.instance.OnPlaySlideSound ();
	}

	public void OnTaskButtonClick ()
	{
		strButtonClickName = "task";
		TaskScreenScript.instance.OnOpenTaskPanel ();

		SoundManagerScript.instance.OnPlayButtonClickSound ();
		SoundManagerScript.instance.InMenuClicked ();
		PreloaderScript.instance.OnEnabledLoder ();
		Cipherhex_WebSocket.instance.OnGetInventoryDetail ();
		Cipherhex_WebSocket.instance.OnGetQuestDetails ();
		RandomPlacement.instance.ChestInsertObject.SetActive (false);
	}

	public void OnUserMenuButtonClick ()
	{
		UserSettingScript.instance.OnSettingPanelOpen ();
		UserSettingScript.instance.UserSettingsPanel.SetActive (true);
		UserSettingScript.instance.UserSettingsPanel.transform.SetAsLastSibling ();
		SoundManagerScript.instance.OnPlayButtonClickSound ();
		Cipherhex_WebSocket.instance.OnGetInventoryDetail ();
		RandomPlacement.instance.ChestInsertObject.SetActive (false);
	}

	public void OnStoreButtonClick ()
	{
		StoreScript.instance.OnStoreOpen ();
		SoundManagerScript.instance.OnPlayButtonClickSound ();
		SoundManagerScript.instance.OnPlaySlideSound ();
	}

	public void OnLocationButtonClick ()
	{
		strButtonClickName = "location";
		if (PlayerPrefs.HasKey ("Quest_Status")) {
			if (PlayerPrefs.GetString ("Quest_Status") == "1") {
				ErrorMessageScript.instance.OnShowErrorMessage ("Please solve the riddle first. Check your task.");
			} else {
				Cipherhex_WebSocket.instance.OnGetLocationList ();
			}
		} else {
			Cipherhex_WebSocket.instance.OnGetQuestDetails ();
		}

//		LocationScript.instance.IsLocationFind = false;
//		LocationScript.instance.OnLocationPanelOpen ();

	}

	public void OnLeaderboardButtonClick ()
	{
		
	}

	public void OnMakeChestOnEnvironment (JSONObject data)
	{
		JSONObject adata = data.GetField ("adata"); 
//		RandomPlacement.instance.OnRemoveAllChest ();
		if (adata.Count > 0) {
			for (int i = 0; i < adata.Count; i++) { 
				string chestid = adata [i].GetField ("chest_id").ToString ().Trim ('"'); 
				string freeze = adata [i].GetField ("freeze").ToString ().Trim ('"');
				string graphics = adata [i].GetField ("graphics").ToString ().Trim ('"'); 
				string lat = adata [i].GetField ("lat").ToString ().Trim ('"');
				string lng = adata [i].GetField ("lng").ToString ().Trim ('"'); 
				RandomPlacement.instance.OnMakeChestOnLocation (chestid, double.Parse (lat), double.Parse (lng), float.Parse (freeze)); 
			}
		}
	}
}
