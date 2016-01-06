using UnityEngine;
using System.Collections;

public class game_Logic : MonoBehaviour {

	public game_EnemyAI[] AIList;
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
		for(int i = 0; i < 3 && !Win && CurrentAITurn; i++)
		{
            CurrentAITurn = false;
            if (AIList[i].GetAiType() == 0)
				AIAnswer = EasyAI(i);
			else if(AIList[i].GetAiType() == 1)
				AIAnswer = MidAI(i);
			else if(AIList[i].GetAiType() == 2)
				AIAnswer = HardAI(i);
			else if(AIList[i].GetAiType() == 3)
				AIAnswer = CheatingAI(i);

            
            TurnOnHighlight(AIAnswer, AIList[i].GetAiID());
			EndAITurn(AIList[i].GetAiID());

			CheckForWinner();
		}
	}
	#region AIList
	bool EasyAI(int value)
	{
		PlayAIDelay(AIList[value].GetBestChoice(), AIList[value].GetAiID());
		if(TileController.tileController.SendResponse(AIList[value].GetBestChoice(), AIList[value].GetAiID()))
		{
			AIList[value].GetChoices();
			return true;
		}
		else{
			AIList[value].RemoveChoice(AIList[value].GetBestChoice());
		}
		return false;
	}

	bool MidAI(int value)
	{
		PlayAIDelay(AIList[value].GetBestChoice(), AIList[value].GetAiID());
		if(TileController.tileController.SendResponse(AIList[value].GetBestChoice(), AIList[value].GetAiID()))
		{
			return true;
		}
		else{
			AIList[value].RemoveChoice(AIList[value].GetBestChoice());
		}
		return false;
	}

	bool HardAI(int value)
	{
		PlayAIDelay(AIList[value].GetBestChoice(1), AIList[value].GetAiID());
		if(TileController.tileController.SendResponse(AIList[value].GetBestChoice(1), AIList[value].GetAiID()))
		{
			AIList[value].GetChoices();
			return true;
		}
		else{
			AIList[value].RemoveChoice(AIList[value].GetBestChoice(value), RemovedTiles[value]);
			RemovedTiles[value]++;
		}
		return false;
	}

	bool CheatingAI(int value)
	{
		int answer = TileController.tileController.GetTile(TileController.tileController.GetList(AIList[value].GetAiID() + 1));
		PlayAIDelay(answer, AIList[value].GetAiID());
		TileController.tileController.SendResponse(answer, AIList[value].GetAiID(), 0);
		return true;
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

	void TurnOnHighlight(bool response, int AiID)
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
    }
	#endregion
}
