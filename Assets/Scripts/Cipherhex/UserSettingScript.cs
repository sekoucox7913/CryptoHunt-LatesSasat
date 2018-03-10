using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UserSettingScript : MonoBehaviour
{
	public static UserSettingScript instance;
	public GameObject UserSettingsPanel;

	public GameObject Clouds, GoMap, GoMapCharacter;
	public GameObject ObjCharCamera, ObjMale, ObjFemale;
	public Image ImgCharacter;
	public Sprite ImgFemale, ImgMale;
	public Toggle TglAR, TglSFX, TglSound, TglVibration;
	public Camera MapCamera;
	public AudioSource BackgroundSound;
	public GameObject ARCanera, NormalCamera;
	internal ArrayList SpecialItemKeyArray, SpecialItemValueArray, SpecialItemArray;

	//	public GameObject soundsContainer;

	public GameObject GPSPanel, GPSSubPanel, NoItemMessage;
	public InventoryCellBtn itemPrefeb;
	public GameObject MainPanel, ScrollPanel;

	void Awake ()
	{
		instance = this;
		UserSettingsPanel.SetActive (false);
		ObjCharCamera.SetActive (false);
//		Invoke("OnEnabledUIView",0.01f);
		OnEnabledUIView ();

		FindSoundOnOff ();
	}

	void Start ()
	{
		float GridSize = (MainPanel.GetComponent<RectTransform> ().rect.height) - 5;
		ScrollPanel.GetComponent<GridLayoutGroup> ().cellSize = new Vector2 (GridSize, GridSize);
	}

	internal void OnChangeCamera (bool IsOn)
	{
		ARCanera.SetActive (!IsOn);
		NormalCamera.SetActive (IsOn);
		 
	}

	public void OnCloseSettingButtonClick ()
	{
		ObjCharCamera.SetActive (false);
		UserSettingsPanel.SetActive (false);
		SoundManagerScript.instance.OnPlayButtonClickSound ();
		SoundManagerScript.instance.OnPlayPlayingSound ();
		RandomPlacement.instance.ChestInsertObject.SetActive (true);
	}

	internal void OnSettingPanelOpen ()
	{
		ObjCharCamera.SetActive (true);
		RandomPlacement.instance.ChestInsertObject.SetActive (false);
		if (Constants.CharSelected == "Male") {
//			ImgCharacter.sprite = ImgMale;
			ObjMale.SetActive (true);
			ObjFemale.SetActive (false);
		} else {
//			ImgCharacter.sprite = ImgFemale;
			ObjMale.SetActive (false);
			ObjFemale.SetActive (true);
		}
		SoundManagerScript.instance.InMenuClicked ();
		FindSoundOnOff ();
//		Constants

		if (SpecialItemArray == null) {
			SpecialItemArray = new ArrayList ();
		}
		if (SpecialItemKeyArray == null) {
			SpecialItemKeyArray = new ArrayList ();

		}
		if (SpecialItemValueArray == null) {
			SpecialItemValueArray = new ArrayList ();

		}
		NoItemMessage.SetActive (true);
		if (SpecialItemKeyArray.Count > 0) {
			NoItemMessage.SetActive (false);
		}
		if (SpecialItemArray.Count != SpecialItemKeyArray.Count) {
			OnSetDataOnSpecialItem (SpecialItemKeyArray, SpecialItemValueArray);	 
		} else {
			for (int i = 0; i < SpecialItemArray.Count; i++) {
				InventoryCellBtn cell = SpecialItemArray [i] as InventoryCellBtn;
				cell.txtNoOfCoin.text = (string)SpecialItemValueArray [i];
			}
		}
	}

	internal void OnSetItem ()
	{

		if (SpecialItemArray == null) {
			SpecialItemArray = new ArrayList ();
		}
		if (SpecialItemKeyArray == null) {
			SpecialItemKeyArray = new ArrayList ();

		}
		if (SpecialItemValueArray == null) {
			SpecialItemValueArray = new ArrayList ();

		}
		if (SpecialItemArray.Count != SpecialItemKeyArray.Count) {
			OnSetDataOnSpecialItem (SpecialItemKeyArray, SpecialItemValueArray);	 
		} else {
			for (int i = 0; i < SpecialItemArray.Count; i++) {
				InventoryCellBtn cell = SpecialItemArray [i] as InventoryCellBtn;
				cell.txtNoOfCoin.text = (string)SpecialItemValueArray [i];
			}
		}
	}

	internal void FindSoundOnOff ()
	{
		if (PlayerPrefs.HasKey ("AR")) {
			TglAR.isOn = bool.Parse (PlayerPrefs.GetString ("AR"));  

		}
		if (PlayerPrefs.HasKey ("SFX")) {
			TglSFX.isOn = bool.Parse (PlayerPrefs.GetString ("SFX"));
		}
		if (PlayerPrefs.HasKey ("Sound")) {
			TglSound.isOn = bool.Parse (PlayerPrefs.GetString ("Sound"));

		}  
		if (PlayerPrefs.HasKey ("Vibration")) {
			TglVibration.isOn = bool.Parse (PlayerPrefs.GetString ("Vibration"));
		}
		if (TglSound.isOn) {
			SoundManagerScript.instance.OnAllSoundUnMute ();
		} else {
			SoundManagerScript.instance.OnAllSoundMute ();
		} 
	}


	public void OnCharacterSelectionButtonClick ()
	{
//		if (TglSound.isOn) {
//			BackgroundSound.mute = false;
//		} else {
//			BackgroundSound.mute = true;
//		}
	}

	public void OnWalletButtonClick ()
	{
		WalletScript.instance.OnWalletopen ();
		ObjCharCamera.SetActive (false);
	}

	public void OnHowToPlayButtonClick ()
	{
		CharcterSeletion.instance.IsCharacterSelection = false;
		HowToPlayScript.instance.OnHowTplayOpen ();
		ObjCharCamera.SetActive (false);
		RandomPlacement.instance.ChestInsertObject.SetActive (false);
	}

	public void OnPlatinumButtonClick ()
	{
		PlatinumScript.instance.OnPlatinumPanelOpen ();
		ObjCharCamera.SetActive (false);
	}

	public void OnARChanged ()
	{ 
		PlayerPrefs.SetString ("AR", (TglAR.isOn).ToString ());
		SoundManagerScript.instance.OnToggleButtonClick ();
//		OnChangeCamera(TglAR.isOn);
//		NormalCamera.GetComponent<GyroCamera>().enabled = !TglAR.isOn;
	}

	public void OnSFXChanged ()
	{ 
		PlayerPrefs.SetString ("SFX", (TglSFX.isOn).ToString ());
		SoundManagerScript.instance.OnToggleButtonClick ();
	}

	public void OnSoundChanged ()
	{
		PlayerPrefs.SetString ("Sound", (TglSound.isOn).ToString ());
//		BackgroundSound.mute = !TglSound.isOn;
		SoundManagerScript.instance.OnToggleButtonClick ();
		if (TglSound.isOn) {
			SoundManagerScript.instance.OnAllSoundUnMute ();
		} else {
			SoundManagerScript.instance.OnAllSoundMute ();
		} 
	}

	public void OnVibrationChanged ()
	{
		PlayerPrefs.SetString ("Vibration", (TglVibration.isOn).ToString ());
		SoundManagerScript.instance.OnToggleButtonClick ();
	}

	internal void OnEnabledUIView ()
	{
		Clouds.SetActive (false); 
//		GoMap.SetActive (false);
		MapCamera.farClipPlane = 1;
		GoMapCharacter.SetActive (false);
	}

	internal void OnDisabledUIView ()
	{
		Clouds.SetActive (true); 
//		GoMap.SetActive (true);
		MapCamera.farClipPlane = 1000;
		GoMapCharacter.SetActive (true);
	}

	internal void OnSetDataOnSpecialItem (ArrayList array, ArrayList array1)
	{
		if (SpecialItemArray == null) {
			SpecialItemArray = new ArrayList ();
		}
		if (SpecialItemKeyArray == null) {
			SpecialItemKeyArray = new ArrayList ();

		}
		if (SpecialItemValueArray == null) {
			SpecialItemValueArray = new ArrayList ();

		}
		NoItemMessage.SetActive (true);
		if (array.Count > 0) {
			NoItemMessage.SetActive (false);
		}
		if (SpecialItemArray.Count != array.Count) {
			OnRemoveAllObjects ();
			for (int i = 0; i < array.Count; i++) {
				string id = array [i] as string; 
				InventoryCellBtn cell = Instantiate (itemPrefeb, ScrollPanel.transform, false);
				cell.ItemId = id;
				cell.ItemType = "S";
				cell.txtNoOfCoin.text = (string)array1 [i];
				//			StartCoroutine (cell.LoadImg (url));
				SpecialItemArray.Add (cell); 
				TaskScreenScript.instance.OnSetImages (id, cell.img); 
			}
		}

	}

	internal void OnSetFirstPosition ()
	{
		Vector3 POS = ScrollPanel.GetComponent<RectTransform> ().localPosition;
		POS.x = MainPanel.GetComponent<RectTransform> ().rect.width / 2;
		ScrollPanel.GetComponent<RectTransform> ().localPosition = POS;
	}

	internal void OnRemoveAllObjects ()
	{ 
		if (SpecialItemArray != null) {
			if (SpecialItemArray.Count > 0) {
				for (int i = 0; i < SpecialItemArray.Count; i++) {
					InventoryCellBtn obj = SpecialItemArray [i] as InventoryCellBtn;
					Destroy (obj.gameObject);
				}
			}

		}
		SpecialItemArray = new ArrayList ();
		SpecialItemKeyArray = new ArrayList ();
		SpecialItemValueArray = new ArrayList ();
	}
}
