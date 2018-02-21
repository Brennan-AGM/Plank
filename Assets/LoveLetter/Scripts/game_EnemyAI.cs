﻿using System.Collections;
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
        private Card Card1st = new Card(eCARDVALUES.INVALID);

        //CARD TO USE
        private Card Card2nd = new Card(eCARDVALUES.INVALID);

        private bool targetable = true;

        private eTargetPlayer targetPlayer = eTargetPlayer.INVALID;
        private eCARDVALUES targetCardValue = eCARDVALUES.INVALID;

        /// <summary>
        /// 
        /// </summary>
        /// <returns>If False, either player is dead or protected via Handmaid</returns>
        public bool IsTargetable()
        {
            return targetable;
        }

        public void SetTargetable(bool isTargetable)
        {
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

        public eCARDVALUES GetTargetCardValue()
        {
            return targetCardValue;
        }

        public void DrawNewCard(eCARDVALUES cardValue)
        {
            if (Card1st.GetCardValue() == eCARDVALUES.INVALID)
            {
                Card1st.SetCardValue(cardValue);
            }
            else
            {
                if(Card1st.GetCardValue() == cardValue)
                {
                    Card2nd.SetCardValue(cardValue);
                }
                else
                {
                    if(CheckWithAICardValue(cardValue))
                    {
                        eCARDVALUES temp = Card1st.GetCardValue();
                        Card1st.SetCardValue(cardValue);
                        Card2nd.SetCardValue(temp);
                    }
                    else
                    {
                        Card2nd.SetCardValue(cardValue);
                    }
                }
            }
            if(Card2nd.GetCardValue() != eCARDVALUES.INVALID)
            {
                AIUseCard();
                //gamelogic call
                //Card2nd.SetCardValue(eCARDVALUES.INVALID);
            }
        }

        public void SetNewCard(eCARDVALUES cardValue)
        {
            Card1st.SetCardValue(cardValue);
        }

        #region Card Selection
        bool CheckWithAICardValue(eCARDVALUES cardValue)
        {
            if (Card1st.GetCardValue() == eCARDVALUES.PRINCESS)
            {
                return false;
            }
            else if (cardValue == eCARDVALUES.PRINCESS)
            {
                return true;
            }
            else if (Card1st.GetCardValue() == eCARDVALUES.DUCHESS && cardValue == eCARDVALUES.KING || cardValue == eCARDVALUES.PRINCE)
            {
                return true;
            }
            else if ((Card1st.GetCardValue() == eCARDVALUES.KING && cardValue == eCARDVALUES.DUCHESS) ||
                    (Card1st.GetCardValue() == eCARDVALUES.PRINCE && cardValue == eCARDVALUES.DUCHESS))
            {
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

        bool CheckCardMediumAI(eCARDVALUES cardValue)
        {
            //Duchess, Prince, King, Handmaid, Guard, Baron, Priest

            if(CardController.instance.CardsLeftInDrawPile() > 3)
            {
                if (cardValue == eCARDVALUES.DUCHESS || cardValue == eCARDVALUES.PRINCE)
                {
                    //Change current card
                    return true;
                }
                else if (cardValue == eCARDVALUES.KING)
                {
                    if(Card1st.GetCardValue() == eCARDVALUES.PRINCE)
                    {
                        //Play King card
                        return false;
                    }
                    //Change current card
                    return true;
                }
                else if (cardValue == eCARDVALUES.HANDMAID)
                {
                    if (Card1st.GetCardValue() >= eCARDVALUES.HANDMAID)
                    {
                        //Play Handmaid card
                        return false;
                    }
                    //Change current card
                    return true;
                }
                else if (cardValue == eCARDVALUES.GUARD)
                {
                    if (Card1st.GetCardValue() != eCARDVALUES.BARON || Card1st.GetCardValue() != eCARDVALUES.PRIEST)
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

        bool CheckCardHardAI(eCARDVALUES cardValue)
        {
            //BETTER HARD AI
            //Duchess, Handmaid, King, Prince, Guard, Baron, Priest

            if (CardController.instance.CardsLeftInDrawPile() > 2)
            {
                if (cardValue == eCARDVALUES.DUCHESS || cardValue == eCARDVALUES.HANDMAID)
                {
                    //Change current card
                    return true;
                }
                else if (cardValue == eCARDVALUES.KING)
                {
                    if (Card1st.GetCardValue() == eCARDVALUES.HANDMAID)
                    {
                        //Play King card
                        return false;
                    }
                    //Change current card
                    return true;
                }
                else if (cardValue == eCARDVALUES.HANDMAID)
                {
                    if (Card1st.GetCardValue() >= eCARDVALUES.HANDMAID)
                    {
                        //Play Handmaid card
                        return false;
                    }
                    //Change current card
                    return true;
                }
                else if (cardValue == eCARDVALUES.GUARD)
                {
                    if (Card1st.GetCardValue() != eCARDVALUES.BARON || Card1st.GetCardValue() != eCARDVALUES.PRIEST)
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

        bool CheckCardInsaneAI(eCARDVALUES cardValue)
        {
            //BETTER INSANE AI
            return true;
        }
        #endregion

        #region Card Usage
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
            List<game_EnemyAI> listOfValidTargets = new List<game_EnemyAI>();
            int target = 0;
            switch (Card2nd.GetCardValue())
            {
                case eCARDVALUES.GUARD:
                    //GetValid
                    //if(playervalid)
                    //{

                    //}
                    //else
                    //{

                    //}
                    if(listOfValidTargets.Count > 1)
                    {
                        listOfValidTargets.Remove(this);
                    }
                    //int target = Random.Range(0, listOfValidTargets.Count + 1);
                    target = Random.Range(0, listOfValidTargets.Count);
                    if(target == listOfValidTargets.Count)
                    {
                        targetPlayer = eTargetPlayer.PLAYER;
                    }
                    else
                    {
                        targetPlayer = listOfValidTargets[target].GetCurrentPlayer();
                    }

                    //CHANCE for possible cards left
                    break;
                case eCARDVALUES.PRIEST:
                case eCARDVALUES.BARON:
                case eCARDVALUES.PRINCE:
                case eCARDVALUES.KING:
                    //GetValid
                    //if(playervalid)
                    //{

                    //}
                    //else
                    //{

                    //}
                    if (listOfValidTargets.Count > 1)
                    {
                        listOfValidTargets.Remove(this);
                    }
                    //int target = Random.Range(0, listOfValidTargets.Count + 1);
                    target = Random.Range(0, listOfValidTargets.Count);
                    if (target == listOfValidTargets.Count)
                    {
                        targetPlayer = eTargetPlayer.PLAYER;
                    }
                    else
                    {
                        targetPlayer = listOfValidTargets[target].GetCurrentPlayer();
                    }
                    break;
            }
        }

        void MediumAIUseCard()
        {
            List<game_EnemyAI> listOfValidTargets = new List<game_EnemyAI>();
            int target = 0;
            switch (Card2nd.GetCardValue())
            {
                case eCARDVALUES.GUARD:
                    //GetValid
                    //if(playervalid)
                    //{

                    //}
                    //else
                    //{

                    //}
                    if (listOfValidTargets.Count > 1)
                    {
                        listOfValidTargets.Remove(this);
                    }
                    //int target = Random.Range(0, listOfValidTargets.Count + 1);
                    target = Random.Range(0, listOfValidTargets.Count);
                    if (target == listOfValidTargets.Count)
                    {
                        targetPlayer = eTargetPlayer.PLAYER;
                    }
                    else
                    {
                        targetPlayer = listOfValidTargets[target].GetCurrentPlayer();
                    }

                    //CHANCE for possible cards left
                    break;
                case eCARDVALUES.PRIEST:
                case eCARDVALUES.BARON:
                case eCARDVALUES.PRINCE:
                case eCARDVALUES.KING:
                    //GetValid
                    //if(playervalid)
                    //{

                    //}
                    //else
                    //{

                    //}
                    if (listOfValidTargets.Count > 1)
                    {
                        listOfValidTargets.Remove(this);
                    }
                    //int target = Random.Range(0, listOfValidTargets.Count + 1);
                    target = Random.Range(0, listOfValidTargets.Count);
                    if (target == listOfValidTargets.Count)
                    {
                        targetPlayer = eTargetPlayer.PLAYER;
                    }
                    else
                    {
                        targetPlayer = listOfValidTargets[target].GetCurrentPlayer();
                    }
                    break;
            }
        }

        void HardAIUseCard()
        {
            List<game_EnemyAI> listOfValidTargets = new List<game_EnemyAI>();
            int target = 0;
            switch (Card2nd.GetCardValue())
            {
                case eCARDVALUES.GUARD:
                    //GetValid
                    //if(playervalid)
                    //{

                    //}
                    //else
                    //{

                    //}
                    if (listOfValidTargets.Count > 1)
                    {
                        listOfValidTargets.Remove(this);
                    }
                    //int target = Random.Range(0, listOfValidTargets.Count + 1);
                    target = Random.Range(0, listOfValidTargets.Count);
                    if (target == listOfValidTargets.Count)
                    {
                        targetPlayer = eTargetPlayer.PLAYER;
                    }
                    else
                    {
                        targetPlayer = listOfValidTargets[target].GetCurrentPlayer();
                    }

                    //CHANCE for possible cards left
                    break;
                case eCARDVALUES.PRIEST:
                case eCARDVALUES.BARON:
                case eCARDVALUES.PRINCE:
                case eCARDVALUES.KING:
                    //GetValid
                    //if(playervalid)
                    //{

                    //}
                    //else
                    //{

                    //}
                    if (listOfValidTargets.Count > 1)
                    {
                        listOfValidTargets.Remove(this);
                    }
                    //int target = Random.Range(0, listOfValidTargets.Count + 1);
                    target = Random.Range(0, listOfValidTargets.Count);
                    if (target == listOfValidTargets.Count)
                    {
                        targetPlayer = eTargetPlayer.PLAYER;
                    }
                    else
                    {
                        targetPlayer = listOfValidTargets[target].GetCurrentPlayer();
                    }
                    break;
            }
        }

        void InsaneAIUseCard()
        {
            List<game_EnemyAI> listOfValidTargets = new List<game_EnemyAI>();
            int target = 0;
            switch (Card2nd.GetCardValue())
            {
                case eCARDVALUES.GUARD:
                    //GetValid
                    //if(playervalid)
                    //{

                    //}
                    //else
                    //{

                    //}
                    if (listOfValidTargets.Count > 1)
                    {
                        listOfValidTargets.Remove(this);
                    }
                    //int target = Random.Range(0, listOfValidTargets.Count + 1);
                    target = Random.Range(0, listOfValidTargets.Count);
                    if (target == listOfValidTargets.Count)
                    {
                        targetPlayer = eTargetPlayer.PLAYER;
                    }
                    else
                    {
                        targetPlayer = listOfValidTargets[target].GetCurrentPlayer();
                    }

                    //CHANCE for possible cards left
                    break;
                case eCARDVALUES.PRIEST:
                case eCARDVALUES.BARON:
                case eCARDVALUES.PRINCE:
                case eCARDVALUES.KING:
                    //GetValid
                    //if(playervalid)
                    //{

                    //}
                    //else
                    //{

                    //}
                    if (listOfValidTargets.Count > 1)
                    {
                        listOfValidTargets.Remove(this);
                    }
                    //int target = Random.Range(0, listOfValidTargets.Count + 1);
                    target = Random.Range(0, listOfValidTargets.Count);
                    if (target == listOfValidTargets.Count)
                    {
                        targetPlayer = eTargetPlayer.PLAYER;
                    }
                    else
                    {
                        targetPlayer = listOfValidTargets[target].GetCurrentPlayer();
                    }
                    break;
            }
        }
        #endregion

        public void EndGame()
        {
            Card1st = new Card(eCARDVALUES.INVALID);
            Card2nd = new Card(eCARDVALUES.INVALID);
        }

        public eCARDVALUES Get1stCardValue()
        {
            return Card1st.GetCardValue();
        }

        public eCARDVALUES UseGet2ndCardValue()
        {
            eCARDVALUES value = Card2nd.GetCardValue();
            Card2nd.SetCardValue(eCARDVALUES.INVALID);
            return value;
        }
    }
}
