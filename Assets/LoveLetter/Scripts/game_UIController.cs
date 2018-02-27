using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

namespace BBSL_LOVELETTER
{
    public class game_UIController : MonoBehaviour
    {
        [Header("SPRITE REFERENCE")]
        [SerializeField]
        private Sprite[] cardSprites;
        [SerializeField]
        private Sprite[] cardSpritesSmall;

        [Header("")][Header("CARD REFERENCE")]
        [SerializeField]
        private GameObject[] cardsToDistribute;
        [SerializeField]
        private GameObject deck;
        [SerializeField]
        private GameObject hiddenCard;
        [SerializeField]
        private GameObject player1SingleCardPos;
        [SerializeField]
        private GameObject player1DoubleCardPos;
        [SerializeField]
        private CardNumber[] players1stCards;
        [SerializeField]
        private CardNumber[] players2ndCards;

        [Header("")][Header("TRANSFORM REFERENCE")]
        [SerializeField]
        private Transform[] tinycardsHolder;

        [Header("")][Header("PREFAB REFERENCE")]
        [SerializeField]
        private GameObject cardsToShow;
        [SerializeField]
        private GameObject tinycardsToShow;

        [Header("")][Header("SCORE REFERENCE")]
        [SerializeField]
        private GameObject[] scoresGmObj;
        [SerializeField]
        private TextMeshProUGUI[] scores;
        
        [Header("")][Header("MESSAGE REFERENCE")]
        [SerializeField]
        private TextMeshProUGUI winMessage;

        private bool hideDetails = false;

        public static game_UIController instance = null;
        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
            }

            instance = this;
        }

        void Start()
        {
            ToggleDetails(false);
            DistributeCards();
        }

        void Update()
        {
            if(Input.GetKeyDown(KeyCode.LeftAlt) || Input.GetKeyDown(KeyCode.RightAlt))
            {
                ToggleDetails(true);
            }
            else if (Input.GetKeyUp(KeyCode.LeftAlt) || Input.GetKeyUp(KeyCode.RightAlt))
            {
                ToggleDetails(false);
            }
        }

        #region TweenAnimations
        void DistributeCards()
        {
            StartCoroutine(DistributeCardsIE());
        }

        IEnumerator DistributeCardsIE()
        {
            float speed = 1.0f;
            //Total Wait Time 1.5 secs
            Reset1stCard();
            Reset2ndCard();
            yield return new WaitForEndOfFrame();

            MoveCard(cardsToDistribute[0], hiddenCard, 0.5f);
            ResizeCard(cardsToDistribute[0], hiddenCard, 0.5f);
            yield return new WaitForSeconds(0.25f * speed);

            MoveCard(cardsToDistribute[1], player1SingleCardPos, 0.5f);
            ResizeCard(cardsToDistribute[1], players1stCards[GetPlayerIndex(eTargetPlayer.PLAYER)].gameObject, 0.5f * speed);
            yield return new WaitForSeconds(0.25f * speed);

            Reset1stCard();
            hiddenCard.SetActive(true);
            yield return new WaitForEndOfFrame();

            MoveCard(cardsToDistribute[0], players1stCards[GetPlayerIndex(eTargetPlayer.AI1)].gameObject, 0.5f * speed);
            ResizeCard(cardsToDistribute[0], players1stCards[GetPlayerIndex(eTargetPlayer.AI1)].gameObject, 0.5f * speed);
            yield return new WaitForSeconds(0.25f * speed);

            Reset2ndCard();
            yield return new WaitForEndOfFrame();

            MoveCard(cardsToDistribute[1], players1stCards[GetPlayerIndex(eTargetPlayer.AI2)].gameObject, 0.5f * speed);
            ResizeCard(cardsToDistribute[1], players1stCards[GetPlayerIndex(eTargetPlayer.AI2)].gameObject, 0.5f * speed);
            yield return new WaitForSeconds(0.25f * speed);

            Reset1stCard();
            players1stCards[GetPlayerIndex(eTargetPlayer.AI1)].gameObject.SetActive(true);
            yield return new WaitForEndOfFrame();

            MoveCard(cardsToDistribute[0], players1stCards[GetPlayerIndex(eTargetPlayer.AI3)].gameObject, 0.5f * speed);
            ResizeCard(cardsToDistribute[0], players1stCards[GetPlayerIndex(eTargetPlayer.AI3)].gameObject, 0.5f * speed);
            yield return new WaitForSeconds(0.25f * speed);

            Reset2ndCard();
            players1stCards[GetPlayerIndex(eTargetPlayer.AI2)].gameObject.SetActive(true);
            yield return new WaitForEndOfFrame();

            MoveCard(cardsToDistribute[1], players2ndCards[GetPlayerIndex(eTargetPlayer.PLAYER)].gameObject, 0.5f * speed);
            ResizeCard(cardsToDistribute[1], players2ndCards[GetPlayerIndex(eTargetPlayer.PLAYER)].gameObject, 0.5f * speed);
            MoveCard(players1stCards[GetPlayerIndex(eTargetPlayer.PLAYER)].gameObject, player1DoubleCardPos, 0.5f * speed);
            yield return new WaitForSeconds(0.25f * speed);

            Reset1stCard();
            players1stCards[GetPlayerIndex(eTargetPlayer.AI3)].gameObject.SetActive(true);
            yield return new WaitForEndOfFrame();

            MoveCard(cardsToDistribute[1], players2ndCards[GetPlayerIndex(eTargetPlayer.PLAYER)].gameObject, 0.5f * speed);
            ResizeCard(cardsToDistribute[1], players2ndCards[GetPlayerIndex(eTargetPlayer.PLAYER)].gameObject, 0.5f * speed);
            yield return new WaitForSeconds(0.25f * speed);

            Reset2ndCard();
            players2ndCards[GetPlayerIndex(eTargetPlayer.PLAYER)].gameObject.SetActive(true);
            yield return new WaitForEndOfFrame();
        }

        void MoveCard(GameObject card, GameObject targetObjsPos, float duration)
        {
            card.SetActive(true);
            card.transform.DOMove(targetObjsPos.transform.position, duration);
        }

        void ResizeCard(GameObject card, GameObject targetObjsSizeDelta, float duration)
        {
            card.GetComponent<RectTransform>().DOSizeDelta(targetObjsSizeDelta.GetComponent<RectTransform>().sizeDelta, duration);
        }
        #endregion

        #region Details
        #region Resets
        public void ResetGame()
        {
            ResetRound();
            ResetAllPlayerScore();
        }

        public void ResetRound()
        {
            hiddenCard.SetActive(false);
            deck.SetActive(true);
            for (int i = 0; i < players1stCards.Length; i++)
            {
                players1stCards[i].gameObject.SetActive(false);
            }
            for (int i = 0; i < players2ndCards.Length; i++)
            {
                players2ndCards[i].gameObject.SetActive(false);
            }
            for (int i = 0; i < tinycardsHolder.Length; i++)
            {
                int childCount = tinycardsHolder[i].childCount;
                for (int j = 0; j < childCount; j++)
                {
                    Destroy(tinycardsHolder[i].GetChild(0));
                }
            }
        }

        void ResetAllPlayerScore()
        {
            for (int i = 0; i < scores.Length; i++)
            {
                scores[i].text = "x 0";
            }
        }

        void Reset1stCard()
        {
            ResetCardPostion(cardsToDistribute[0], deck);
            ResetCardRectTransform(cardsToDistribute[0], new Vector2(75, 100));
        }

        void Reset2ndCard()
        {
            ResetCardPostion(cardsToDistribute[1], deck);
            ResetCardRectTransform(cardsToDistribute[1], new Vector2(75, 100));
        }

        void ResetCardPostion(GameObject card, GameObject targetObjsPos)
        {
            card.transform.DOMove(targetObjsPos.transform.position, 0);
            card.SetActive(false);
        }

        void ResetCardRectTransform(GameObject card, Vector2 sizeDelta)
        {
            card.GetComponent<RectTransform>().DOSizeDelta(sizeDelta, 0);
        }
        #endregion

        void SetPlayerScore(int score, eTargetPlayer player)
        {
            scores[GetPlayerIndex(player)].text = "x " + score;
        }
        
        void ToggleDetails(bool unhide)
        {
            if(hideDetails != unhide)
            {
                hideDetails = unhide;
                for (int i = 0; i < scoresGmObj.Length; i++)
                {
                    scoresGmObj[i].SetActive(unhide);
                }
            }
        }
        #endregion

        public Sprite GetCardSprites(eCardValues value)
        {
            return cardSprites[(int)value];
        }

        public Sprite GetCardSpritesSmall(eCardValues value)
        {
            return cardSpritesSmall[(int)value];
        }
        
        int GetPlayerIndex(eTargetPlayer player)
        {
            switch (player)
            {
                case eTargetPlayer.PLAYER:
                    return 0;
                case eTargetPlayer.AI1:
                    return 1;
                case eTargetPlayer.AI2:
                    return 2;
                case eTargetPlayer.AI3:
                    return 3;
            }
            return -1;
        }
    }
}
