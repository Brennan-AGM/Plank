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

        public void SetPlay(bool isDead)
        {
            canPlay = isDead;
        }
    }
}
