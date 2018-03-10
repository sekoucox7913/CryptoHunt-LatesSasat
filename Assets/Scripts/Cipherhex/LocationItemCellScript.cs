using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationItemCellScript : MonoBehaviour {

	public void OnItemButtonClick()
	{
		iTween.MoveTo (gameObject,iTween.Hash("x",-LocationScript.instance.screenwidth,"islocal",true,"time",0.5f, "easetype",iTween.EaseType.linear));
		SoundManagerScript.instance.OnPlaySlideSound ();
		if (gameObject.name == "ItemCoin") { 
			Invoke ("OnHideItemPanel", 0.5f);
		} else {
			LocationScript.instance.OnGenerateItems ();
		}
		Invoke ("OnDestroy",0.5f);
	}
	void OnDestroy()
	{
		Destroy (this.gameObject);
	}
	void OnHideItemPanel()
	{
		LocationScript.instance.ItemTransform.SetActive(false);
	}
}
