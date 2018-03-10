using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;


public class Cipherhex_plugins : MonoBehaviour
{

	public static Cipherhex_plugins instance;

	[DllImport ("__Internal")]
	private static extern void PlayVideoWithBufferurl (string url);

	[DllImport ("__Internal")]
	private static extern void OnSignInGoogle (string Objectname_forResponse);

	[DllImport ("__Internal")]
	private static extern void OnSignOutGoogle ();

	[DllImport ("__Internal")]
	private static extern void ShowSharingOptions (string data, int imgorvideo);

	[DllImport ("__Internal")]
	private static extern string GetThumbnailFromUrl (string url, int interval);

	void Awake ()
	{
		instance = this;
//		OnTest ();
	}

	internal void OnTest ()
	{
		if (Application.platform == RuntimePlatform.Android) {
			AndroidJavaClass ajc = new AndroidJavaClass ("com.unity3d.player.UnityPlayer");
			var obj = ajc.GetStatic<AndroidJavaObject> ("currentActivity");
			AndroidJavaObject ajo = new AndroidJavaObject ("com.cryptohunt.Cipherhex_Native");
			//		AndroidJavaObject ajo = new AndroidJavaObject ("com.gameshore.Gameshore_UnityConnect");
			ajo.CallStatic ("onNativeLogin", obj);
		}
	}

	#region PlayVideoWith Strimming

	/// <summary>
	/// Play the video URL.
	/// </summary>
	/// <param name="VideoUrl">Video URL.</param>
	public void PlayVideoUrl (string VideoUrl)
	{
		if (!VideoUrl.Contains ("http://")) {
			VideoUrl = "http://" + VideoUrl;
		}
		#if UNITY_ANDROID
		AndroidJavaClass ajc = new AndroidJavaClass ("com.unity3d.player.UnityPlayer");
		var obj = ajc.GetStatic<AndroidJavaObject> ("currentActivity");
		AndroidJavaObject ajo = new AndroidJavaObject ("com.Cipherhex_videoplayer.UnityBinder");
		ajo.CallStatic ("playVideo", obj, VideoUrl);
		#elif UNITY_IOS
		PlayVideoWithBufferurl (VideoUrl);
		#endif
	}

	#endregion

	public void SaveImage_VideoDataInDevice (string data, int imgOrvideo)
	{
		#if UNITY_ANDROID
		AndroidJavaClass ajc = new AndroidJavaClass ("com.unity3d.player.UnityPlayer");
		var obj = ajc.GetStatic<AndroidJavaObject> ("currentActivity");
		AndroidJavaObject ajo = new AndroidJavaObject ("com.cryptohunt.Gameshore_UnityConnect_download");
		ajo.CallStatic ("Download_call", obj, data, imgOrvideo);
		 
		 
		#endif

	}

	#region Share Image Or Video

	/// <summary>
	/// Shows the sharing popups.
	/// data : Contain String url for Image or Video Sharing
	/// imgOrvideo : 0= Image , 1 = video Share
	/// </summary>
	/// <param name="data">Data.</param>
	/// <param name="imgorvido">imgOrvideo.</param>
	public void ShowSharingPopups (string data, int imgOrvideo)
	{
		#if UNITY_ANDROID
		AndroidJavaClass ajc = new AndroidJavaClass ("com.unity3d.player.UnityPlayer");
		var obj = ajc.GetStatic<AndroidJavaObject> ("currentActivity");
		AndroidJavaObject ajo = new AndroidJavaObject ("com.cryptohunt.Cipherhex_Native");
		ajo.CallStatic ("Share_call", obj, data, imgOrvideo);
		#elif UNITY_IOS || UNITY_IPHONE
		ShowSharingOptions (data, imgOrvideo);
		#endif
	}

	#endregion



	#region GooglePlugins

	public void SignInGoogle ()
	{
		
		#if UNITY_ANDROID
		if (Application.platform == RuntimePlatform.Android) {
			AndroidJavaClass ajc = new AndroidJavaClass ("com.unity3d.player.UnityPlayer");
			var obj = ajc.GetStatic<AndroidJavaObject> ("currentActivity");
			AndroidJavaObject ajo = new AndroidJavaObject ("com.cryptohunt.Cipherhex_Native");
			ajo.CallStatic ("StartConnection", obj);
		}
		#elif UNITY_IOS || UNITY_IPHONE
		OnSignInGoogle ("Cipherhex_Plugin");
		#endif
	}

	public void SignOutGoogle ()
	{
		#if UNITY_ANDROID
		AndroidJavaClass ajc = new AndroidJavaClass ("com.unity3d.player.UnityPlayer");
		var obj = ajc.GetStatic<AndroidJavaObject> ("currentActivity");
		AndroidJavaObject ajo = new AndroidJavaObject ("com.cryptohunt.Cipherhex_Native");
		ajo.CallStatic ("OnSignOutGoogle", obj);

		#elif UNITY_IOS || UNITY_IPHONE
		OnSignOutGoogle ();
		#endif
	}

	//THIS IS RESPONSE OF GOOGGE REPONSE FOR IOSSSS
	public void OnGetGoogleData (string data)
	{
		//		Constant.DesablePreloader();
		JSONObject obj = new JSONObject (data);
		 
		//LoginScript.Inst.OnGoogleDataResponce(obj);
	}

	public void OnCancelGoogleLogin (string err)
	{
		//		Constant.DesablePreloader();
		//		LoginScript.Inst.OnCancelGoogleResponse(err);
	}

	#endregion


	#region PushNotification

	public void GetAccessTokenForRemoteNotification (string accesstoken)
	{
		print ("Token : " + accesstoken);
		//		Constant.NotificationId = accesstoken;
		//		PlayerPrefs.SetString("Token", Constant.NotificationId);
	}

	#endregion

	[DllImport ("__Internal")]
	private static extern void LoadImagePicker (int maxWidth, int maxHeight);

	[DllImport ("__Internal")]
	private static extern void LoadCameraPicker (int maxWidth, int maxHeight);

	WWW www;

	Texture2D texture;
	public ArrayList colorarray;

	public void OnCameraOpen ()
	{
		#if UNITY_ANDROID
		AndroidJavaClass ajc = new AndroidJavaClass ("com.unity3d.player.UnityPlayer");
		var obj = ajc.GetStatic<AndroidJavaObject> ("currentActivity");
		AndroidJavaObject ajo = new AndroidJavaObject ("com.Cipherhex_imagegetter.UnituBinder");
		ajo.CallStatic ("OpenGallery", obj); 
		#elif UNITY_IOS 
		LoadCameraPicker (Screen.width, Screen.height);
		#endif
	}

	public void OnGalleryOpen ()
	{
		#if UNITY_ANDROID 
		AndroidJavaClass ajc = new AndroidJavaClass ("com.unity3d.player.UnityPlayer");
		var obj = ajc.GetStatic<AndroidJavaObject> ("currentActivity");
		AndroidJavaObject ajo = new AndroidJavaObject ("com.Cipherhex_imagegetter.UnituBinder");
		ajo.CallStatic ("OpenGallery_real", obj); 
		#elif UNITY_IOS
		LoadImagePicker (Screen.width, Screen.height);
		#endif
	}

	public void OnPhotoPick (string filePath)
	{

		#if UNITY_ANDROID
		var data = System.Convert.FromBase64String (filePath); 
		texture = new Texture2D (1, 1);
		texture.LoadImage (data);
		Rect rect = new Rect (0, 0, texture.width, texture.height);
		Sprite sprite = Sprite.Create (texture, rect, new Vector2 (0.5f, 0.5f)); 
		#elif UNITY_IOS 
		var data = System.Convert.FromBase64String (filePath); 
		texture = new Texture2D (1, 1);
		texture.LoadImage (data);
		Rect rect = new Rect (0, 0, texture.width, texture.height);
		Sprite sprite = Sprite.Create (texture, rect, new Vector2 (0.5f, 0.5f)); 
		#endif 
	}

}
