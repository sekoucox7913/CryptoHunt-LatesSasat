using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class OnMouseDownActions : MonoBehaviour {
	public GameObject canvas3D, canvasUI, panel;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	void OnMouseDown()
	{
		
		StartCoroutine ("showUI");
	}

	public void HideCanvas3D()
	{
		canvas3D.SetActive (false);
	}


	IEnumerator showUI ()
	{
		
		Debug.Log ("skrinja opened");
		gameObject.GetComponent<Animator>().SetBool ("open",true);

		canvas3D.transform.SetParent (null);

		yield return new WaitForSeconds (1.5f);

		gameObject.GetComponent<Animator>().SetBool ("scaleDown",true);


		canvas3D.GetComponent<Animator> ().SetBool ("scaleUp", true);


		yield return new WaitForSeconds (3f);

		canvasUI.SetActive (true);

		panel.GetComponent<Animator>().SetBool ("setAlpha",true);

		yield return new WaitForSeconds (1f);

		Destroy (canvas3D.gameObject);
		Destroy (gameObject);
		// StopCoroutine ("showUI");

	}

}
