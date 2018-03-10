using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class ChestCoinScript : MonoBehaviour
{

	public void OnMouseUp ()
	{
		 
		iTween.MoveTo (this.gameObject, iTween.Hash ("y", this.gameObject.transform.position.y + Screen.height, "time", 1.501f, "easetype", iTween.EaseType.linear));
		Invoke ("OnHideCoin", 1.501f);
	}

	void OnHideCoin ()
	{
		gameObject.SetActive (false);
	}
}
