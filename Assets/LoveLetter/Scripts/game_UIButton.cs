using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace BBSL_LOVELETTER
{
    public enum eButton
    {
        INVALID = -1,
        STARTGAME,
        INSTRUCTION,
        QUITGAME,
        OPENGUARDPANEL,
        CLOSEGUARDPANEL,
        NUMBERSELECTION,
        RESETCONFIRM,
        RESETCANCEL,
    }

    public class game_UIButton : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
    {
        [SerializeField]
        eButton target = eButton.INVALID;

        Button button;

        void Start()
        {
            button = GetComponent<Button>();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            switch (target)
            {
                case eButton.QUITGAME:
                case eButton.INSTRUCTION:
                case eButton.STARTGAME:
                case eButton.OPENGUARDPANEL:
                case eButton.RESETCONFIRM:
                case eButton.RESETCANCEL:
                    SoundController.instance.PlaySE(eSoundFX.ToggleSound, 0.5f);
                    break;
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            switch (target)
            {
                case eButton.QUITGAME:
                case eButton.INSTRUCTION:
                case eButton.STARTGAME:
                case eButton.NUMBERSELECTION:
                case eButton.RESETCONFIRM:
                case eButton.RESETCANCEL:
                    SoundController.instance.PlaySE(eSoundFX.ConfirmSound, 0.5f);
                    break;

                case eButton.OPENGUARDPANEL:
                    if (button.interactable)
                    {
                        SoundController.instance.PlaySE(eSoundFX.ConfirmSound, 0.5f);
                    }
                    else
                    {
                        SoundController.instance.PlaySE(eSoundFX.WrongSound, 0.5f);
                    }
                    break;
                case eButton.CLOSEGUARDPANEL:
                    SoundController.instance.PlaySE(eSoundFX.WrongSound, 0.5f);
                    break;

            }
        }
    }
}
