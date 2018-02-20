using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;

public class game_UIController : MonoBehaviour {

	[Header("TILEREFERENCE")]
    [SerializeField]
    private GameObject[]  ShownTileHolder;
    [SerializeField]
    private GameObject[]  PlayerTileHolder;
    [SerializeField]
    private GameObject  HiddenTileHolder;

	[Header("RESPONSE")]
    [SerializeField]
	private TileNumber[]  PlayerAnswer;
    [SerializeField]
    private GameObject[]  PlayerTurn;

	[Header("RESULTS")]
    [SerializeField]
    private GameObject  Correct_gmobj;
    [SerializeField]
    private GameObject  Wrong_gmobj;
    [SerializeField]
    private GameObject  Winner_gmobj;


    [Header("Button")]
    public Button pick_btn;

    public static game_UIController instance = null;
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }

        instance = this;
    }

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
		case 1:
			//id = (id > 3) ? id : 0;
			return PlayerTileHolder[id];
		case 2:
			return HiddenTileHolder;
		default:
			return ShownTileHolder[0];
		}
	}

	public void DetermineResult(eTURNRESULT result)
	{
		if(result == eTURNRESULT.CORRECT)
        {
            Correct_gmobj.SetActive(true);
        }
		else if (result == eTURNRESULT.WRONG)
        {
            Wrong_gmobj.SetActive(true);
        }
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

	public void SetPlayerTurn(int value, eTURNRESULT result)
	{
		Image target = PlayerTurn[value].GetComponent<Image>();
        switch(result)
        {
            case eTURNRESULT.CORRECT:
                target.DOColor(Color.green, 0.5f);
                break;
            case eTURNRESULT.WRONG:
                target.DOColor(Color.red, 0.5f);
                break;
            case eTURNRESULT.TURN:
                target.DOColor(Color.white, 0.0f);
                break;
        }
	}

	public void GetPlayerAnswer(int value, int AiID)
	{
		PlayerAnswer[AiID].gameObject.SetActive(true);
        PlayerAnswer[AiID].SetNumber(value);
	}

	public void RemovePlayerAnswer(int AiID)
	{
		PlayerAnswer[AiID].gameObject.SetActive(false);
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

    public void ShowTiles(int value)
    {
        TileController.instance.InstantiateTiles(value, -1);
    }
}
