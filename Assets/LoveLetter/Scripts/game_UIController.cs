using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;
using System.Text;

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
        private GameObject MessageBox_gmobj;
        [SerializeField]
        private TextMeshProUGUI Message_Text;

        [Header("")][Header("BUTTON REFERENCE")]
        [SerializeField]
        private Button[] playerUseButton;
        [SerializeField]
        private GameObject[] playerTargetButton;
        [SerializeField]
        private GameObject[] playerCantTargetButton;
        [SerializeField]
        private GameObject guardTargetSelection;

        [Header("")][Header("SHOWDOWN REFERENCE")]
        [SerializeField]
        private Image initialPlayerPanel;
        [SerializeField]
        private CardNumber initialPlayerHiddenCard;
        [SerializeField]
        private CardNumber initialPlayerCard;
        [SerializeField]
        private Image targetPlayerPanel;
        [SerializeField]
        private CardNumber targetPlayerHiddenCard;
        [SerializeField]
        private CardNumber targetPlayerCard;
        [SerializeField]
        private TextMeshProUGUI vs_Text;
        [SerializeField]
        private TextMeshProUGUI result_Text;
        [SerializeField]
        private TextMeshProUGUI initialPlayer_Text;
        [SerializeField]
        private TextMeshProUGUI targetPlayer_Text;
        [SerializeField]
        private TextMeshProUGUI targetPlayer_GuardText;
        private Vector3 targetPlayer_GuardTextPos;

        [Header("")][Header("OTHERS")]
        [SerializeField]
        private TextMeshProUGUI cardsRemaining;
        private int storedcardsRemaining;

        private bool hideDetails = false;
        StringBuilder text = new StringBuilder();

        #region Awake, Start, Update
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
            targetPlayer_GuardTextPos = targetPlayer_GuardText.transform.position;
            ResetShowdownPanel();
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
        #endregion

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

            ReduceCards();
            MoveCard(cardsToDistribute[0], missingCard, 0.5f * speed);
            ResizeCard(cardsToDistribute[0], missingCard, 0.5f * speed);
            yield return new WaitForSeconds(0.25f * speed);

            ReduceCards();
            MoveCard(cardsToDistribute[1], player1SingleCardPos, 0.5f * speed);
            ResizeCard(cardsToDistribute[1], players1stCards[GetPlayerIndex(eTargetPlayer.PLAYER)].gameObject, 0.5f * speed);
            yield return new WaitForSeconds(0.25f * speed);

            Reset1stCard();
            missingCard.SetActive(true);
            yield return new WaitForEndOfFrame();

            ReduceCards();
            MoveCard(cardsToDistribute[0], players1stCards[GetPlayerIndex(eTargetPlayer.AI1)].gameObject, 0.5f * speed);
            ResizeCard(cardsToDistribute[0], players1stCards[GetPlayerIndex(eTargetPlayer.AI1)].gameObject, 0.5f * speed);
            yield return new WaitForSeconds(0.25f * speed);

            Reset2ndCard();
            players1stCards[GetPlayerIndex(eTargetPlayer.PLAYER)].ToggleCard(true);
            yield return new WaitForEndOfFrame();

            ReduceCards();
            MoveCard(cardsToDistribute[1], players1stCards[GetPlayerIndex(eTargetPlayer.AI2)].gameObject, 0.5f * speed);
            ResizeCard(cardsToDistribute[1], players1stCards[GetPlayerIndex(eTargetPlayer.AI2)].gameObject, 0.5f * speed);
            yield return new WaitForSeconds(0.25f * speed);

            Reset1stCard();
            players1stCards[GetPlayerIndex(eTargetPlayer.AI1)].ToggleCard(true);
            yield return new WaitForEndOfFrame();

            ReduceCards();
            MoveCard(cardsToDistribute[0], players1stCards[GetPlayerIndex(eTargetPlayer.AI3)].gameObject, 0.5f * speed);
            ResizeCard(cardsToDistribute[0], players1stCards[GetPlayerIndex(eTargetPlayer.AI3)].gameObject, 0.5f * speed);
            yield return new WaitForSeconds(0.25f * speed);

            Reset2ndCard();
            players1stCards[GetPlayerIndex(eTargetPlayer.AI2)].ToggleCard(true);
            yield return new WaitForEndOfFrame();

            ReduceCards();
            MoveCard(cardsToDistribute[1], players2ndCards[GetPlayerIndex(eTargetPlayer.PLAYER)].gameObject, 0.5f * speed);
            ResizeCard(cardsToDistribute[1], players2ndCards[GetPlayerIndex(eTargetPlayer.PLAYER)].gameObject, 0.5f * speed);
            MoveCard(players1stCards[GetPlayerIndex(eTargetPlayer.PLAYER)].gameObject, player1DoubleCardPos, 0.5f * speed);
            yield return new WaitForSeconds(0.25f * speed);

            Reset1stCard();
            players1stCards[GetPlayerIndex(eTargetPlayer.AI3)].ToggleCard(true);
            yield return new WaitForEndOfFrame();

            ReduceCards();
            MoveCard(cardsToDistribute[1], players2ndCards[GetPlayerIndex(eTargetPlayer.PLAYER)].gameObject, 0.5f * speed);
            ResizeCard(cardsToDistribute[1], players2ndCards[GetPlayerIndex(eTargetPlayer.PLAYER)].gameObject, 0.5f * speed);
            yield return new WaitForSeconds(0.25f * speed);

            Reset2ndCard();
            players2ndCards[GetPlayerIndex(eTargetPlayer.PLAYER)].ToggleCard(true);
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
        private bool doneShowingCard = true;
        private bool doneDrawingCard = true;
        private bool doneDiscardingCard = true;
        private bool doneShowdown = true;

        void ShowPlayerCards()
        {
            players1stCards[0].SetCard(game_Logic.instance.GetPlayer().Get1stCardValue());
            players2ndCards[0].SetCard(game_Logic.instance.GetPlayer().Get2ndCardValue());

            if(game_Logic.instance.GetPlayer().Get1stCardValue() == eCardValues.COUNTESS && 
                (game_Logic.instance.GetPlayer().Get2ndCardValue() == eCardValues.PRINCE ||
                game_Logic.instance.GetPlayer().Get2ndCardValue() == eCardValues.KING))
            {
                TogglePlayerButtons(true, 0);
            }
            else if (game_Logic.instance.GetPlayer().Get2ndCardValue() == eCardValues.COUNTESS &&
                (game_Logic.instance.GetPlayer().Get1stCardValue() == eCardValues.PRINCE ||
                game_Logic.instance.GetPlayer().Get1stCardValue() == eCardValues.KING))
            {
                TogglePlayerButtons(true, 1);
            }
            else
            {
                TogglePlayerButtons(true);
            }
        }

        public void ShowAICardUse(eTargetPlayer initialplayer, eCardValues cardused, eTargetPlayer targetplayer, float delay)
        {
            SetShowingCard(false);
            StartCoroutine(ShowAICardUseIE(initialplayer, cardused, targetplayer, delay));
        }

        IEnumerator ShowAICardUseIE(eTargetPlayer initialplayer, eCardValues cardused, eTargetPlayer targetplayer, float delay = 1.0f)
        {
            Debug.Log("AI USE CARD");
            float speed = 1.0f;
            yield return new WaitUntil(() => doneDrawingCard);
            yield return new WaitForSeconds(delay * speed);
            MoveCard(cardsToDistribute[0], players2ndCards[GetPlayerIndex(initialplayer)].gameObject, 0.0f);
            ResizeCard(cardsToDistribute[0], players2ndCards[GetPlayerIndex(initialplayer)].gameObject, 0.0f);
            players2ndCards[GetPlayerIndex(initialplayer)].gameObject.SetActive(false);
            yield return new WaitForEndOfFrame();

            MoveCard(cardsToDistribute[0], playerTargetPosition[GetPlayerIndex(targetplayer)].gameObject, 0.5f * speed);
            yield return new WaitForSeconds(0.5f * speed);
            
            //flip card
            cardsToDistribute[0].GetComponent<Image>().sprite = GetCardSprites(cardused);
            yield return new WaitUntil(() => doneShowdown);
            SetShowingCard(true);
            yield return new WaitForSeconds(1.0f * speed);

            MoveCard(cardsToDistribute[0], tinycardsHolder[GetPlayerIndex(initialplayer)].gameObject, 0.5f * speed);
            ResizeCard(cardsToDistribute[0], playerTargetPosition[GetPlayerIndex(targetplayer)].gameObject, 0.5f * speed);
            yield return new WaitForSeconds(0.5f * speed);
            UpdateCardHolder(initialplayer, cardused);
            yield return new WaitForEndOfFrame();
            Reset1stCard();
            game_Logic.DoneRunning();
        }

        public void ShowPlayerCardUse(eTargetPlayer initialplayer, eCardValues cardused, eTargetPlayer targetplayer, eButtonUsage button = eButtonUsage.INVALID)
        {
            game_Logic.StartRunning();
            SetShowingCard(false);
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
            
            MoveCard(cardsToDistribute[0], playerTargetPosition[GetPlayerIndex(targetplayer)].gameObject, 0.5f * speed);
            yield return new WaitForSeconds(0.5f * speed);
            
            //flip card
            cardsToDistribute[0].GetComponent<Image>().sprite = GetCardSprites(cardused);
            yield return new WaitUntil(() => doneShowdown);
            SetShowingCard(true);
            yield return new WaitForSeconds(1.0f * speed);

            MoveCard(cardsToDistribute[0], tinycardsHolder[GetPlayerIndex(initialplayer)].gameObject, 0.5f * speed);
            ResizeCard(cardsToDistribute[0], playerTargetPosition[GetPlayerIndex(targetplayer)].gameObject, 0.5f * speed);
            yield return new WaitForSeconds(0.5f * speed);
            UpdateCardHolder(initialplayer, cardused);
            yield return new WaitForEndOfFrame();
            Reset1stCard();
            game_Logic.DoneRunning();
        }

        void UpdateCardHolder(eTargetPlayer targetplayer, eCardValues cardused)
        {
            GameObject smallCard;
            smallCard = Instantiate(tinycardsToShow);
            smallCard.GetComponent<CardNumber>().SetCard(cardused);
            smallCard.transform.parent = tinycardsHolder[GetPlayerIndex(targetplayer)];
        }

        public void PlayerDiscardCard(eTargetPlayer targetplayer, eCardValues cardused, bool killplayer = false)
        {
            SetDiscardingCard(false);
            StartCoroutine(PlayerDiscardCardIE(targetplayer, cardused));
        }

        IEnumerator PlayerDiscardCardIE(eTargetPlayer targetplayer, eCardValues cardused, bool killplayer = false)
        {
            Debug.Log("Player discard CARD");
            yield return new WaitUntil(() => doneShowingCard);
            float speed = 1.0f;
            MoveCard(cardsToDistribute[1], players1stCards[GetPlayerIndex(targetplayer)].gameObject, 0.0f);
            ResizeCard(cardsToDistribute[1], players1stCards[GetPlayerIndex(targetplayer)].gameObject, 0.0f);
            players1stCards[GetPlayerIndex(targetplayer)].ToggleCard(false);
            yield return new WaitForEndOfFrame();

            MoveCard(cardsToDistribute[1], tinycardsHolder[GetPlayerIndex(targetplayer)].gameObject, 0.5f * speed);
            ResizeCard(cardsToDistribute[1], playerTargetPosition[GetPlayerIndex(targetplayer)].gameObject, 0.5f * speed);
            yield return new WaitForSeconds(0.5f * speed);
            UpdateCardHolder(targetplayer, cardused);
            Reset1stCard();
            yield return new WaitForEndOfFrame();
            SetDiscardingCard(true);
            game_Logic.DoneRunning();
        }

        public void PlayerDrawCard(eTargetPlayer targetplayer, eCardValues cardused = eCardValues.INVALID)
        {
            SetDrawingDone(false);
            StartCoroutine(PlayerDrawCardIE(targetplayer, cardused));
        }

        IEnumerator PlayerDrawCardIE(eTargetPlayer targetplayer, eCardValues cardused = eCardValues.INVALID)
        {
            float speed = 1.0f;
            MoveCard(cardsToDistribute[1], deck, 0.0f);
            ResizeCard(cardsToDistribute[1], deck, 0.0f);
            ReduceCards();
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
            SetDrawingDone(true);
            game_Logic.DoneRunning();
        }

        public void PlayerDrawForPrinceCard(eTargetPlayer targetplayer, eCardValues cardused = eCardValues.INVALID, bool ismissingCard = false)
        {
            SetDrawingDone(false);
            StartCoroutine(PlayerDrawForPrinceCardIE(targetplayer, cardused, ismissingCard));
        }

        IEnumerator PlayerDrawForPrinceCardIE(eTargetPlayer targetplayer, eCardValues cardused, bool ismissingCard)
        {
            yield return new WaitUntil(() => doneShowingCard);
            yield return new WaitUntil(() => doneDiscardingCard);
            float speed = 1.0f;
            if(ismissingCard)
            {
                MoveCard(cardsToDistribute[1], missingCard, 0.0f);
                ResizeCard(cardsToDistribute[1], missingCard, 0.0f);
                missingCard.gameObject.SetActive(false);
            }
            else
            {
                MoveCard(cardsToDistribute[1], deck, 0.0f);
                ResizeCard(cardsToDistribute[1], deck, 0.0f);
                ReduceCards();
            }
            yield return new WaitForEndOfFrame();

            MoveCard(cardsToDistribute[1], players1stCards[GetPlayerIndex(targetplayer)].gameObject, 0.5f * speed);
            ResizeCard(cardsToDistribute[1], players1stCards[GetPlayerIndex(targetplayer)].gameObject, 0.5f * speed);
            yield return new WaitForSeconds(0.5f * speed);
            players1stCards[GetPlayerIndex(targetplayer)].ToggleCard(true);
            yield return new WaitForEndOfFrame();
            Reset2ndCard();
            if (targetplayer == eTargetPlayer.PLAYER)
            {
                players1stCards[0].SetCard(game_Logic.instance.GetPlayer().Get1stCardValue());
                //ShowCard
            }
            SetDrawingDone(true);
            game_Logic.DoneRunning();
        }

        public void PlayerSwapCard(eTargetPlayer initialplayer, eTargetPlayer targetplayer, eCardValues cardused, eCardValues cardget = eCardValues.INVALID)
        {
            StartCoroutine(PlayerSwapCardIE(initialplayer, targetplayer, cardused, cardget));
        }

        IEnumerator PlayerSwapCardIE(eTargetPlayer initialplayer, eTargetPlayer targetplayer, eCardValues cardused, eCardValues cardget)
        {
            yield return new WaitUntil(() => doneShowingCard);
            float speed = 1.0f;
            MoveCard(cardsToDistribute[0], players1stCards[GetPlayerIndex(targetplayer)].gameObject, 0.0f);
            ResizeCard(cardsToDistribute[0], players1stCards[GetPlayerIndex(targetplayer)].gameObject, 0.0f);
            players1stCards[GetPlayerIndex(targetplayer)].ToggleCard(false);

            MoveCard(cardsToDistribute[1], players1stCards[GetPlayerIndex(initialplayer)].gameObject, 0.0f);
            ResizeCard(cardsToDistribute[1], players1stCards[GetPlayerIndex(initialplayer)].gameObject, 0.0f);
            players1stCards[GetPlayerIndex(initialplayer)].ToggleCard(false);
            yield return new WaitForEndOfFrame();

            MoveCard(cardsToDistribute[0], players1stCards[GetPlayerIndex(initialplayer)].gameObject, 0.5f * speed);
            ResizeCard(cardsToDistribute[0], players1stCards[GetPlayerIndex(initialplayer)].gameObject, 0.5f * speed);
            MoveCard(cardsToDistribute[1], players1stCards[GetPlayerIndex(targetplayer)].gameObject, 0.5f * speed);
            ResizeCard(cardsToDistribute[1], players1stCards[GetPlayerIndex(targetplayer)].gameObject, 0.5f * speed);
            yield return new WaitForSeconds(0.5f * speed);

            yield return new WaitForEndOfFrame();
            Reset1stCard();
            Reset2ndCard();
            players1stCards[GetPlayerIndex(initialplayer)].ToggleCard(true);
            players1stCards[GetPlayerIndex(targetplayer)].ToggleCard(true);
            if (initialplayer == eTargetPlayer.PLAYER || targetplayer == eTargetPlayer.PLAYER)
            {
                players1stCards[0].SetCard(cardget);
            }
            game_Logic.DoneRunning();
        }

        public void OpenShowdownPanel(eCardValues cardused, eTargetPlayer initialplayer, eTargetPlayer targetplayer, eResult result, float aiWaitTime = 0.0f)
        {
            SetShowdown(false);
            StartCoroutine(OpenShowdownPanelIE(cardused, initialplayer, targetplayer, result, aiWaitTime));
        }

        IEnumerator OpenShowdownPanelIE(eCardValues cardused, eTargetPlayer initialplayer, eTargetPlayer targetplayer, eResult result, float aiWaitTime = 0.0f)
        {
            float speed = 1.0f;
            eCardValues initialPlayerCardValue = game_Logic.instance.GetCard(initialplayer);
            eCardValues targetPlayerCardValue = game_Logic.instance.GetCard(targetplayer);

            yield return new WaitForSeconds(aiWaitTime);
            yield return new WaitForSeconds(1.0f * speed);
            initialPlayerPanel.gameObject.SetActive(true);
            targetPlayerPanel.gameObject.SetActive(true);

            #region Guard, Prince, Priest
            if (cardused != eCardValues.BARON)
            {
                targetPlayerHiddenCard.ToggleCard(true);
                initialPlayerCard.ToggleCard(true);
                initialPlayerCard.SetCard(cardused);
                targetPlayerCard.SetCard(targetPlayerCardValue);
                yield return new WaitForSeconds(1.0f);
                if (cardused == eCardValues.GUARD || cardused == eCardValues.PRINCE)
                {
                    if(cardused == eCardValues.GUARD)
                    {
                        if (initialplayer == eTargetPlayer.PLAYER)
                        {
                            Debug.Log("PLAYER GUESS");
                            targetPlayer_GuardText.text = tempguardcard.ToString();
                        }
                        else
                        {
                            Debug.Log("AI GUESS");
                            targetPlayer_GuardText.text = game_Logic.instance.GetAIList(initialplayer).GetTargetCardValue().ToString();
                            Debug.Log(targetPlayer_GuardText.text);
                        }

                        targetPlayer_GuardText.DOFade(1.0f, 0.0f);
                        targetPlayer_GuardText.transform.DOLocalMoveY(0, 1.0f);
                        yield return new WaitForSeconds(1.0f);
                    }

                    result_Text.gameObject.SetActive(true);
                    result_Text.DOFade(1.0f, 1.0f);
                    vs_Text.gameObject.SetActive(true);
                    if (result == eResult.WIN || cardused == eCardValues.PRINCE)
                    {
                        ShowPlayerWin(true);
                        ShowPlayerLose(false);
                        RotateCard(false);
                        yield return new WaitForSeconds(0.3f);
                        ShowTargetPlayerCard();
                        yield return new WaitForSeconds(0.7f);
                    }
                    else
                    {
                        ShowPlayerLose(true, "Wrong");
                        targetPlayer_GuardText.DOFade(0.0f, 1.0f);
                        targetPlayerPanel.DOColor(GetColor("D4D41E80"), 0);
                        yield return new WaitForSeconds(1.0f);
                    }
                }
                else
                {
                    RotateCard(false);
                    yield return new WaitForSeconds(0.3f);
                    ShowTargetPlayerCard();
                    yield return new WaitForSeconds(0.7f);
                }
            }
            #endregion
            else
            {
                bool hasPlayer = false;
                if (initialplayer == eTargetPlayer.PLAYER || targetplayer == eTargetPlayer.PLAYER)
                {
                    hasPlayer = true;
                }

                initialPlayerCard.SetCard(initialPlayerCardValue);
                targetPlayerCard.SetCard(targetPlayerCardValue);
                if (initialplayer == eTargetPlayer.PLAYER) { initialPlayerCard.ToggleCard(true); }
                else { initialPlayerHiddenCard.ToggleCard(true); }

                if(targetplayer == eTargetPlayer.PLAYER) { targetPlayerCard.ToggleCard(true); }
                else { targetPlayerHiddenCard.ToggleCard(true); }

                result_Text.gameObject.SetActive(true);
                result_Text.DOFade(1.0f, 1.0f);
                vs_Text.gameObject.SetActive(true);
                yield return new WaitForSeconds(1.0f);
                #region Baron Win
                if (result == eResult.WIN)
                {
                    ShowPlayerLose(false);
                    ShowPlayerWin(true);
                    if (targetplayer != eTargetPlayer.PLAYER)
                    {
                        RotateCard(false);
                    }

                    if (initialplayer != eTargetPlayer.PLAYER)
                    {
                        if(hasPlayer)
                        {
                            RotateCard(true);
                        }
                    }
                    yield return new WaitForSeconds(0.3f);
                    if (hasPlayer)
                    {
                        ShowInitialPlayerCard();
                    }
                    ShowTargetPlayerCard();
                    yield return new WaitForSeconds(0.7f);
                }
                #endregion
                #region Baron Lose
                else if (result == eResult.LOSE)
                {
                    ShowPlayerLose(true);
                    ShowPlayerWin(false);
                    if (targetplayer != eTargetPlayer.PLAYER)
                    {
                        if (hasPlayer)
                        {
                            RotateCard(false);
                        }
                    }

                    if (initialplayer != eTargetPlayer.PLAYER)
                    {
                        RotateCard(true);
                    }
                    yield return new WaitForSeconds(0.3f);
                    if (hasPlayer)
                    {
                        ShowTargetPlayerCard();
                    }
                    ShowInitialPlayerCard();
                    yield return new WaitForSeconds(0.7f);
                }
                #endregion
                #region Baron Draw
                else if (result == eResult.DRAW)
                {
                    if (hasPlayer)
                    {
                        if (targetplayer == eTargetPlayer.PLAYER)
                        {
                            RotateCard(true);
                        }
                        else if (initialplayer == eTargetPlayer.PLAYER)
                        {
                            RotateCard(false);
                        }
                    }
                    ShowPlayerDraw();
                    yield return new WaitForSeconds(0.3f);

                    if (hasPlayer)
                    {
                        ShowInitialPlayerCard();
                    }

                    if (hasPlayer)
                    {
                        if (targetplayer == eTargetPlayer.PLAYER)
                        {
                            ShowInitialPlayerCard();
                        }
                        else if (initialplayer == eTargetPlayer.PLAYER)
                        {
                            ShowTargetPlayerCard();
                        }
                    }
                    yield return new WaitForSeconds(0.7f);
                }
                #endregion
            }
            yield return new WaitForSeconds(0.5f);
            FadeShowdownPanel();
            yield return new WaitForSeconds(1.0f);
            SetShowdown(true);
            game_Logic.DoneRunning();
            ResetShowdownPanel();
        }

        void FadeShowdownPanel()
        {
            initialPlayerPanel.DOFade(0, 1);
            initialPlayerHiddenCard.ToggleCard(false);
            initialPlayerCard.ToggleCard(false);
            initialPlayer_Text.DOFade(0, 1);
            targetPlayerPanel.DOFade(0, 1);
            targetPlayerHiddenCard.ToggleCard(false);
            targetPlayerCard.ToggleCard(false);
            targetPlayer_GuardText.text = string.Empty;
            targetPlayer_Text.DOFade(0, 1);
            vs_Text.gameObject.SetActive(false);
            result_Text.DOFade(0, 0);
        }

        void ShowInitialPlayerCard()
        {
            initialPlayerCard.ToggleCard(true);
            initialPlayerHiddenCard.ToggleCard(false);
        }

        void ShowTargetPlayerCard()
        {
            targetPlayerCard.ToggleCard(true);
            targetPlayerHiddenCard.ToggleCard(false);
        }

        void ShowPlayerDraw()
        {
            initialPlayer_Text.text = "Draw";
            initialPlayer_Text.DOFade(1.0f, 1.0f);
            targetPlayer_Text.text = "Draw";
            targetPlayer_Text.DOFade(1.0f, 1.0f);
        }

        void ShowPlayerLose(bool initialPlayer = false, string text = "Lose")
        {
            if(initialPlayer)
            {
                initialPlayerPanel.DOColor(GetColor("FB5E5E80"), 0.5f);
                initialPlayer_Text.text = text;
                initialPlayer_Text.DOFade(1.0f, 1.0f);
            }
            else
            {
                targetPlayerPanel.DOColor(GetColor("FB5E5E80"), 0.5f);
                targetPlayer_Text.text = text;
                targetPlayer_Text.DOFade(1.0f, 1.0f);
            }
        }

        void ShowPlayerWin(bool initialPlayer = false)
        {
            if (initialPlayer)
            {
                initialPlayerPanel.DOColor(GetColor("5EFB7380"), 0.5f);
                initialPlayer_Text.text = "Win";
                initialPlayer_Text.DOFade(1.0f, 1.0f);
            }
            else
            {
                targetPlayerPanel.DOColor(GetColor("5EFB7380"), 0.5f);
                targetPlayer_Text.text = "Win";
                targetPlayer_Text.DOFade(1.0f, 1.0f);
            }
        }

        void RotateCard(bool initialPlayers = false)
        {
            if(initialPlayers)
            {
                initialPlayerCard.transform.DORotate(new Vector3(0, 180), 0, RotateMode.Fast);
                initialPlayerCard.transform.DORotate(new Vector3(0, 0), 1.0f, RotateMode.Fast);
                initialPlayerHiddenCard.transform.DORotate(new Vector3(0, 180), 1.0f, RotateMode.Fast);
            }
            else
            {
                targetPlayerCard.transform.DORotate(new Vector3(0, 180), 0, RotateMode.Fast);
                targetPlayerCard.transform.DORotate(new Vector3(0, 0), 1.0f, RotateMode.Fast);
                targetPlayerHiddenCard.transform.DORotate(new Vector3(0, 180), 1.0f, RotateMode.Fast);
            }
        }
        #endregion
        #endregion

        #region UseCard
        eCardValues tempcard = eCardValues.INVALID;
        eButtonUsage playerChoice = eButtonUsage.INVALID;
        eCardValues tempguardcard = eCardValues.INVALID;
        eTargetPlayer temptarget = eTargetPlayer.INVALID;

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
                ShowPlayerCardUse(eTargetPlayer.PLAYER, tempcard, eTargetPlayer.PLAYER, playerChoice);
                game_Logic.instance.PlayerUseCard(playerChoice, tempcard);
                ResetCardUseValues();
            }
            else if(game_Logic.instance.GetValidAIPlayers().Count > 0 && tempcard != eCardValues.PRINCE)
            {
                ShowPlayerCardUse(eTargetPlayer.PLAYER, tempcard, eTargetPlayer.PLAYER, playerChoice);
                game_Logic.instance.PlayerUseCard(playerChoice, tempcard);
                ResetCardUseValues();
            }
            else
            {
                //target still required
                TogglePlayerButtons(false);
                ToggleTargetPlayerPanel(true, tempcard);
            }
        }

        public void FinishTargetPlayerPanel(eTargetPlayer targetPlayer)
        {
            temptarget = targetPlayer;
            if (tempcard == eCardValues.GUARD)
            {
                //need to select which card
                ToggleTargetPlayerPanel(false, tempcard);
                ToggleGuardSelectionPanel(true);
            }
            else
            {
                ToggleTargetPlayerPanel(false);
                ShowPlayerCardUse(eTargetPlayer.PLAYER, tempcard, temptarget, playerChoice);
                game_Logic.instance.PlayerUseCard(playerChoice, tempcard, temptarget);
                ResetCardUseValues();
            }
        }

        public void FinishGuardSelectionPanel(eCardValues cardSelected)
        {
            tempguardcard = cardSelected;
            ToggleGuardSelectionPanel(false);
            if(tempguardcard != eCardValues.INVALID)
            {
                ShowPlayerCardUse(eTargetPlayer.PLAYER, tempcard, temptarget, playerChoice);
                game_Logic.instance.PlayerUseCard(playerChoice, tempcard, temptarget, tempguardcard);
            }
            ResetCardUseValues();
        }

        void ToggleGuardSelectionPanel(bool unhide)
        {
            guardTargetSelection.SetActive(unhide);
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

        void TogglePlayerButtons(bool unhide, int only1CardOn)
        {
            for (int i = 0; i < playerUseButton.Length; i++)
            {
                if(only1CardOn == i)
                {
                    playerUseButton[i].gameObject.SetActive(unhide);
                }
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
                players1stCards[i].ToggleCard(false);
            }
            for (int i = 0; i < players2ndCards.Length; i++)
            {
                players2ndCards[i].ToggleCard(false);
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
            cardsToDistribute[0].GetComponent<Image>().sprite = cardbackSprite;
            ResetCardPostion(cardsToDistribute[0], deck);
            ResetCardRectTransform(cardsToDistribute[0], new Vector2(75, 100));
        }

        void Reset2ndCard()
        {
            cardsToDistribute[1].GetComponent<Image>().sprite = cardbackSprite;
            ResetCardPostion(cardsToDistribute[1], deck);
            ResetCardRectTransform(cardsToDistribute[1], new Vector2(75, 100));
        }

        void ResetCardPostion(GameObject card, GameObject targetObjsPos)
        {
            card.SetActive(false);
            card.transform.DOMove(targetObjsPos.transform.position, 0);
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
        }

        void ResetShowdownPanel()
        {
            initialPlayerPanel.gameObject.SetActive(false);
            initialPlayerPanel.DOColor(GetColor("D8D8D880"), 0);
            initialPlayerHiddenCard.ToggleCard(false);
            initialPlayerCard.ToggleCard(false);
            initialPlayerCard.transform.DORotate(new Vector3(0, 0), 0, RotateMode.Fast);
            //initialPlayer_Text.gameObject.SetActive(false);
            initialPlayer_Text.DOFade(0, 0);
            initialPlayer_Text.text = string.Empty;
            targetPlayerPanel.gameObject.SetActive(false);
            targetPlayerPanel.DOColor(GetColor("D8D8D880"), 0);
            targetPlayerHiddenCard.ToggleCard(false);
            targetPlayerCard.ToggleCard(false);
            targetPlayerCard.transform.DORotate(new Vector3(0, 0), 0, RotateMode.Fast);
            //targetPlayer_GuardText.gameObject.SetActive(false);
            targetPlayer_GuardText.text = string.Empty;
            targetPlayer_GuardText.transform.DOMove(targetPlayer_GuardTextPos, 0);
            //targetPlayer_Text.gameObject.SetActive(false);
            targetPlayer_Text.text = string.Empty;
            targetPlayer_Text.DOFade(0, 0);
            vs_Text.gameObject.SetActive(false);
            vs_Text.text = string.Empty;
            result_Text.gameObject.SetActive(false);
            result_Text.DOFade(0, 0);
        }
        #endregion

        void SetPlayerScore(int score, eTargetPlayer player)
        {
            scores[GetPlayerIndex(player)].text = "x " + score;
        }

        public void SetCardsRemaining(int cards)
        {
            storedcardsRemaining = cards;
            cardsRemaining.text = cards.ToString();
            deck.SetActive(true);
        }

        void ReduceCards()
        {
            storedcardsRemaining--;
            cardsRemaining.text = storedcardsRemaining.ToString();
            if(storedcardsRemaining == 0)
            {
                deck.SetActive(false);
                
            }
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
                cardsRemaining.gameObject.SetActive(unhide);
                //for (int i = 0; i < scoresGmObj.Length; i++)
                //{
                //    scoresGmObj[i].SetActive(unhide);
                //}
            }
        }
        #endregion

        #region Sprite & Color
        public Sprite GetCardSprites(eCardValues value)
        {
            return cardSprites[(int)value];
        }

        public Sprite GetCardSpritesSmall(eCardValues value)
        {
            Debug.Log((int)value);
            return cardSpritesSmall[(int)value];
        }
        
        Color GetColor(string colorCode)
        {
            Color tempColor;
            Color color = Color.white;
            if (ColorUtility.TryParseHtmlString("#" + colorCode, out tempColor))
            {
                color = tempColor;
            }
            return color;
        }
        #endregion

        #region MessageBox
        public void SetMessageBox(string text, float delay = 0.0f)
        {
            Debug.Log("MESSAGE: " + text);
            Message_Text.text = text;
            StartCoroutine(MoveMessageBoxIE(delay));
        }

        IEnumerator MoveMessageBoxIE(float delay = 0.0f)
        {
            yield return new WaitUntil(() => doneShowingCard);
            yield return new WaitForSeconds(delay);
            MessageBox_gmobj.transform.DOLocalMoveY(360f, 0.0f);
            yield return new WaitForEndOfFrame();
            MessageBox_gmobj.transform.DOLocalMoveY(275f, 1.0f);
            yield return new WaitForSeconds(2.0f);
            MessageBox_gmobj.transform.DOLocalMoveY(360f, 1.0f);
            yield return new WaitForSeconds(1.0f);
            game_Logic.DoneRunning();
        }

        public void PlayerUnaffected(eTargetPlayer targetPlayer, float delay = 0.0f)
        {
            text.Length = 0;
            text.Append(GetPlayerText(targetPlayer)).Append(" is Unaffected!");
            SetMessageBox(text.ToString(), delay);
        }

        public void PlayerShield(eTargetPlayer targetPlayer, float delay = 0.0f)
        {
            text.Length = 0;
            text.Append(GetPlayerText(targetPlayer)).Append(" is Protected!");
            SetMessageBox(text.ToString(), delay);
        }

        public void PlayerElimination(eTargetPlayer initialPlayer, eTargetPlayer targetPlayer, float delay = 0.0f)
        {
            text.Length = 0;
            if(targetPlayer != eTargetPlayer.INVALID)
            {
                text.Append(GetPlayerText(initialPlayer)).Append(" eliminates ").Append(GetPlayerText(targetPlayer)).Append("!");
            }
            else
            {
                text.Append(GetPlayerText(initialPlayer)).Append(" commits seppuku!");
            }
            SetMessageBox(text.ToString(), delay);
        }

        string GetPlayerText(eTargetPlayer player)
        {
            switch (player)
            {
                case eTargetPlayer.PLAYER:
                    return "<#00FF02FF>Player</color>";
                case eTargetPlayer.AI1:
                    return "<#002BCAFF>Player 2</color>";
                case eTargetPlayer.AI2:
                    return "<#8D00CAFF>Player 3</color>";
                case eTargetPlayer.AI3:
                    return "<#ff8000>Player 4</color>";
            }
            return "";
        }
        #endregion

        #region Etc. code
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

        void SetShowingCard(bool isDone)
        {
            //Debug.Log("SHOWING:" + isDone);
            doneShowingCard = isDone;
        }

        void SetDrawingDone(bool isDone)
        {
            //Debug.Log("DRAWING:" + isDone);
            doneDrawingCard = isDone;
        }

        void SetDiscardingCard(bool isDone)
        {
            //Debug.Log("SHOWING:" + isDone);
            doneDiscardingCard = isDone;
        }

        void SetShowdown(bool isDone)
        {
            doneShowdown = isDone;
        }
        #endregion
    }
}
