using UnityEngine;
using System.Collections;
using System;

public class EchoTest : MonoBehaviour
{
	//	WebSocket w = new WebSocket (new Uri ("wss://proxy.cryptohuntga.me/ws"));
	// Use this for initialization
	//	IEnumerator Start ()
	//	{
	//
	////		yield return StartCoroutine (w.Connect ());
	//
	////		w.SendString("Hi there");
	//
	//		int i = 0;
	////		while (true) {
	////			string reply = w.RecvString ();
	////			
	////			if (reply != null) {
	////				Debug.Log ("Received: " + reply);
	//////				w.SendString("Hi there"+i++);
	////			}
	////			if (w.error != null) {
	////				Debug.LogError ("Error: " + w.error);
	////				break;
	////			}
	////			yield return 0;
	////		}
	////		w.Close ();
	//	}

	public void OnLogin ()
	{
		//"{ \"type\": \"login\", \"data\":{ \"email\": $user_email, \"password\": $user_password, \"lat\": $latitude, \"lng\": $longitude }}"
		JSONObject jdata = new JSONObject ();
		jdata.AddField ("email", "a@a.com");
		jdata.AddField ("password", "aaa");
		jdata.AddField ("lat", "45.05548");
		jdata.AddField ("lng", "14.025801");
//		jdata.AddField ("","");
//		SendMessage (jdata);
		Cipherhex_WebSocket.instance.SendMessage (jdata, "login");
	}

	public void SendMessage (JSONObject jdata)
	{
		JSONObject data = new JSONObject ();// = new JSONObject("{ \"type\": \"login\", \"data\":{ \"email\": $user_email, \"password\": $user_password, \"lat\": $latitude, \"lng\": $longitude }}");
		data.AddField ("type", "login");
		data.AddField ("data", jdata);
	 
//		w.SendString (data.Print ());
	}
}
