using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


public class InventoryCellBtn : MonoBehaviour
{
	public string ItemId, url, ItemType;
	public Image img;
	public Text txtNoOfCoin;

	public void OnItemButtonClick ()
	{
		if (ItemType != "S" && ItemType != "s") {
			StartCoroutine (TaskScreenScript.instance.CalledChestItemCollectAuto (ItemId)); 
			Cipherhex_WebSocket.instance.InventoryCell = this;
		}

	}

	public IEnumerator LoadImg (string imageurl)
	{
		if (imageurl != url) {
			url = imageurl;
			Texture2D texture = img.sprite.texture;
			imageurl = imageurl.Replace (@"\", "");
			if (!imageurl.Contains ("https://") && !imageurl.Contains ("http://") && imageurl.Length >= 5) {
				imageurl = " http://s3-eu-west-1.amazonaws.com/ch-game-items/200x200/" + imageurl;
				//ws://games.cipherhex.com:3000/socket.io/?EIO=4&transport=websocket
			}
			WWW www = new WWW (imageurl);
			yield return www;
			if (texture != null && img != null && www.texture != null) {
				if (www.error == "Null" || www.error == "null" || www.error == null) {
					texture = www.texture;
					Rect rect = new Rect (0, 0, texture.width, texture.height);
					Sprite sprite = Sprite.Create (texture, rect, new Vector2 (0.5f, 0.5f));
					img.sprite = sprite;
				}

			}
		}

	}

}
