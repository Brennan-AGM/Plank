using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum eEnemyDifficulty
{
    INVALID = 0,
    EASY,
    MEDIUM,
    HARD,
    INSANE,
}

public enum ePlayer
{
    INVALID = -1,
    PLAYER,
    AI1,
    AI2,
    AI3,
}

namespace BBSL_DOMEMO
{
    public class game_EnemyAI : MonoBehaviour
    {
        private int[] number = new int[7];
        private int unknownNumber = 0;

        [SerializeField]
        private eEnemyDifficulty AiType;
        [SerializeField]
        private ePlayer AiID;

        private List<Tile> TileHolder = new List<Tile>();
        private List<KeyValuePair<int, float>> Choice = new List<KeyValuePair<int, float>>();
        private int[] RemovedTiles = new int[7];

        public void Reset()
        {
            for (int i = 0; i < 7; i++)
            {
                RemovedTiles[i] = 0;
            }
            GetChoices();
        }

        public int GetAiID()
        {
            return (int)AiID;
        }

        public eEnemyDifficulty GetAiType()
        {
            return AiType;
        }

        public void GetChoices()
        {
            Choice.Clear();
            TileHolder = TileController.instance.GetList((int)AiID + 4);
            unknownNumber = TileController.instance.GetList(8).Count;
            for (int i = 0; i < 7; i++)
            {
                number[i] = 0;
                int numcount = 0;
                foreach (Tile T in TileHolder)
                {
                    if (T.GetTileValue() == i + 1)
                    {
                        numcount++;
                        number[i] = numcount;
                    }
                }
                float num = (float)((float)((i + 1) - number[i]) / (float)unknownNumber) * 100;
                AddChoice(i + 1, num);

                //Debug.Log("Number " + (i + 1).ToString() + " has " + num +"%");
            }
            //RemoveZeroes();
            SortChoices();
            //ShowChoices();
        }

        void AddChoice(int num, float chance)
        {
            Choice.Add(new KeyValuePair<int, float>(num, chance));
        }

        public void RemoveChoice(int num)
        {
            int counter = 0, index = 0;
            bool found = false;
            foreach (KeyValuePair<int, float> tile in Choice)
            {
                if (tile.Key == num)
                {
                    index = counter;
                    found = true;
                }
                counter++;
            }
            if (found)
            {
                Choice.RemoveAt(index);
            }
        }

        public void RemoveChoice(int num, int index)
        {
            RemovedTiles[index] = num;
        }

        public void RemoveZeroes()
        {
            int counter = 0, index = 0;
            bool found = false;
            for (int i = 0; i < Choice.Count; i++)
            {
                counter = 0;
                foreach (KeyValuePair<int, float> tile in Choice)
                {
                    if (tile.Value == 0)
                    {
                        index = counter;
                        found = true;
                    }
                    counter++;
                }
                if (found)
                {
                    Choice.RemoveAt(index);
                }
            }
        }

        void SortChoices()
        {
            Choice.Sort(delegate (KeyValuePair<int, float> firstPair, KeyValuePair<int, float> nextPair)
            {
                return nextPair.Value.CompareTo(firstPair.Value);
            }
            );
        }

        void ShowChoices()
        {
            int number;
            foreach (KeyValuePair<int, float> tile in Choice)
            {
                number = System.Convert.ToInt32(tile.Key);
            }
        }

        public int GetBestChoiceForEasyMed()
        {
            foreach (KeyValuePair<int, float> tile in Choice)
            {
                return tile.Key;
            }
            return 0;
        }

        public int GetBestChoiceHard()
        {
            int counter = 0, index = 0;
            foreach (KeyValuePair<int, float> tile in Choice)
            {
                for (int i = 0; i < RemovedTiles.Length; i++)
                {
                    if (tile.Key == RemovedTiles[i])
                        index++;
                }
            }
            foreach (KeyValuePair<int, float> tile in Choice)
            {
                if (counter == index)
                    return tile.Key;
                counter++;
            }
            return 0;
        }
    }
}
