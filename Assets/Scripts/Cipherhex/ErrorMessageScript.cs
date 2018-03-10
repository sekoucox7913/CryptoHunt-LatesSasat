using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


public class ErrorMessageScript : MonoBehaviour
{
	public static ErrorMessageScript instance;
	public GameObject ErrorMessagePanel, ErrorMessageSubPanel;
	public Text txtErrorMessage;

	void Awake ()
	{
		instance = this;
		ErrorMessagePanel.SetActive (false);
	}

	internal void OnShowErrorMessage (string ErrorMessage)
	{
 
		txtErrorMessage.text = ErrorMessage; 
		 
		AnimationScript.Inst.OnMoveUpAnimation (null, ErrorMessagePanel, ErrorMessageSubPanel, true, 0.359f);
	}

	public void OnCloseErrorMessage ()
	{
		  
		AnimationScript.Inst.OnMoveUpAnimation (null, ErrorMessagePanel, ErrorMessageSubPanel, false, 0.359f);
	}
}
