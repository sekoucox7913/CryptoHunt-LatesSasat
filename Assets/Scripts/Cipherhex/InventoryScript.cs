using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


public class InventoryScript : MonoBehaviour
{
	public static InventoryScript instance;
	public ArrayList InventoryListArray, InventoryCount, InventoryArray;
	public Sprite defaultSprite;

	public InventoryCellBtn itemPrefeb;
	public GameObject MainPanel, ScrollPanel, NoItemMessage;

	void Awake ()
	{
		instance = this;

	}

	void Start ()
	{
//		JSONObject data = new JSONObject ();
//		data.AddField ("2", "5");
//		data.AddField ("3", "1");
//		data.AddField ("4", "2");
//		JSONObject jdata = new JSONObject ();
//		jdata.AddField ("data", data);
//		OnInventoryListResponse (jdata);

		float GridSize = (MainPanel.GetComponent<RectTransform> ().rect.height / 2) - 10;
		ScrollPanel.GetComponent<GridLayoutGroup> ().cellSize = new Vector2 (GridSize, GridSize);
	}

	internal void OnInventoryListResponse (JSONObject data)
	{
		 
		if (UserSettingScript.instance.SpecialItemKeyArray == null) {
			UserSettingScript.instance.SpecialItemKeyArray = new ArrayList (); 
		} 
		if (UserSettingScript.instance.SpecialItemValueArray == null) {
			UserSettingScript.instance.SpecialItemValueArray = new ArrayList (); 
		} 
		if (InventoryListArray == null) {
			InventoryListArray = new ArrayList (); 
		} 
		if (InventoryArray == null) {
			InventoryArray = new ArrayList (); 
		}
		if (InventoryCount == null) {
			InventoryCount = new ArrayList (); 
		}
		 
		JSONObject jdata = data.GetField ("data");  
		NoItemMessage.SetActive (true);
		if (jdata.Count > 0) {
			NoItemMessage.SetActive (false);
		}
		if (jdata.Count != (InventoryListArray.Count + UserSettingScript.instance.SpecialItemKeyArray.Count)) {		 
			OnRemoveAllObjects ();
			UserSettingScript.instance.OnRemoveAllObjects ();
			print (" JdataCount " + jdata.Count);
			InventoryCount = new ArrayList ();
			InventoryArray = new ArrayList ();
			InventoryListArray = new ArrayList (); 

			if (jdata.Count > 0) {
				string[] list = jdata.Print ().Trim ('"').ToString ().Trim ('"').ToString ().Trim ('{').ToString ().Trim ('}').Split (','); 

				for (int i = 0; i < list.Length; i++) {
					string[] newlist = list [i].ToString ().Trim ('"').Split (':');

					int value = int.Parse (newlist [1].ToString ().Trim ('"')); 
					string key = newlist [0].ToString ().Trim ('"'); 
					if (TaskScreenScript.instance.ItemType (key).ToUpper () == "S") {
						UserSettingScript.instance.SpecialItemKeyArray.Add (key.ToString ());
						UserSettingScript.instance.SpecialItemValueArray.Add (value.ToString ());
					} else {
						InventoryCount.Add (value.ToString ());  
						InventoryListArray.Add (key.ToString ());

					}

					//				}
				}
			} 
			if (InventoryArray.Count != InventoryListArray.Count) {
//				print ("18" + InventoryListArray.Count);
				OnSetDataOnInventory (InventoryListArray, InventoryCount);	
				SwapItemScript.instance.OnSetAllItems (InventoryListArray, InventoryCount);	
				UserSettingScript.instance.OnSetItem ();
			}
		} else {
			UserSettingScript.instance.OnSetItem ();
			for (int i = 0; i < InventoryArray.Count; i++) {
				InventoryCellBtn cell = InventoryArray [i] as InventoryCellBtn;
				cell.txtNoOfCoin.text = (string)InventoryCount [i];
			}
		}
	}

	internal void OnSetDataOnInventory (ArrayList array, ArrayList array1)
	{
		if (InventoryArray == null) {
			InventoryArray = new ArrayList ();
		}
		OnRemoveAllObjects ();
		for (int i = 0; i < array.Count; i++) {
			string id = array [i] as string; 
			InventoryCellBtn cell = Instantiate (itemPrefeb, ScrollPanel.transform, false);
			cell.ItemId = id;
			cell.ItemType = "I";
			cell.txtNoOfCoin.text = (string)array1 [i];
//			StartCoroutine (cell.LoadImg (url));
			InventoryArray.Add (cell);

			TaskScreenScript.instance.OnSetImages (id, cell.img);
//			if (i < TaskScreenScript.instance.BtnImgItem.Length) {
//				TaskScreenScript.instance.OnSetImages (id, TaskScreenScript.instance.BtnImgItem [i]);
//			}
		}
	}

	internal void OnSetFirstPosition ()
	{
		Vector3 POS = ScrollPanel.GetComponent<RectTransform> ().localPosition;
		POS.x = MainPanel.GetComponent<RectTransform> ().rect.width / 2;
		ScrollPanel.GetComponent<RectTransform> ().localPosition = POS;
	}

	internal void OnRemoveAllObjects ()
	{ 
		if (InventoryArray != null) {
			for (int i = 0; i < InventoryArray.Count; i++) {
				InventoryCellBtn obj = InventoryArray [i] as InventoryCellBtn;
				Destroy (obj.gameObject);
			
			}
		}
	}
}
