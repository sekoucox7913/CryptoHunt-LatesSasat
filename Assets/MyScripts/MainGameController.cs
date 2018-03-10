using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainGameController : MonoBehaviour
{
	public static MainGameController instance;
	public Camera cam1;
	//    public Camera cam2;

	public Camera closecam1;
	public Camera closecam2;
	public Canvas closeCanvas;


	public GameObject sporniPanel;

	//    public GameObject MenuCont;
	public GameObject CloseContainer;
	public GameObject MapContainer;
	public GameObject closeArea;
	public GameObject chest;
    
	public AudioSource btnClick;

	public GameObject ObjChestCamera;
    
	int brojac = 0;
	internal bool IsARScene;
	internal float HideChestTime = 40;

	public AudioSource MainGameMusic;
	public AudioSource CharSelectMusic;
	public AudioSource slideEffect;
	public GameObject ObjForCoin;
	// public GameObject compasNeedle;
	GameObject objChest1;

	public Canvas baner;
	public GameObject ObjCharacter;




	//    public Game	Object editPop;

	 

	//    public GameObject PopUp;


	bool popIsActivated = true;


	private void Start ()
	{
		Input.compass.enabled = true;
		Input.location.Start ();
//        pozicija = PopUp.transform.position;

//        MainGameMusic.Pause();
//        CharSelectMusic.Play();

//        cam1.enabled = false;
////        cam2.enabled = true;
//        canv1.enabled = false;
//
//        closecam1.enabled = false;
//        closecam2.enabled = false;
//        closeCanvas.enabled = false;

//        MenuCont.SetActive(true);
		CloseContainer.SetActive (false);
//        MapContainer.SetActive(true);



	}

	void Awake ()
	{
		instance = this;
	}

	public void Update ()
	{
//        Tab();
		//compasNeedle.transform.rotation = Quaternion.Euler(-0, 0, Input.compass.magneticHeading);
	}


	public void OnButtonClick ()
	{
//        cam1.enabled = false;
//        canv1.enabled = false;
//        cam2.enabled = true;
//        baner.enabled = false;


//        MenuCont.SetActive(true);
//        btnClick.Play();
//
//        MainGameMusic.Pause();
//        CharSelectMusic.Play();


	}


	public void OnButtonBackClick ()
	{

//        cam2.enabled = false;
		cam1.enabled = true;
//        canv1.enabled = true;

//        CharSelectMusic.Pause();
//        MainGameMusic.Play();



//        MenuCont.transform.Translate(1800f, 0, 0);
//        MenuCont.SetActive(false);
//        btnClick.Play();
		baner.enabled = true;
	}


	public void OnButtonSettingsClick ()
	{
		cam1.enabled = false;
//        cam2.enabled = true;
//        canv1.enabled = false;

//        MenuCont.SetActive(true);
//        MenuCont.transform.Translate(-2700f, 0, 0);
//        btnClick.Play();

//        MainGameMusic.Pause();
//        CharSelectMusic.Play();
		baner.enabled = false;
	}


	public void OnButtonBackFromSettingsClick ()
	{
		cam1.enabled = true;
//        cam2.enabled = false;
//        canv1.enabled = true;

//        MenuCont.transform.Translate(2700f, 0, 0);
//        MenuCont.SetActive(false);
//        btnClick.Play();
//        CharSelectMusic.Pause();
//        MainGameMusic.Play();
		baner.enabled = true;
	}



	public void OnButtonRiddleClick ()
	{
		cam1.enabled = false;
//        cam2.enabled = true;
//        canv1.enabled = false;

//        MenuCont.SetActive(true);
//        MenuCont.transform.Translate(-3600f, 0, 0);
//        btnClick.Play();
//        MainGameMusic.Pause();
//        CharSelectMusic.Play();
		baner.enabled = false;
	}

	public void OnButtonBackFromRiddleClick ()
	{
		cam1.enabled = true;
//        cam2.enabled = false;
//        canv1.enabled = true;

//        MenuCont.transform.Translate(3600f, 0, 0);
//        MenuCont.SetActive(false);
//        btnClick.Play();
//        CharSelectMusic.Pause();
//        MainGameMusic.Play();
		baner.enabled = true;
	}







	public void FromUserMenuToWalletClick ()
	{

//        MenuCont.transform.Translate(-3600f, 0, 0);
//        btnClick.Play();
	}


	public void FromUserMenuToHowToClick ()
	{

//        MenuCont.transform.Translate(-4500f, 0, 0);
//        btnClick.Play();
	}

	public void FromWalletToUserMenuClick ()
	{

//        MenuCont.transform.Translate(3600f, 0, 0);
//        btnClick.Play();
	}

	public void FromHowToToUserMenuClick ()
	{

//        MenuCont.transform.Translate(4500f, 0, 0);
//        btnClick.Play();
	}


  


	public void ShowStoreAndLeaderPopupMessage ()
	{


		if (popIsActivated == true) {
//            PopUp.transform.position = pozicija;
			StartCoroutine (PopUpMetod ());
		}


	}

	IEnumerator PopUpMetod ()
	{
		popIsActivated = false;
//        PopUp.GetComponent<Animation>().Play("msgScale");

		yield return new WaitForSeconds (3f);

//        PopUp.GetComponent<Animation>().Play("msgTrans");
		popIsActivated = true;
		yield return null;
	}




	public void EditPopOpen ()
	{

//        editPop.GetComponent<Animation>().Play("editOpen");

	}

	public void EditPopClose ()
	{

//        editPop.GetComponent<Animation>().Play("editClose");

	}


	// i ovdje potrebna dorada



	// Tab
	internal void OnChestOnAR (GameObject OldObject)
	{
		if (UserSettingScript.instance.TglAR.isOn) {
			cam1.enabled = false; 
			MapContainer.SetActive (false);
			CloseContainer.SetActive (true);
			closecam1.enabled = true;
			closecam2.enabled = true; 
			IsARScene = true;
			closeCanvas.enabled = true;
			CloseContainer.SetActive (true);  
			GameObject objChest = Instantiate (chest, closeArea.transform.position, closeArea.transform.rotation);
			objChest.transform.LookAt (ObjChestCamera.transform);
			Vector3 chestPos = closeArea.transform.position;
//			chestPos.z += 5;
			objChest.transform.localRotation = Quaternion.Euler (0, 180f, 0);
			objChest.transform.position = chestPos;
			objChest.transform.localScale = new Vector3 (3, 3, 3);
			objChest1 = objChest;
			ObjCharacter.SetActive (false);

			objChest.GetComponent<ItemAnimation> ().ChestId = OldObject.GetComponent<ItemAnimation> ().ChestId;
			objChest.GetComponent<ItemAnimation> ().IsInteractable = OldObject.GetComponent<ItemAnimation> ().IsInteractable;
			objChest.GetComponent<ItemAnimation> ().ChestLocation = OldObject.GetComponent<ItemAnimation> ().ChestLocation;
			objChest.GetComponent<ItemAnimation> ().ItemCoin = OldObject.GetComponent<ItemAnimation> ().ItemCoin;
			objChest.GetComponent<ItemAnimation> ().ItemChest = OldObject.GetComponent<ItemAnimation> ().ItemChest;


//			hit.collider.gameObject.SetActive (false);
//			StartCoroutine (ShowChestOnMap (hit.collider.gameObject));
		} 
	}

	#region MyRegion

	void Tab ()
	{
		if (Input.touchCount > 0 && Input.GetTouch (0).phase == TouchPhase.Ended) {
			Vector3 mousePosFar = new Vector3 (Input.mousePosition.x, Input.mousePosition.y, Camera.main.farClipPlane);
			Vector3 mousePosNear = new Vector3 (Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane);

			Vector3 mousePosF = Camera.main.ScreenToWorldPoint (mousePosFar);
			Vector3 mousePosN = Camera.main.ScreenToWorldPoint (mousePosNear);

			RaycastHit hit;

			if (Physics.Raycast (mousePosN, mousePosF - mousePosN, out hit)) {
				if (hit.collider.tag.Equals ("MapChest")) {
					print ("AR part is On============================");
					if (UserSettingScript.instance.TglAR.isOn) {
//						cam1.enabled = false;
//						canv1.enabled = false;
//						MapContainer.SetActive (false);
//						CloseContainer.SetActive (true);
//						closecam1.enabled = true;
//						closecam2.enabled = true;
//
//
//						closeCanvas.enabled = true;
//
//						// jako bitna stvar
//						// closeImg.enabled = true;
//
//
////                    Instantiate(chest, closeArea.transform.position, closeArea.transform.rotation);
//						GameObject objChest = Instantiate (chest, closeArea.transform.position, closeArea.transform.rotation);
//						objChest.transform.LookAt (ObjChestCamera.transform);
//						objChest.transform.Rotate (0, 130f, 0);
//
//						hit.collider.gameObject.SetActive (false);
//						StartCoroutine (ShowChestOnMap (hit.collider.gameObject));sssss
					} else {
						
					}
				}


			}
		}


	}

	#endregion

	public IEnumerator ShowChestOnMap (GameObject ObjectChest)
	{
		yield return new WaitForSeconds (HideChestTime);
		ObjectChest.SetActive (true);
	}

	//void CloseTab()
	//{
	//    if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
	//    {
	//        Vector3 mousePosFar = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.farClipPlane);
	//        Vector3 mousePosNear = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane);

	//        Vector3 mousePosF = Camera.main.ScreenToWorldPoint(mousePosFar);
	//        Vector3 mousePosN = Camera.main.ScreenToWorldPoint(mousePosNear);

	//        RaycastHit hit;

	//        if (Physics.Raycast(mousePosN, mousePosF - mousePosN, out hit))
	//        {


	//            if (hit.collider.tag.Equals("CloseChest"))
	//            {

	//                chest.SetActive(false);

	//            }
	//        }
	//    }


	//}



	public void OnButtonBackFromCloseClick ()
	{


//        cam2.enabled = false;

		ObjCharacter.SetActive (true);
		closecam1.enabled = false;
		closecam2.enabled = false;
		closeCanvas.enabled = false;
		CloseContainer.SetActive (false);
//        MenuCont.SetActive(false);
		// closeImg.enabled = false;
		sporniPanel.SetActive (false);
        
		MapContainer.SetActive (true);
		cam1.enabled = true;
//        canv1.enabled = true;
//        btnClick.Play();
		if (objChest1 != null) {
			Destroy (objChest1);
		}

	}

	public void OnButtonSliderClick ()
	{
		if (brojac % 2 == 0) {
//            btn1.GetComponent<Animation>().Play("TaskRight");
//            btn2.GetComponent<Animation>().Play("UserMenuRight");
//            btn3.GetComponent<Animation>().Play("StoreRight");
//            btn4.GetComponent<Animation>().Play("LeaderRight");
//            slideEffect.Play();
		} else if (brojac % 2 == 1) {
//            btn1.GetComponent<Animation>().Play("TaskLeft");
//            btn2.GetComponent<Animation>().Play("UserMenuLeft");
//            btn3.GetComponent<Animation>().Play("StoreLeft");
//            btn4.GetComponent<Animation>().Play("LeaderLeft");
//            slideEffect.Play();
		}

		brojac++;
		if (brojac == 256) {
			brojac = 0;
		}

	}
}
