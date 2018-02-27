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
            game_UIController.instance.temptarget = targetplayer;
            game_UIController.instance.FinishTargetPlayerPanel();
            game_UIController.instance.temptarget = eTargetPlayer.INVALID;
        }

        public void SelectGuardCard()
        {
            game_UIController.instance.tempguardcard = targetcard;
            game_UIController.instance.FinishGuardSelectionPanel();
            game_UIController.instance.tempguardcard = eCardValues.INVALID;
        }
    }
}
