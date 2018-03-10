/*
just use the “dropPin(double lat, double lng, GameObject go)” method in GOMap class.
Coordinates coordinates = new Coordinates (lat, lng,0);
gameobject.transform.localPosition = coordinates.convertCoordinateToVector(0);

created: 13/12/2017 

*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoMap;
using GoShared;
using UnityEngine.Events;

public class RandomPlacement : MonoBehaviour
{
	public static RandomPlacement instance;
	public GOMap map;
	public GameObject[] prefabs;
	public int[] howmany;
	public Coordinates[] coordinates;
	public Vector2 randomMinMax = new Vector2 (-0.1f, 0.1f);
	public double defAltitude;
	int coordNumber = -1;
	internal ArrayList ChestArray, AnimationArray;
	internal ArrayList ChestObjectArrayList;
	public GameObject ChestInsertObject;
	// Use this for initialization
	void Awake ()
	{
		instance = this;

		map = GameObject.Find ("Map - Flat").GetComponent<GOMap> ();


		if (map == null) {
			Debug.LogWarning ("GOObject - Map property not set");
			return;
		}
		



	}

	internal void OnRemoveAllChest ()
	{
//		if (ChestArray != null) {
//			for (int i = 0; i < ChestArray.Count; i++) {
//				GameObject Objchest = ChestArray [i] as GameObject;
//				if (Objchest != null) {
//					Destroy (Objchest);
//				}
//			}
//		}
//		ChestArray = new ArrayList ();
	}

	internal void OnMakeChestOnLocation (string Chestid, double latitude, double longitude, float FreezeTime)
	{
		if (ChestArray == null) {
			ChestArray = new ArrayList ();
		}
		if (ChestObjectArrayList == null) {
			ChestObjectArrayList = new ArrayList ();
		}
		if (AnimationArray == null) {
			AnimationArray = new ArrayList ();
		}
		if (!ChestArray.Contains (Chestid)) {
			print (Chestid);
			GameObject instanceObj = (GameObject)Instantiate (prefabs [0]);
			coordNumber++;
			instanceObj.transform.SetParent (ChestInsertObject.transform, false);
			instanceObj.name = prefabs [0].name + "_" + Chestid;
			instanceObj.GetComponent<ItemAnimation> ().ChestId = Chestid;
			instanceObj.SetActive (true);
			instanceObj.GetComponent<ItemAnimation> ().FreezeTime = FreezeTime;
			instanceObj.GetComponent<ItemAnimation> ().OnStartTImer (FreezeTime);

			instanceObj.GetComponent<ItemAnimation> ().ChestLocation = new Vector2 ((float)latitude, (float)longitude); 

			if (Mathf.Approximately ((float)coordinates [coordNumber].latitude, 0f)) {
				coordinates [coordNumber] = new Coordinates (
					System.Math.Round (LocationManager.CenterWorldCoordinates.latitude + (double)Random.Range (randomMinMax.x, randomMinMax.y), 4),
					System.Math.Round (LocationManager.CenterWorldCoordinates.longitude + (double)Random.Range (randomMinMax.x, randomMinMax.y), 4),
					defAltitude);
				
				//	Debug.Log ("it's 0, lat: "+coordNumber+ " :: "+coordinates [coordNumber].latitude);
			}
		
			map.dropPin (latitude, longitude, instanceObj);

			Coordinates coordinat = new Coordinates (latitude, longitude, 0); 
			instanceObj.transform.localPosition = coordinat.convertCoordinateToVector ();
			instanceObj.transform.Translate (Vector3.up * instanceObj.transform.localScale.y / 2f, Space.World);

			ChestArray.Add (Chestid); 
			ChestObjectArrayList.Add (instanceObj);
		} else {
			for (int i = 0; i < ChestObjectArrayList.Count; i++) {
				GameObject obj = ChestObjectArrayList [i] as GameObject;
//				if (!AnimationArray.Contains (obj.GetComponent<ItemAnimation> ().ChestId)) {
//					print ("Chest Id Is Not Found");
//				} else if (obj.GetComponent<ItemAnimation> ().ChestId == Chestid) {
//
//					print ("ChestIdIsFound");
//				obj.SetActive (true);
//				obj.GetComponent<ItemAnimation> ().StopAllCoroutines ();
				if (obj.GetComponent<ItemAnimation> ().ChestId == Chestid & obj.GetComponent<ItemAnimation> ().IsAnimated == false) {
					if (FreezeTime > 0) {
						print (FreezeTime + "   " + obj.GetComponent<ItemAnimation> ().ChestId);
						obj.SetActive (false);
						obj.GetComponent<ItemAnimation> ().FreezeTime = 0;
						obj.GetComponent<ItemAnimation> ().OnStartTImer (FreezeTime);
					} else {
						obj.GetComponent<ItemAnimation> ().FreezeTime = FreezeTime;
						obj.GetComponent<ItemAnimation> ().OnStartTImer (FreezeTime);
						print (FreezeTime + "   " + obj.GetComponent<ItemAnimation> ().ChestId);
					}
				}



//				}
			}
		}
	}

	void Start ()
	{

		StartCoroutine (CreateObjects ()); 
	}

	IEnumerator CreateObjects ()
	{


		while (!LocationManager.IsOriginSet) {
			yield return null;
		}

		yield return new WaitForSeconds (0.25f);

		for (int p = 0; p < prefabs.Length; p++) {
			for (int i = 0; i < howmany.Length; i++) {


//
//                GameObject instance = (GameObject)Instantiate(prefabs[p]);
//
//                coordNumber++;
//                instance.name = prefabs[p].name + "_" + coordNumber.ToString();
//
//
//
//                if (Mathf.Approximately((float)coordinates[coordNumber].latitude, 0f))
//                {
//                    coordinates[coordNumber] = new Coordinates(
//                            System.Math.Round(LocationManager.CenterWorldCoordinates.latitude + (double)Random.Range(randomMinMax.x, randomMinMax.y), 4),
//                            System.Math.Round(LocationManager.CenterWorldCoordinates.longitude + (double)Random.Range(randomMinMax.x, randomMinMax.y), 4),
//                            defAltitude);
//
//                    //	Debug.Log ("it's 0, lat: "+coordNumber+ " :: "+coordinates [coordNumber].latitude);
//                }
//
//                map.dropPin(coordinates[coordNumber].latitude, coordinates[coordNumber].longitude, instance);
//
//
//
//                instance.transform.localPosition = coordinates[coordNumber].convertCoordinateToVector();
//                instance.transform.Translate(Vector3.up * instance.transform.localScale.y / 2f, Space.World);

				yield return new WaitForSeconds (0.01f); // relax processor
			}
		}
		// yield return new WaitForSeconds (1f);
		StopAllCoroutines ();
	}

   
}
