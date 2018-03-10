using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomItemGenerator : MonoBehaviour {

	public static RandomItemGenerator Inst;
	public Sprite[] ChestItem;
	public string indexOfItem;
	public int indexItem;
	ArrayList PastSpriteShow = new ArrayList();
	internal Sprite NewSprite;
	void Awake()
	{
		Inst = this;
	}
	internal void OnCreteNewSprite(string name)
	{
		SoundManagerScript.instance.OnVibration ();
		if ("Chest_" + indexOfItem == name) {
			NewSprite = ChestItem [indexItem];
		}

		else if (PastSpriteShow.Count < ChestItem.Length) { 
			bool isFind = false; 
			while (!isFind) {
				int index = Random.Range (0, ChestItem.Length);
				if (!PastSpriteShow.Contains (index)) { 
					isFind = true;
					PastSpriteShow.Add (index);
					NewSprite = ChestItem[index];
				}
			}
		} else {
			PastSpriteShow = new ArrayList();
			OnCreteNewSprite (gameObject.name);
		}
	}

}
