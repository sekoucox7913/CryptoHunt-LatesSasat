using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public  class AnimationScript : MonoBehaviour {
	public static AnimationScript Inst;

	internal float Showtime = 1f;
	void Awake()
	{
		Inst = this;
	}
	public void OnMoveUpAnimation (GameObject CloseOpenPanelAfreAnimation, GameObject BackgroundPanel, GameObject AnimationPanel,bool isshow,float timescal)
	{
		BackgroundPanel.transform.SetAsLastSibling ();
		BackgroundPanel.SetActive (true);
		Showtime = timescal;
		if (isshow) {
			AnimationPanel.GetComponent<RectTransform> ().localScale = Vector3.one;
			if (CloseOpenPanelAfreAnimation != null) {
				CloseOpenPanelAfreAnimation.SetActive (true);
			}
//			AnimationPanel.SetActive (true);
			StartCoroutine (HideMainPanel (CloseOpenPanelAfreAnimation));
			StartCoroutine (ShowMainPanel (BackgroundPanel));
			Vector3 posion = AnimationPanel.GetComponent<RectTransform> ().localPosition;
			posion.y = BackgroundPanel.GetComponent<RectTransform> ().rect.height;
			AnimationPanel.GetComponent<RectTransform> ().localPosition = posion;
			iTween.MoveTo (AnimationPanel,iTween.Hash("y",0,"islocal",true,"time",timescal, "easetype",iTween.EaseType.easeInCubic));
		} 
		else
		{
			iTween.MoveTo (AnimationPanel,iTween.Hash("y",BackgroundPanel.GetComponent<RectTransform>().rect.height,"islocal",true,"time",timescal, "easetype",iTween.EaseType.linear));
			if (CloseOpenPanelAfreAnimation != null) {
				CloseOpenPanelAfreAnimation.SetActive (true);
			}
//			AnimationPanel.SetActive (true);
			StartCoroutine (ShowMainPanel (CloseOpenPanelAfreAnimation));
			StartCoroutine (HideMainPanel (BackgroundPanel));
		}
	}
	public void OnMoveCenterAnimation (GameObject CloseOpenPanelAfreAnimation, GameObject BackgroundPanel, GameObject AnimationPanel,bool isshow,float timescal)
	{
		Showtime = timescal;
		BackgroundPanel.SetActive (true);
		BackgroundPanel.transform.SetAsLastSibling ();
		Vector3 localScal = AnimationPanel.GetComponent<RectTransform> ().localScale;
		if (isshow) {
			if (CloseOpenPanelAfreAnimation != null) {
				CloseOpenPanelAfreAnimation.SetActive (true);
			}
//			AnimationPanel.SetActive (true);
			StartCoroutine (HideMainPanel (CloseOpenPanelAfreAnimation));
			StartCoroutine (ShowMainPanel (BackgroundPanel));
			AnimationPanel.GetComponent<RectTransform> ().localScale = Vector3.one;
			Vector3 posion = AnimationPanel.GetComponent<RectTransform> ().localPosition;
			posion.x = BackgroundPanel.GetComponent<RectTransform> ().rect.width;
			AnimationPanel.GetComponent<RectTransform> ().localPosition = posion;
			iTween.MoveTo (AnimationPanel,iTween.Hash("x",0,"islocal",true,"time",timescal, "easetype",iTween.EaseType.easeInCubic));
		} 
		else
		{
			iTween.MoveTo (AnimationPanel,iTween.Hash("x",-BackgroundPanel.GetComponent<RectTransform>().rect.width,"islocal",true,"time",timescal, "easetype",iTween.EaseType.easeOutCubic));
			if (CloseOpenPanelAfreAnimation != null) {
				CloseOpenPanelAfreAnimation.SetActive (true);
			}
//			AnimationPanel.SetActive (true);
			StartCoroutine (ShowMainPanel (CloseOpenPanelAfreAnimation));
			StartCoroutine (HideMainPanel (BackgroundPanel));
		}
	}
	internal IEnumerator ShowMainPanel(GameObject mainPanel)
	{
		yield return new WaitForSeconds (Showtime+0.1f);
		if (mainPanel != null) {
			mainPanel.SetActive (true);
		}
	}
	internal IEnumerator HideMainPanel(GameObject mainPanel)
	{
		yield return new WaitForSeconds (Showtime+0.1f);
		if (mainPanel != null) {
			mainPanel.SetActive (false);
		}
	}
 

	public void OnScalByXAnimation (GameObject CloseOpenPanelAfreAnimation,GameObject BackgroundPanel, GameObject AnimationPanel,bool isshow,float timescal)
	{
		Showtime = timescal;
		BackgroundPanel.transform.SetAsLastSibling ();
		BackgroundPanel.SetActive (true);
		Vector3 localScal = AnimationPanel.GetComponent<RectTransform> ().localScale;
		if (isshow) {  
			if (CloseOpenPanelAfreAnimation != null) {
				CloseOpenPanelAfreAnimation.SetActive (true);
			}
//			AnimationPanel.SetActive (true);
			StartCoroutine (HideMainPanel (CloseOpenPanelAfreAnimation));
			StartCoroutine (ShowMainPanel (BackgroundPanel));
			AnimationPanel.GetComponent<RectTransform> ().localScale = Vector3.one;
			localScal.x = 0;

			AnimationPanel.GetComponent<RectTransform> ().localScale = localScal;
			iTween.ScaleTo(AnimationPanel, iTween.Hash("x", 1,"islocal",true,"time",Showtime,"easetype",iTween.EaseType.easeInCubic));
			//iTween.ScaleFrom (AnimationPanel,iTween.Hash("x",255,"time",timescal, "easetype",iTween.EaseType.));
		} 
		else
		{
			if (CloseOpenPanelAfreAnimation != null) {
				CloseOpenPanelAfreAnimation.SetActive (true);
			}
//			AnimationPanel.SetActive (true);
			StartCoroutine (ShowMainPanel (CloseOpenPanelAfreAnimation));
			StartCoroutine (HideMainPanel (BackgroundPanel));
			localScal.x = 1; 
			AnimationPanel.GetComponent<RectTransform> ().localScale = localScal;
			iTween.ScaleTo(AnimationPanel, iTween.Hash("x", 0, "islocal",true,"time",Showtime,"easetype",iTween.EaseType.easeOutCubic));
			 
		}
	}
	public void OnScalByXYAnimation (GameObject CloseOpenPanelAfreAnimation, GameObject BackgroundPanel, GameObject AnimationPanel,bool isshow,float timescal)
	{
		Showtime = timescal;
		BackgroundPanel.transform.SetAsLastSibling ();
		Vector3 localScal = AnimationPanel.GetComponent<RectTransform> ().localScale;
		BackgroundPanel.SetActive (true);
		if (isshow) {
			if (CloseOpenPanelAfreAnimation != null) {
				CloseOpenPanelAfreAnimation.SetActive (true);
			}
//			AnimationPanel.SetActive (true);
			StartCoroutine (HideMainPanel (CloseOpenPanelAfreAnimation));
			StartCoroutine (ShowMainPanel (BackgroundPanel));
			AnimationPanel.GetComponent<RectTransform> ().localScale = Vector3.one;
			localScal.x = 0.6f;
			localScal.x = 0.7f;
			AnimationPanel.GetComponent<RectTransform> ().localScale = localScal;
			iTween.ScaleTo(AnimationPanel, iTween.Hash("x", 1.3f,"y", 1.3f,"islocal",true,"time",Showtime,"easetype",iTween.EaseType.easeInCubic));
			iTween.ScaleTo(AnimationPanel, iTween.Hash("x", 1f,"y", 1f,"islocal",true,"time",Showtime,"delay",Showtime,"easetype",iTween.EaseType.easeInCubic));
		} 
		else
		{
			localScal.x = 1;
			localScal.y = 1;
			if (CloseOpenPanelAfreAnimation != null) {
				CloseOpenPanelAfreAnimation.SetActive (true);
			}
//			AnimationPanel.SetActive (true);
			StartCoroutine (ShowMainPanel (CloseOpenPanelAfreAnimation));
			StartCoroutine (HideMainPanel (BackgroundPanel));
			AnimationPanel.GetComponent<RectTransform> ().localScale = localScal;
			iTween.ScaleTo(AnimationPanel, iTween.Hash("x", 0,"y", 0, "islocal",true,"time",Showtime,"easetype",iTween.EaseType.easeOutCubic)); 
		}
	}

	public void OnMoveCenterFromRightAnimation (GameObject AnimationPanel,bool isshow,float timescal)
	{
		Showtime = timescal;
		 
		Vector3 localScal = AnimationPanel.GetComponent<RectTransform> ().localScale;
		float ScreenWidth = Screen.width;
		if (isshow) {
			 
			AnimationPanel.GetComponent<RectTransform> ().localScale = Vector3.one;
			Vector3 posion = AnimationPanel.GetComponent<RectTransform> ().localPosition;
			posion.x = ScreenWidth/2;
			AnimationPanel.GetComponent<RectTransform> ().localPosition = posion;
			iTween.MoveTo (AnimationPanel,iTween.Hash("x",0,"islocal",true,"time",timescal, "easetype",iTween.EaseType.linear));
		} 
		else
		{
			iTween.MoveTo (AnimationPanel,iTween.Hash("x",-ScreenWidth/2,"islocal",true,"time",timescal, "easetype",iTween.EaseType.linear));
			 
		}
	}
	public void OnMoveCenterFromLeftAnimation (GameObject AnimationPanel,bool isshow,float timescal)
	{
		Showtime = timescal;

		Vector3 localScal = AnimationPanel.GetComponent<RectTransform> ().localScale;
		float ScreenWidth = Screen.width;
		if (isshow) {

			AnimationPanel.GetComponent<RectTransform> ().localScale = Vector3.one;
			Vector3 posion = AnimationPanel.GetComponent<RectTransform> ().localPosition;
			posion.x = -(ScreenWidth/2);
			AnimationPanel.GetComponent<RectTransform> ().localPosition = posion;
			iTween.MoveTo (AnimationPanel,iTween.Hash("x",0,"islocal",true,"time",timescal, "easetype",iTween.EaseType.linear));
		} 
		else
		{
			iTween.MoveTo (AnimationPanel,iTween.Hash("x",ScreenWidth/2,"islocal",true,"time",timescal, "easetype",iTween.EaseType.linear));

		}
	}
}
