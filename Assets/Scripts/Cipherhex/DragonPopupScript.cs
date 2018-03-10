using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonPopupScript : MonoBehaviour 
{
	public static DragonPopupScript instance;
	public GameObject DragonPanel, DragonSubPanel;


	void Awake()
	{
		instance = this;
		DragonPanel.SetActive (false);
	}
	internal void OnDragonPanelOpen()
	{
		DragonPanel.SetActive (true);
		DragonPanel.transform.SetAsLastSibling ();
		AnimationScript.Inst.OnScalByXYAnimation (null,DragonPanel,DragonSubPanel,true,0.35f);
	}
	public void OnCloseButtonClick()
	{
		AnimationScript.Inst.OnScalByXYAnimation (null,DragonPanel,DragonSubPanel,false,0.35f);
	}
}
