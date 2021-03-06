﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BBSL_LOVELETTER
{
    public enum eTargetPlayer
    {
        INVALID = -1,
        PLAYER,
        AI1,
        AI2,
        AI3,
    }

    public enum eResult
    {
        INVALID = -1,
        LOSE,
        DRAW,
        WIN,
    }

    public class game_Logic : MonoBehaviour
    {
        [SerializeField]
        private game_EnemyAI[] AIList;
        [SerializeField]
        private game_Player Player;

        private eTargetPlayer CurrentPlayer = eTargetPlayer.PLAYER;
        private int CurrentPlayerIndex = 0;
        private List<eTargetPlayer> ListOfPlayers = new List<eTargetPlayer>();

        public int running = 0;
        private static Stack<byte> stillRunning = new Stack<byte>();

        public static game_Logic instance = null;
        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
            }

            instance = this;
        }

        public void StartGame()
        {
            Reset();
        }

        /// <summary>
        /// Reset the game for a new round/game
        /// </summary>
        /// <param name="hardReset">Refers if we will reset the game</param>
        void Reset(bool hardReset = false)
        {
            //CurrentPlayer = eTargetPlayer.INVALID;
            CurrentPlayerIndex = 0;
            for (int i = 0; i < AIList.Length; i++)
            {
                AIList[i].Reset(hardReset);
            }
            Player.Reset(hardReset);
            ResetListOfPlayers();
            if (hardReset)
            {
                CardController.instance.ResetGame();
                game_UIController.instance.ResetGame();
            }
            else
            {
                CardController.instance.ResetRound();
                game_UIController.instance.ResetRound();
            }
        }

        void ResetListOfPlayers()
        {
            ListOfPlayers.Clear();
            ListOfPlayers.Add(eTargetPlayer.PLAYER);
            ListOfPlayers.Add(eTargetPlayer.AI1);
            ListOfPlayers.Add(eTargetPlayer.AI2);
            ListOfPlayers.Add(eTargetPlayer.AI3);
        }

        bool HasPlayers()
        {
            int players = 0;
            if (Player.CanStillPlay())
            {
                players++;
            }
            for (int i = 0; i < AIList.Length; i++)
            {
                if (AIList[i].CanAIStillPlay())
                {
                    players++;
                }
            }
            if (players > 1)
            {
                return true;
            }
            return false;
        }

        #region TurnController
        IEnumerator GetNextPlayerIE()
        {
            yield return new WaitUntil(() => IsDoneRunning());
            Debug.Log("STARTING NEXT TURN");
            //if (CurrentPlayerIndex == -1)
            //{
            //    CurrentPlayerIndex = 0;
            //    CurrentPlayer = ListOfPlayers[CurrentPlayerIndex];
            //    CheckNextPlayer();
            //}
            //else
            //{
                if(CardController.instance.CardsLeftInDrawPile() > 0 && HasPlayers())
                {
                    CheckNextPlayer();
                }
                else
                {
                    CheckWinner();
                    CheckForTies();
                    ShowAllRemainingCards();
                    OneRoundWinner();
                    //AnnouncementOfWinner();
                }
            //}
        }

        void CheckNextPlayer()
        {
            if(CurrentPlayerIndex < ListOfPlayers.Count)
            {
                if (CurrentPlayer == ListOfPlayers[CurrentPlayerIndex])
                {
                    if (CurrentPlayerIndex < ListOfPlayers.Count - 1)
                    {
                        CurrentPlayerIndex++;
                    }
                    else
                    {
                        CurrentPlayerIndex = 0;
                    }
                }
            }
            else
            {
                CurrentPlayerIndex = 0;
            }
            
            CurrentPlayer = ListOfPlayers[CurrentPlayerIndex];
            ResetProtection(CurrentPlayer);
            Debug.Log(CurrentPlayer);
            PlayerDrawCard();
        }

        void PlayerDrawCard()
        {
            if (CurrentPlayer == eTargetPlayer.PLAYER)
            {
                CardController.instance.PlayerDrawCard(CurrentPlayer);
                StartRunning();
                game_UIController.instance.PlayerDrawCard(CurrentPlayer);
            }
            else
            {
                StartRunning();
                game_UIController.instance.PlayerDrawCard(CurrentPlayer);
                CardController.instance.PlayerDrawCard(CurrentPlayer);
            }
        }

        eTargetPlayer WinningPlayer;
        eCardValues WinningPlayerCard;
        eTargetPlayer TiePlayer;
        eCardValues TiePlayerCard;

        void AnnouncementOfWinner()
        {
            if(WinningPlayer != eTargetPlayer.INVALID)
            {
                //there is a valid winner
                if (WinningPlayer == eTargetPlayer.PLAYER)
                {
                    if(Player.IsWinnerYet())
                    {
                        //finish game
                        game_UIController.instance.PlayerWinning(WinningPlayer, 1.0f);
                    }
                    else
                    {
                        //finish round
                        StartCoroutine(StartNextRoundIE());
                    }
                }
                else
                {
                    if (GetAIList(WinningPlayer).IsWinnerYet())
                    {
                        game_UIController.instance.PlayerWinning(WinningPlayer, 1.0f);
                    }
                    else
                    {
                        //finish round
                        StartCoroutine(StartNextRoundIE());
                    }
                }
            }
            else
            {
                Debug.Log("SUPER TIE: " + WinningPlayer);
                //Super Tie has occured
                StartCoroutine(StartNextRoundIE());
            }
        }

        void OneRoundWinner()
        {
            if (WinningPlayer != eTargetPlayer.INVALID)
            {
                game_UIController.instance.PlayerWinning(WinningPlayer, WinningPlayerCard, 1.0f);
            }
            else
            {
                Debug.Log("SUPER TIE: " + WinningPlayer);
                //Super Tie has occured
                StartCoroutine(StartNextRoundIE());
            }
        }

        void ShowAllRemainingCards()
        {
            for (int i = 0; i < ListOfPlayers.Count; i++)
            {
                if (IsPlayerAlive(ListOfPlayers[i]))
                {
                    game_UIController.instance.FlipCard(ListOfPlayers[i], GetCard(ListOfPlayers[i]));
                }
            }
        }

        IEnumerator StartNextGameIE()
        {
            yield return new WaitForSeconds(1.0f);
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
            Reset(true);
            StartCoroutine(GetNextPlayerIE());
        }

        IEnumerator StartNextRoundIE()
        {
            yield return new WaitForSeconds(1.0f);
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
            Reset();
            StartCoroutine(GetNextPlayerIE());
        }

        void CheckWinner()
        {
            WinningPlayer = eTargetPlayer.INVALID;
            WinningPlayerCard = eCardValues.INVALID;
            TiePlayer = eTargetPlayer.INVALID;
            TiePlayerCard = eCardValues.INVALID;
            int firstPlayer = 0;
            for (int i = 0; i < ListOfPlayers.Count; i++)
            {
                if(IsPlayerAlive(ListOfPlayers[i]))
                {
                    WinningPlayer = ListOfPlayers[i];
                    WinningPlayerCard = GetCard(ListOfPlayers[i]);
                    firstPlayer = i;
                    break;
                }
            }
            for (int i = firstPlayer + 1; i < ListOfPlayers.Count; i++)
            {
                if(GetCard(ListOfPlayers[i]) > WinningPlayerCard)
                {
                    WinningPlayer = ListOfPlayers[i];
                    WinningPlayerCard = GetCard(ListOfPlayers[i]);
                    TiePlayer = eTargetPlayer.INVALID;
                    TiePlayerCard = eCardValues.INVALID;
                }
                else
                {
                    TiePlayer = ListOfPlayers[i];
                    TiePlayerCard = WinningPlayerCard;
                }
            }
        }

        void CheckForTies()
        {
            //When Tie happens
            if (TiePlayer != eTargetPlayer.INVALID)
            {
                int winplayerScore = 0;
                int tieplayerScore = 0;
                if (WinningPlayer == eTargetPlayer.PLAYER)
                {
                    winplayerScore = Player.GetTotalCards();
                }
                else
                {
                    winplayerScore = GetAIList(WinningPlayer).GetTotalCards();
                }
                tieplayerScore = GetAIList(TiePlayer).GetTotalCards();

                if (winplayerScore < tieplayerScore)
                {
                    WinningPlayer = TiePlayer;
                }
                else if (winplayerScore == tieplayerScore)
                {
                    WinningPlayer = eTargetPlayer.INVALID;
                }
            }
        }
        #endregion

        public void PlayerUse1stCard()
        {
            Player.ForceSetValue(Player.Get2ndCardValue());
        }

        public void PlayerUseCard(eButtonUsage button, eCardValues card, eTargetPlayer targetPlayer = eTargetPlayer.INVALID, eCardValues guardcard = eCardValues.INVALID)
        {
            CardController.instance.PlayerUseCard(card);
            eResult result = eResult.DRAW;
            if(card == eCardValues.HANDMAID || card == eCardValues.PRINCESS || card == eCardValues.COUNTESS || targetPlayer != eTargetPlayer.INVALID)
            {
                switch (card)
                {
                    case eCardValues.GUARD:
                        result = GuardCardUsed(guardcard, targetPlayer);
                        StartRunning();
                        game_UIController.instance.OpenShowdownPanel(card, eTargetPlayer.PLAYER, targetPlayer, result);
                        if (result == eResult.WIN)
                        {
                            StartRunning();
                            game_UIController.instance.PlayerDiscardCard(targetPlayer, guardcard, true);
                            KillPlayer(targetPlayer);
                            StartRunning();
                            game_UIController.instance.PlayerElimination(eTargetPlayer.PLAYER, targetPlayer);
                        }
                        else
                        {
                            StartRunning();
                            game_UIController.instance.PlayerUnaffected(targetPlayer, 1.0f);
                        }
                        break;
                    case eCardValues.PRIEST:
                        StartRunning();
                        game_UIController.instance.OpenShowdownPanel(card, eTargetPlayer.PLAYER, targetPlayer, result);
                        PriestCardUsed(eTargetPlayer.PLAYER, targetPlayer);
                        break;
                    case eCardValues.BARON:
                        result = BaronCardUsed(eTargetPlayer.PLAYER, targetPlayer);
                        StartRunning();
                        game_UIController.instance.OpenShowdownPanel(card, eTargetPlayer.PLAYER, targetPlayer, result);
                        if (result == eResult.WIN)
                        {
                            StartRunning();
                            game_UIController.instance.PlayerDiscardCard(targetPlayer, GetCard(targetPlayer), true);
                            KillPlayer(targetPlayer);
                            StartRunning();
                            game_UIController.instance.PlayerElimination(eTargetPlayer.PLAYER, targetPlayer);
                        }
                        else if (result == eResult.LOSE)
                        {
                            StartRunning();
                            game_UIController.instance.PlayerDiscardCard(eTargetPlayer.PLAYER, GetCard(eTargetPlayer.PLAYER), true);
                            KillPlayer(eTargetPlayer.PLAYER);
                            StartRunning();
                            game_UIController.instance.PlayerElimination(targetPlayer, eTargetPlayer.PLAYER);

                            StartCoroutine(GetNextPlayerIE());
                            return;
                        }
                        else
                        {
                            StartRunning();
                            game_UIController.instance.PlayerUnaffected(targetPlayer, 1.0f);
                            //SHOW DRAW more wait time
                        }
                        break;
                    case eCardValues.HANDMAID:
                        HandMaidCardUsed(eTargetPlayer.PLAYER);
                        StartRunning();
                        game_UIController.instance.PlayerShield(eTargetPlayer.PLAYER);
                        break;
                    case eCardValues.PRINCE:
                        eCardValues targetPlayersCardValue = GetCard(targetPlayer);
                        result = PrinceCardUsed(targetPlayer);
                        if (result == eResult.DRAW)
                        {
                            StartRunning();
                            game_UIController.instance.PlayerDiscardCard(targetPlayer, targetPlayersCardValue);
                            if (!CardController.instance.CheckIfDrawPileEmpty())
                            {
                                CardController.instance.PlayerDrawCard(targetPlayer);
                                StartRunning();
                                game_UIController.instance.PlayerDrawForPrinceCard(targetPlayer, card);
                            }
                            else
                            {
                                CardController.instance.DrawMissingCard(targetPlayer);
                                StartRunning();
                                game_UIController.instance.PlayerDrawForPrinceCard(targetPlayer, card, true);
                            }
                        }
                        else
                        {
                            StartRunning();
                            game_UIController.instance.OpenShowdownPanel(card, eTargetPlayer.PLAYER, targetPlayer, result);
                            StartRunning();
                            game_UIController.instance.PlayerDiscardCard(targetPlayer, targetPlayersCardValue, true);
                            KillPlayer(targetPlayer);
                            StartRunning();
                            game_UIController.instance.PlayerElimination(eTargetPlayer.PLAYER, targetPlayer);
                            
                            StartCoroutine(GetNextPlayerIE());
                            return;
                        }
                        break;
                    case eCardValues.KING:
                        eCardValues obtainedCard = GetCard(targetPlayer);
                        KingCardUsed(eTargetPlayer.PLAYER, targetPlayer);
                        StartRunning();
                        game_UIController.instance.PlayerSwapCard(eTargetPlayer.PLAYER, targetPlayer, card, obtainedCard);
                        break;
                    case eCardValues.COUNTESS:
                        break;
                    case eCardValues.PRINCESS:
                        eCardValues unusedCard = GetPlayer().Get2ndCardValue();
                        if (GetPlayer().Get2ndCardValue() == card)
                        {
                            unusedCard = GetPlayer().Get1stCardValue();
                        }
                        StartRunning();
                        game_UIController.instance.PlayerDiscardCard(eTargetPlayer.PLAYER, unusedCard, true);
                        KillPlayer(eTargetPlayer.PLAYER);
                        StartRunning();
                        game_UIController.instance.PlayerElimination(eTargetPlayer.PLAYER, eTargetPlayer.INVALID);

                        StartCoroutine(GetNextPlayerIE());
                        return;
                }
            }

            if(button == eButtonUsage.FIRSTCARDUSE)
            {
                GetPlayer().UseCardValue(false);
                GetPlayer().ForceSetValue(GetPlayer().Get2ndCardValue());
            }
            else if (button == eButtonUsage.SECONDCARDUSE)
            {
                GetPlayer().UseCardValue(true);
            }
            //DO ANIMATION

            StartCoroutine(GetNextPlayerIE());
        }

        public void AIUseCard(eCardValues card, eTargetPlayer AIIndex, eTargetPlayer targetPlayer = eTargetPlayer.INVALID, eCardValues guardcard = eCardValues.INVALID)
        {
            Debug.Log(AIIndex +" USE CARD " + card + " on " + targetPlayer);
            CardController.instance.AIUseCard(card, AIIndex);
            eResult result = eResult.DRAW;
            float aiThinkingTime = 4.0f;

            if(targetPlayer != eTargetPlayer.INVALID || (card == eCardValues.PRINCE))
            {
                if(card == eCardValues.PRINCE && targetPlayer == eTargetPlayer.INVALID)
                {
                    targetPlayer = AIIndex;
                }
                StartRunning();
                game_UIController.instance.ShowAICardUse(AIIndex, card, targetPlayer, aiThinkingTime);
            }
            else
            {
                StartRunning();
                game_UIController.instance.ShowAICardUse(AIIndex, card, AIIndex, aiThinkingTime);
                StartCoroutine(GetNextPlayerIE());
                return;
            }
            switch (card)
            {
                case eCardValues.GUARD:
                    result = GuardCardUsed(guardcard, targetPlayer);
                    StartRunning();
                    game_UIController.instance.OpenShowdownPanel(card, AIIndex, targetPlayer, result, aiThinkingTime);
                    if (result == eResult.WIN)
                    {
                        StartRunning();
                        game_UIController.instance.PlayerDiscardCard(targetPlayer, guardcard, true);
                        KillPlayer(targetPlayer);
                        StartRunning();
                        game_UIController.instance.PlayerElimination(AIIndex, targetPlayer);
                    }
                    else
                    {
                        StartRunning();
                        game_UIController.instance.PlayerUnaffected(targetPlayer, 1.0f);
                    }
                    break;
                case eCardValues.PRIEST:
                    PriestCardUsed(AIIndex, targetPlayer);
                    //StartRunning();
                    //game_UIController.instance.OpenShowdownPanel(card, AIIndex, targetPlayer, result);
                    break;
                case eCardValues.BARON:
                    result = BaronCardUsed(AIIndex, targetPlayer);
                    StartRunning();
                    game_UIController.instance.OpenShowdownPanel(card, AIIndex, targetPlayer, result, aiThinkingTime);
                    if (result == eResult.WIN)
                    {
                        StartRunning();
                        game_UIController.instance.PlayerDiscardCard(targetPlayer, GetCard(targetPlayer), true);
                        KillPlayer(targetPlayer);
                        StartRunning();
                        game_UIController.instance.PlayerElimination(AIIndex, targetPlayer);
                    }
                    else if(result == eResult.LOSE)
                    {
                        StartRunning();
                        game_UIController.instance.PlayerDiscardCard(AIIndex, GetCard(AIIndex), true);
                        KillPlayer(AIIndex);
                        StartRunning();
                        game_UIController.instance.PlayerElimination(targetPlayer, AIIndex);
                    }
                    else
                    {
                        StartRunning();
                        game_UIController.instance.PlayerUnaffected(targetPlayer, 1.0f);
                    }
                    break;
                case eCardValues.HANDMAID:
                    HandMaidCardUsed(AIIndex);
                    StartRunning();
                    game_UIController.instance.PlayerShield(AIIndex, 1.0f);
                    break;
                case eCardValues.PRINCE:
                    eCardValues targetPlayersCardValue = GetCard(targetPlayer);
                    result = PrinceCardUsed(targetPlayer);
                    if (result == eResult.DRAW)
                    {
                        StartRunning();
                        game_UIController.instance.PlayerDiscardCard(targetPlayer, targetPlayersCardValue);
                        if (!CardController.instance.CheckIfDrawPileEmpty())
                        {
                            //drawAnim
                            CardController.instance.PlayerDrawCard(targetPlayer);
                            StartRunning();
                            game_UIController.instance.PlayerDrawForPrinceCard(targetPlayer, card);
                        }
                        else
                        {
                            //SpecialdrawAnim
                            CardController.instance.DrawMissingCard(targetPlayer);
                            StartRunning();
                            game_UIController.instance.PlayerDrawForPrinceCard(targetPlayer, card, true);
                        }
                    }
                    else
                    {
                        StartRunning();
                        game_UIController.instance.OpenShowdownPanel(card, AIIndex, targetPlayer, result, aiThinkingTime);
                        StartRunning();
                        game_UIController.instance.PlayerDiscardCard(targetPlayer, targetPlayersCardValue, true);
                        KillPlayer(targetPlayer);
                        StartRunning();
                        game_UIController.instance.PlayerElimination(AIIndex, targetPlayer);
                    }
                    break;
                case eCardValues.KING:
                    KingCardUsed(AIIndex, targetPlayer);
                    StartRunning();
                    game_UIController.instance.PlayerSwapCard(AIIndex, targetPlayer, card);
                    break;
                case eCardValues.COUNTESS:
                    //do nothing
                    break;
                case eCardValues.PRINCESS:
                    //lose
                    KillPlayer(AIIndex);
                    StartRunning();
                    game_UIController.instance.PlayerDiscardCard(AIIndex, GetCard(AIIndex), true);
                    StartRunning();
                    game_UIController.instance.PlayerElimination(AIIndex, eTargetPlayer.INVALID);
                    break;
            }
            //Show card used
            StartCoroutine(GetNextPlayerIE());
        }

        #region Card Used
        /// <summary>
        /// Baron card let's AI compare card with a target player; the player with the higher value wins otherwise nothing happens
        /// </summary>
        /// <param name="targetCard"></param>
        /// <param name="targetPlayer"></param>
        /// <returns></returns>
        eResult GuardCardUsed(eCardValues targetCard, eTargetPlayer targetPlayer)
        {
            eResult result = eResult.DRAW;
            if (targetCard == GetCard(targetPlayer) && targetCard != eCardValues.INVALID)
            {
                result = eResult.WIN;
            }
            return result;
        }

        /// <summary>
        /// Priest card lets AI see target player's card
        /// </summary>
        /// <param name="targetPlayer"></param>
        /// <param name="AIIndex"></param>
        void PriestCardUsed(eTargetPlayer targetPlayer, eTargetPlayer AIIndex = eTargetPlayer.PLAYER)
        {
            eCardValues card = GetCard(targetPlayer);
            if (AIIndex != eTargetPlayer.PLAYER)
            {
                CardController.instance.AddKnownCard(card, AIIndex);
            }
        }

        /// <summary>
        /// Baron card let's AI compare card with a target player; the player with the higher value wins otherwise nothing happens
        /// </summary>
        /// <param name="initialPlayer"></param>
        /// <param name="targetPlayer"></param>
        /// <returns></returns>
        eResult BaronCardUsed(eTargetPlayer initialPlayer, eTargetPlayer targetPlayer)
        {
            eResult result = eResult.DRAW;
            if (targetPlayer != eTargetPlayer.INVALID)
            {
                if (GetCard(initialPlayer) > GetCard(targetPlayer))
                {
                    result = eResult.WIN;
                }
                else if (GetCard(initialPlayer) < GetCard(targetPlayer))
                {
                    result = eResult.LOSE;
                }
            }
            return result;
        }

        /// <summary>
        /// Handmaid protects player from getting targetted till the next draw
        /// </summary>
        /// <param name="player"></param>
        void HandMaidCardUsed(eTargetPlayer player)
        {
            switch (player)
            {
                case eTargetPlayer.PLAYER:
                    Player.SetTargetable(false);
                    break;
                case eTargetPlayer.AI1:
                    AIList[0].SetTargetable(false);
                    break;
                case eTargetPlayer.AI2:
                    AIList[1].SetTargetable(false);
                    break;
                case eTargetPlayer.AI3:
                    AIList[2].SetTargetable(false);
                    break;
            }
        }

        void ResetProtection(eTargetPlayer player)
        {
            //Debug.Log("RESET PROTECTION: " + player);
            switch (player)
            {
                case eTargetPlayer.PLAYER:
                    if(Player.IsTargetable() == false)
                    {
                        Player.SetTargetable(true);
                    }
                    break;
                case eTargetPlayer.AI1:
                    if (AIList[0].IsTargetable() == false)
                    {
                        AIList[0].SetTargetable(true);
                    }
                    break;
                case eTargetPlayer.AI2:
                    if (AIList[1].IsTargetable() == false)
                    {
                        AIList[1].SetTargetable(true);
                    }
                    break;
                case eTargetPlayer.AI3:
                    if (AIList[2].IsTargetable() == false)
                    {
                        AIList[2].SetTargetable(true);
                    }
                    break;
            }
        }

        /// <summary>
        /// Prince card lets AI force a player to discard his card
        /// </summary>
        /// <param name="targetPlayer"></param>
        eResult PrinceCardUsed(eTargetPlayer targetPlayer)
        {
            eResult result = eResult.DRAW;
            eCardValues card = GetCard(targetPlayer);

            if (card == eCardValues.PRINCESS)
            {
                result = eResult.LOSE;
            }

            if (targetPlayer != eTargetPlayer.PLAYER)
            {
                GetAIList(targetPlayer).ForceDiscard();
            }
            else
            {
                Player.ForceDiscard();
            }

            return result;
        }

        /// <summary>
        /// King card swaps cards with target player
        /// </summary>
        /// <param name="initialPlayer"></param>
        /// <param name="targetPlayer"></param>
        void KingCardUsed(eTargetPlayer initialPlayer, eTargetPlayer targetPlayer)
        {
            eCardValues card = GetCard(initialPlayer);
            eCardValues card2 = GetCard(targetPlayer);
            ChangeCard(initialPlayer, card2);
            ChangeCard(targetPlayer, card);

            if (initialPlayer != eTargetPlayer.PLAYER)
            {
                CardController.instance.AddKnownCard(card, initialPlayer);
            }

            if(targetPlayer != eTargetPlayer.PLAYER)
            {
                CardController.instance.AddKnownCard(card, targetPlayer);
            }
        }
        #endregion

        #region Player related
        public eCardValues GetCard(eTargetPlayer player)
        {
            eCardValues card = eCardValues.INVALID;
            switch (player)
            {
                case eTargetPlayer.PLAYER:
                    card = Player.Get1stCardValue();
                    break;
                case eTargetPlayer.AI1:
                    card = AIList[0].Get1stCardValue();
                    break;
                case eTargetPlayer.AI2:
                    card = AIList[1].Get1stCardValue();
                    break;
                case eTargetPlayer.AI3:
                    card = AIList[2].Get1stCardValue();
                    break;
            }
            return card;
        }

        void ChangeCard(eTargetPlayer player, eCardValues newCard)
        {
            switch (player)
            {
                case eTargetPlayer.PLAYER:
                    Player.ForceSetValue(newCard);
                    break;
                case eTargetPlayer.AI1:
                    AIList[0].SetNewCard(newCard);
                    break;
                case eTargetPlayer.AI2:
                    AIList[1].SetNewCard(newCard);
                    break;
                case eTargetPlayer.AI3:
                    AIList[2].SetNewCard(newCard);
                    break;
            }
        }

        public List<game_EnemyAI> GetValidAIPlayers()
        {
            List<game_EnemyAI> enemyAIs = new List<game_EnemyAI>();
            for (int i = 0; i < AIList.Length; i++)
            {
                if(AIList[i].CanAIStillPlay())
                {
                    if (AIList[i].IsTargetable())
                    {
                        enemyAIs.Add(AIList[i]);
                    }
                }
            }
            return enemyAIs;
        }

        public bool IsPlayerValidTarget(eTargetPlayer target)
        {
            if (IsPlayerAlive(target) == false)
            {
                return false;
            }
            switch (target)
            {
                case eTargetPlayer.PLAYER:
                    return Player.IsTargetable();
                case eTargetPlayer.AI1:
                    return AIList[0].IsTargetable();
                case eTargetPlayer.AI2:
                    return AIList[1].IsTargetable();
                case eTargetPlayer.AI3:
                    return AIList[2].IsTargetable();
            }
            return false;
        }

        public bool IsPlayerAlive(eTargetPlayer target)
        {
            switch (target)
            {
                case eTargetPlayer.PLAYER:
                    return Player.CanStillPlay();
                case eTargetPlayer.AI1:
                    return AIList[0].CanAIStillPlay();
                case eTargetPlayer.AI2:
                    return AIList[1].CanAIStillPlay();
                case eTargetPlayer.AI3:
                    return AIList[2].CanAIStillPlay();
            }
            return false;
        }

        public game_EnemyAI GetAIList(eTargetPlayer AI)
        {
            switch (AI)
            {
                case eTargetPlayer.AI1:
                    return AIList[0];
                case eTargetPlayer.AI2:
                    return AIList[1];
                case eTargetPlayer.AI3:
                    return AIList[2];
            }
            Debug.Log("Something went wrong please check this");
            return null;
        }

        public game_Player GetPlayer()
        {
            return Player;
        }

        void KillPlayer(eTargetPlayer targetPlayer)
        {
            if (targetPlayer == eTargetPlayer.PLAYER)
            {
                Player.SetPlay(false);
            }
            else
            {
                GetAIList(targetPlayer).SetPlay(false);
            }
            ListOfPlayers.Remove(targetPlayer);
        }

        public static void StartRunning()
        {
            stillRunning.Push(0);
            //Debug.Log("Start Running: " + stillRunning.Count);
        }

        public bool IsDoneRunning()
        {
            if (stillRunning.Count > 0)
            {
                return false;
            }
            return true;
        }

        public static void DoneRunning()
        {
            if (stillRunning.Count > 0)
            {
                stillRunning.Pop();
            }
            //Debug.Log("Done Running, Remaining: " + stillRunning.Count);
        }
    }
    #endregion
}
