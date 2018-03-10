using UnityEngine;
using System.Collections;

using UnityEngine.UI;
using System.Linq;
using System.Text.RegularExpressions;
using System;
using GoMap;
using System.Collections.Generic;

namespace GoShared {

	public class GOToLocationDemo : MonoBehaviour {

		public InputField inputField;
		public Button button;
		public GOMap goMap;
		public GameObject addressMenu;

		GameObject addressTemplate;

		public void Start () {
		
			addressTemplate = addressMenu.transform.Find ("Address template").gameObject;

			inputField.onEndEdit.AddListener(delegate(string text) {
				GoToAddress();
			});
		}


		public void GoToAddress() {

			if (inputField.text.Any (char.IsLetter)) { //Text contains letters
				SearchAddress();
			} else if (inputField.text.Contains(",")){

				string s = inputField.text;
				Coordinates coords = new Coordinates (inputField.text);
				goMap.locationManager.SetLocation (coords);
				Debug.Log ("NewCoords: " + coords.latitude +" "+coords.longitude);
			}
		}

		public void SearchAddress () {
		
			addressMenu.SetActive (false);

			string baseUrl = "https://search.mapzen.com/v1/search?";
			string apiKey = goMap.mapzen_api_key;
			string text = inputField.text;
			string completeUrl = baseUrl + "&text=" + WWW.EscapeURL(text) + "&api_key=" + apiKey;
			Debug.Log (completeUrl);
			IEnumerator request = GOUrlRequest.jsonRequest (this, completeUrl, false, null, (Dictionary<string,object> response, string error) => {

				if (string.IsNullOrEmpty(error)){
					IList features = (IList)response["features"];
					LoadChoices(features);
				}

			});

			StartCoroutine (request);
		}

		public void LoadChoices(IList features) {

			while (addressMenu.transform.childCount > 1) {
				foreach (Transform child in addressMenu.transform) {
					if (!child.gameObject.Equals (addressTemplate)) {
						DestroyImmediate (child.gameObject);
					}
				}
			}


			for (int i = 0; i<Math.Min(features.Count,5); i++) {

				IDictionary feature = (IDictionary) features [i];

				IDictionary geometry = (IDictionary)feature["geometry"];
				IList coordinates = (IList)geometry["coordinates"];

				IDictionary properties = (IDictionary)feature["properties"];
				Coordinates coords = new Coordinates(Convert.ToDouble( coordinates[1]), Convert.ToDouble(coordinates[0]),0);

				GOLocation location = new GOLocation ();
				location.coordinates = coords;
				location.properties = properties;

				GameObject cell = Instantiate (addressTemplate);
				cell.transform.SetParent(addressMenu.transform);
				cell.transform.GetComponentInChildren<Text> ().text = location.addressString();
				cell.name = location.addressString();
				cell.SetActive (true);

				Button btn = cell.GetComponent<Button> ();
				btn.onClick.AddListener(() => { LoadLocation(location); }); 
			
			}


			addressMenu.SetActive (true);


		}

		public void LoadLocation (GOLocation location) {

			inputField.text = location.addressString();
			addressMenu.SetActive (false);
			goMap.locationManager.SetLocation (location.coordinates);

		}

	}

	[System.Serializable]
	public class GOLocation {

		public Coordinates coordinates;
		public IDictionary properties;

		public string addressString (){
			
			string s = (string)properties ["label"];

			return s;
		}

	}
}
