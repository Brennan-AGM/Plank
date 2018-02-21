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
        private int AiID;
        
        //CARD TO KEEP
        private Card Card1st = new Card(eCARDVALUES.INVALID);

        //CARD TO USE
        private Card Card2nd = new Card(eCARDVALUES.INVALID);

        private bool targetable = true;

        public bool IsTargetable()
        {
            return targetable;
        }

        public void SetTargetable(bool isTargetable)
        {
            targetable = isTargetable;
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
        }

        public void SetNewCard(eCARDVALUES cardValue)
        {
            Card1st.SetCardValue(cardValue);
        }

        bool CheckWithAICardValue(eCARDVALUES cardValue)
        {
            if (Card1st.GetCardValue() == eCARDVALUES.PRINCESS)
            {
                return false;
            }
            else if(Card1st.GetCardValue() == eCARDVALUES.DUCHESS && cardValue == eCARDVALUES.KING || cardValue == eCARDVALUES.PRINCE)   
            {
                return true;
            }
            else if((Card1st.GetCardValue() == eCARDVALUES.KING && cardValue == eCARDVALUES.DUCHESS) ||
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
