using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

namespace BBSL_LOVELETTER
{
    public enum eButtonUsage
    {
        INVALID = -1,
        FIRSTCARDUSE,
        SECONDCARDUSE,
        CANCELACTION,
    }

    public class game_UIController : MonoBehaviour
    {
        [Header("SPRITE REFERENCE")]
        [SerializeField]
        private Sprite[] cardSprites;
        [SerializeField]
        private Sprite[] cardSpritesSmall;
        [SerializeField]
        private Sprite cardbackSprite;

        [Header("")][Header("CARD REFERENCE")]
        [SerializeField]
        private GameObject[] cardsToDistribute;
        [SerializeField]
        private GameObject deck;
        [SerializeField]
        private GameObject missingCard;
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
        [SerializeField]
        private Transform[] playerTargetPosition;

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
        
        [Header("")]
        [Header("BUTTON REFERENCE")]
        [SerializeField]
        private Button[] playerUseButton;
        [SerializeField]
        private GameObject[] playerTargetButton;
        [SerializeField]
        private GameObject[] playerCantTargetButton;
        [SerializeField]
        private Button[] guardTargetButton;

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

            MoveCard(cardsToDistribute[0], missingCard, 0.5f * speed);
            ResizeCard(cardsToDistribute[0], missingCard, 0.5f * speed);
            yield return new WaitForSeconds(0.25f * speed);

            MoveCard(cardsToDistribute[1], player1SingleCardPos, 0.5f * speed);
            ResizeCard(cardsToDistribute[1], players1stCards[GetPlayerIndex(eTargetPlayer.PLAYER)].gameObject, 0.5f * speed);
            yield return new WaitForSeconds(0.25f * speed);

            Reset1stCard();
            missingCard.SetActive(true);
            yield return new WaitForEndOfFrame();

            MoveCard(cardsToDistribute[0], players1stCards[GetPlayerIndex(eTargetPlayer.AI1)].gameObject, 0.5f * speed);
            ResizeCard(cardsToDistribute[0], players1stCards[GetPlayerIndex(eTargetPlayer.AI1)].gameObject, 0.5f * speed);
            yield return new WaitForSeconds(0.25f * speed);

            Reset2ndCard();
            players1stCards[GetPlayerIndex(eTargetPlayer.PLAYER)].gameObject.SetActive(true);
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
            ShowPlayerCards();
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

        #region ShowCards
        private bool doneShowingCard = false;
        private bool doneDrawingCard = false;

        void ShowPlayerCards()
        {
            players1stCards[0].SetCard(game_Logic.instance.GetPlayer().Get1stCardValue());
            players2ndCards[0].SetCard(game_Logic.instance.GetPlayer().Get2ndCardValue());
            TogglePlayerButtons(true);
        }

        public void ShowPlayerCardUse(eTargetPlayer initialplayer, eCardValues cardused, eTargetPlayer targetplayer, eButtonUsage button = eButtonUsage.INVALID)
        {
            doneShowingCard = false;
            StartCoroutine(ShowPlayerCardUseIE(initialplayer, cardused, targetplayer, button));
        }

        IEnumerator ShowPlayerCardUseIE(eTargetPlayer initialplayer, eCardValues cardused, eTargetPlayer targetplayer, eButtonUsage button = eButtonUsage.INVALID)
        {
            float speed = 1.0f;
            if(button == eButtonUsage.FIRSTCARDUSE)
            {
                MoveCard(cardsToDistribute[0], players1stCards[GetPlayerIndex(initialplayer)].gameObject, 0.0f);
                ResizeCard(cardsToDistribute[0], players1stCards[GetPlayerIndex(initialplayer)].gameObject, 0.0f);
                cardsToDistribute[0].GetComponent<Image>().sprite = GetCardSprites(cardused);

                MoveCard(players1stCards[GetPlayerIndex(initialplayer)].gameObject, players2ndCards[GetPlayerIndex(initialplayer)].gameObject, 0.0f);
                players1stCards[GetPlayerIndex(initialplayer)].GetComponent<Image>().sprite = players2ndCards[GetPlayerIndex(initialplayer)].GetComponent<Image>().sprite;
                players2ndCards[GetPlayerIndex(initialplayer)].gameObject.SetActive(false);
                yield return new WaitForEndOfFrame();

                MoveCard(players1stCards[GetPlayerIndex(initialplayer)].gameObject, player1SingleCardPos, 0.5f * speed);
            }
            else if(button == eButtonUsage.SECONDCARDUSE)
            {
                MoveCard(cardsToDistribute[0], players2ndCards[GetPlayerIndex(initialplayer)].gameObject, 0.0f);
                ResizeCard(cardsToDistribute[0], players2ndCards[GetPlayerIndex(initialplayer)].gameObject, 0.0f);
                players2ndCards[GetPlayerIndex(initialplayer)].gameObject.SetActive(false);
                cardsToDistribute[0].GetComponent<Image>().sprite = GetCardSprites(cardused);
                yield return new WaitForEndOfFrame();

                MoveCard(players1stCards[GetPlayerIndex(initialplayer)].gameObject, player1SingleCardPos, 0.5f * speed);
            }
            else
            {
                MoveCard(cardsToDistribute[0], players2ndCards[GetPlayerIndex(initialplayer)].gameObject, 0.0f);
                ResizeCard(cardsToDistribute[0], players2ndCards[GetPlayerIndex(initialplayer)].gameObject, 0.0f);
                players2ndCards[GetPlayerIndex(initialplayer)].gameObject.SetActive(false);
                yield return new WaitForEndOfFrame();
            }

            MoveCard(cardsToDistribute[0], playerTargetPosition[GetPlayerIndex(targetplayer)].gameObject, 0.5f * speed);
            yield return new WaitForSeconds(0.5f * speed);

            //flip card
            cardsToDistribute[0].GetComponent<Image>().sprite = GetCardSprites(cardused);
            yield return new WaitForSeconds(1.0f * speed);

            MoveCard(cardsToDistribute[0], tinycardsHolder[GetPlayerIndex(initialplayer)].gameObject, 0.5f * speed);
            ResizeCard(cardsToDistribute[0], playerTargetPosition[GetPlayerIndex(targetplayer)].gameObject, 0.5f * speed);
            yield return new WaitForSeconds(0.5f * speed);
            UpdateCardHolder(initialplayer, cardused);
            yield return new WaitForEndOfFrame();
            Reset1stCard();
            doneShowingCard = true;
        }

        void UpdateCardHolder(eTargetPlayer targetplayer, eCardValues cardused)
        {
            GameObject smallCard;
            smallCard = Instantiate((GameObject)Resources.Load("Prefabs/GameAssets/CardSmall"));
            smallCard.GetComponent<CardNumber>().SetCard(cardused);
            smallCard.transform.parent = tinycardsHolder[GetPlayerIndex(targetplayer)];
        }

        public void PlayerDiscardCard(eTargetPlayer targetplayer, eCardValues cardused, bool killplayer = false)
        {
            StartCoroutine(PlayerDiscardCardIE(targetplayer, cardused));
        }

        IEnumerator PlayerDiscardCardIE(eTargetPlayer targetplayer, eCardValues cardused, bool killplayer = false)
        {
            yield return new WaitUntil(() => doneShowingCard);
            float speed = 1.0f;
            MoveCard(cardsToDistribute[0], players1stCards[GetPlayerIndex(targetplayer)].gameObject, 0.0f);
            ResizeCard(cardsToDistribute[0], players1stCards[GetPlayerIndex(targetplayer)].gameObject, 0.0f);
            players1stCards[GetPlayerIndex(targetplayer)].gameObject.SetActive(false);
            yield return new WaitForEndOfFrame();

            MoveCard(cardsToDistribute[0], tinycardsHolder[GetPlayerIndex(targetplayer)].gameObject, 0.5f * speed);
            ResizeCard(cardsToDistribute[0], playerTargetPosition[GetPlayerIndex(targetplayer)].gameObject, 0.5f * speed);
            yield return new WaitForSeconds(0.5f * speed);
            UpdateCardHolder(targetplayer, cardused);
            yield return new WaitForEndOfFrame();
            Reset1stCard();
        }

        public void PlayerDrawCard(eTargetPlayer targetplayer, eCardValues cardused = eCardValues.INVALID)
        {
            doneDrawingCard = false;
            StartCoroutine(PlayerDrawCardIE(targetplayer, cardused));
        }

        IEnumerator PlayerDrawCardIE(eTargetPlayer targetplayer, eCardValues cardused = eCardValues.INVALID)
        {
            yield return new WaitUntil(() => doneShowingCard);
            float speed = 1.0f;
            MoveCard(cardsToDistribute[1], deck, 0.0f);
            ResizeCard(cardsToDistribute[1], deck, 0.0f);
            yield return new WaitForEndOfFrame();

            if (targetplayer == eTargetPlayer.PLAYER)
            {
                MoveCard(players1stCards[GetPlayerIndex(targetplayer)].gameObject, player1DoubleCardPos, 0.5f * speed);
            }
            MoveCard(cardsToDistribute[1], players2ndCards[GetPlayerIndex(targetplayer)].gameObject, 0.5f * speed);
            ResizeCard(cardsToDistribute[1], players2ndCards[GetPlayerIndex(targetplayer)].gameObject, 0.5f * speed);
            yield return new WaitForSeconds(0.5f * speed);
            players2ndCards[GetPlayerIndex(targetplayer)].gameObject.SetActive(true);
            yield return new WaitForEndOfFrame();
            Reset2ndCard();
            if(targetplayer == eTargetPlayer.PLAYER)
            {
                ShowPlayerCards();
            }
            doneDrawingCard = false;
        }

        public void PlayerDrawMissingCard(eTargetPlayer targetplayer, eCardValues cardused = eCardValues.INVALID)
        {
            doneDrawingCard = false;
            StartCoroutine(PlayerDrawMissingCardIE(targetplayer, cardused));
        }

        IEnumerator PlayerDrawMissingCardIE(eTargetPlayer targetplayer, eCardValues cardused = eCardValues.INVALID)
        {
            yield return new WaitUntil(() => doneShowingCard);
            float speed = 1.0f;
            MoveCard(cardsToDistribute[1], missingCard, 0.0f);
            ResizeCard(cardsToDistribute[1], missingCard, 0.0f);
            missingCard.gameObject.SetActive(false);
            yield return new WaitForEndOfFrame();

            MoveCard(cardsToDistribute[1], players1stCards[GetPlayerIndex(targetplayer)].gameObject, 0.5f * speed);
            ResizeCard(cardsToDistribute[1], players1stCards[GetPlayerIndex(targetplayer)].gameObject, 0.5f * speed);
            yield return new WaitForSeconds(0.5f * speed);
            yield return new WaitForEndOfFrame();
            Reset2ndCard();
            if (targetplayer == eTargetPlayer.PLAYER)
            {
                //ShowCard
            }
            doneDrawingCard = true;
        }
        #endregion
        #endregion

        #region UseCard
        eCardValues tempcard = eCardValues.INVALID;
        eButtonUsage playerChoice = eButtonUsage.INVALID;
        [HideInInspector]
        public eCardValues tempguardcard = eCardValues.INVALID;
        [HideInInspector]
        public eTargetPlayer temptarget = eTargetPlayer.INVALID;

        public void UseCard(eButtonUsage button)
        {
            TogglePlayerButtons(false);
            playerChoice = button;
            if (playerChoice == eButtonUsage.FIRSTCARDUSE)
            {
                tempcard = game_Logic.instance.GetPlayer().Get1stCardValue();
            }
            else if(playerChoice == eButtonUsage.SECONDCARDUSE)
            {
                tempcard = game_Logic.instance.GetPlayer().Get2ndCardValue();
            }

            if (tempcard == eCardValues.COUNTESS ||
                tempcard == eCardValues.PRINCESS ||
                tempcard == eCardValues.HANDMAID)
            {
                game_Logic.instance.PlayerUseCard(tempcard);
                ShowPlayerCardUse(eTargetPlayer.PLAYER, tempcard, eTargetPlayer.PLAYER, playerChoice);
                ResetCardUseValues();
            }
            else
            {
                //target still required
                TogglePlayerButtons(false);
                ToggleTargetPlayerPanel(true, tempcard);
            }
        }

        public void FinishTargetPlayerPanel()
        {
            if (tempcard == eCardValues.GUARD)
            {
                //need to select which card
                ToggleTargetPlayerPanel(false, tempcard);
                ToggleGuardSelectionPanel(true);
            }
            else
            {
                ToggleTargetPlayerPanel(false);
                game_Logic.instance.PlayerUseCard(tempcard, temptarget);
                ShowPlayerCardUse(eTargetPlayer.PLAYER, tempcard, temptarget, playerChoice);
                ResetCardUseValues();
            }
        }

        public void FinishGuardSelectionPanel()
        {
            ToggleGuardSelectionPanel(false);
            game_Logic.instance.PlayerUseCard(tempcard, temptarget, tempguardcard);
            ShowPlayerCardUse(eTargetPlayer.PLAYER, tempcard, temptarget, playerChoice);
            ResetCardUseValues();
        }

        void ToggleGuardSelectionPanel(bool unhide)
        {

        }
        
        /// <summary>
        /// Show Panel to select target player from
        /// </summary>
        /// <param name="unhide"></param>
        void ToggleTargetPlayerPanel(bool unhide, eCardValues card = eCardValues.INVALID)
        {
            if(unhide == false)
            {
                for (int i = 0; i < playerTargetButton.Length; i++)
                {
                    playerTargetButton[i].gameObject.SetActive(unhide);
                }
                for (int i = 0; i < playerCantTargetButton.Length; i++)
                {
                    playerCantTargetButton[i].gameObject.SetActive(unhide);
                }
            }
            else
            {
                if(card == eCardValues.PRINCE)
                {
                    playerTargetButton[GetPlayerIndex(eTargetPlayer.PLAYER)].gameObject.SetActive(unhide);
                }
                else
                {
                    playerCantTargetButton[GetPlayerIndex(eTargetPlayer.PLAYER)].gameObject.SetActive(unhide);
                }
                for (int i = 1; i < playerTargetButton.Length; i++)
                {
                    if(game_Logic.instance.IsPlayerValidTarget((eTargetPlayer) i))
                    {
                        playerTargetButton[i].gameObject.SetActive(unhide);
                    }
                    else
                    {
                        playerCantTargetButton[i].gameObject.SetActive(unhide);
                    }
                }
            }
        }

        void TogglePlayerButtons(bool unhide)
        {
            for (int i = 0; i < playerUseButton.Length; i++)
            {
                playerUseButton[i].gameObject.SetActive(unhide);
            }
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
            missingCard.SetActive(false);
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
                    Destroy(tinycardsHolder[i].GetChild(0).gameObject);
                }
            }
            DistributeCards();
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

        void ResetCardUseValues()
        {
            tempcard = eCardValues.INVALID;
            playerChoice = eButtonUsage.INVALID;
            temptarget = eTargetPlayer.INVALID;
            tempguardcard = eCardValues.INVALID;
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
            Debug.Log((int)value);
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
