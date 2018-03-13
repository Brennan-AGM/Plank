using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace BBSL_LOVELETTER
{
    public class CardNumberDistribute : CardNumber
    {
        [SerializeField]
        TextMeshProUGUI topDescription;
        [SerializeField]
        TextMeshProUGUI btmDescription;

        public void SetDescription(eCardValues cardValue, bool topdesc = true)
        {
            if(topdesc)
            {
                topDescription.text = GetCardDesc(cardValue);
            }
            else
            {
                btmDescription.text = GetCardDesc(cardValue);
            }
        }

        public void ResetDescription()
        {
            topDescription.text = GetCardDesc(eCardValues.INVALID);
            btmDescription.text = GetCardDesc(eCardValues.INVALID);
        }
    }
}
