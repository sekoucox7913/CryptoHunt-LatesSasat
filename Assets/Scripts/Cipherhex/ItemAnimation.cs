using GoShared;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

//using System.Device.Location;

public class ItemAnimation : MonoBehaviour
{

	internal bool IsAnimated, IsRotation;
	public float DelayTime, AnimationTIme, AnimationOverTime;
	internal Vector3 ItemPosition, OldPosition;
	public GameObject Itemcamera, ItemChest, ItemCoin;
	public string ChestId;
	public float FreezeTime;
	public bool IsInteractable;
	public int waitForSec;
	internal Vector2 ChestLocation;

	void Awake ()
	{
		OldPosition = ItemCoin.transform.localPosition;
		Itemcamera = GameObject.FindGameObjectWithTag ("MainCamera");
		InvokeRepeating ("CountDownTime", 1f, 1f);
	}

	void ResetCoin ()
	{
		IsAnimated = false;
		ItemCoin.transform.localScale = new Vector3 (0.3f, 0.3f, 0.3f);
		ItemCoin.transform.localPosition = OldPosition;
		iTween.MoveTo (ItemCoin, iTween.Hash ("position", OldPosition, "time", 0.01f, "islocal", true, "easetype", iTween.EaseType.linear));
		if (RandomPlacement.instance.AnimationArray.Contains (ChestId)) {
			RandomPlacement.instance.AnimationArray.Remove (ChestId);
		}

		gameObject.SetActive (false);
		IsInteractable = true;
	}

	internal void OnStartCoinAnimation ()
	{
		//		Invoke ("StartAnimation", DelayTime);
		StartAnimation ();
		Invoke ("ResetCoin", 15f);
	}

	void StartAnimation ()
	{ 
		IsRotation = true;
		Invoke ("StopRotation", AnimationTIme);
 
		iTween.MoveTo (ItemCoin, iTween.Hash ("position", ItemPosition, "time", AnimationTIme, "easetype", iTween.EaseType.linear));
		iTween.ScaleTo (ItemCoin, iTween.Hash ("x", 5, "y", 5, "z", 5, "time", AnimationTIme, "easetype", iTween.EaseType.linear));
	}

	void Update ()
	{
		if (IsAnimated) {
			if (IsRotation) {
				ItemCoin.transform.Rotate (10, 17, 10); 
			} else {
				//				ItemCoin.transform.LookAt (Itemcamera.transform);
				iTween.LookTo (ItemCoin, Itemcamera.transform.position, 0.5f);

			} 
		}  
	}

	void StopRotation ()
	{
		IsRotation = false;
	}

	public void StartCoinAnimation ()
	{
		//		Itemcamera = MainGameController.instance.cam1;
		if (!IsAnimated && IsInteractable) {
			if (!UserSettingScript.instance.TglAR.isOn) { 
				ItemPosition = Dashboard.instance.ObjectForCoin.transform.position;
			} else {
				ItemPosition = MainGameController.instance.ObjForCoin.transform.position;
			}
			Cipherhex_WebSocket.instance.Chest = this.gameObject;
			ItemCoin.SetActive (true);
			SoundManagerScript.instance.OnPlayMagicSound (); 
			gameObject.GetComponent<Animator> ().SetTrigger ("chestopen");
			IsAnimated = true;    
			OldPosition = ItemCoin.transform.position; 

		}
	}

	public void OnMouseUp ()
	{  
		if (IsInteractable) {
			Cipherhex_WebSocket.instance.Chest = this.gameObject; 
//			GeoCoordinate MyCurrentLocation = new GeoCoordinate(0,0);
//			GeoCoordinate NewChestLocation = new GeoCoordinate((double)ChestLocation.x,(double)ChestLocation.y);
//
//			if (LocationManager.CenterWorldCoordinates != null) {
//				if (LocationManager.CenterWorldCoordinates.latitude != null) { 
//					Cipherhex_WebSocket.instance.ChestId = ChestId;
//					Cipherhex_WebSocket.instance.Chest = this.gameObject;
//					MyCurrentLocation = new GeoCoordinate(LocationManager.CenterWorldCoordinates.latitude,LocationManager.CenterWorldCoordinates.longitude); 
//					print ("asdasdasd   " + MyCurrentLocation.GetDistanceTo(NewChestLocation));
//				}
//			}
//			if (MyCurrentLocation.GetDistanceTo (NewChestLocation) <= 200) {
//				print ("aaaaaaaaaaa" + MyCurrentLocation.GetDistanceTo (NewChestLocation));
			if (!UserSettingScript.instance.TglAR.isOn) { 
				if (!IsAnimated) { 
					ItemPosition = Dashboard.instance.ObjectForCoin.transform.position;
						
					Cipherhex_WebSocket.instance.Chest = this.gameObject;
//						StartCoinAnimation (); 
					OnChestOpenServiceCall ();
				}
			} else { 
				if (MainGameController.instance.CloseContainer.activeSelf) {  
					ItemPosition = MainGameController.instance.ObjForCoin.transform.position;
//						StartCoinAnimation (); 
					Cipherhex_WebSocket.instance.Chest = this.gameObject;
					OnChestOpenServiceCall ();
				} else {  
					ItemPosition = MainGameController.instance.ObjForCoin.transform.position;
					MainGameController.instance.OnChestOnAR (this.gameObject);	
				}
			}
//			}

		}
	}

	internal void OnChestOpenServiceCall ()
	{
		JSONObject jdata = new JSONObject (); 
		if (Constants.IsSocketConnected) {
			if (LocationManager.CenterWorldCoordinates != null) {
				if (LocationManager.CenterWorldCoordinates.latitude != null) { 
					Cipherhex_WebSocket.instance.ChestId = ChestId;
					Cipherhex_WebSocket.instance.Chest = this.gameObject;
					jdata.AddField ("lat", LocationManager.CenterWorldCoordinates.latitude.ToString ());
					jdata.AddField ("lng", LocationManager.CenterWorldCoordinates.longitude.ToString ());
					jdata.AddField ("chest_id", ChestId);
					print ("ChestId On Touch" + ChestId);
					Cipherhex_WebSocket.instance.SendMessage (jdata, "interact_chest/open/try");

				}
			}
		}
	}

	internal void OnStartTImer (float waitForSecTIme)
	{
		//		waitForSec = (int)waitForSecTIme;
		 
		if (waitForSecTIme > 0) {
			IsInteractable = false;
//			CancelInvoke ("CountDownTime");
			this.gameObject.SetActive (false);
//			Invoke ("CountDownTime", waitForSec + 2);
		} else {
			IsInteractable = true;
			this.gameObject.SetActive (true);
		}
	}

	public void CountDownTime ()
	{ 

		if (waitForSec == 0) {
			IsInteractable = true;
			this.gameObject.SetActive (true);
			CancelInvoke ("CountDownTime");
		} else if (waitForSec >= 0) {
			waitForSec--;
		}

		 
	}
}

