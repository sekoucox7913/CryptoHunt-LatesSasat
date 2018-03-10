using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class SwapItemScript : MonoBehaviour
{
	public static SwapItemScript instance;
	public Image ImgQuestQuestionIcon;
	public GameObject BtnNext, BtnPrevious;
	public GameObject SwapItemPanel, SwapItemSubPanel;
	//	public GameObject[] ImgButtonLayer;
	public ArrayList ItemButtonPanel;
	internal int ButtonPanelIndex = 0;
	string itemId;
	public SwapItemCellScript itemPrefeb;
	public GameObject MainPanel, ScrollPanel;
	internal int ItemCount, OldCount, rowCount, columnCount = 1;

	void Awake ()
	{
		instance = this;
		SwapItemPanel.SetActive (false);
	}

	internal void OnSwapItemOpen ()
	{
		AnimationScript.Inst.OnScalByXYAnimation (null, SwapItemPanel, SwapItemSubPanel, true, 0.359f);
		OnResetSelection ();
	}

	public void OnSwapItemCloseButtonClick ()
	{
		AnimationScript.Inst.OnScalByXYAnimation (null, SwapItemPanel, SwapItemSubPanel, false, 0.359f);
	}

	internal void OnSetQuesIcon (string item_id)
	{
		itemId = item_id;
		TaskScreenScript.instance.OnSetImages (itemId, ImgQuestQuestionIcon);

	}

	public void OnNextButtonClick ()
	{
		ButtonPanelIndex++;
		if (ButtonPanelIndex < ItemButtonPanel.Count) {
			SwapItemCellScript objCurrent = ItemButtonPanel [ButtonPanelIndex - 1] as SwapItemCellScript;
			SwapItemCellScript objNext = ItemButtonPanel [ButtonPanelIndex] as SwapItemCellScript;
			AnimationScript.Inst.OnMoveCenterFromRightAnimation (objCurrent.gameObject, false, 0.539f);
			AnimationScript.Inst.OnMoveCenterFromRightAnimation (objNext.gameObject, true, 0.539f);
			BtnPrevious.GetComponent<Button> ().interactable = true;
		}
		if (ItemButtonPanel.Count - 1 <= ButtonPanelIndex) {
			ButtonPanelIndex = ItemButtonPanel.Count - 1;
			BtnNext.GetComponent<Button> ().interactable = false;
			BtnPrevious.GetComponent<Button> ().interactable = true;
		}
		SoundManagerScript.instance.OnPlaySlideSound ();
	}

	public void OnPreviousButtonClick ()
	{
		ButtonPanelIndex--;
		if (ButtonPanelIndex >= 0) {
			SwapItemCellScript objCurrent = ItemButtonPanel [ButtonPanelIndex + 1] as SwapItemCellScript;
			SwapItemCellScript objPrevious = ItemButtonPanel [ButtonPanelIndex] as SwapItemCellScript;
			AnimationScript.Inst.OnMoveCenterFromLeftAnimation (objCurrent.gameObject, false, 0.539f);
			AnimationScript.Inst.OnMoveCenterFromLeftAnimation (objPrevious.gameObject, true, 0.539f);
			BtnNext.GetComponent<Button> ().interactable = true;
		}

		if (ButtonPanelIndex <= 0) {
			ButtonPanelIndex = 0;
			BtnNext.GetComponent<Button> ().interactable = true;
			BtnPrevious.GetComponent<Button> ().interactable = false;
		}
		SoundManagerScript.instance.OnPlaySlideSound ();
	}

	public void OnSwapButtonClick ()
	{
//		OnSwapItemCloseButtonClick ();
//		interact_items/exchange/try
		PreloaderScript.instance.OnEnabledLoder ();
		JSONObject newobj = new JSONObject ();
		if (ItemButtonPanel == null) {
			ItemButtonPanel = new ArrayList ();
		}
		for (int a = 0; a < ItemButtonPanel.Count; a++) {
			SwapItemCellScript obj = ItemButtonPanel [a] as SwapItemCellScript;
			for (int i = 0; i < obj.ImgButtonLayer.Length; i++) {
				if (obj.ImgButtonLayer [i].activeSelf) {
					JSONObject itemarray = new JSONObject ();

					itemarray.AddField ("item_id", obj.ItemBtns [i].ItemId);
					newobj.Add (itemarray);
				}
			}
		}
		Cipherhex_WebSocket.instance.OnGetInteract_ItemsExchangeDetails (newobj);
	}


	internal void OnSetAllItems (ArrayList array, ArrayList array1)
	{
		print ("Hellooo");
		if (ItemButtonPanel == null) {
			ItemButtonPanel = new ArrayList ();
		}

		ItemCount = array.Count;
		if (ItemCount != 0) {
			if (ItemCount > 10) {
				columnCount = ItemCount / 10;
				if (ItemCount % columnCount > 0) {
					columnCount++;
				}
			} else {
				columnCount = 1;
			}
		}  
		if (ItemButtonPanel.Count != columnCount) {
			OnRemoveAllItems ();
		}
		if (ItemButtonPanel.Count != array.Count) {  
			for (int i = 0; i < columnCount; i++) { 
				SwapItemCellScript cell = Instantiate (itemPrefeb, ScrollPanel.transform, false);
				for (int j = 0; j < cell.ItemBtns.Length; j++) { 
					int index = (i * cell.ItemBtns.Length) + j;
					if (index < array.Count) {
						string id = array [index] as string;

						cell.ItemBtns [j].ItemId = id;
						cell.ItemBtns [j].txtNoOfCoin.text = (string)array1 [index];  
						TaskScreenScript.instance.OnSetImages (id, cell.ItemBtns [j].img);
					}

				}
				ItemButtonPanel.Add (cell);
			}
		} else { 
			for (int i = 0; i < columnCount; i++) { 
				SwapItemCellScript cell = ItemButtonPanel [i] as SwapItemCellScript;
				for (int j = 0; j < cell.ItemBtns.Length; j++) {
					int index = (i * cell.ItemBtns.Length) + j;
					if (index < array.Count) {
						string id = array [index] as string;

						cell.ItemBtns [j].ItemId = id;
						cell.ItemBtns [j].txtNoOfCoin.text = (string)array1 [index];  
						TaskScreenScript.instance.OnSetImages (id, cell.ItemBtns [j].img);
					}
				}
			}
		}
		for (int a = 1; a < ItemButtonPanel.Count; a++) {
			SwapItemCellScript obj = ItemButtonPanel [a] as SwapItemCellScript;
			AnimationScript.Inst.OnMoveCenterFromLeftAnimation (obj.gameObject, false, 0.01f);
		}
	}

	internal void OnRemoveAllItems ()
	{
		if (ItemButtonPanel != null) {
			for (int i = 0; i < ItemButtonPanel.Count; i++) {
				SwapItemCellScript obj = ItemButtonPanel [i] as SwapItemCellScript;
				Destroy (obj.gameObject);
			}
		}
	}

	internal  void OnResetSelection ()
	{
		if (ItemButtonPanel == null) {
			ItemButtonPanel = new ArrayList ();
		}
		for (int a = 0; a < ItemButtonPanel.Count; a++) {
			SwapItemCellScript obj = ItemButtonPanel [a] as SwapItemCellScript;
			for (int i = 0; i < obj.ImgButtonLayer.Length; i++) {
				obj.ImgButtonLayer [i].SetActive (false);
			}
		}
	}
}
