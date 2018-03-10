using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


public class WalletScript : MonoBehaviour
{

	public static WalletScript instance;
	public GameObject WalletPanel, WalletEditPanel, WalletEditSubPanel;
	public GameObject WalletWithdrawDislinePanel, WalletWithdrawDislineSubPanel;
	public GameObject WalletWithdrawAcceptedPanel, WalletWithdrawAcceptedSubPanel;
	
	public Text txterc20Number, txtemail, txtCHCoin;
	internal string strEthAddress;
	public InputField txtEditERC20Number, txtEditEmail, txtWithdrawAmount;

	void Awake ()
	{
		instance = this;
		WalletPanel.SetActive (false);
		WalletEditPanel.SetActive (false);
		WalletWithdrawDislinePanel.SetActive (false);
		WalletWithdrawAcceptedPanel.SetActive (false);
	}

	internal void OnWalletopen ()
	{
		WalletPanel.SetActive (true); 
		WalletPanel.transform.SetAsLastSibling ();
		txtemail.text = Constants.emailid;
		txterc20Number.text = Constants.RC20Number;
		txtWithdrawAmount.text = "";  
		SoundManagerScript.instance.OnPlayButtonClickSound ();
		txtCHCoin.text = string.Format ("{0:0}", float.Parse (Constants.CH_Balance));
	}

	public void OnShowWalletEditPopup ()
	{
		txtEditEmail.text = "";
		txtEditERC20Number.text = "";
		AnimationScript.Inst.OnMoveUpAnimation (null, WalletEditPanel, WalletEditSubPanel, true, 0.359f);
		SoundManagerScript.instance.OnPlaySlideSound ();
	}

	public void OnWithdrawButtonClick ()
	{
		float value;
//		if (txtWithdrawAmount.text.Trim (' ') = "") {
//			AnimationScript.Inst.OnMoveUpAnimation (null,WalletWithdrawDislinePanel,WalletWithdrawDislineSubPanel,true,0.359f);
//		}
		if (!float.TryParse (txtWithdrawAmount.text, out value)) {
			AnimationScript.Inst.OnMoveUpAnimation (null, WalletWithdrawDislinePanel, WalletWithdrawDislineSubPanel, true, 0.359f);
		 
		} else {
			AnimationScript.Inst.OnMoveUpAnimation (null, WalletWithdrawDislinePanel, WalletWithdrawDislineSubPanel, true, 0.359f);
		}
		SoundManagerScript.instance.OnPlaySlideSound ();
//		OnCloseWalletPanel ();

//		else if (float.Parse (txtWithdrawAmount.text) >= 100) {
//			AnimationScript.Inst.OnMoveUpAnimation (null, WalletWithdrawAcceptedPanel, WalletWithdrawAcceptedSubPanel, true, 0.359f);
//		}
	}

	public void OnSubmitButtonClick ()
	{ 
		PreloaderScript.instance.OnEnabledLoder ();
		Cipherhex_WebSocket.instance.OnSendEthAddress (txtEditERC20Number.text);
		txtemail.text = txtEditEmail.text;
		txterc20Number.text = txtEditERC20Number.text;
		OnCloseEditPopup ();
	}

	public void OnCloseEditPopup ()
	{
		AnimationScript.Inst.OnMoveUpAnimation (null, WalletEditPanel, WalletEditSubPanel, false, 0.359f);
		SoundManagerScript.instance.OnPlaySlideSound ();
	}

	public void OnCloseWalletPanel ()
	{
		WalletPanel.SetActive (false);
		UserSettingScript.instance.ObjCharCamera.SetActive (true);
		SoundManagerScript.instance.OnPlayButtonClickSound ();
	}

	public void OnCloseWithdrawDisliedPopup ()
	{
//		Invoke("OnCloseWalletPanel",0.359f);
		AnimationScript.Inst.OnMoveUpAnimation (null, WalletWithdrawDislinePanel, WalletWithdrawDislineSubPanel, false, 0.359f);
		SoundManagerScript.instance.OnPlaySlideSound ();
	}

	public void OnCloseWithdrawAcceptedPopup ()
	{
//		Invoke("OnCloseWalletPanel",0.359f);	
		AnimationScript.Inst.OnMoveUpAnimation (null, WalletWithdrawAcceptedPanel, WalletWithdrawAcceptedSubPanel, false, 0.359f);
		SoundManagerScript.instance.OnPlaySlideSound ();
	}
}
