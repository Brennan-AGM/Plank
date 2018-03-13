using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace BBSL_LOVELETTER
{
    public class CardNumberHover : CardNumber, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField]
        TextMeshProUGUI btmDescription;
        bool descriptionSet = false;

        public void OnPointerEnter(PointerEventData eventData)
        {
            if(!descriptionSet)
            {
                descriptionSet = true;
                btmDescription.text = GetCardDesc(currentCardValue);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            ResetDescription();
        }

        public void ResetDescription()
        {
            if (descriptionSet)
            {
                descriptionSet = false;
                btmDescription.text = GetCardDesc(eCardValues.INVALID);
            }
        }
    }
}
