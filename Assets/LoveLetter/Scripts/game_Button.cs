using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BBSL_LOVELETTER
{
    public class game_Button : MonoBehaviour
    {
        [SerializeField]
        eButtonUsage button = eButtonUsage.INVALID;
        [SerializeField]
        eTargetPlayer targetplayer = eTargetPlayer.INVALID;
        [SerializeField]
        eCardValues targetcard = eCardValues.INVALID;

        public void UseCard()
        {
            game_UIController.instance.UseCard(button);
        }

        public void SelectPlayerTarget()
        {
            game_UIController.instance.FinishTargetPlayerPanel(targetplayer);
        }

        public void SelectGuardCard()
        {
            game_UIController.instance.FinishGuardSelectionPanel(targetcard);
        }
    }
}
