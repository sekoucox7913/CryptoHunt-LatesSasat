using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


public class RegisterScript : MonoBehaviour 
{
	public static RegisterScript instance;
	public GameObject RegisterPanel,MessagePopup,MessageSubPopup;
	public Toggle TglTermsOfService, TglPrivacyPolicy, TglDisclamer;
	public InputField txtemail;
	public Text txtmsg;
	public GameObject ScrollPanel;
	public GameObject[] ContainPanels;

	void Awake()
	{
		instance = this;
		RegisterPanel.SetActive (false);
		MessagePopup.SetActive (false);
		for (int i = 0; i < ContainPanels.Length; i++) {
			ContainPanels [i].SetActive (false);
		}
		ContainPanels [2].SetActive (true);
	}

	public void OnRegisterButtonClick()
	{
		if (TglDisclamer.isOn && TglPrivacyPolicy.isOn && TglTermsOfService.isOn) {
//			if (txtemail.text.Trim (' ').Length == 0) {
//				txtmsg.text = "\nPlease enter email id.\n ";	
//				AnimationScript.Inst.OnMoveUpAnimation (null,MessagePopup,MessageSubPopup,true,0.359f);
//				SoundManagerScript.instance.OnPlaySlideSound ();
//			}
//			else
//			{
				RegisterPanel.SetActive (false);
				CharcterSeletion.instance.CharacerSelectionPanel.SetActive (true);
				CharcterSeletion.instance.OnShowCharacterPanel ();

				PlayerPrefs.SetString ("email", txtemail.text.ToString ());
				SoundManagerScript.instance.OnPlayButtonClickSound();
				SoundManagerScript.instance.OnCharacterSelectSound ();
//			}
		} else {
			SoundManagerScript.instance.OnPlaySlideSound ();
			txtmsg.text = "IN ORDER TO PROCEED,\nPLEASE AGREE TO ALL\nTHE TERMS";
			AnimationScript.Inst.OnMoveUpAnimation (null,MessagePopup,MessageSubPopup,true,0.359f);
		}

		//			else if (!Constants.ValidateEmailAddress (txtemail.text)) {
		//				txtmsg.text = "\nPlease enter valid email.\n  ";
		//				AnimationScript.Inst.OnMoveUpAnimation (null,MessagePopup,MessageSubPopup,true,0.359f);
		//				SoundManagerScript.instance.OnPlaySlideSound ();
		//			} 
	}

	public void OnClosePopupButtonClick()
	{
		SoundManagerScript.instance.OnPlaySlideSound ();
		AnimationScript.Inst.OnMoveUpAnimation (null,MessagePopup,MessageSubPopup,false,0.359f);
		SoundManagerScript.instance.OnPlayButtonClickSound ();
	}
	public void OnContainButtonClick(int index)
	{
		for (int i = 0; i < ContainPanels.Length; i++) {
			ContainPanels [i].SetActive (false);
		}
		ContainPanels [index].SetActive (true);
		ScrollPanel.GetComponent<ScrollRect> ().content = ContainPanels [index].GetComponent<RectTransform>();
		ScrollPanel.GetComponent<ScrollRect> ().verticalScrollbar.value = 1;
		SoundManagerScript.instance.OnPlayButtonClickSound();
	}
	public void OnToggleChange()
	{
		SoundManagerScript.instance.OnToggleButtonClick ();
	}
}
