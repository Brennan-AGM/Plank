using UnityEngine;
using System.Collections;
using System;

public class game_Logic : MonoBehaviour {

	public game_EnemyAI[] AIList;
    private int AIcount;
    private int[] RemovedTiles = new int[3];
    private bool CurrentAITurn;
    private bool Win;
	private bool AIAnswer;

	void Start()
	{
        CurrentAITurn = true;
        for (int i = 0; i < 3; i++)
		{
			RemovedTiles[i] = 0;
		}
	}

	void Reset()
	{
        CurrentAITurn = true;
        for (int i = 0; i < 3 && !Win; i++)
		{
			AIList[i].Reset();
		}
		Win = false;
	}

	public void PlayAI()
	{
		CheckForWinner();
        AIcount = 0;
        while (AIcount < 3 && !Win && CurrentAITurn)
		{
            CurrentAITurn = false;
            if (AIList[AIcount].GetAiType() == 0)
                StartCoroutine(EasyAI(AIcount));
			else if(AIList[AIcount].GetAiType() == 1)
                StartCoroutine(MidAI(AIcount));
			else if(AIList[AIcount].GetAiType() == 2)
                StartCoroutine(HardAI(AIcount));
			else if(AIList[AIcount].GetAiType() == 3)
                StartCoroutine(CheatingAI(AIcount));
		}
	}
    #region AIList
    IEnumerator EasyAI(int value)
	{
		PlayAIDelay(AIList[value].GetBestChoice(), AIList[value].GetAiID());
		if(TileController.tileController.SendResponse(AIList[value].GetBestChoice(), AIList[value].GetAiID()))
		{
			AIList[value].GetChoices();
            AIAnswer = true;
		}
		else{
			AIList[value].RemoveChoice(AIList[value].GetBestChoice());
            AIAnswer = true;
        }
        TurnOnHighlight(AIAnswer, AIList[value].GetAiID(), EndAITurn);
        yield return new WaitForEndOfFrame();
    }

    IEnumerator MidAI(int value)
	{
		PlayAIDelay(AIList[value].GetBestChoice(), AIList[value].GetAiID());
		if(TileController.tileController.SendResponse(AIList[value].GetBestChoice(), AIList[value].GetAiID()))
		{
            AIAnswer = true;
        }
		else{
			AIList[value].RemoveChoice(AIList[value].GetBestChoice());
            AIAnswer = true;
        }
        TurnOnHighlight(AIAnswer, AIList[value].GetAiID(), EndAITurn);
        yield return new WaitForEndOfFrame();
    }

    IEnumerator HardAI(int value)
	{
		PlayAIDelay(AIList[value].GetBestChoice(1), AIList[value].GetAiID());
		if(TileController.tileController.SendResponse(AIList[value].GetBestChoice(1), AIList[value].GetAiID()))
		{
			AIList[value].GetChoices();
            AIAnswer = true;
        }
		else{
			AIList[value].RemoveChoice(AIList[value].GetBestChoice(value), RemovedTiles[value]);
			RemovedTiles[value]++;
            AIAnswer = false;
        }
        TurnOnHighlight(AIAnswer, AIList[value].GetAiID(), EndAITurn);
        yield return new WaitForEndOfFrame();
    }

	IEnumerator CheatingAI(int value)
	{
		int answer = TileController.tileController.GetTile(TileController.tileController.GetList(AIList[value].GetAiID() + 1));
		PlayAIDelay(answer, AIList[value].GetAiID());
		TileController.tileController.SendResponse(answer, AIList[value].GetAiID(), 0);
        AIAnswer = true;
        TurnOnHighlight(AIAnswer, AIList[value].GetAiID(), EndAITurn);
        yield return new WaitForEndOfFrame();
    }
	#endregion

	#region Winner
	void CheckForWinner()
	{
		string chosenWinner = "";
		if(TileController.tileController.GetList(1).Count  == 0)
			chosenWinner = "Player 1 Wins";
		else if(TileController.tileController.GetList(2).Count  == 0)
			chosenWinner = "Player 2 Wins";
		else if(TileController.tileController.GetList(3).Count  == 0)
			chosenWinner = "Player 3 Wins";
		else if(TileController.tileController.GetList(4).Count  == 0)
			chosenWinner = "Player 4 Wins";

		if(chosenWinner != "")
		{
			Win = true;
			ShowWinner(chosenWinner);
			Reset();
		}
	}

	void ShowWinner(string value)
	{
		Camera.main.GetComponent<game_UIController>().GetWinner(value);
	}
	#endregion

	void TurnOnHighlight(bool response, int AiID, Action<int> callback = null)
	{
		if(response)
		{
            Debug.Log("AI2!!!");
            Camera.main.GetComponent<game_UIController>().SetPlayerTurn(2, AiID);
		}
		else
		{
            Debug.Log("AI3!!!");
            Camera.main.GetComponent<game_UIController>().SetPlayerTurn(1, AiID);
		}
        if(callback != null)
        {
            callback(AiID);
        }
	}

	#region AIResponse
	void PlayAIDelay(int value, int AiID)
	{
        Debug.Log("AI!!!");
		StartCoroutine(DelayAI(value, AiID));
	}

	void EndAITurn(int AiID)
	{
		DelayAIEnd(AiID);
	}

	IEnumerator DelayAI(int value, int AiID)
	{
		Camera.main.GetComponent<game_UIController>().GetPlayerTurn(AiID).SetActive(true);
		Camera.main.GetComponent<game_UIController>().SetPlayerTurn(0, AiID);
		yield return new WaitForSeconds (3.0f);
        Debug.Log("AIDONE!!!");
        Camera.main.GetComponent<game_UIController>().GetPlayerAnswer(value, AiID);
		yield return new WaitForSeconds (1.5f);
	}

	IEnumerator DelayAIEnd(int AiID)
	{
        Debug.Log("AIDONE2!!!");
        yield return new WaitForSeconds (1.5f);
		Camera.main.GetComponent<game_UIController>().GetPlayerTurn(AiID).SetActive(false);
		Camera.main.GetComponent<game_UIController>().RemovePlayerAnswer(AiID);
        CurrentAITurn = true;
        CheckForWinner();
        AIcount++;
        Debug.Log("DASDASDASDA");
    }
	#endregion
}
