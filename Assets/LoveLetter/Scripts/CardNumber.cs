﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace BBSL_LOVELETTER
{
    public class CardNumber : MonoBehaviour
    {
        private Image image;
        private LayoutElement layoutelement;
        private float width;
        private float height;

        [SerializeField]
        private TextMeshProUGUI textnum;

        void Awake()
        {
            image = GetComponent<Image>();
        }

        public void SetCard(eCardValues cardValue)
        {
            if (cardValue == eCardValues.INVALID)
            {
                return;
            }
            if (textnum != null)
            {
                int value = (int)cardValue + 1;
                image.sprite = game_UIController.instance.GetCardSpritesSmall(cardValue);
                textnum.text = value.ToString();
            }
            else
            {
                image.sprite = game_UIController.instance.GetCardSprites(cardValue);
            }
        }

        public void ToggleCard(bool unhide)
        {
            gameObject.SetActive(unhide);
        }
    }
}
