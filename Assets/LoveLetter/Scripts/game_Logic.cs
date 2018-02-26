﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BBSL_LOVELETTER
{
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

        public static game_Logic instance = null;
        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
            }

            instance = this;
        }

        void PlayerUseCard(eCardValues card, eTargetPlayer targetPlayer = eTargetPlayer.INVALID)
        {
            CardController.instance.PlayerUseCard(card);
            switch (card)
            {
                case eCardValues.GUARD:
                    break;
                case eCardValues.PRIEST:
                    PriestCardUsed(eTargetPlayer.PLAYER, targetPlayer);
                    break;
                case eCardValues.BARON:
                    BaronCardUsed(eTargetPlayer.PLAYER, targetPlayer);
                    break;
                case eCardValues.HANDMAID:
                    HandMaidCardUsed(eTargetPlayer.PLAYER);
                    break;
                case eCardValues.PRINCE:
                    if (!CardController.instance.CheckIfDrawPileEmpty())
                    {
                        CardController.instance.DrawCard(targetPlayer);
                    }
                    else
                    {
                        CardController.instance.DrawMissingCard(targetPlayer);
                    }
                    break;
                case eCardValues.KING:
                    KingCardUsed(eTargetPlayer.PLAYER, targetPlayer);
                    break;
                case eCardValues.COUNTESS:
                    //do nothing
                    break;
                case eCardValues.PRINCESS:
                    //lose
                    break;
            }
        }

        public void AIUseCard(eCardValues card, eTargetPlayer AIIndex, eTargetPlayer targetPlayer = eTargetPlayer.INVALID)
        {
            CardController.instance.AIUseCard(card, AIIndex);
            switch (card)
            {
                case eCardValues.GUARD:

                    KillPLayer(targetPlayer);
                    break;
                case eCardValues.PRIEST:
                    PriestCardUsed(AIIndex, targetPlayer);
                    break;
                case eCardValues.BARON:
                    eResult result = BaronCardUsed(AIIndex, targetPlayer);
                    if(result == eResult.WIN)
                    {
                        KillPLayer(targetPlayer);
                    }
                    else if(result == eResult.LOSE)
                    {
                        KillPLayer(AIIndex);
                    }
                    break;
                case eCardValues.HANDMAID:
                    HandMaidCardUsed(AIIndex);
                    break;
                case eCardValues.PRINCE:
                    if(PrinceCardUsed(targetPlayer) == eResult.DRAW)
                    {
                        if (!CardController.instance.CheckIfDrawPileEmpty())
                        {
                            CardController.instance.DrawCard(targetPlayer);
                        }
                        else
                        {
                            CardController.instance.DrawMissingCard(targetPlayer);
                        }
                    }
                    else
                    {
                        KillPLayer(targetPlayer);
                    }
                    break;
                case eCardValues.KING:
                    KingCardUsed(AIIndex, targetPlayer);
                    break;
                case eCardValues.COUNTESS:
                    //do nothing
                    break;
                case eCardValues.PRINCESS:
                    //lose
                    KillPLayer(AIIndex);
                    PostResult(eResult.LOSE, AIIndex);
                    break;
            }
            //Show card used
            //Get Next player
        }

        //void PostResult(eResult result, eTargetPlayer initialPlayer, eTargetPlayer targetPlayer = eTargetPlayer.INVALID)
        //{
        //    switch(result)
        //    {
        //        case eResult.WIN:
        //            if(GetAIList(targetPlayer) != null)
        //            {
        //                GetAIList(targetPlayer).SetPlay(false);
        //            }
        //            else
        //            {
        //                Player.SetPlay(false);
        //            }
        //            break;
        //        case eResult.LOSE:
        //            if (GetAIList(initialPlayer) != null)
        //            {
        //                GetAIList(initialPlayer).SetPlay(false);
        //            }
        //            else
        //            {
        //                Player.SetPlay(false);
        //            }
        //            break;
        //    }
        //}

        /// <summary>
        /// Handmaid protects player from getting targetted till the next draw
        /// </summary>
        /// <param name="player"></param>
        void HandMaidCardUsed(eTargetPlayer player)
        {
            switch(player)
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
        /// Baron card let's AI compare card with a target player; the player with the higher value wins otherwise nothing happens
        /// </summary>
        /// <param name="initialPlayer"></param>
        /// <param name="targetPlayer"></param>
        /// <returns></returns>
        eResult BaronCardUsed(eTargetPlayer initialPlayer, eTargetPlayer targetPlayer)
        {
            eResult result = eResult.DRAW;
            if(GetCard(initialPlayer) > GetCard(targetPlayer))
            {
                result = eResult.WIN;
            }
            else if (GetCard(initialPlayer) > GetCard(targetPlayer))
            {
                result = eResult.LOSE;
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
            //show card
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

        eCardValues GetCard(eTargetPlayer player)
        {
            eCardValues card = eCardValues.INVALID;
            switch (player)
            {
                case eTargetPlayer.PLAYER:
                    card = CardController.instance.player1Card.GetCardValue();
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
                    CardController.instance.player1Card.SetCardValue(newCard);
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

        public bool IsPlayerValidTarget()
        {
            return Player.IsTargetable();
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

        void KillPLayer(eTargetPlayer targetPlayer)
        {
            if(targetPlayer == eTargetPlayer.PLAYER)
            {
                Player.SetPlay(false);
            }
            else
            {
                GetAIList(targetPlayer).SetPlay(false);
            }
        }
    }
}
