using UnityEngine;
using System.Collections;
using System;

public enum eTURNRESULT
{
    INVALID = 0,
    TURN,
    CORRECT,
    WRONG,
}

public class game_Logic : MonoBehaviour {

	public game_EnemyAI[] AIList;
    private int currentAITurn;
    private int[] RemovedTiles = new int[3];
    private bool CurrentAITurn;
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
        for (int i = 0; i < 3; i++)
		{
			AIList[i].Reset();
		}
	}

	public void StartAITurn()
	{
		if(!HasWinner())
        {
            currentAITurn = 0;
            ContinueAI();
        }
	}

    void ContinueAI()
    {
        if(currentAITurn < 3)
        {
            switch (AIList[currentAITurn].GetAiType())
            {
                case eEnemyDifficulty.EASY:
                    StartCoroutine(EasyAI(currentAITurn));
                    break;
                case eEnemyDifficulty.MEDIUM:
                    StartCoroutine(MidAI(currentAITurn));
                    break;
                case eEnemyDifficulty.HARD:
                    StartCoroutine(HardAI(currentAITurn));
                    break;
                case eEnemyDifficulty.INSANE:
                    StartCoroutine(InsaneAI(currentAITurn));
                    break;
            }
        }
        else
        {
            //Player Turn
            Debug.Log("PLAYER TURN");
        }
    }

    #region AIList
    IEnumerator EasyAI(int value)
	{
        Debug.Log("START");
        int bestChoice = AIList[value].GetBestChoiceForEasyMed();
        int aiID = AIList[value].GetAiID();
        if (TileController.instance.SendResponse(bestChoice, aiID))
		{
			AIList[value].GetChoices();
            AIAnswer = true;
		}
		else
        {
			AIList[value].RemoveChoice(bestChoice);
            AIAnswer = false;
        }
        StartCoroutine(DelayAI(bestChoice, aiID));
        yield return new WaitForEndOfFrame();
    }

    IEnumerator MidAI(int value)
    {
        int bestChoice = AIList[value].GetBestChoiceForEasyMed();
        int aiID = AIList[value].GetAiID();
        if (TileController.instance.SendResponse(bestChoice, aiID))
        {
            AIList[value].GetChoices();
            AIAnswer = true;
        }
		else
        {
			AIList[value].RemoveChoice(bestChoice);
            AIAnswer = false;
        }
        StartCoroutine(DelayAI(bestChoice, aiID));
        yield return new WaitForEndOfFrame();
    }

    IEnumerator HardAI(int value)
	{
        int bestChoice = AIList[value].GetBestChoiceHard();
        int aiID = AIList[value].GetAiID();
		if(TileController.instance.SendResponse(bestChoice, aiID))
		{
			AIList[value].GetChoices();
            AIAnswer = true;
        }
		else
        {
			AIList[value].RemoveChoice(bestChoice, RemovedTiles[value]);
			RemovedTiles[value]++;
            AIAnswer = false;
        }
        StartCoroutine(DelayAI(bestChoice, aiID));
        yield return new WaitForEndOfFrame();
    }

	IEnumerator InsaneAI(int value)
    {
        int aiID = AIList[value].GetAiID();
        int answer = TileController.instance.GetTile(TileController.instance.GetList(aiID + 1));
        TileController.instance.SendResponseInsaneAI(answer, aiID);
        AIAnswer = true;
        StartCoroutine(DelayAI(answer, aiID));
        yield return new WaitForEndOfFrame();
    }
	#endregion

	#region Winner
	bool HasWinner()
	{
        bool winner = true;
		string chosenWinner = "";
		if(TileController.instance.GetList(1).Count  == 0)
        {
            chosenWinner = "Player 1 Wins";
        }
		else if(TileController.instance.GetList(2).Count  == 0)
        {
            chosenWinner = "Player 2 Wins";
        }
		else if(TileController.instance.GetList(3).Count  == 0)
        {
            chosenWinner = "Player 3 Wins";
        }
		else if(TileController.instance.GetList(4).Count  == 0)
        {
            chosenWinner = "Player 4 Wins";
        }
        else
        {
            winner = false;
        }

		if(chosenWinner != "")
		{
			ShowWinner(chosenWinner);
			Reset();
		}

        return winner;
    }

	void ShowWinner(string value)
	{
		game_UIController.instance.GetWinner(value);
	}
	#endregion

	void TurnOnHighlight(bool response, int aiID)
    {
        if (response)
        {
            game_UIController.instance.SetPlayerTurn(aiID, eTURNRESULT.CORRECT);
        }
		else
        {
            game_UIController.instance.SetPlayerTurn(aiID, eTURNRESULT.WRONG);
        }
        //if(callback != null)
        //{
        //    callback(AiID);
        //}
	}

	#region AIResponse
	void EndAITurn(int AiID)
	{
		DelayAIEnd(AiID);
	}

	IEnumerator DelayAI(int value, int aiID)
	{
        Debug.Log("DELAY START");
        game_UIController.instance.GetPlayerTurn(aiID).SetActive(true);
        game_UIController.instance.SetPlayerTurn(aiID, eTURNRESULT.TURN);
        yield return new WaitForSeconds (3.0f);
        game_UIController.instance.GetPlayerAnswer(value, aiID);
        game_UIController.instance.ShowTiles(value);
        TurnOnHighlight(AIAnswer, aiID);
        yield return new WaitForSeconds (1.5f);
        Debug.Log("DELAY END");
        StartCoroutine(DelayAIEnd(aiID));
    }

	IEnumerator DelayAIEnd(int AiID)
    {
        Debug.Log("END START");
        yield return new WaitForSeconds (1.5f);
		game_UIController.instance.GetPlayerTurn(AiID).SetActive(false);
		game_UIController.instance.RemovePlayerAnswer(AiID);
        CurrentAITurn = true;
        if(!HasWinner())
        {
            currentAITurn++;
            ContinueAI();
        }
        Debug.Log("END END");
    }
	#endregion
}
