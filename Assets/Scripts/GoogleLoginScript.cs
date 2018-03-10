using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SocialPlatforms;
using System.Runtime.InteropServices;

public class GoogleLoginScript : MonoBehaviour
{
	

	public Text txtMessage;
	string googleIdToken = "182775792911-npl5dsbobp54d8o15ei9okbl1uh8c7tu.apps.googleusercontent.com";
	string googleAccessToken = "tQ3FdS18OYjK32njdtwZ2G41";

	[DllImport ("__Internal")]
	private static extern void OnSignInGoogle (string Objectname_forResponse);

	[DllImport ("__Internal")]
	private static extern void OnSignOutGoogle ();

	//
	void Start ()
	{
//		PlayGamesPlatform.Activate();
	}

	public void GooglePlusAuthentication ()
	{
		//We set the access token to the newly added built in funtion in Google Play Games to get our token
//		googleAccessToken.SetAccessToken(PlayGamesPlatform.Instance.GetIdToken()).Send((googleAuthResponse) =>
//			{
//				if (!googleAuthResponse.HasErrors)
//				{
//					Debug.Log(googleAuthResponse.DisplayName + " Logged In!");
//					log.text = googleAuthResponse.DisplayName + " Logged In!";
//				}
//				else
//				{
//					Debug.Log("Not Logged In!");
//					log.text = googleAuthResponse.JSONString;
//				}
//			});
	}


	#region GooglePlugins

	public void SignInGoogle ()
	{
		#if UNITY_ANDROID
		AndroidJavaClass ajc = new AndroidJavaClass ("com.unity3d.player.UnityPlayer");
		var obj = ajc.GetStatic<AndroidJavaObject> ("currentActivity");
		AndroidJavaObject ajo = new AndroidJavaObject ("com.cipherhex.Cipherhex_Native");
		//		AndroidJavaObject ajo = new AndroidJavaObject ("com.gameshore.Gameshore_UnityConnect");
		ajo.CallStatic ("StartConnection", obj);

		#elif UNITY_IOS || UNITY_IPHONE
		OnSignInGoogle("Cipherhex_Plugin");
		#endif
	}

	public void SignOutGoogle ()
	{
		#if UNITY_ANDROID
		AndroidJavaClass ajc = new AndroidJavaClass ("com.unity3d.player.UnityPlayer");
		var obj = ajc.GetStatic<AndroidJavaObject> ("currentActivity");
		AndroidJavaObject ajo = new AndroidJavaObject ("com.cipherhex.Cipherhex_Native");
		//		AndroidJavaObject ajo = new AndroidJavaObject ("com.gameshore.Gameshore_UnityConnect");
		ajo.CallStatic ("OnSignOutGoogle", obj);

		#elif UNITY_IOS || UNITY_IPHONE
		OnSignOutGoogle();
		#endif
	}


	#endregion

	public void OnSignButtonClick ()
	{
//		txtMessage.text = PlayGamesPlatform.Instance.GetUserDisplayName ();

		SignInGoogle ();
//		Social.localUser.Authenticate((bool success	) => {
//			// handle success or failure
//			txtMessage.text = "sucessfull login \n" + Social.localUser.id +"\n"+ Social.localUser.userName + "\n" + Social.localUser.isFriend;
//
//		});

	}

	public void OnGetProfileData ()
	{
//		googleIdToken = PlayGamesPlatform.Instance.GetIdToken ();
//		googleAccessToken = PlayGamesPlatform.Instance.GetServerAuthCode ();
//		FirebaseAuth auth = FirebaseAuth.DefaultInstance;
//				  
//
//				Firebase.Auth.Credential credential =
//					Firebase.Auth.GoogleAuthProvider.GetCredential(googleIdToken, googleAccessToken);
//				auth.SignInWithCredentialAsync(credential).ContinueWith(task => {
//					if (task.IsCanceled) {
//						Debug.LogError("SignInWithCredentialAsync was canceled.");
//						return;
//					}
//					if (task.IsFaulted) {
//						Debug.LogError("SignInWithCredentialAsync encountered an error: " + task.Exception);
//						return;
//					}
//		
//					Firebase.Auth.FirebaseUser newUser = task.Result;
//					Debug.LogFormat("User signed in successfully: {0} ({1})",
//						newUser.DisplayName, newUser.UserId);
//					txtMessage.text = newUser.DisplayName;
//				});
//
//
//						Firebase.Auth.FirebaseUser user = auth.CurrentUser;
//						if (user != null) {
//					
//							string name = user.DisplayName;
//							string email = user.Email;
//							System.Uri photo_url = user.PhotoUrl;
//							// The user's Id, unique to the Firebase project.
//							// Do NOT use this value to authenticate with your backend server, if you
//							// have one; use User.TokenAsync() instead.
//							string uid = user.UserId;
//							txtMessage.text = name + "\n" + email + "\n" + uid + "\n" + photo_url.ToString ();
//						}
	}

	public void OnErrorData (string data)
	{
		JSONObject obj = new JSONObject (data);
		 
		txtMessage.text = data;
	}
	//THIS IS RESPONSE OF GOOGGE REPONSE FOR IOSSSS
	public void OnGetGoogleData (string data)
	{
		//		Constant.DesablePreloader();
		JSONObject obj = new JSONObject (data); 
		txtMessage.text = data;
		//		LoginScript.Inst.OnGoogleDataResponce(obj);
	}

	public void OnCancelGoogleLogin (string err)
	{
		 
		txtMessage.text = err;
		//		Constant.DesablePreloader();
		//		LoginScript.Inst.OnCancelGoogleResponse(err);
	}
}
