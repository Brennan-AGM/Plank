using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace BBSL_LOVELETTER
{
    public class CardNumber : MonoBehaviour
    {
        protected eCardValues currentCardValue = eCardValues.INVALID;

        [SerializeField]
        private TextMeshProUGUI textnum;
        [SerializeField]
        private Image cardimage;
        [SerializeField]
        private GameObject cardtextBack;
        [SerializeField]
        private TextMeshProUGUI cardtext;

        public void SetCard(eCardValues cardValue)
        {
            currentCardValue = cardValue;
            if (cardValue == eCardValues.INVALID)
            {
                cardimage.sprite = game_UIController.instance.GetCardSprites(cardValue);
                cardtextBack.SetActive(false);
                return;
            }
            if (textnum != null)
            {
                int value = (int)cardValue + 1;
                cardimage.sprite = game_UIController.instance.GetCardSpritesSmall(cardValue);
                textnum.text = value.ToString();
            }
            else
            {
                cardtextBack.SetActive(true);
                cardimage.sprite = game_UIController.instance.GetCardSprites(cardValue);
                cardtext.text = GetCardText(cardValue);
            }
        }

        public void HideText()
        {
            cardtextBack.SetActive(false);
        }

        public void ToggleCard(bool unhide)
        {
            gameObject.SetActive(unhide);
        }

        string GetCardText(eCardValues cardValue)
        {
            string text = "";
            switch(cardValue)
            {
                case eCardValues.GUARD:
                    text = "<b>Attack</b> (Target)";
                    break;
                case eCardValues.PRIEST:
                    text = "<b>Reveal</b> (Target)";
                    break;
                case eCardValues.BARON:
                    text = "<b>Wager</b> (Target)";
                    break;
                case eCardValues.HANDMAID:
                    text = "<b>Shield</b>";
                    break;
                case eCardValues.PRINCE:
                    text = "<b>Discard</b> (Target)";
                    break;
                case eCardValues.KING:
                    text = "<b>Trade</b> (Target)";
                    break;
                case eCardValues.COUNTESS:
                    text = "<b>Sabotage</b>";
                    break;
                case eCardValues.PRINCESS:
                    text = "<b>Treasure</b>";
                    break;
            }
            return text;
        }

        protected string GetCardDesc(eCardValues cardValue)
        {
            string text = "";
            switch (cardValue)
            {
                case eCardValues.GUARD:
                    text = "Guess a player's hand";
                    break;
                case eCardValues.PRIEST:
                    text = "Look at hand (Target)";
                    break;
                case eCardValues.BARON:
                    text = "Compare hands; lower hand is out";
                    break;
                case eCardValues.HANDMAID:
                    text = "Protected until next turn";
                    break;
                case eCardValues.PRINCE:
                    text = "Discard hands";
                    break;
                case eCardValues.KING:
                    text = "Trade hands";
                    break;
                case eCardValues.COUNTESS:
                    text = "Discarded if hand with King/Prince";
                    break;
                case eCardValues.PRINCESS:
                    text = "Lose if discarded";
                    break;
            }
            return text;
        }

        public Image GetCardImage()
        {
            return cardimage;
        }
    }
}
