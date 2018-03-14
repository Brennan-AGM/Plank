using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BBSL_LOVELETTER
{
    public class game_EnemyAI : MonoBehaviour
    {
        [SerializeField]
        private eEnemyDifficulty AiType;
        [SerializeField]
        private eTargetPlayer currentPlayer;
        
        //CARD TO KEEP
        private Card Card1st = new Card(eCardValues.INVALID);

        //CARD TO USE
        private Card Card2nd = new Card(eCardValues.INVALID);

        private bool targetable = true;
        private eTargetPlayer targetPlayer = eTargetPlayer.INVALID;
        private eCardValues targetCardValue = eCardValues.INVALID;
        private int totalUsedCards = 0;
        private bool canPlay = true;
        private int pointsEarned = 0;

        public bool CanAIStillPlay()
        {
            return canPlay;
        }

        public void SetPlay(bool canPlay)
        {
            this.canPlay = canPlay;
            if(!canPlay)
            {
                Card1st.SetCardValue(eCardValues.INVALID);
                Card2nd.SetCardValue(eCardValues.INVALID);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>If False player is protected via Handmaid</returns>
        public bool IsTargetable()
        {
            return targetable;
        }

        public void SetTargetable(bool isTargetable)
        {
            if(isTargetable)
            {
                Debug.Log(currentPlayer + " is now targetable");
            }
            else
            {
                Debug.Log(currentPlayer + " is now protected");
            }
            targetable = isTargetable;
        }

        public eTargetPlayer GetCurrentPlayer()
        {
            return currentPlayer;
        }

        public eTargetPlayer GetTargetPlayer()
        {
            return targetPlayer;
        }

        public eCardValues GetTargetCardValue()
        {
            return targetCardValue;
        }

        public void ForceSetValue(eCardValues cardValue)
        {
            Card1st.SetCardValue(cardValue);
        }

        public void DrawNewCard(eCardValues cardValue)
        {
            if (Card1st.GetCardValue() == eCardValues.INVALID)
            {
                //1st draw, no cards distributed yet
                Card1st.SetCardValue(cardValue);
            }
            else
            {
                if(Card1st.GetCardValue() == cardValue)
                {
                    //if you drew a duplicate of the same card
                    Card2nd.SetCardValue(cardValue);
                }
                else
                {
                    if(CheckWithAICardValue(cardValue))
                    {
                        //swap 1st and 2nd card
                        eCardValues temp = Card1st.GetCardValue();
                        Card1st.SetCardValue(cardValue);
                        Card2nd.SetCardValue(temp);
                    }
                    else
                    {
                        //keep 1st card and have drawn card as used value
                        Card2nd.SetCardValue(cardValue);
                    }
                }
            }
            if(Card2nd.GetCardValue() != eCardValues.INVALID)
            {
                totalUsedCards += (int)Card2nd.GetCardValue();
                targetPlayer = eTargetPlayer.INVALID;
                AIUseCard();
                game_Logic.instance.AIUseCard(Card2nd.GetCardValue(), currentPlayer, targetPlayer);
                Card2nd.SetCardValue(eCardValues.INVALID);
            }
        }

        public void SetNewCard(eCardValues cardValue)
        {
            Card1st.SetCardValue(cardValue);
        }

        #region Card Selection
        bool CheckWithAICardValue(eCardValues cardValue)
        {
            if (Card1st.GetCardValue() == eCardValues.PRINCESS)
            {
                //AI automatically will not change cards if current card held is the princess
                return false;
            }
            else if (cardValue == eCardValues.PRINCESS)
            {
                //AI automatically will change cards if drawn card is the princess
                return true;
            }
            else if (Card1st.GetCardValue() == eCardValues.COUNTESS && cardValue == eCardValues.KING || cardValue == eCardValues.PRINCE)
            {
                //Playes are forced to remove the 'countess' card from their hand if a king/prince is drawn
                return true;
            }
            else if ((Card1st.GetCardValue() == eCardValues.KING && cardValue == eCardValues.COUNTESS) ||
                    (Card1st.GetCardValue() == eCardValues.PRINCE && cardValue == eCardValues.COUNTESS))
            {
                //Playes are forced to remove the 'countess' card from their hand if a king/prince is held
                return false;
            }
            switch (AiType)
            {
                case eEnemyDifficulty.EASY:
                    return CheckCardEasyAI();
                case eEnemyDifficulty.MEDIUM:
                    return CheckCardMediumAI(cardValue);
                case eEnemyDifficulty.HARD:
                    return CheckCardHardAI(cardValue);
                case eEnemyDifficulty.INSANE:
                    return CheckCardInsaneAI(cardValue);
                default:
                    return false;
            }
        }

        /// <summary>
        /// Easy AI just randomizes with all 2 cards drawn after above condition
        /// </summary>
        /// <returns></returns>
        bool CheckCardEasyAI()
        {
            if(Random.Range(0, 2) == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Medium AI has a card priority of Duchess, Prince, King, Handmaid, Guard, Baron, Priest
        /// When the draw pile is low though, it will prioritize high value card
        /// </summary>
        /// <param name="cardValue"></param>
        /// <returns></returns>
        bool CheckCardMediumAI(eCardValues cardValue)
        {
            if(CardController.instance.CardsLeftInDrawPile() > 3)
            {
                if (cardValue == eCardValues.COUNTESS || cardValue == eCardValues.PRINCE)
                {
                    //Change current card
                    return true;
                }
                else if (cardValue == eCardValues.KING)
                {
                    if(Card1st.GetCardValue() == eCardValues.PRINCE)
                    {
                        //Play King card
                        return false;
                    }
                    //Change current card
                    return true;
                }
                else if (cardValue == eCardValues.HANDMAID)
                {
                    if (Card1st.GetCardValue() >= eCardValues.HANDMAID)
                    {
                        //Play Handmaid card
                        return false;
                    }
                    //Change current card
                    return true;
                }
                else if (cardValue == eCardValues.GUARD)
                {
                    if (Card1st.GetCardValue() != eCardValues.BARON || Card1st.GetCardValue() != eCardValues.PRIEST)
                    {
                        //Play Guard card
                        return false;
                    }
                    //Change current card
                    return true;
                }
                //Play obtained card
                return false;
            }
            else
            {
                //Last Card maybe
                if (Card1st.GetCardValue() >= cardValue)
                {
                    //Play Obtained card
                    return false;
                }
                //Change current card
                return true;
            }
        }

        bool CheckCardHardAI(eCardValues cardValue)
        {
            //BETTER HARD AI
            //Duchess, Handmaid, King, Prince, Guard, Baron, Priest

            if (CardController.instance.CardsLeftInDrawPile() > 2)
            {
                if (cardValue == eCardValues.COUNTESS || cardValue == eCardValues.HANDMAID)
                {
                    //Change current card
                    return true;
                }
                else if (cardValue == eCardValues.KING)
                {
                    if (Card1st.GetCardValue() == eCardValues.HANDMAID)
                    {
                        //Play King card
                        return false;
                    }
                    //Change current card
                    return true;
                }
                else if (cardValue == eCardValues.HANDMAID)
                {
                    if (Card1st.GetCardValue() >= eCardValues.HANDMAID)
                    {
                        //Play Handmaid card
                        return false;
                    }
                    //Change current card
                    return true;
                }
                else if (cardValue == eCardValues.GUARD)
                {
                    if (Card1st.GetCardValue() != eCardValues.BARON || Card1st.GetCardValue() != eCardValues.PRIEST)
                    {
                        //Play Guard card
                        return false;
                    }
                    //Change current card
                    return true;
                }
                //Play obtained card
                return false;
            }
            else
            {
                //Last Card maybe
                if (Card1st.GetCardValue() >= cardValue)
                {
                    //Play Obtained card
                    return false;
                }
                //Change current card
                return true;
            }
        }

        bool CheckCardInsaneAI(eCardValues cardValue)
        {
            //BETTER INSANE AI
            return true;
        }
        #endregion

        #region Card Usage
        /// <summary>
        /// Called when the AI is ready to use a card and check it's decision
        /// </summary>
        void AIUseCard()
        {
            switch (AiType)
            {
                case eEnemyDifficulty.EASY:
                    EasyAIUseCard();
                    break;
                case eEnemyDifficulty.MEDIUM:
                    MediumAIUseCard();
                    break;
                case eEnemyDifficulty.HARD:
                    HardAIUseCard();
                    break;
                case eEnemyDifficulty.INSANE:
                    InsaneAIUseCard();
                    break;
            }
        }

        void EasyAIUseCard()
        {
            switch (Card2nd.GetCardValue())
            {
                case eCardValues.GUARD:
                    targetPlayer = GetValidTargetPlayer();
                    targetCardValue = GetValidRandomGuardTarget();
                    break;
                case eCardValues.PRIEST:
                case eCardValues.BARON:
                case eCardValues.KING:
                    targetPlayer = GetValidTargetPlayer();
                    break;
                case eCardValues.PRINCE:
                    targetPlayer = GetValidTargetPlayer();
                    if(targetPlayer == eTargetPlayer.INVALID)
                    {
                        if(IsTargetable())
                        {
                            targetPlayer = currentPlayer;
                        }
                    }
                    break;
                default:
                    targetPlayer = currentPlayer;
                    break;
            }
        }

        void MediumAIUseCard()
        {
            switch (Card2nd.GetCardValue())
            {
                case eCardValues.GUARD:
                    targetPlayer = GetValidTargetPlayer();
                    targetCardValue = GetValidRandomGuardTarget();
                    break;
                case eCardValues.PRIEST:
                case eCardValues.BARON:
                case eCardValues.KING:
                    targetPlayer = GetValidTargetPlayer();
                    break;
                case eCardValues.PRINCE:
                    targetPlayer = GetValidTargetPlayer();
                    if (targetPlayer == eTargetPlayer.INVALID)
                    {
                        if (IsTargetable())
                        {
                            targetPlayer = currentPlayer;
                        }
                    }
                    break;
                default:
                    targetPlayer = currentPlayer;
                    break;
            }
        }

        void HardAIUseCard()
        {
            switch (Card2nd.GetCardValue())
            {
                case eCardValues.GUARD:
                    targetPlayer = GetValidTargetPlayer();
                    targetCardValue = GetValidRandomGuardTarget();
                    break;
                case eCardValues.PRIEST:
                case eCardValues.BARON:
                case eCardValues.KING:
                    targetPlayer = GetValidTargetPlayer();
                    break;
                case eCardValues.PRINCE:
                    targetPlayer = GetValidTargetPlayer();
                    if (targetPlayer == eTargetPlayer.INVALID)
                    {
                        if (IsTargetable())
                        {
                            targetPlayer = currentPlayer;
                        }
                    }
                    break;
                default:
                    targetPlayer = currentPlayer;
                    break;
            }
        }

        void InsaneAIUseCard()
        {
            switch (Card2nd.GetCardValue())
            {
                case eCardValues.GUARD:
                    targetPlayer = GetValidTargetPlayer();
                    targetCardValue = GetValidRandomGuardTarget();
                    break;
                case eCardValues.PRIEST:
                case eCardValues.BARON:
                case eCardValues.KING:
                    targetPlayer = GetValidTargetPlayer();
                    break;
                case eCardValues.PRINCE:
                    targetPlayer = GetValidTargetPlayer();
                    if (targetPlayer == eTargetPlayer.INVALID)
                    {
                        if (IsTargetable())
                        {
                            targetPlayer = currentPlayer;
                        }
                    }
                    break;
                default:
                    targetPlayer = currentPlayer;
                    break;
            }
        }
        
        /// <summary>
        /// Used by AI(Easy ,Medium) to get valid target when using Guard Card
        /// </summary>
        /// <returns></returns>
        eCardValues GetValidRandomGuardTarget()
        {
            List<eCardValues> cardList = CardController.instance.CardsAvailable();

            //this is used because Guard can only look for non-Guard cards
            cardList.Remove(eCardValues.GUARD);
            int targetCard = 0;

            //this is for the probability that all remaining cards might be Guards
            if (cardList.Count == 0)
            {
                targetCard = Random.Range(1, 8);
                return (eCardValues)targetCard;
            }
            else
            {
                targetCard = Random.Range(0, cardList.Count);
                return cardList[targetCard];
            }
        }

        /// <summary>
        /// Gets a random valid targetable player for Easy and Medium AI
        /// </summary>
        /// <returns></returns>
        eTargetPlayer GetValidTargetPlayer()
        {
            List<game_EnemyAI> listOfValidTargets = game_Logic.instance.GetValidAIPlayers();
            int target = 0;
            bool canTargetPlayer = false;

            //Used to remove AI self from target pool
            if (listOfValidTargets.Count > 0 && targetable)
            {
                listOfValidTargets.Remove(this);
            }

            if (game_Logic.instance.IsPlayerAlive(eTargetPlayer.PLAYER) && game_Logic.instance.IsPlayerValidTarget(eTargetPlayer.PLAYER))
            {
                target = Random.Range(0, listOfValidTargets.Count + 1);
                canTargetPlayer = true;
            }
            else
            {
                target = Random.Range(0, listOfValidTargets.Count);
            }

            //Debug.Log("VALID TARGETs: " + listOfValidTargets.Count);
            //for (int i = 0; i < listOfValidTargets.Count; i++)
            //{
            //    Debug.Log("VALID TARGET: " + listOfValidTargets[i]);
            //}

            if(!canTargetPlayer && listOfValidTargets.Count == 0)
            {
                return eTargetPlayer.INVALID;
            }
            else if (target == listOfValidTargets.Count)
            {
                return eTargetPlayer.PLAYER;
            }
            else
            {
                return listOfValidTargets[target].GetCurrentPlayer();
            }
        }
        #endregion

        /// <summary>
        /// Reset The AI's card values, hardReset includes points earned
        /// </summary>
        public void Reset(bool hardReset = false)
        {
            Card1st = new Card(eCardValues.INVALID);
            Card2nd = new Card(eCardValues.INVALID);
            targetable = true;
            canPlay = true;
            targetPlayer = eTargetPlayer.INVALID;
            targetCardValue = eCardValues.INVALID;
            totalUsedCards = 0;
            if (hardReset)
            {
                pointsEarned = 0;
            }
        }

        public eCardValues Get1stCardValue()
        {
            return Card1st.GetCardValue();
        }

        public eCardValues UseGet2ndCardValue()
        {
            eCardValues value = Card2nd.GetCardValue();
            totalUsedCards += (int)Card2nd.GetCardValue();
            Card2nd.SetCardValue(eCardValues.INVALID);
            return value;
        }

        public void ForceDiscard()
        {
            totalUsedCards += (int)Card1st.GetCardValue();
            Card1st.SetCardValue(eCardValues.INVALID);
        }

        public int GetTotalCards()
        {
            return totalUsedCards;
        }

        public bool IsWinnerYet()
        {
            pointsEarned++;
            if(pointsEarned == 4)
            {
                return true;
            }
            return false;
        }
    }
}
