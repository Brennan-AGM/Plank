using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BBSL_LOVELETTER
{
    public class game_UIController : MonoBehaviour
    {
        [Header("SPRITE REFERENCE")]
        [SerializeField]
        private Sprite[] cardSprites;
        [SerializeField]
        private Sprite[] cardSpritesSmall;

        public Sprite GetCardSprites(eCardValues value)
        {
            return cardSprites[(int)value];
        }

        public Sprite GetCardSpritesSmall(eCardValues value)
        {
            return cardSpritesSmall[(int)value];
        }
    }
}
