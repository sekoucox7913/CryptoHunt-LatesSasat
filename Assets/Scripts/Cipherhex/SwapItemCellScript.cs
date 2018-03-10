using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwapItemCellScript : MonoBehaviour
{
	
	public InventoryCellBtn[] ItemBtns;
	public GameObject[] ImgButtonLayer;

	public void OnItemButtonClick (int ItemIndex)
	{
		if (ItemBtns [ItemIndex].ItemId != "0") {
			if (!ImgButtonLayer [ItemIndex].activeSelf) {
				ImgButtonLayer [ItemIndex].SetActive (true);
			} else {
				ImgButtonLayer [ItemIndex].SetActive (false);
			}
		}

	}

}
