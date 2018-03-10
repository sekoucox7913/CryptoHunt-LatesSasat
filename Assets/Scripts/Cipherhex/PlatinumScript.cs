using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatinumScript : MonoBehaviour
{

	public static PlatinumScript instance;
	public GameObject PlatinumPanel, PlatinumSubPanel;

	void Awake ()
	{
		instance = this;
	}

	internal void OnPlatinumPanelOpen ()
	{
		PlatinumPanel.SetActive (true);
		PlatinumPanel.transform.SetAsLastSibling ();
		AnimationScript.Inst.OnMoveUpAnimation (null, PlatinumPanel, PlatinumSubPanel, true, 0.35f);
		SoundManagerScript.instance.OnPlaySlideSound ();
		SoundManagerScript.instance.OnPlayButtonClickSound ();
	}

	public void OnPlatinumCloseButtonClick ()
	{
		AnimationScript.Inst.OnMoveUpAnimation (null, PlatinumPanel, PlatinumSubPanel, false, 0.35f); 
		Invoke ("OnClose", 0.35f);
		SoundManagerScript.instance.OnPlayButtonClickSound ();
		SoundManagerScript.instance.OnPlaySlideSound ();

	}

	void OnClose ()
	{
		UserSettingScript.instance.ObjCharCamera.SetActive (true);
	}
}
