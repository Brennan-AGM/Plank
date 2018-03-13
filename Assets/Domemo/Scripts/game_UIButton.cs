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

                case eButton.OPENNUMBERPANEL:
                    if(button.interactable)
                    {
                        SoundController.instance.PlaySE(eSoundFX.ConfirmSound, 0.5f);
                    }
                    else
                    {
                        SoundController.instance.PlaySE(eSoundFX.WrongSound, 0.5f);
                    }
                    break;
                case eButton.CLOSENUMBERPANEL:
                    SoundController.instance.PlaySE(eSoundFX.WrongSound, 0.5f);
                    break;

            }
        }
    }
}
