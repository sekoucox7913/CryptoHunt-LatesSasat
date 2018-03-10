using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoreScript : MonoBehaviour {

	public static StoreScript instance;
	public GameObject StorePanel,StoreSubPanel;


	void Awake()
	{
		instance = this;
	}
	internal void OnStoreOpen()
	{
		StorePanel.SetActive (true);
		StorePanel.transform.SetAsLastSibling ();
		AnimationScript.Inst.OnMoveUpAnimation (null,StorePanel,StoreSubPanel,true,0.35f);
		SoundManagerScript.instance.OnPlaySlideSound ();
	}
	public void OnStoreCloseButtonClick()
	{
		AnimationScript.Inst.OnMoveUpAnimation (null,StorePanel,StoreSubPanel,false,0.35f);
//		Invoke ("OnCloseStore", 0.359f);
		SoundManagerScript.instance.OnPlaySlideSound ();
	}
	void OnCloseStore()
	{
		
	}
}
