using GoMap;
using GoShared;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class TaskScreenScript : MonoBehaviour
{
	public static TaskScreenScript instance;
	public GameObject TaskPanel;
	public GameObject ItemMessagePanel, ItemTrueAnsMessage, ItemFalseAnsMessage;
	public Image ImgCharacter;
	public Sprite ImgFemale, ImgMale;
	public Text txtusername, txtCoinCount;
	public Text txtDescription;
	//	public Image[] BtnImgItem;
	internal int ItemIndex = 9;
	internal ArrayList AllItemArray;
	internal string QuestDescription, QuestGraphics, QuestId, QuestName, QuestType, QuestItemId, QuestStatus;

	// Inventory Item Prefeb and panel

	internal ArrayList InventoryArray;


	/// <summary>
	/// All items detail.
	/// </summary>

	public class AllItemsDetail
	{
		internal string description, graphics, id, name, type;
	}

	void Awake ()
	{
		instance = this;
		TaskPanel.SetActive (false);
		ItemMessagePanel.SetActive (false);
	}


	internal void OnOpenTaskPanel ()
	{
		RandomPlacement.instance.ChestInsertObject.SetActive (false);
		txtCoinCount.text = string.Format ("{0:0}", float.Parse (Constants.CH_Balance));
		txtusername.text = Constants.Username;
		if (Constants.CharSelected == "Male") {
			ImgCharacter.sprite = ImgMale;
		} else {
			ImgCharacter.sprite = ImgFemale;
		}
		//txtName.text = Constants.Username;
		txtusername.text = Constants.Username;
		SoundManagerScript.instance.InMenuClicked ();
	}

	public void OnCloseTaskPanelButtonClick ()
	{
		TaskPanel.SetActive (false); 
		RandomPlacement.instance.ChestInsertObject.SetActive (true);
		SoundManagerScript.instance.OnPlayPlayingSound ();
	}

	public IEnumerator CalledChestItemCollectAuto (string ItemId)
	{
		yield return new WaitForSeconds (0);
		JSONObject jdata = new JSONObject (); 
		if (Constants.IsSocketConnected) {
			if (LocationManager.CenterWorldCoordinates != null) {
				if (LocationManager.CenterWorldCoordinates.latitude != null) { 
					jdata.AddField ("lat", LocationManager.CenterWorldCoordinates.latitude.ToString ());
					jdata.AddField ("lng", LocationManager.CenterWorldCoordinates.longitude.ToString ());
					jdata.AddField ("item_id", ItemId);
					Cipherhex_WebSocket.instance.SendMessage (jdata, "interact_items/givetoprof/try");
					 
				} else {
					StartCoroutine (CalledChestItemCollectAuto (ItemId)); 
				}
			} else {
				StartCoroutine (CalledChestItemCollectAuto (ItemId)); 
			}
		}

	}

	public void OnItemButtonClick (int index)
	{
//		if (!BtnImgItem [index].name.Contains ("Btn")) {
//			
////			print ("aaaaaaaaaaaaaa" + BtnImgItem [index].name.Replace ("Btn", ""));
//		}
//		if (QuestItemId == BtnImgItem[index].name) {
//			ItemMessagePanel.SetActive (true);
//			ItemTrueAnsMessage.SetActive (true); 
//			ItemFalseAnsMessage.SetActive(false);
//			AnimationScript.Inst.OnMoveUpAnimation (null,ItemMessagePanel,ItemTrueAnsMessage,true,0.359f);
//			Invoke ("ShowLocationPanel",3f);
//			Invoke ("OnClosePoupMessagePanel", 2.5f);
//		}
//		else 
//		{
//			ItemMessagePanel.SetActive (true);
//			ItemTrueAnsMessage.SetActive (false); 
//			ItemFalseAnsMessage.SetActive(true);
//			AnimationScript.Inst.OnMoveUpAnimation (null,ItemMessagePanel,ItemFalseAnsMessage,true,0.359f);
//		}
	}

	public void OnSwapItemButtonClick ()
	{
		SwapItemScript.instance.OnSwapItemOpen ();
	}

	public void OnClosePoupMessagePanel ()
	{
		if (ItemTrueAnsMessage.activeSelf) {
			AnimationScript.Inst.OnMoveUpAnimation (null, ItemMessagePanel, ItemTrueAnsMessage, false, 0.359f);
		} else if (ItemFalseAnsMessage.activeSelf) {
			AnimationScript.Inst.OnMoveUpAnimation (null, ItemMessagePanel, ItemFalseAnsMessage, false, 0.359f);
		}
			
	}

	internal void ShowLocationPanel ()
	{
		OnCloseTaskPanelButtonClick ();
		LocationScript.instance.IsLocationFind = true;
//		LocationScript.instance.OnLocationPanelOpen ();
	}

	internal void onGetItemAllResponse (JSONObject data)
	{
 
	 
		JSONObject jdata = data.GetField ("adata");

		if (AllItemArray == null)
			AllItemArray = new ArrayList ();

		for (int i = 0; i < jdata.Count; i++) {
			AllItemArray.Add (jdata [i]);
		}

//		InventoryArray
//		if (data.Count == InventoryArray.Count) {
//			for (int i = 0; i < jdata.Count; i++) {
//				 
//				InventoryCellBtn cell = InventoryArray [i] as InventoryCellBtn;
//				cell.ItemId = jdata [i].GetField ("id").ToString ().Trim ('"');
//				string url = jdata [i].GetField ("graphics").ToString ().Trim ('"');
//				StartCoroutine (cell.LoadImg (url)); 
//			}
//		} else {
//
////			OnRemoveAllObjects ();
//			InventoryArray = new ArrayList ();
//			for (int i = 0; i < jdata.Count; i++) {
// 
//				InventoryCellBtn cell = Instantiate (itemPrefeb, ScrollPanel.transform, false);
//				cell.ItemId = jdata [i].GetField ("id").ToString ().Trim ('"');
//				string url = jdata [i].GetField ("graphics").ToString ().Trim ('"');
//				StartCoroutine (cell.LoadImg (url));
//				InventoryArray.Add (cell);
//			}
//		}
//		OnSetFirstPosition ();

	}

	internal void OnDisplayTaskScreenMessage (JSONObject data)
	{
		
		JSONObject jdata = data.GetField ("data"); 
		QuestDescription = jdata.GetField ("description").ToString ().Trim ('"');
		QuestGraphics = jdata.GetField ("graphics").ToString ().Trim ('"');
		QuestId = jdata.GetField ("id").ToString ().Trim ('"');
		QuestName = jdata.GetField ("name").ToString ().Trim ('"');
		QuestType = jdata.GetField ("type").ToString ().Trim ('"');
		QuestItemId = jdata.GetField ("item_id").ToString ().Trim ('"');
		QuestStatus = jdata.GetField ("status").ToString ().Trim ('"');

		//txtName.text = QuestName;
		txtDescription.text = QuestDescription;

		TaskScreenScript.instance.TaskPanel.SetActive (true);
		TaskScreenScript.instance.TaskPanel.transform.SetAsLastSibling ();
	}

	internal void OnSetImages (string id, Image img)
	{
		if (AllItemArray == null) {
			AllItemArray = new ArrayList ();
		}
		 
		for (int i = 0; i < AllItemArray.Count; i++) {
			JSONObject jdata = AllItemArray [i] as JSONObject;
			if (jdata.GetField ("id").ToString ().Trim ('"') == id) {
				img.name = id; 
				StartCoroutine (Constants.LoadImg (jdata.GetField ("graphics").ToString ().Trim ('"'), img));
//				StartCoroutine (OnSetImage (id, jdata.GetField ("graphics").ToString ().Trim ('"'), img));
			}
		}
	}

	internal string ItemType (string id)
	{
		if (AllItemArray == null) {
			AllItemArray = new ArrayList ();
		}
		string itemType = "";
		for (int i = 0; i < AllItemArray.Count; i++) {
			JSONObject jdata = AllItemArray [i] as JSONObject;
			if (jdata.GetField ("id").ToString ().Trim ('"') == id) { 
				itemType = jdata.GetField ("type").ToString ().Trim ('"');
			}
		}
		return itemType;
	}

	public IEnumerator OnSetImage (string id, string url, Image img)
	{
		Texture2D texture = img.sprite.texture;
		string keyName = url + id;
		url = url.Replace (@"\", "");
		if (!url.Contains ("https://") && !url.Contains ("http://") && url.Length >= 5) {
			url = " http://s3-eu-west-1.amazonaws.com/ch-game-items/200x200/" + url;
			//ws://games.cipherhex.com:3000/socket.io/?EIO=4&transport=websocket
		}
		using (WWW www = WWW.LoadFromCacheOrDownload (url, int.Parse (id))) {
			yield return www;
			if (www.error != null) {
				
				///will remove the old file from cashe which cause the problem 
//				TaskScreenScript.Unload (url, int.Parse (id), false); 

				Debug.Log ("Error : " + www.error);
//				throw new Exception ("WWW download:" + www.error);
			} else {
				texture = www.texture;
				Rect rect = new Rect (0, 0, texture.width, texture.height);
				Sprite sprite = Sprite.Create (texture, rect, new Vector2 (0.5f, 0.5f));
				img.sprite = sprite;
			}
//			AssetBundleRef abRef = new AssetBundleRef (url, int.Parse (id));
//			abRef.assetBundle = www.assetBundle;
//
//			if (!dictAssetBundleRefs.ContainsKey (keyName)) {
//				dictAssetBundleRefs.Add (keyName, abRef);
//			} else {
//				 
//				Debug.Log ("This is Just Test that how we can unload asset which is in cache");
//				TaskScreenScript.Unload (url, int.Parse (id), false);
//			}
		}
	}
	//
	//	private class AssetBundleRef
	//	{
	//		public AssetBundle assetBundle = null;
	//		public int version;
	//		public string url;
	//
	//		public AssetBundleRef (string strUrlIn, int intVersionIn)
	//		{
	//			url = strUrlIn;
	//			version = intVersionIn;
	//		}
	//	};
	//
	//	public static AssetBundle getAssetBundle (string url, int version)
	//	{
	//		string keyName = url + version.ToString ();
	//		AssetBundleRef abRef;
	//		if (dictAssetBundleRefs.TryGetValue (keyName, out abRef))
	//			return abRef.assetBundle;
	//		else
	//			return null;
	//	}
	//
	//	public static void Unload (string url, int version, bool allObjects)
	//	{
	//		string keyName = url + version.ToString ();
	//		AssetBundleRef abRef;
	//		if (dictAssetBundleRefs.TryGetValue (keyName, out abRef)) {
	//			abRef.assetBundle.Unload (allObjects);
	//			abRef.assetBundle = null;
	//			dictAssetBundleRefs.Remove (keyName);
	//		}
	//	}

	internal void OnSetImages (string id, SpriteRenderer img)
	{
		if (AllItemArray == null) {
			AllItemArray = new ArrayList ();
		} 
		for (int i = 0; i < AllItemArray.Count; i++) {
			JSONObject jdata = AllItemArray [i] as JSONObject;
			if (jdata.GetField ("id").ToString ().Trim ('"') == id) {
				img.name = id;  
				StartCoroutine (Constants.LoadImg (jdata.GetField ("graphics").ToString ().Trim ('"'), img));

			}
		}
	}
	//	internal void OnSetImages1 (string id, Image img)
	//	{
	//		if (AllItemArray == null) {
	//			AllItemArray = new ArrayList ();
	//		}
	//		for (int i = 0; i < AllItemArray.Count; i++) {
	//			JSONObject jdata = AllItemArray [i] as JSONObject;
	//			if (jdata.GetField ("id").ToString ().Trim ('"') == id) {
	//				img.name = id;
	//				StartCoroutine (LoadImg (jdata.GetField ("graphics").ToString ().Trim ('"'), img));
	//			}
	//		}
	//	}

	public static IEnumerator LoadImg (string imageurl, Image img)
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
				Cipherhex_WebSocket.instance.Chest.GetComponent<ItemAnimation> ().OnStartCoinAnimation ();
			} 
		}
	}

	public void SetStoreData (JSONObject data)
	{
		
	}



}
