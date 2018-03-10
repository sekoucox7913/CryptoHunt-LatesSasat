using GoMap;
using GoShared;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LoginScript : MonoBehaviour
{
	public static LoginScript instance;

	void Awake ()
	{
		instance = this;
		Screen.sleepTimeout = SleepTimeout.NeverSleep;
	}

	void Start ()
	{
		Cipherhex_plugins.instance.SignInGoogle ();
		//OnLogin ();
//		Invoke ("OnUserInfo", 5f);
	}

	public void OnUserInfo ()
	{
		//"{ \"type\": \"login\", \"data\":{ \"email\": $user_email, \"password\": $user_password, \"lat\": $latitude, \"lng\": $longitude }}"
		JSONObject jdata = new JSONObject ();
		jdata.AddField ("email", "test@example.com");
		jdata.AddField ("password", "Test123");
		jdata.AddField ("lat", "45.05548");
		jdata.AddField ("lng", "14.025801");
		Cipherhex_WebSocket.instance.SendMessage (jdata, "login");
	}

	public void OnLogin ()
	{
		//"{ \"type\": \"login\", \"data\":{ \"email\": $user_email, \"password\": $user_password, \"lat\": $latitude, \"lng\": $longitude }}"
		JSONObject jdata = new JSONObject ();
		jdata.AddField ("email", "bhimanibhavu@gmail.com");
		jdata.AddField ("password", "aaa");
		jdata.AddField ("lat", LocationManager.CenterWorldCoordinates.longitude.ToString ());
		jdata.AddField ("lng", LocationManager.CenterWorldCoordinates.latitude.ToString ());
		Cipherhex_WebSocket.instance.SendMessage (jdata, "login");
	}

	//THIS IS RESPONSE OF GOOGGE REPONSE FOR IOSSSS
	public void OnGetGoogleData (string data)
	{
		JSONObject obj = new JSONObject (data);
		print ("On Login Success " + obj.Print ());
		Constants.IdToken = obj.GetField ("idToken").ToString ().Trim ('"');
		Constants.emailid = obj.GetField ("email").ToString ().Trim ('"');
		CharcterSeletion.instance.txtUserName.text = (obj.GetField ("name").ToString ().Trim ('"').Split (' ')) [0]; 
	}

	internal void OnAutoLogin ()
	{
		JSONObject jdata = new JSONObject ();
		jdata.AddField ("id_token", Constants.IdToken);
		jdata.AddField ("email", Constants.emailid);
		jdata.AddField ("gender", Constants.gender);
		jdata.AddField ("nickname", Constants.Username);   
		if (LocationManager.CenterWorldCoordinates == null) {
			print ("Location not found in your mobile");
		}
		print ("Socket Connection - " + Constants.IsSocketConnected);
		if (LocationManager.CenterWorldCoordinates != null && Constants.IsSocketConnected) { 
			 
			if (Constants.emailid != "" && Constants.IdToken != "") {  
				jdata.AddField ("lat", LocationManager.CenterWorldCoordinates.latitude.ToString ());
				jdata.AddField ("lng", LocationManager.CenterWorldCoordinates.longitude.ToString ());
				Cipherhex_WebSocket.instance.SendMessage (jdata, "google_login");
			} else {
				Invoke ("OnAutoLogin", 1.5f);
//				print ("Called In Login Script ....."); 
			}
		} else {
			print ("Called In Login Script 44444");
			Invoke ("OnAutoLogin", 1.5f);
		}

	}

	public void OnCancelGoogleLogin (string err)
	{
//		print ("google login error " + err);
//		//		Constant.DesablePreloader();
//		//		LoginScript.Inst.OnCancelGoogleResponse(err);
	}

	internal void OnLoginSucessResponse (JSONObject data)
	{
		
		Constants.CH_Balance = data.GetField ("data").GetField ("ch_balance").ToString ().Trim ('"');
		PlayingScript.instance.txtCHCoin.text = string.Format ("{0:0}", float.Parse (Constants.CH_Balance));
		WalletScript.instance.txtCHCoin.text = string.Format ("{0:0}", float.Parse (Constants.CH_Balance));
		Constants.RC20Number = data.GetField ("data").GetField ("eth_address").ToString ().Trim ('"');
		if (PlayerPrefs.GetString ("IsRegister") == "true") {
			PlayingScript.instance.OnPlayingScreenDisplay ();
		} else {
			CharcterSeletion.instance.OnLoginSucess ();
		}
		PlayerPrefs.SetString ("IsRegister", "true");
	}
}
