using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace BBSL_DOMEMO
{
    public class TileController : MonoBehaviour
    {
        private List<Tile> TileList = new List<Tile>();
        private List<Tile> Player1 = new List<Tile>();
        private List<Tile> Player2 = new List<Tile>();
        private List<Tile> Player3 = new List<Tile>();
        private List<Tile> Player4 = new List<Tile>();
        private List<Tile> ShownTile = new List<Tile>();
        private List<Tile> HiddenTile = new List<Tile>();
        private List<Tile> Player2KnownTiles = new List<Tile>();
        private List<Tile> Player3KnownTiles = new List<Tile>();
        private List<Tile> Player4KnownTiles = new List<Tile>();
        private List<KeyValuePair<int, GameObject>> Player1TilesReference = new List<KeyValuePair<int, GameObject>>();
        private List<KeyValuePair<int, GameObject>> Player2TilesReference = new List<KeyValuePair<int, GameObject>>();
        private List<KeyValuePair<int, GameObject>> Player3TilesReference = new List<KeyValuePair<int, GameObject>>();
        private List<KeyValuePair<int, GameObject>> Player4TilesReference = new List<KeyValuePair<int, GameObject>>();
        private int distribNum;

        [Header("Prefab Reference")]
        [SerializeField]
        private GameObject tilePrefab;
        [SerializeField]
        private GameObject hiddentilePrefab;
        
        public static TileController instance { get; private set; }
        #region Awake
        void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
            }

            instance = this;

            DontDestroyOnLoad(gameObject);
        }
        #endregion

        #region Reset
        public void Reset()
        {
            TileList.Clear();
            Player1.Clear();
            Player2.Clear();
            Player3.Clear();
            Player4.Clear();
            ShownTile.Clear();
            HiddenTile.Clear();
            Player2KnownTiles.Clear();
            Player3KnownTiles.Clear();
            Player4KnownTiles.Clear();
            Player1TilesReference.Clear();
            Player2TilesReference.Clear();
            Player3TilesReference.Clear();
            Player4TilesReference.Clear();
            distribNum = 0;
            Begin();
        }
        #endregion

        #region Setup/Begin
        void Begin()
        {
            for (int i = 1; i <= 7; i++)
            {
                for (int y = 7 - i; y < 7; y++)
                {
                    TileList.Add(new Tile(i));
                }
            }  

            distribNum = 0;
            int count = 0;
            while (TileList.Count > 0)
            {
                int randomnum = Random.Range(0, TileList.Count);

                if (count < 4)
                {
                    distribNum = -2;
                }
                else if (count < 8)
                {
                    distribNum = -1;
                }
                else if (count == 8)
                {
                    distribNum = 0;
                }

                DistributeTiles(TileList[randomnum].GetTileValue());
                TileList.RemoveAt(randomnum);

                distribNum = (distribNum > 2) ? 0 : ++distribNum;

                count++;
            }
        }
        #endregion

        #region Tile Related
        void DistributeTiles(int value)
        {
            GameObject tile;
            tile = InstantiateTiles(value, distribNum, true);
            switch (distribNum)
            {
                case -2:
                    HiddenTile.Add(new Tile(value));
                    break;
                case -1:
                    ShownTile.Add(new Tile(value));
                    AddKnownTiles(value);
                    break;
                case 0:
                    Player1.Add(new Tile(value));
                    AddKnownTiles(value);
                    break;
                case 1:
                    Player2.Add(new Tile(value));
                    AddKnownTiles(value, distribNum);
                    break;
                case 2:
                    Player3.Add(new Tile(value));
                    AddKnownTiles(value, distribNum);
                    break;
                case 3:
                    Player4.Add(new Tile(value));
                    AddKnownTiles(value, distribNum);
                    break;
            }
            game_UIController.instance.AddTileToPool(tile);
        }

        void AddKnownTiles(int value, int id = 0)
        {
            if (id == 0)
            {
                Player2KnownTiles.Add(new Tile(value));
                Player3KnownTiles.Add(new Tile(value));
                Player4KnownTiles.Add(new Tile(value));
            }
            else if (id == 1)
            {
                Player3KnownTiles.Add(new Tile(value));
                Player4KnownTiles.Add(new Tile(value));
            }
            else if (id == 2)
            {
                Player2KnownTiles.Add(new Tile(value));
                Player4KnownTiles.Add(new Tile(value));
            }
            else if (id == 3)
            {
                Player2KnownTiles.Add(new Tile(value));
                Player3KnownTiles.Add(new Tile(value));
            }
            else if (id == 4)
            {
                Player2KnownTiles.Add(new Tile(value));
            }
            else if (id == 5)
            {
                Player3KnownTiles.Add(new Tile(value));
            }
            else if (id == 6)
            {
                Player4KnownTiles.Add(new Tile(value));
            }
        }

        public GameObject InstantiateTiles(int value, int type, bool hide)
        {
            GameObject target;
            if (type == -2 || type == 0)
            {
                target = Instantiate(hiddentilePrefab);
                if (hide)
                {
                    target.GetComponentInChildren<TileNumber>().ToggleTile(false);
                }
            }
            else
            {
                target = Instantiate(tilePrefab);
                target.GetComponentInChildren<TileNumber>().SetNumber(value, hide);
            }

            if (type >= 0 && type <= 3)
            {
                SetTileReference(target, value, type);
                //AllPlayerTiles[type] = target;
            }
            target.transform.SetParent(Getholder(value, type), false);
            return target;
        }

        void ReduceTile(GameObject target)
        {
            Destroy(target);
        }

        public int GetTile(List<Tile> TileList_new)
        {
            int counter = 0,
            randomN = Random.Range(0, TileList_new.Count - 1);
            foreach (Tile T in TileList_new)
            {
                if (randomN == counter)
                    return T.GetTileValue();
                counter++;
            }
            return 0;
        }

        void SetTileReference(GameObject target, int value, int type)
        {
            switch (type)
            {
                case 0:
                    Player1TilesReference.Add(new KeyValuePair<int, GameObject>(value, target));
                    break;
                case 1:
                    //Player2KnownTiles.Add(new Tile(value));
                    Player2TilesReference.Add(new KeyValuePair<int, GameObject>(value, target));
                    break;
                case 2:
                    //Player3KnownTiles.Add(new Tile(value));
                    Player3TilesReference.Add(new KeyValuePair<int, GameObject>(value, target));
                    break;
                case 3:
                    //Player4KnownTiles.Add(new Tile(value));
                    Player4TilesReference.Add(new KeyValuePair<int, GameObject>(value, target));
                    break;
            }
        }

        GameObject GetTileReference(int value, int type, bool remove = true)
        {
            List<KeyValuePair<int, GameObject>> TempList = new List<KeyValuePair<int, GameObject>>();
            GameObject Temp_gmobj = null;
            if (type == 0)
            {
                TempList = Player1TilesReference;
            }
            else if (type == 1)
            {
                TempList = Player2TilesReference;
            }
            else if (type == 2)
            {
                TempList = Player3TilesReference;
            }
            else if (type == 3)
            {
                TempList = Player4TilesReference;
            }

            foreach (KeyValuePair<int, GameObject> tile in TempList)
            {
                if (tile.Key == value)
                {
                    Temp_gmobj = tile.Value;
                }
            }
            if (remove)
            {
                TempList.Remove(new KeyValuePair<int, GameObject>(value, Temp_gmobj));
            }

            if (type == 0)
            {
                Player1TilesReference = TempList;
            }
            else if (type == 1)
            {
                Player2TilesReference = TempList;
            }
            else if (type == 2)
            {
                Player3TilesReference = TempList;
            }
            else if (type == 3)
            {
                Player4TilesReference = TempList;
            }

            return Temp_gmobj;
        }

        public void RemoveTile(int value, int aiID)
        {
            ReduceTile(GetTileReference(value, aiID));
        }

        public GameObject GetTile(int value, int aiID)
        {
            return GetTileReference(value, aiID, false);
        }

        public GameObject GetTile()
        {
            return tilePrefab;
        }
        #endregion

        #region List Related
        public List<Tile> GetList(int value = 0)
        {
            switch (value)
            {
                case 0:
                    return ShownTile;
                case 1:
                    return Player1;
                case 2:
                    return Player2;
                case 3:
                    return Player3;
                case 4:
                    return Player4;
                case 5:
                    return Player2KnownTiles;
                case 6:
                    return Player3KnownTiles;
                case 7:
                    return Player4KnownTiles;
                case 8:
                    return HiddenTile;
                default:
                    return null;
            }
        }

        public void SetList(List<Tile> TileList_new, int value = -1)
        {
            switch (value)
            {
                case 0:
                    ShownTile = TileList_new;
                    break;
                case 1:
                    Player1 = TileList_new;
                    break;
                case 2:
                    Player2 = TileList_new;
                    break;
                case 3:
                    Player3 = TileList_new;
                    break;
                case 4:
                    Player4 = TileList_new;
                    break;
                case 5:
                    Player2KnownTiles = TileList_new;
                    break;
                case 6:
                    Player3KnownTiles = TileList_new;
                    break;
                case 7:
                    Player4KnownTiles = TileList_new;
                    break;
                default:

                    break;
            }
        }

        public bool CheckList(int value, int List)
        {
            List++;
            List<Tile> TempList = GetList(List);
            int counter = 0;
            foreach (Tile T in TempList)
            {
                if (T.GetTileValue() == value)
                {
                    TempList.RemoveAt(counter);
                    SetList(TempList, List);
                    ShownTile.Add(T);
                    return true;
                }
                counter++;
            }
            return false;
        }

        Transform Getholder(int value, int type)
        {
            switch (type)
            {
                case -2:
                    return game_UIController.instance.GetTileHolder(0, 2);
                case -1:
                    return game_UIController.instance.GetTileHolder(value - 1, 0);
                default:
                    return game_UIController.instance.GetTileHolder(type, 1);
            }
        }
        #endregion

        #region SendResponse
        public bool SendResponse(int value, int ai)
        {
            if (CheckList(value, ai))
            {
                AddKnownTiles(value, ai + 3);
                return true;
            }
            else
            {
                return false;
            }
        }

        public void SendResponseInsaneAI(int value, int ai)
        {
            if (CheckList(value, ai))
            {
                ReduceTile(GetTileReference(value, ai));
                AddKnownTiles(value, ai + 3);
            }
        }
        #endregion
    }
}
