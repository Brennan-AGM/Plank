﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BBSL_LOVELETTER
{
    public enum eTargetPlayer
    {
        INVALID = -1,
        PLAYER,
        AI1,
        AI2,
        AI3,
    }

    public class CardController : MonoBehaviour
    {
        public static CardController instance { get; private set; }

        private List<Card> ShownCard = new List<Card>();
        private List<Card> Player2AddKhownCards = new List<Card>();
        private List<Card> Player3AddKhownCards = new List<Card>();
        private List<Card> Player4AddKhownCards = new List<Card>();
        private int totalCards = 16;
        private List<Card> MaxCardsList = new List<Card>();
        private List<Card> GameCardsList = new List<Card>();
        private List<Card> CurrentDrawPileList = new List<Card>();
        public Card player1Card = new Card(eCardValues.INVALID);
        private Card missingCard = new Card(eCardValues.INVALID);

        [SerializeField]
        private game_Logic game_logic;
        
        void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
            }

            instance = this;

            DontDestroyOnLoad(gameObject);
        }

        void Start()
        {
            SetupMaxCardsList();
            SetupGameCards();
        }

        void Reset()
        {
            ShownCard.Clear();
            Player2AddKhownCards.Clear();
            Player3AddKhownCards.Clear();
            Player4AddKhownCards.Clear();
        }
        
        void SetupMaxCardsList()
        {
            MaxCardsList.Add(new Card(eCardValues.GUARD, 5));
            MaxCardsList.Add(new Card(eCardValues.PRIEST, 2));
            MaxCardsList.Add(new Card(eCardValues.BARON, 2));
            MaxCardsList.Add(new Card(eCardValues.HANDMAID, 2));
            MaxCardsList.Add(new Card(eCardValues.PRINCE, 2));
            MaxCardsList.Add(new Card(eCardValues.KING, 1));
            MaxCardsList.Add(new Card(eCardValues.COUNTESS, 1));
            MaxCardsList.Add(new Card(eCardValues.PRINCESS, 1));
        }

        void SetupGameCards()
        {
            for (int i = 0; i < MaxCardsList.Count; i++)
            {
                for (int j = 0; j < MaxCardsList[i].GetMaxCard(); j++)
                {
                    GameCardsList.Add(new Card((eCardValues) i ));
                }
            }
            CurrentDrawPileList = new List<Card>(GameCardsList);
            for (int i = 0; i < 5; i++)
            {
                int value = Random.Range(0, CurrentDrawPileList.Count);
                SetPlayerCardValues(CurrentDrawPileList[value].GetCardValue(), (eTargetPlayer) i);
                CurrentDrawPileList.RemoveAt(value);
            }
        }

        public void DrawCard(eTargetPlayer player)
        {
            int value = Random.Range(0, CurrentDrawPileList.Count);
            if (player >= eTargetPlayer.AI1)
            {
                SetPlayerCardValues(CurrentDrawPileList[value].GetCardValue(), player);
            }
            else
            {
                SetPlayerCardValues(CurrentDrawPileList[value].GetCardValue(), 0);
            }
            CurrentDrawPileList.RemoveAt(value);
        }

        public void DrawMissingCard(eTargetPlayer player)
        {
            if (player >= eTargetPlayer.AI1)
            {
                SetPlayerCardValues(missingCard.GetCardValue(), player);
            }
            else
            {
                SetPlayerCardValues(missingCard.GetCardValue(), 0);
            }
        }

        public void AddKnownCard(eCardValues value, eTargetPlayer AIIndex = eTargetPlayer.INVALID)
        {
            if (AIIndex == eTargetPlayer.INVALID)
            {
                ShownCard.Add(new Card(value));
            }
            else if (AIIndex == eTargetPlayer.AI1)
            {
                Player2AddKhownCards.Add(new Card(value));
            }
            else if (AIIndex == eTargetPlayer.AI2)
            {
                Player3AddKhownCards.Add(new Card(value));
            }
            else if (AIIndex == eTargetPlayer.AI3)
            {
                Player4AddKhownCards.Add(new Card(value));
            }
        }

        void SetPlayerCardValues(eCardValues card, eTargetPlayer index)
        {
            switch (index)
            {
                case eTargetPlayer.PLAYER:
                    player1Card.SetCardValue(card);
                    break;
                case eTargetPlayer.AI1:
                    game_logic.GetAIList(eTargetPlayer.AI1).DrawNewCard(card);
                    Player2AddKhownCards.Add(new Card(card));
                    break;
                case eTargetPlayer.AI2:
                    game_logic.GetAIList(eTargetPlayer.AI2).DrawNewCard(card);
                    Player3AddKhownCards.Add(new Card(card));
                    break;
                case eTargetPlayer.AI3:
                    game_logic.GetAIList(eTargetPlayer.AI3).DrawNewCard(card);
                    Player4AddKhownCards.Add(new Card(card));
                    break;
                case eTargetPlayer.INVALID:
                    missingCard.SetCardValue(card);
                    break;
            }
        }

        public void PlayerUseCard(eCardValues card)
        {
            ShownCard.Add(new Card(card));
        }

        public void AIUseCard(eCardValues card, eTargetPlayer AIIndex)
        {
            ShownCard.Add(new Card(card));
            if(AIIndex == eTargetPlayer.AI1)
            {
                Player2AddKhownCards.Remove(new Card(card));
            }
            else if (AIIndex == eTargetPlayer.AI2)
            {
                Player3AddKhownCards.Remove(new Card(card));
            }
            else if (AIIndex == eTargetPlayer.AI3)
            {
                Player4AddKhownCards.Remove(new Card(card));
            }
        }

        public bool CheckIfDrawPileEmpty()
        {
            if(CurrentDrawPileList.Count == 0)
            {
                return true;
            }
            return false;
        }

        public int CardsLeftInDrawPile()
        {
            return CurrentDrawPileList.Count;
        }

        public List<eCardValues> CardsAvailable()
        {
            List<eCardValues> cardList = new List<eCardValues>();
            bool found = false;
            for (int i = 0; i < CurrentDrawPileList.Count; i++)
            {
                found = false;
                for (int j = 0; j < cardList.Count; j++)
                {
                    if(CurrentDrawPileList[i].GetCardValue() == cardList[j])
                    {
                        found = true;
                        break;
                    }
                }
                if(!found)
                {
                    cardList.Add(CurrentDrawPileList[i].GetCardValue());
                }
            }
            found = false;
            for (int i = 0; i < cardList.Count; i++)
            {
                if (missingCard.GetCardValue() == cardList[i])
                {
                    found = true;
                    break;
                }
            }
            if (!found)
            {
                cardList.Add(missingCard.GetCardValue());
            }
            return cardList;
        }
    }
}
