using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

public static class Constants
{
	public static string DeviceType = "Android", CharSelected = "Male", Username = "bhavesh", emailid = "bhimanibhavu@gmail.com", Firstname = "Bhavesh", gender = "M", CH_Balance = "0", RC20Number = "";
	public static string IdToken = "";
	public static readonly string EmailRegex = @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})|((\.(\w){2,3})(\.(\w){2,3})))$";
	// Email Validation
	internal static bool IsSocketConnected, IsLogin;

	public static IEnumerator LoadImgWithDestroyLoader (bool destroy, string url, Image img, GameObject obj)
	{
		Texture2D texture = new Texture2D (1, 1);
		if (!url.Contains ("https://graph.facebook.com/") && !url.Contains ("http://games.cipherhex.com/") && !url.Contains ("https://fbcdn-profile-a.akamaihd.net/")) {
			url = " http://s3-eu-west-1.amazonaws.com/ch-game-items/200x200/" + url;
		}
		WWW www = new WWW (url);
		yield return www;
		texture = www.texture;
		Rect rect = new Rect (0, 0, texture.width, texture.height);
		Sprite sprite = Sprite.Create (texture, rect, new Vector2 (0.5f, 0.5f));
		img.sprite = sprite;
		if (sprite != null) {
			 
		}
		if (destroy)
			MonoBehaviour.Destroy (obj);
		else
			obj.SetActive (false);
	}

	public static bool ValidateEmailAddress (string email)
	{
		if (Regex.Match (email, EmailRegex).Success)
			return true;
		else
			return false;
	}

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
			}

		}
	}

	public static IEnumerator LoadImg (string imageurl, SpriteRenderer img)
	{
 
		Texture2D texture = img.sprite.texture;
	 
		imageurl = imageurl.Replace (@"\", "");
		if (!imageurl.Contains ("https://") && !imageurl.Contains ("http://") && imageurl.Length >= 5) {
			imageurl = " http://s3-eu-west-1.amazonaws.com/ch-game-items/200x200/" + imageurl;
			//ws://games.cipherhex.com:3000/socket.io/?EIO=4&transport=websocket
		}
		WWW www = new WWW (imageurl);
		yield return www;
		if (img != null && www.texture != null) {
 
			if (www.error == "Null" || www.error == "null" || www.error == null) {
 
				texture = www.texture;
				Rect rect = new Rect (0, 0, texture.width, texture.height);
				Sprite sprite = Sprite.Create (texture, rect, new Vector2 (0.5f, 0.5f));
				img.sprite = sprite;
				Cipherhex_WebSocket.instance.OnCoinAnimation ();
			}

		}
	}


	public static string CheckURLContain (string userurl)
	{
		if (!userurl.Contains ("https://graph.facebook.com/") && !userurl.Contains ("http://games.cipherhex.com/")) {
			userurl = "http://games.cipherhex.com/" + userurl;
		}
		return userurl;
	}
}
