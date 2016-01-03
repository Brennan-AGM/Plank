using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class game_UIController : MonoBehaviour {

	[Header("TILEREFERENCE")]
	public GameObject[]  ShownTileHolder;
	public GameObject[]  PlayerTileHolder;
	public GameObject  HiddenTileHolder;

	[Header("RESPONSE")]
	public GameObject[]  PlayerAnswer;
	public GameObject[]  PlayerTurn;

	[Header("RESULTS")]
	public GameObject  Correct_gmobj;
	public GameObject  Wrong_gmobj;
	public GameObject  Winner_gmobj;

	public void Reset()
	{
		ClearChildren(ShownTileHolder);
		ClearChildren(PlayerTileHolder);
		ClearChildren(HiddenTileHolder);
	}

	public GameObject GetTileHolder(int id = 0, int type = 0)
	{
		switch(type)
		{
		case 0:
			//id = (id > 6) ? id : 0;
			return ShownTileHolder[id];
			break;
		case 1:
			//id = (id > 3) ? id : 0;
			return PlayerTileHolder[id];
			break;
		case 2:
			return HiddenTileHolder;
			break;
		default:
			return ShownTileHolder[0];
		}
	}

	public void DetermineResult(bool value)
	{
		if(value)
			Correct_gmobj.SetActive(true);
		else
			Wrong_gmobj.SetActive(true);
	}

	public void GetWinner(string value)
	{
		Winner_gmobj.SetActive(true);
		Winner_gmobj.GetComponentInChildren<Text>().text = value;
	}

	public GameObject GetPlayerTurn(int value)
	{
		return PlayerTurn[value];
	}

	public void SetPlayerTurn(int value, int result)
	{
		Image target = PlayerTurn[value].GetComponent<Image>();
		if(result == 1)
		{
			target.color = Color.green;
		}
		else if(result == 2)
		{
			target.color = Color.red;
		}
		else if(result == 0)
		{
			target.color = Color.white;
		}

	}

	public void GetPlayerAnswer(int value, int AiID)
	{
		PlayerAnswer[AiID].SetActive(true);
		PlayerAnswer[AiID].GetComponent<Image>().sprite = (Sprite)Resources.Load("Number/Number" + value, typeof(Sprite)); 
	}

	public void RemovePlayerAnswer(int AiID)
	{
		PlayerAnswer[AiID].SetActive(false);
	}

	void ClearChildren(GameObject[] List)
	{
		for(int i = 0; i < List.Length; i++)
		{
			for(int j = 0; j < List[i].transform.childCount; j++)
			{
				Destroy(List[i].transform.GetChild(j).gameObject);
			}
		}
	}

	void ClearChildren(GameObject List)
	{
		for(int i = 0; i < List.transform.childCount; i++)
		{
			Destroy(List.transform.GetChild(i).gameObject);
		}
	}
}
