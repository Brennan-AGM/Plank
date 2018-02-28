using System.Collections;
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

        private eTargetPlayer CurrentPlayer = eTargetPlayer.INVALID;
        private int CurrentPlayerIndex = -1;
        private List<eTargetPlayer> ListOfPlayers = new List<eTargetPlayer>();

        public static game_Logic instance = null;
        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
            }

            instance = this;
        }

        void Start()
        {
            Reset();
        }

        /// <summary>
        /// Reset the game for a new round/game
        /// </summary>
        /// <param name="hardReset">Refers if we will reset the game</param>
        void Reset(bool hardReset = false)
        {
            CurrentPlayer = eTargetPlayer.INVALID;
            CurrentPlayerIndex = -1;
            for (int i = 0; i < AIList.Length; i++)
            {
                AIList[i].Reset(hardReset);
            }
            Player.Reset(hardReset);
            ResetListOfPlayers();

            if(hardReset)
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
            if (Player.CanAIStillPlay())
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
            if (players > 0)
            {
                return true;
            }
            return false;
        }

        #region TurnController
        IEnumerator GetNextPlayerIE(float waitTime = 0.0f)
        {
            yield return new WaitForSeconds(waitTime);
            if (CurrentPlayerIndex == -1)
            {
                CurrentPlayerIndex = 0;
                CurrentPlayer = ListOfPlayers[CurrentPlayerIndex];
                CheckNextPlayer();
            }
            else
            {
                if(CardController.instance.CardsLeftInDrawPile() > 0 && HasPlayers())
                {
                    CheckNextPlayer();
                }
                else
                {
                    CheckWinner();
                    CheckForTies();
                    AnnouncementOfWinner();
                }
            }
        }

        void CheckNextPlayer()
        {
            if(CurrentPlayerIndex < ListOfPlayers.Count - 1)
            {
                CurrentPlayerIndex++;
            }
            else
            {
                CurrentPlayerIndex = 0;
            }
            Debug.Log(CurrentPlayerIndex);
            CurrentPlayer = ListOfPlayers[CurrentPlayerIndex];
            PlayerDrawCard();
        }

        void PlayerDrawCard()
        {
            CardController.instance.PlayerDrawCard(CurrentPlayer);
            if (CurrentPlayer == eTargetPlayer.PLAYER)
            {
                game_UIController.instance.PlayerDrawCard(CurrentPlayer, Player.Get2ndCardValue());
            }
            else
            {
                game_UIController.instance.PlayerDrawCard(CurrentPlayer);
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
                        Debug.Log("GAME FINISH WINNER IS: " + WinningPlayer);
                        StartCoroutine(StartNextGameIE());
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
                        StartCoroutine(StartNextGameIE());
                        //finish game
                        Debug.Log("GAME FINISH WINNER IS: " + WinningPlayer);
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
                //Super Tie has occured
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
            WinningPlayer = ListOfPlayers[0];
            WinningPlayerCard = GetCard(ListOfPlayers[0]);
            for (int i = 1; i < ListOfPlayers.Count; i++)
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

        public void PlayerUseCard(eCardValues card, eTargetPlayer targetPlayer = eTargetPlayer.INVALID, eCardValues guardcard = eCardValues.INVALID)
        {
            CardController.instance.PlayerUseCard(card);
            float waitTime = 1.5f;
            switch (card)
            {
                case eCardValues.GUARD:
                    if (GuardCardUsed(guardcard, targetPlayer) == eResult.WIN)
                    {
                        game_UIController.instance.PlayerDiscardCard(targetPlayer, guardcard, true);
                        KillPlayer(targetPlayer);
                    }
                    break;
                case eCardValues.PRIEST:
                    PriestCardUsed(eTargetPlayer.PLAYER, targetPlayer);
                    break;
                case eCardValues.BARON:
                    eResult result = BaronCardUsed(eTargetPlayer.PLAYER, targetPlayer);
                    if (result == eResult.WIN)
                    {
                        //SHOW enemy card wait time
                        game_UIController.instance.PlayerDiscardCard(targetPlayer, GetCard(targetPlayer), true);
                        KillPlayer(targetPlayer);
                    }
                    else if (result == eResult.LOSE)
                    {
                        //SHOW enemy card wait time
                        game_UIController.instance.PlayerDiscardCard(eTargetPlayer.PLAYER, card, true);
                        KillPlayer(eTargetPlayer.PLAYER);
                    }
                    else
                    {
                        //SHOW DRAW more wait time
                    }
                    break;
                case eCardValues.HANDMAID:
                    HandMaidCardUsed(eTargetPlayer.PLAYER);
                    break;
                case eCardValues.PRINCE:
                    if (PrinceCardUsed(targetPlayer) == eResult.DRAW)
                    {
                        if (!CardController.instance.CheckIfDrawPileEmpty())
                        {
                            CardController.instance.PlayerDrawCard(targetPlayer);
                            game_UIController.instance.PlayerDrawCard(targetPlayer, GetCard(targetPlayer));
                        }
                        else
                        {
                            CardController.instance.DrawMissingCard(targetPlayer);
                            game_UIController.instance.PlayerDrawMissingCard(targetPlayer, GetCard(targetPlayer));
                        }
                    }
                    else
                    {
                        game_UIController.instance.PlayerDiscardCard(targetPlayer, GetCard(targetPlayer), true);
                        KillPlayer(targetPlayer);
                    }
                    break;
                case eCardValues.KING:
                    KingCardUsed(eTargetPlayer.PLAYER, targetPlayer);
                    break;
                case eCardValues.COUNTESS:
                    //do nothing
                    break;
                case eCardValues.PRINCESS:
                    eCardValues unusedCard = GetPlayer().Get2ndCardValue();
                    if(GetPlayer().Get2ndCardValue() == card)
                    {
                        unusedCard = GetPlayer().Get1stCardValue();
                    }
                    game_UIController.instance.PlayerDiscardCard(eTargetPlayer.PLAYER, unusedCard, true);
                    KillPlayer(eTargetPlayer.PLAYER);
                    break;
            }
            if(GetPlayer().Get1stCardValue() == card)
            {
                GetPlayer().ForceSetValue(GetPlayer().UseCardValue(false));
            }
            else
            {
                GetPlayer().UseCardValue(true);
            }
            //DO ANIMATION

            StartCoroutine(GetNextPlayerIE(waitTime));
        }

        public void AIUseCard(eCardValues card, eTargetPlayer AIIndex, eTargetPlayer targetPlayer = eTargetPlayer.INVALID, eCardValues guardcard = eCardValues.INVALID)
        {
            Debug.Log("AI USE CARD");
            CardController.instance.AIUseCard(card, AIIndex);
            float waitTime = 5.0f;
            switch (card)
            {
                case eCardValues.GUARD:
                    if(GuardCardUsed(guardcard, targetPlayer) == eResult.WIN)
                    {
                        game_UIController.instance.PlayerDiscardCard(targetPlayer, guardcard, true);
                        KillPlayer(targetPlayer);
                    }
                    game_UIController.instance.ShowAICardUse(AIIndex, card, targetPlayer, 5.0f);
                    break;
                case eCardValues.PRIEST:
                    PriestCardUsed(AIIndex, targetPlayer);
                    game_UIController.instance.ShowAICardUse(AIIndex, card, targetPlayer, 5.0f);
                    break;
                case eCardValues.BARON:
                    eResult result = BaronCardUsed(AIIndex, targetPlayer);
                    if(result == eResult.WIN)
                    {
                        game_UIController.instance.PlayerDiscardCard(targetPlayer, GetCard(targetPlayer), true);
                        KillPlayer(targetPlayer);
                    }
                    else if(result == eResult.LOSE)
                    {
                        game_UIController.instance.PlayerDiscardCard(AIIndex, card, true);
                        KillPlayer(AIIndex);
                    }
                    else
                    {

                    }
                    game_UIController.instance.ShowAICardUse(AIIndex, card, targetPlayer, 5.0f);
                    break;
                case eCardValues.HANDMAID:
                    HandMaidCardUsed(AIIndex);
                    game_UIController.instance.ShowAICardUse(AIIndex, card, AIIndex, 5.0f);
                    break;
                case eCardValues.PRINCE:
                    if(PrinceCardUsed(targetPlayer) == eResult.DRAW)
                    {
                        game_UIController.instance.PlayerDiscardCard(targetPlayer, GetCard(targetPlayer));
                        if (!CardController.instance.CheckIfDrawPileEmpty())
                        {
                            //drawAnim
                            CardController.instance.PlayerDrawCard(targetPlayer);
                            game_UIController.instance.PlayerDrawCard(targetPlayer, GetCard(targetPlayer));
                        }
                        else
                        {
                            //SpecialdrawAnim
                            CardController.instance.DrawMissingCard(targetPlayer);
                            game_UIController.instance.PlayerDrawMissingCard(targetPlayer, GetCard(targetPlayer));
                        }
                    }
                    else
                    {
                        game_UIController.instance.PlayerDiscardCard(targetPlayer, GetCard(targetPlayer), true);
                        KillPlayer(targetPlayer);
                    }
                    game_UIController.instance.ShowAICardUse(AIIndex, card, targetPlayer, 5.0f);
                    break;
                case eCardValues.KING:
                    KingCardUsed(AIIndex, targetPlayer);
                    game_UIController.instance.ShowAICardUse(AIIndex, card, targetPlayer, 5.0f);
                    break;
                case eCardValues.COUNTESS:
                    //do nothing
                    game_UIController.instance.ShowAICardUse(AIIndex, card, AIIndex, 5.0f);
                    break;
                case eCardValues.PRINCESS:
                    //lose
                    game_UIController.instance.PlayerDiscardCard(AIIndex, GetCard(AIIndex), true);
                    KillPlayer(AIIndex);
                    game_UIController.instance.ShowAICardUse(AIIndex, card, AIIndex, 5.0f);
                    break;
            }
            //Show card used
            StartCoroutine(GetNextPlayerIE(waitTime));
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
            else
            {
                //show card
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
            if(targetPlayer != eTargetPlayer.INVALID)
            {
                if (GetCard(initialPlayer) > GetCard(targetPlayer))
                {
                    result = eResult.WIN;
                }
                else if (GetCard(initialPlayer) > GetCard(targetPlayer))
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
        eCardValues GetCard(eTargetPlayer player)
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
                    Player.SetNewCard(newCard);
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
                if(AIList[i].IsTargetable())
                {
                    enemyAIs.Add(AIList[i]);
                }
            }
            return enemyAIs;
        }

        public bool IsPlayerValidTarget(eTargetPlayer target)
        {
            switch(target)
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
    }
    #endregion
}
