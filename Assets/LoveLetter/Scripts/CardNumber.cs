using System.Collections;
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
            layoutelement = GetComponent<LayoutElement>();
            width = layoutelement.preferredWidth;
            height = layoutelement.preferredHeight;
        }

        public void SetCard(eCardValues cardValue)
        {
            if(textnum != null)
            {
                image.sprite = game_UIController.instance.GetCardSpritesSmall(cardValue);
            }
            else
            {
                image.sprite = game_UIController.instance.GetCardSprites(cardValue);
            }
        }
    }
}
