using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BBSL_LOVELETTER
{
    public class game_Player : MonoBehaviour
    {
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

        public bool IsTargetable()
        {
            return targetable;
        }

        public void SetTargetable(bool isTargetable)
        {
            targetable = isTargetable;
        }

        public bool CanAIStillPlay()
        {
            return canPlay;
        }

        public void SetPlay(bool canPlay)
        {
            this.canPlay = canPlay;
            if (!canPlay)
            {
                Card1st.SetCardValue(eCardValues.INVALID);
                Card2nd.SetCardValue(eCardValues.INVALID);
            }
        }

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
            if(hardReset)
            {
                pointsEarned = 0;
            }
        }

        public void SetNewCard(eCardValues cardValue)
        {
            if(Card1st.GetCardValue() == eCardValues.INVALID)
            {
                Card1st.SetCardValue(cardValue);
            }
            else
            {
                Card2nd.SetCardValue(cardValue);
            }
        }

        public eCardValues Get1stCardValue()
        {
            return Card1st.GetCardValue();
        }

        public eCardValues Get2ndCardValue()
        {
            return Card2nd.GetCardValue();
        }

        public eCardValues UseCardValue(bool use2ndCard = true)
        {
            eCardValues value = eCardValues.INVALID;
            if(use2ndCard)
            {
                value = Card2nd.GetCardValue();
                totalUsedCards += (int)Card2nd.GetCardValue();
                Card2nd.SetCardValue(eCardValues.INVALID);
            }
            else
            {
                value = Card1st.GetCardValue();
                totalUsedCards += (int)Card1st.GetCardValue();
                Card1st.SetCardValue(eCardValues.INVALID);
            }
            return value;
        }

        public void ForceDiscard()
        {
            totalUsedCards += (int)Card1st.GetCardValue();
            Card1st.SetCardValue(eCardValues.INVALID);
        }

        public void ForceSetValue(eCardValues cardValue)
        {
            Card1st.SetCardValue(cardValue);
        }

        public int GetTotalCards()
        {
            return totalUsedCards;
        }

        public bool IsWinnerYet()
        {
            pointsEarned++;
            if (pointsEarned == 4)
            {
                return true;
            }
            return false;
        }
    }
}
