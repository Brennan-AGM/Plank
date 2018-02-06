using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class game_TileAsset : MonoBehaviour {

	public Image currentImage;

	public void LoadAsset(int value)
	{
		//Debug.Log("VALUE:" + value);
		//Image currentImage = GetComponentInChildren<Image>();
		currentImage.sprite = (Sprite)Resources.Load("Number/Number" + value, typeof(Sprite));
	}
}
