using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


public class LocationScript : MonoBehaviour
{
	public static LocationScript instance;
	public GameObject LocationPanel, LocationSubPanel;
	public GameObject ItemMessagePanel, ItemTrueAnsMessage, ItemFalseAnsMessage;
	internal int LocationIndex = 0, ItemIndex;
	internal bool IsLocationFind = false;
	public GameObject Item, ItemTransform;
	internal ArrayList itemArray;
	public Sprite ImgCoin;
	internal float screenwidth;
	internal int[] Id = new int[4], location_type_id = new int[4];
	internal ArrayList LocationListArray;
	public string[] txtLocationName = new string[3];
	public Text[] txtLocationDescription;
	internal ArrayList list = new ArrayList ();

	void Awake ()
	{
		instance = this;
		LocationPanel.SetActive (false);
		screenwidth = ItemTransform.GetComponent<RectTransform> ().rect.width;
		ItemTransform.SetActive (false);
	}

	internal void OnLocationPanelOpen (JSONObject data)
	{
		LocationPanel.SetActive (true);
		LocationPanel.transform.SetAsLastSibling ();
		AnimationScript.Inst.OnScalByXYAnimation (null, LocationPanel, LocationSubPanel, true, 0.35f);
		SoundManagerScript.instance.OnPlayButtonClickSound ();
		SoundManagerScript.instance.OnPlaySlideSound ();

		RandomPlacement.instance.ChestInsertObject.SetActive (false);
	}

	public void OnSetLocationData (JSONObject data)
	{
		  

		for (int i = 0; i < data.Count; i++) {
			Id [i] = int.Parse (data [i].GetField ("id").ToString ().Trim ('"'));
			location_type_id [i] = int.Parse (data [i].GetField ("location_type_id").ToString ().Trim ('"'));
			OnSetLocation (location_type_id [i].ToString ());
		}
	}

	public void OnCheckLocationButtonClick (int index)
	{
		Cipherhex_WebSocket.instance.OnGetInteract_LocDetail (Id [index].ToString ());
		Cipherhex_WebSocket.instance.LocationId = Id [index].ToString ();
		PreloaderScript.instance.OnEnabledLoder ();
//		if (index == LocationIndex) {
//			ItemMessagePanel.SetActive (true);
//			ItemTrueAnsMessage.SetActive (true); 
//			ItemFalseAnsMessage.SetActive (false);
//			AnimationScript.Inst.OnMoveUpAnimation (null, ItemMessagePanel, ItemTrueAnsMessage, true, 0.359f);
//			Invoke ("OnCloseLocationPanel", 2.5f);
//			Invoke ("OnClosePoupMessagePanel", 2.5f);
//			Invoke ("OnGenerateItems", 2.5f);
//			ItemIndex = 0;
//			itemArray = new ArrayList ();
//			Cipherhex_WebSocket.instance.OnGetInteract_LocDetail (location_type_id [index].ToString ());
//			SoundManagerScript.instance.OnPlaySlideSound ();
//		} else {
//			ItemMessagePanel.SetActive (true);
//			ItemTrueAnsMessage.SetActive (false); 
//			ItemFalseAnsMessage.SetActive (true);
//			Invoke ("OnCloseLocationPanel", 2.5f);
//			Invoke ("OnClosePoupMessagePanel", 2.5f);
//			AnimationScript.Inst.OnMoveUpAnimation (null, ItemMessagePanel, ItemFalseAnsMessage, true, 0.359f);
//			SoundManagerScript.instance.OnPlaySlideSound ();
//		}
	}

	public void OnCloseLocationPanel ()
	{
		AnimationScript.Inst.OnScalByXYAnimation (null, LocationPanel, LocationSubPanel, false, 0.35f);
		SoundManagerScript.instance.OnPlaySlideSound ();
		SoundManagerScript.instance.OnPlayButtonClickSound ();
		if (!TaskScreenScript.instance.TaskPanel.activeSelf) {
			RandomPlacement.instance.ChestInsertObject.SetActive (true);
		}
	}

	public void OnClosePoupMessagePanel ()
	{
		if (ItemTrueAnsMessage.activeSelf) {
			AnimationScript.Inst.OnMoveUpAnimation (null, ItemMessagePanel, ItemTrueAnsMessage, false, 0.359f);
			SoundManagerScript.instance.OnPlaySlideSound ();
			SoundManagerScript.instance.OnPlayButtonClickSound ();
		} else if (ItemFalseAnsMessage.activeSelf) {
			AnimationScript.Inst.OnMoveUpAnimation (null, ItemMessagePanel, ItemFalseAnsMessage, false, 0.359f);
			SoundManagerScript.instance.OnPlaySlideSound ();
			SoundManagerScript.instance.OnPlayButtonClickSound ();
		}

	}

	public void OnGenerateItems ()
	{
		GameObject obj = Instantiate (Item) as GameObject;
		obj.transform.SetParent (ItemTransform.transform, false);
		obj.GetComponent<RectTransform> ().localScale = Vector3.one;
		Vector3 posion = obj.GetComponent<RectTransform> ().localPosition;
		posion.x = screenwidth;
		obj.GetComponent<RectTransform> ().localPosition = posion; 
		ItemIndex++;
		if (ItemIndex < list.Count) {
			print ("===================== --------------------- " + list [ItemIndex]); 
			int itemNo = int.Parse ((string)list [ItemIndex]);
			if (itemNo > 0) { 
				TaskScreenScript.instance.OnSetImages (itemNo.ToString (), obj.GetComponent<Image> ()); 
				obj.name = itemNo.ToString ();
			 
			} else {
				obj.GetComponent<Image> ().sprite = ImgCoin; 
				if (ItemIndex >= list.Count) {
					obj.name = "ItemCoin";
				}
			}
		}


	}

	internal void OnSetImages (string id, Image img)
	{
		if (TaskScreenScript.instance.AllItemArray == null) {
			TaskScreenScript.instance.AllItemArray = new ArrayList ();
		}

		for (int i = 0; i < TaskScreenScript.instance.AllItemArray.Count; i++) {
			JSONObject jdata = TaskScreenScript.instance.AllItemArray [i] as JSONObject;
			if (jdata.GetField ("id").ToString ().Trim ('"') == id) {
				img.name = id; 
				StartCoroutine (LoadImg (jdata.GetField ("graphics").ToString ().Trim ('"'), img));
				//				StartCoroutine (OnSetImage (id, jdata.GetField ("graphics").ToString ().Trim ('"'), img));
			}
		}
	}

	public IEnumerator LoadImg (string imageurl, Image img)
	{
		Texture2D texture = img.sprite.texture;
		imageurl = imageurl.Replace (@"\", "");
		if (!imageurl.Contains ("https://") && !imageurl.Contains ("http://") && imageurl.Length >= 5) {
			imageurl = " http://s3-eu-west-1.amazonaws.com/ch-game-items/200x200/" + imageurl;
			//ws://games.cipherhex.com:3000/socket.io/?EIO=4&transport=websocket
		}
		WWW www = new WWW (imageurl);
		yield return www;
		if (texture != null && img != null && www.texture != null) {
			if (www.error == "Null" || www.error == "null" || www.error == null) {
				texture = www.texture;
				Rect rect = new Rect (0, 0, texture.width, texture.height);
				Sprite sprite = Sprite.Create (texture, rect, new Vector2 (0.5f, 0.5f));
				img.sprite = sprite;
				iTween.MoveTo (img.gameObject, iTween.Hash ("x", 0, "islocal", true, "time", 0.5f, "easetype", iTween.EaseType.linear));
				ItemTransform.SetActive (true);
				ItemTransform.transform.SetAsLastSibling ();
			}

		}
	}

	internal void onGetLocationListResponse (JSONObject data)
	{
		//		print ("items_all  " + data.Print ());
		if (LocationListArray == null) {
			LocationListArray = new ArrayList ();
		}
		JSONObject jdata = data.GetField ("adata");
		 
		if (LocationListArray.Count != jdata.Count) {
			for (int i = 0; i < jdata.Count; i++) {
				JSONObject adata = jdata [i];
				LocationListArray.Add (adata);

			}
		} 

	}

	internal void OnSetLocation (string Locationid)
	{
		if (LocationListArray == null) {
			LocationListArray = new ArrayList ();
		}
	 
		for (int i = 0; i < LocationListArray.Count; i++) { 
			JSONObject data = LocationListArray [i] as JSONObject;
			if (data.GetField ("id").ToString ().Trim ('"') == Locationid) {
				if (i < txtLocationName.Length) {
					txtLocationName [i] = data.GetField ("name").ToString ().Trim ('"');
					txtLocationDescription [i].text = data.GetField ("description").ToString ().Trim ('"');
				}

			}

		}
	}

	public void OnSuffleButtonClick ()
	{
		PreloaderScript.instance.OnEnabledLoder ();
		Cipherhex_WebSocket.instance.OnGetLocation_ListSuffleDetail ();
	}

	public void OnInteract_locationCollectResponse (JSONObject data)
	{
		ItemIndex = -1;
		list = new ArrayList ();
		JSONObject adata = data.GetField ("adata");

		for (int i = 0; i < adata.Count; i++) {
			list.Add (adata [i].GetField ("item_id").ToString ().Trim ('"'));
		}
		int count = int.Parse (data.GetField ("data").GetField ("ch_reward").ToString ().Trim ('"'));
		for (int i = 0; i < count; i++) {
			list.Add ("0");
		}

		OnCloseLocationPanel ();
		OnGenerateItems ();
	}


}

