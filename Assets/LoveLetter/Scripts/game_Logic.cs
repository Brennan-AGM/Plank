using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BBSL_LOVELETTER
{
    public enum eRESULT
    {
        INVALID = -1,
        LOSE,
        DRAW,
        WIN,
    }

    public class game_Logic : MonoBehaviour
    {
        private bool playerTargetable = true;
        public game_EnemyAI[] AIList;

        void PlayerUseCard(eCARDVALUES card, eTargetPlayer targetPlayer = eTargetPlayer.INVALID)
        {
            CardController.instance.PlayerUseCard(card);
            switch (card)
            {
                case eCARDVALUES.GUARD:
                    break;
                case eCARDVALUES.PRIEST:
                    PriestCardUsed(eTargetPlayer.PLAYER, targetPlayer);
                    break;
                case eCARDVALUES.BARON:
                    BaronCardUsed(eTargetPlayer.PLAYER, targetPlayer);
                    break;
                case eCARDVALUES.HANDMAID:
                    playerTargetable = false;
                    break;
                case eCARDVALUES.PRINCE:
                    if (!CardController.instance.CheckIfDrawPileEmpty())
                    {
                        CardController.instance.DrawCard(targetPlayer);
                    }
                    else
                    {
                        CardController.instance.DrawMissingCard(targetPlayer);
                    }
                    break;
                case eCARDVALUES.KING:
                    KingCardUsed(eTargetPlayer.PLAYER, targetPlayer);
                    break;
                case eCARDVALUES.DUCHESS:
                    //do nothing
                    break;
                case eCARDVALUES.PRINCESS:
                    //lose
                    break;
            }
        }

        void AIUseCard(eCARDVALUES card, eTargetPlayer AIIndex, eTargetPlayer targetPlayer = eTargetPlayer.INVALID)
        {
            CardController.instance.AIUseCard(card, AIIndex);
            switch (card)
            {
                case eCARDVALUES.GUARD:
                    break;
                case eCARDVALUES.PRIEST:
                    PriestCardUsed(AIIndex, targetPlayer);
                    break;
                case eCARDVALUES.BARON:
                    BaronCardUsed(AIIndex, targetPlayer);
                    break;
                case eCARDVALUES.HANDMAID:
                    AIUsedHandMaid(AIIndex);
                    break;
                case eCARDVALUES.PRINCE:
                    if (!CardController.instance.CheckIfDrawPileEmpty())
                    {
                        CardController.instance.DrawCard(targetPlayer);
                    }
                    else
                    {
                        CardController.instance.DrawMissingCard(targetPlayer);
                    }
                    break;
                case eCARDVALUES.KING:
                    KingCardUsed(AIIndex, targetPlayer);
                    break;
                case eCARDVALUES.DUCHESS:
                    //do nothing
                    break;
                case eCARDVALUES.PRINCESS:
                    //lose
                    break;
            }
        }

        void AIUsedHandMaid(eTargetPlayer AIIndex)
        {
            switch(AIIndex)
            {
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

        eRESULT BaronCardUsed(eTargetPlayer initialPlayer, eTargetPlayer targetPlayer)
        {
            eRESULT result = eRESULT.DRAW;
            if(GetCard(initialPlayer) > GetCard(targetPlayer))
            {
                result = eRESULT.WIN;
            }
            else if (GetCard(initialPlayer) > GetCard(targetPlayer))
            {
                result = eRESULT.LOSE;
            }
            return result;
        }

        void PriestCardUsed(eTargetPlayer targetPlayer, eTargetPlayer AIIndex = eTargetPlayer.PLAYER)
        {
            eCARDVALUES card = GetCard(targetPlayer);
            if (AIIndex != eTargetPlayer.PLAYER)
            {
                CardController.instance.AddKnownCard(card, AIIndex);
            }
            //show card
        }

        void KingCardUsed(eTargetPlayer initialPlayer, eTargetPlayer targetPlayer)
        {
            eCARDVALUES card = GetCard(initialPlayer);
            eCARDVALUES card2 = GetCard(targetPlayer);
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

        eCARDVALUES GetCard(eTargetPlayer player)
        {
            eCARDVALUES card = eCARDVALUES.INVALID;
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

        void ChangeCard(eTargetPlayer player, eCARDVALUES newCard)
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
    }
}
