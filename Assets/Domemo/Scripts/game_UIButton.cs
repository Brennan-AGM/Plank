using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace BBSL_DOMEMO
{
    public enum eButton
    {
        INVALID = -1,
        STARTGAME,
        INSTRUCTION,
        QUITGAME,
        OPENNUMBERPANEL,
        CLOSENUMBERPANEL,
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
                case eButton.OPENNUMBERPANEL:
                case eButton.RESETCONFIRM:
                case eButton.RESETCANCEL:
                    SoundController.instance.PlaySE(eSoundFX.ToggleSound);
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
                    SoundController.instance.PlaySE(eSoundFX.ConfirmSound);
                    break;

                case eButton.OPENNUMBERPANEL:
                    if(button.interactable)
                    {
                        SoundController.instance.PlaySE(eSoundFX.ConfirmSound);
                    }
                    else
                    {
                        SoundController.instance.PlaySE(eSoundFX.WrongSound);
                    }
                    break;
                case eButton.CLOSENUMBERPANEL:
                    SoundController.instance.PlaySE(eSoundFX.WrongSound);
                    break;

            }
        }
    }
}
