using GoMap;
using System;
using GoShared;
using UnityEngine;
using UnityEngine.UI;
using WebSocketSharp;
using System.Collections;

public class Cipherhex_WebSocket : MonoBehaviour
{
	public static Cipherhex_WebSocket instance;
	//	internal WebSocket w = new WebSocket (new Uri ("wss://proxy.cryptohuntga.me/ws"))
	internal WebSocket w = new WebSocket ("wss://proxy.cryptohuntga.me/ws");
	internal float PingInterval = 60f, LocationChangeInterval = 5f;
	public string ChestId, LocationId;
	public GameObject Chest;
	int PingCount = 0;
	internal InventoryCellBtn InventoryCell;

	public bool IsReusable {
		get { return true; }
	}

	void Awake ()
	{
		instance = this; 
//		InvokeRepeating ("",PingInterval,PingInterval);

		Application.runInBackground = true;
		StartCoroutine (StartWebSocket ());
	}

	 
	public IEnumerator StartWebSocket ()
	{
		w.Connect ();
		yield return w.IsConnected;
		print (w.IsConnected);
		while (true) {
//			print ("IsConnected"); 
			w.OnMessage = (object sender, MessageEventArgs e) => {
				Console.WriteLine ("<- " + e.Data);
//				Console.Write ("-> ");
				JSONObject jdatanew = new JSONObject (e.Data);
//				OnGetData ();
				StartCoroutine (OnGetData (jdatanew));
//				Invoke ("OnGetData", 0.01f);
			}; 
			w.OnError = (object sender, ErrorEventArgs e) => {
//				Console.WriteLine ("ERROR: " + e.Message);
//				Console.Write ("-> ");
//				Invoke ("OnError", 0.01f);
				StartCoroutine (OnError (e.Message));
			};
			w.OnClose = (object sender, CloseEventArgs e) => {
				Console.WriteLine ("Closed " + e.Code + e.Reason + e.WasClean);
//				OnReconnect ();
				StartCoroutine (OnReconnect ());
//				Invoke ("OnReconnect", 0.01f);
			};
			Constants.IsSocketConnected = true;
			yield return 0;
		}
//		Invoke ("OnReconnect",2.1f);
		w.Close ();
	}

	IEnumerator OnError (string msg)
	{
		yield return new WaitForSeconds (0.01f);
		print ("Error In Socket Connection " + msg);
	}

	IEnumerator OnReconnect ()
	{
		yield return new WaitForSeconds (0.01f);
		CancelInvoke ();
		StopAllCoroutines ();
		StartCoroutine (StartWebSocket ());
		Constants.IsLogin = false;
		Debug.LogError ("Socket is close  =============== Reconnecting");
		Constants.IsSocketConnected = false;
		LoginScript.instance.Invoke ("OnAutoLogin", 1f);
	}

	internal void SendMessage (string en)
	{
		JSONObject data = new JSONObject (); 
		data.AddField ("type", en); 
		w.Send (data.Print ()); 
		 
	}

	internal void SendMessage (JSONObject jdata, string en)
	{
		JSONObject data = new JSONObject ();// = new JSONObject("{ \"type\": \"login\", \"data\":{ \"email\": $user_email, \"password\": $user_password, \"lat\": $latitude, \"lng\": $longitude }}");
		data.AddField ("type", en);
		data.AddField ("data", jdata); 
		w.Send (data.Print ());
		print ("Send Data To Server " + data.Print ());
	}

	internal void SendMessage (JSONObject data, JSONObject adata, string en)
	{
		JSONObject jdata = new JSONObject ();// = new JSONObject("{ \"type\": \"login\", \"data\":{ \"email\": $user_email, \"password\": $user_password, \"lat\": $latitude, \"lng\": $longitude }}");
		jdata.AddField ("type", en);
		jdata.AddField ("data", data); 
		jdata.AddField ("adata", adata); 
		w.Send (jdata.Print ());
		print ("bhav" + data.Print ());
	}

	IEnumerator OnGetData (JSONObject data)
	{ 
		yield return new WaitForSeconds (0.1f);
		PreloaderScript.instance.OnDisabledLoder ();
		string en = data.GetField ("type").ToString ().Trim ('"');
		print ("Recieved Data == " + data.Print ()); 
		if (en == "pong") {
			OnPong (data);
		} else if (en == "error") {
			print ("error ==" + data.Print ()); 
			string errormessage = data.GetField ("data").GetField ("error").ToString ().Trim ('"'); 
			if (data.GetField ("data").GetField ("error").ToString ().Trim ('"') == "506" && data.GetField ("data").GetField ("error").ToString ().Trim ('"') == "606" && Chest != null) {
				Chest.SetActive (false);
			}
			ErrorMessageScript.instance.OnShowErrorMessage (errormessage);
		} else if (en == "login_ok") {
			Constants.IsLogin = true;
			LoginScript.instance.OnLoginSucessResponse (data);
			Invoke ("OnGetLocation", 0.1f);
			Invoke ("OnGetItemDetails", 0.11f);
			OnGetLocal_GPSDetails1 ();
			InvokeRepeating ("OnGetLocal_GPSDetails1", LocationChangeInterval, LocationChangeInterval); 
			InvokeRepeating ("OnPing", 1.1f, PingInterval);
		} else if (en == "items_all/get/response") { 
			TaskScreenScript.instance.onGetItemAllResponse (data);

		} else if (en == "user_info/get/response") {
			print ("user_info  " + data.Print ());

		} else if (en == "inventory/get/response") { 
			InventoryScript.instance.OnInventoryListResponse (data);

		} else if (en == "quest_info/get/response") { 
			string status = data.GetField ("data").GetField ("status").ToString ().Trim ('"');
			string item_id = data.GetField ("data").GetField ("item_id").ToString ().Trim ('"');
			SwapItemScript.instance.OnSetQuesIcon (item_id);
			PlayerPrefs.SetString ("Quest_Status", status);
			if (PlayingScript.instance.strButtonClickName == "task") {
				TaskScreenScript.instance.OnDisplayTaskScreenMessage (data);
			} else {
				if (status == "1") {
					ErrorMessageScript.instance.OnShowErrorMessage ("Please solve the riddle first. Check your task.");
				} else {
					OnGetLocationList ();
				}
			}


			//			if (status == "1") {
//			} else {
//				OnGetLocationList ();
//				PreloaderScript.instance.OnEnabledLoder ();
//			}


		} else if (en == "location_list/get/response") {
			print ("location_list  " + data.Print ());
			LocationScript.instance.OnLocationPanelOpen (data.GetField ("adata"));
//			
		} else if (en == "wallet/address/response") {
//			print ("wallet_address_response  " + data.Print ());
			if (data.GetField ("data").GetField ("ok").ToString ().Trim ('"') == "1") {
				Constants.RC20Number = WalletScript.instance.txterc20Number.text;
				print ("Update ERC20 Sucessfully");
			} else {
				print ("Error in ERC20 updates");
			}
			//			
		} else if (en == "location_types/get/response") {
			print ("location_types  " + data.Print ());
			LocationScript.instance.onGetLocationListResponse (data);

		} else if (en == "local_gps_env/get/response") { 
			PlayingScript.instance.OnMakeChestOnEnvironment (data);

		} else if (en == "interact_chest/open/response") {
 
			RandomPlacement.instance.AnimationArray.Add (Chest.GetComponent<ItemAnimation> ().ChestId);
			Chest.GetComponent<ItemAnimation> ().StartCoinAnimation (); 
			float wait_timeout = float.Parse (data.GetField ("data").GetField ("wait_timeout").ToString ().Trim ('"'));
			StartCoroutine (CalledChestItemCollectAuto (wait_timeout)); 
		} else if (en == "interact_chest/collect/response") {
			if (Chest != null) { 
				try {
					if (Chest.GetComponent<ItemAnimation> ().ItemCoin != null) {  
						TaskScreenScript.instance.OnSetImages (data.GetField ("data").GetField ("item_id").ToString ().Trim ('"'), Chest.GetComponent<ItemAnimation> ().ItemCoin.GetComponent<SpriteRenderer> ());
					}
				} catch (Exception e) {
					print ("Error for Coin Animation " + e.Message);
				}

			}

		} else if (en == "interact_items/givetoprof/response") {
			print ("  " + data.Print ());
			int status = int.Parse (data.GetField ("data").GetField ("status").ToString ().Trim ('"'));
			PlayerPrefs.SetString ("Quest_Status", status.ToString ());
			if (status == 2) {
				LocationScript.instance.OnLocationPanelOpen (data.GetField ("adata"));
				InventoryCell.txtNoOfCoin.text = (int.Parse (InventoryCell.txtNoOfCoin.text) - 1).ToString ();
				if (int.Parse (InventoryCell.txtNoOfCoin.text) <= 0) {
					Destroy (InventoryCell.gameObject);
				}

			}
		} else if (en == "interact_loc/check/response") {
 
			PreloaderScript.instance.OnEnabledLoder ();
			float wait_timeout = float.Parse (data.GetField ("data").GetField ("wait_timeout").ToString ().Trim ('"'));
			Invoke ("OnGetInteract_LocCollectDetail", wait_timeout);
		} else if (en == "interact_loc/collect/response") {
			print ("  " + data.Print ());
			LocationScript.instance.OnInteract_locationCollectResponse (data);

		} else if (en == "location_list/new/response") {
 
			LocationScript.instance.OnSetLocationData (data.GetField ("adata"));
		} else if (en == "interact_items/exchange/response") { 
//			LocationScript.instance.OnSetLocationData (data.GetField ("adata"));
			InventoryScript.instance.OnInventoryListResponse (data);
			SwapItemScript.instance.OnSwapItemCloseButtonClick ();
		} else {
 
		}

	}

	internal void OnCoinAnimation ()
	{
		Chest.GetComponent<ItemAnimation> ().OnStartCoinAnimation ();
	}

	IEnumerator CalledChestItemCollectAuto (float wait_timeout)
	{
		yield return new WaitForSeconds (wait_timeout);
		JSONObject jdata = new JSONObject (); 
		if (Constants.IsSocketConnected) {
			if (LocationManager.CenterWorldCoordinates != null) {
				if (LocationManager.CenterWorldCoordinates.latitude != null) { 
					jdata.AddField ("lat", LocationManager.CenterWorldCoordinates.latitude.ToString ());
					jdata.AddField ("lng", LocationManager.CenterWorldCoordinates.longitude.ToString ());
					jdata.AddField ("chest_id", ChestId);
					SendMessage (jdata, "interact_chest/collect/try");
					print ("ChestIsOpen");
				} else {
					StartCoroutine (CalledChestItemCollectAuto (0)); 
				}
			} else {
				StartCoroutine (CalledChestItemCollectAuto (0)); 
			}
		}

	}

	void Update ()
	{
//		final LocationManager manager = (LocationManager) getSystemService( Context.LOCATION_SERVICE );
//
//		if ( !manager.isProviderEnabled( LocationManager.GPS_PROVIDER ) ) {
//			// Call your Alert message
//		}
		if (Input.location.isEnabledByUser) {
//			print ("GPS is Enabled");
			if (UserSettingScript.instance != null) {
				UserSettingScript.instance.GPSPanel.SetActive (false);//, GPSSubPanel;
			}
		} else {
			if (UserSettingScript.instance != null) {
				UserSettingScript.instance.GPSPanel.SetActive (true);
				UserSettingScript.instance.GPSSubPanel.SetActive (true);
				UserSettingScript.instance.GPSPanel.transform.SetAsLastSibling ();

			}
//			print ("GPS is Disabled");
		}
	}

	/// <summary>
	/// ping response , ( A message which keeps the connection with the backend alive. )
	/// </summary>
	/// <param name="jdata">Jdata.</param>
	public void OnPong (JSONObject jdata)
	{ 
//		OnGetUserDetails ();

//		OnGetLocationList ();
		PingCount++;

//		OnGetLocal_GPSDetails (); 
//		OnGetInteract_ChestDetails ("5");   //chest detail
//		OnGetInteract_ChestCollectDetail ("5");  //chest detail
//		OnGetInteract_ItemsDetails ("5");   //chest detail
//		OnGetInteract_ItemsExchangeDetails ();
//		OnGetInteract_LocDetail("hospital");   //location detail
//		OnGetInteract_LocCollectDetail ("hospital");  //location detail
//		OnGetLocation_ListDetail ();
	}

	/// <summary>
	/// A message which keeps the connection with the backend alive. 
	/// </summary>
	public void OnPing ()
	{ 
		if (Constants.IsLogin) { 
			if (Constants.IsSocketConnected) { 
				if (LocationManager.CenterWorldCoordinates != null) { 
					if (LocationManager.CenterWorldCoordinates.latitude != null) {   
						JSONObject jdata = new JSONObject (); 
						jdata.AddField ("lat", LocationManager.CenterWorldCoordinates.latitude.ToString ());
						jdata.AddField ("lng", LocationManager.CenterWorldCoordinates.longitude.ToString ());
						SendMessage (jdata, "ping");
//						print ("Ping ========Ping " + jdata.Print ());
					} else {
						Invoke ("OnPing", 0.1f);
					} 
				} else {
					Invoke ("OnPing", 0.1f);
				}
			}
		}
	}

	internal void OnGetLocation ()
	{
		SendMessage ("location_types/get/try");
	}

	internal void OnGetItemDetails ()
	{
		SendMessage ("items_all/get/try");//
	}

	internal void OnGetUserDetails ()
	{
		SendMessage ("user_info/get/try");//
	}

	internal void OnGetInventoryDetail ()
	{
		SendMessage ("inventory/get/try");//
	}

	internal void OnGetLocationList ()
	{
		SendMessage ("location_list/get/try");//
	}

	internal void OnGetQuestDetails ()
	{
		SendMessage ("quest_info/get/try");
	}

	internal void OnGetLocal_GPSDetails1 ()
	{
		JSONObject jdata = new JSONObject (); 
		if (Constants.IsSocketConnected) {
			if (LocationManager.CenterWorldCoordinates != null) {
				if (LocationManager.CenterWorldCoordinates.latitude != null) {  
					jdata.AddField ("lat", LocationManager.CenterWorldCoordinates.latitude.ToString ());
					jdata.AddField ("lng", LocationManager.CenterWorldCoordinates.longitude.ToString ());
					SendMessage (jdata, "local_gps_env/get/try");
//					SendMessage ("local_gps_env/get/try");
				}
			}
		}

	}

	internal void OnGetInteract_ChestDetails (string chest_id)
	{
		JSONObject jdata = new JSONObject (); 
		if (Constants.IsSocketConnected) {
			if (LocationManager.CenterWorldCoordinates != null) {
				if (LocationManager.CenterWorldCoordinates.latitude != null) { 
					jdata.AddField ("chest_id", chest_id);
					jdata.AddField ("lat", (long)LocationManager.CenterWorldCoordinates.latitude);
					jdata.AddField ("lng", (long)LocationManager.CenterWorldCoordinates.longitude);
					SendMessage (jdata, "interact_chest/open/try"); 
				}
			}
		}

//		SendMessage ("interact_chest/open/try");
	}

	internal void OnGetInteract_ChestCollectDetail (string chest_id)
	{
		JSONObject jdata = new JSONObject (); 
		if (Constants.IsSocketConnected) {
			if (LocationManager.CenterWorldCoordinates != null) {
				if (LocationManager.CenterWorldCoordinates.latitude != null) { 
					jdata.AddField ("chest_id", chest_id);
					jdata.AddField ("lat", LocationManager.CenterWorldCoordinates.latitude.ToString ());
					jdata.AddField ("lng", LocationManager.CenterWorldCoordinates.longitude.ToString ());
					SendMessage (jdata, "interact_chest/collect/try"); 
				}
			}
		}
		//		SendMessage ("interact_chest/collect/try");
	}

	internal void OnGetInteract_ItemsDetails (string chest_id)
	{
		
		JSONObject jdata = new JSONObject (); 
		if (Constants.IsSocketConnected) {
			if (LocationManager.CenterWorldCoordinates != null) {
				if (LocationManager.CenterWorldCoordinates.latitude != null) { 
					jdata.AddField ("chest_id", chest_id);
					jdata.AddField ("lat", LocationManager.CenterWorldCoordinates.latitude.ToString ());
					jdata.AddField ("lng", LocationManager.CenterWorldCoordinates.longitude.ToString ());
					SendMessage (jdata, "interact_items/givetoprof/try"); 
				}
			}
		}
//		SendMessage ("interact_items/givetoprof/try");
	}

	internal void OnGetInteract_ItemsExchangeDetails (JSONObject obj)
	{
		SendMessage (JSONObject.nullJO, obj, "interact_items/exchange/try");
		print (obj.Print ());
	}

	internal void OnGetInteract_LocDetail (string location_id)
	{

		JSONObject jdata = new JSONObject (); 
		if (Constants.IsSocketConnected) {
			if (LocationManager.CenterWorldCoordinates != null) {
				if (LocationManager.CenterWorldCoordinates.latitude != null) { 
					jdata.AddField ("location_id", location_id);
					jdata.AddField ("lat", LocationManager.CenterWorldCoordinates.latitude.ToString ());
					jdata.AddField ("lng", LocationManager.CenterWorldCoordinates.longitude.ToString ());
					SendMessage (jdata, "interact_loc/check/try"); 
				}
			}
		} 
	}

	internal void OnGetInteract_LocCollectDetail ()
	{
		PreloaderScript.instance.OnEnabledLoder ();
		JSONObject jdata = new JSONObject (); 
		if (Constants.IsSocketConnected) {
			if (LocationManager.CenterWorldCoordinates != null) {
				if (LocationManager.CenterWorldCoordinates.latitude != null) { 
					jdata.AddField ("location_id", LocationId);
					jdata.AddField ("lat", LocationManager.CenterWorldCoordinates.latitude.ToString ());
					jdata.AddField ("lng", LocationManager.CenterWorldCoordinates.longitude.ToString ());
					SendMessage (jdata, "interact_loc/collect/try"); 
				}
			}
		} 
	}

	internal void OnGetLocation_ListSuffleDetail ()
	{
		JSONObject jdata = new JSONObject (); 
		if (Constants.IsSocketConnected) {
			if (LocationManager.CenterWorldCoordinates != null) {
				if (LocationManager.CenterWorldCoordinates.latitude != null) { 

					jdata.AddField ("lat", LocationManager.CenterWorldCoordinates.latitude.ToString ());
					jdata.AddField ("lng", LocationManager.CenterWorldCoordinates.longitude.ToString ());
					SendMessage (jdata, "location_list/new/try"); 
				}
			}
		}
//		SendMessage ("location_list/new/try");
	}

	internal void OnSendEthAddress (string ethaddress)
	{
		JSONObject jdata = new JSONObject ();
		jdata.AddField ("eth_address", ethaddress);
		SendMessage (jdata, "wallet/address/try"); 

	}
}
