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

        public void DrawNewCard2()
        {

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
            //CardController.
            return true;
        }

        bool CheckCardHardAI(eCARDVALUES cardValue)
        {
            return true;
        }

        bool CheckCardInsaneAI(eCARDVALUES cardValue)
        {
            return true;
        }

        public void EndGame()
        {
            Card1st = new Card(eCARDVALUES.INVALID);
            Card2nd = new Card(eCARDVALUES.INVALID);
        }
    }
}
