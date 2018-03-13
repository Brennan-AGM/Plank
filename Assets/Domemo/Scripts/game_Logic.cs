using UnityEngine;
using System.Collections;
using System;
using DG.Tweening;

namespace BBSL_DOMEMO
{
    public enum eTURNRESULT
    {
        INVALID = 0,
        TURN,
        CORRECT,
        WRONG,
    }

    public class game_Logic : MonoBehaviour
    {
        public game_EnemyAI[] AIList;
        private int currentAITurn;
        private int[] RemovedTiles = new int[3];
        private bool CurrentAITurn;
        private bool AIAnswer;

        #region Setup/Reset
        public void StartGame()
        {
            //SoundController.instance.PlayBGM(false);
            Reset();
        }

        public void Reset()
        {
            ActivatePlayerButton();
            CurrentAITurn = true;
            game_UIController.instance.ToggleTitlePanel(false);
            game_UIController.instance.Reset();
            TileController.instance.Reset();
            game_UIController.instance.StartDistribution();
            for (int i = 0; i < 3; i++)
            {
                AIList[i].Reset();
            }
            for (int i = 0; i < 3; i++)
            {
                RemovedTiles[i] = 0;
            }
        }

        public void ReturnToMainMenu()
        {
            //SoundController.instance.PlayBGM(true);
            game_UIController.instance.ToggleTitlePanel(true);
            game_UIController.instance.ToggleFrontPanel(false);
            game_UIController.instance.Reset();
        }
        #endregion

        #region Turn Setup
        public void StartAITurn()
        {
            if (!HasWinner())
            {
                currentAITurn = 0;
                ContinueAI();
            }
        }

        void ContinueAI()
        {
            if (currentAITurn < 3)
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
                game_UIController.instance.SetMessageBox("Player's turn");
                Invoke("ActivatePlayerButton", 2.0f);
            }
        }

        void ActivatePlayerButton()
        {
            SoundController.instance.PlaySE(eSoundFX.PlayerTurnSound);
            game_UIController.instance.pick_btn.interactable = true;
        }

        void TurnOnHighlight(bool response, int aiID)
        {
            if (response)
            {
                SoundController.instance.PlaySE(eSoundFX.CorrectSound);
                game_UIController.instance.SetPlayerTurn(aiID, eTURNRESULT.CORRECT);
            }
            else
            {
                SoundController.instance.PlaySE(eSoundFX.WrongSound);
                game_UIController.instance.SetPlayerTurn(aiID, eTURNRESULT.WRONG);
            }
        }
        #endregion

        #region AIList
        IEnumerator EasyAI(int value)
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
            if (TileController.instance.SendResponse(bestChoice, aiID))
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
            bool player1Winner = false;
            string chosenWinner = "";
            if (TileController.instance.GetList(1).Count == 0)
            {
                chosenWinner = "Player <#00FF02FF>Wins</color>!!!";
                player1Winner = true;
            }
            else if (TileController.instance.GetList(2).Count == 0)
            {
                chosenWinner = "<#002BCAFF>AI 1</color> Wins!!!";
            }
            else if (TileController.instance.GetList(3).Count == 0)
            {
                chosenWinner = "<#8D00CAFF>AI 2</color> Wins!!!";
            }
            else if (TileController.instance.GetList(4).Count == 0)
            {
                chosenWinner = "<#ff8000>AI 3</color> Wins!!!";
            }
            else
            {
                winner = false;
            }

            if (chosenWinner != "")
            {
                ShowWinner(chosenWinner);
                game_UIController.instance.OpenResetBox(player1Winner);
            }

            return winner;
        }

        void ShowWinner(string value)
        {
            SoundController.instance.PlaySE(eSoundFX.CorrectSound);
            game_UIController.instance.SetMessageBox(value);
        }
        #endregion

        #region AIResponse
        void EndAITurn(int AiID)
        {
            DelayAIEnd(AiID);
        }

        IEnumerator DelayAI(int value, int aiID)
        {
            game_UIController.instance.TogglePlayerGlow(aiID, true, 1.0f);
            game_UIController.instance.SetPlayerTurn(aiID, eTURNRESULT.TURN);
            yield return new WaitForSeconds(1.0f);
            game_UIController.instance.GetPlayerAnswer(value, aiID);
            yield return new WaitForSeconds(2.0f);
            TurnOnHighlight(AIAnswer, aiID);
            if (AIAnswer)
            {
                //Delay
                GameObject newTile = Instantiate(TileController.instance.GetTile(), game_UIController.instance.TileHolders.transform);
                newTile.GetComponentInChildren<TileNumber>().SetNumber(value);
                newTile.transform.position = TileController.instance.GetTile(value, aiID).transform.position;
                Transform targetParent = game_UIController.instance.GetTileTargetPos(value);
                Vector3 pos = targetParent.position;
                if (targetParent.childCount > 0)
                {
                    pos = targetParent.GetChild(targetParent.childCount - 1).transform.position;
                    pos = new Vector3(pos.x + 20, pos.y);
                    targetParent.DOLocalMoveX(targetParent.localPosition.x - 20f, 1.0f);
                }
                newTile.transform.DOMove(pos, 1.0f);
                TileController.instance.RemoveTile(value, aiID);
                yield return new WaitForSeconds(1.0f);
                yield return new WaitForEndOfFrame();
                Destroy(newTile);
                if (targetParent.childCount > 0)
                {
                    targetParent.DOLocalMoveX(targetParent.localPosition.x + 20f, 0.0f);
                }
                game_UIController.instance.ShowTiles(value);
            }
            yield return new WaitForSeconds(1.0f);
            StartCoroutine(DelayAIEnd(aiID));
        }

        IEnumerator DelayAIEnd(int AiID)
        {
            game_UIController.instance.RemovePlayerAnswer(AiID);
            game_UIController.instance.TogglePlayerGlow(AiID, false, 1.0f);
            yield return new WaitForSeconds(1.0f);
            CurrentAITurn = true;
            if (!HasWinner())
            {
                currentAITurn++;
                ContinueAI();
            }
        }

        public void SendResponse(int value)
        {
            StartCoroutine(SendResponseIE(value));
        }

        IEnumerator SendResponseIE(int value)
        {
            game_UIController.instance.pick_btn.interactable = false;
            yield return new WaitForSeconds(0.5f);
            if (TileController.instance.CheckList(value, 0))
            {
                game_UIController.instance.DetermineResult(eTURNRESULT.CORRECT);
                yield return new WaitForSeconds(0.5f);
                //Delay
                GameObject newTile = Instantiate(TileController.instance.GetTile(), game_UIController.instance.TileHolders.transform);
                newTile.GetComponentInChildren<TileNumber>().SetNumber(value);
                newTile.transform.position = TileController.instance.GetTile(value, 0).transform.position;
                Transform targetParent = game_UIController.instance.GetTileTargetPos(value);
                Vector3 pos = targetParent.position;
                if (targetParent.childCount > 0)
                {
                    pos = targetParent.GetChild(targetParent.childCount - 1).transform.position;
                    pos = new Vector3(pos.x + 20, pos.y);
                    targetParent.DOLocalMoveX(targetParent.localPosition.x - 20f, 1.0f);
                }
                newTile.transform.DOMove(pos, 1.0f);

                TileController.instance.RemoveTile(value, 0);
                yield return new WaitForSeconds(1.0f);
                yield return new WaitForEndOfFrame();
                Destroy(newTile);
                if (targetParent.childCount > 0)
                {
                    targetParent.DOLocalMoveX(targetParent.localPosition.x + 20f, 0.0f);
                }
                game_UIController.instance.ShowTiles(value);
                yield return new WaitForSeconds(1.0f);
            }
            else
            {
                game_UIController.instance.DetermineResult(eTURNRESULT.WRONG);
                yield return new WaitForSeconds(3.0f);
            }
            StartAITurn();
        }
        #endregion
    }
}
