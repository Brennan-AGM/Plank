using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;
using System.Collections.Generic;
using TMPro;

namespace BBSL_DOMEMO
{
    public class game_UIController : MonoBehaviour
    {

        [Header("TILEREFERENCE")]
        [SerializeField]
        private Transform[] ShownTileHolder;
        [SerializeField]
        private Transform[] PlayerTileHolder;
        [SerializeField]
        private Transform HiddenTileHolder;

        public Transform TileHolders;

        [Header("RESPONSE")]
        [SerializeField]
        private TileNumber[] PlayerAnswer;
        [SerializeField]
        private Image[] PlayerGlow;
        [SerializeField]
        private Image[] PlayerChatBubble;

        [Header("RESULTS")]
        [SerializeField]
        private GameObject MessageBox_gmobj;
        [SerializeField]
        private TMPro.TextMeshProUGUI Message_Text;
        [SerializeField]
        private Transform MessageBox_targetpos;
        private Vector3 MessageBox_initialpos;
        [SerializeField]
        private GameObject ResetBox_gmobj;
        private Vector3 ResetBox_initialpos;

        [Header("BUTTON")]
        public Button pick_btn;

        [Header("PANEL")]
        [SerializeField]
        private GameObject FrontPanel;
        [SerializeField]
        private GameObject TitlePanel;

        [Header("Shuffle Tiles")]
        [SerializeField]
        private Image[] tileToShuffle;
        private Vector3[] tileToShufflePos;

        private List<Image> allTiles = new List<Image>();

        public static game_UIController instance = null;
        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
            }

            instance = this;

            ResetBox_initialpos = ResetBox_gmobj.transform.position;
            MessageBox_initialpos = MessageBox_gmobj.transform.position;
        }

        void Start()
        {
            SetupTileToShuffle();
        }

        public void Reset()
        {
            allTiles.Clear();
            ClearChildren(ShownTileHolder);
            ClearChildren(PlayerTileHolder);
            ClearChildren(HiddenTileHolder);
            HideResetBox();
        }

        void SetupTileToShuffle()
        {
            tileToShufflePos = new Vector3[tileToShuffle.Length];
            for (int i = 0; i < tileToShuffle.Length; i++)
            {
                tileToShufflePos[i] = tileToShuffle[i].transform.position;
            }
        }

        void ResetTileToShufflePos()
        {
            for (int i = 0; i < tileToShuffle.Length; i++)
            {
                tileToShuffle[i].transform.position = tileToShufflePos[i];
            }
        }

        public Transform GetTileHolder(int id = 0, int type = 0)
        {
            switch (type)
            {
                case 0:
                    return ShownTileHolder[id];
                case 1:
                    return PlayerTileHolder[id];
                case 2:
                    return HiddenTileHolder;
                default:
                    return ShownTileHolder[0];
            }
        }

        public void DetermineResult(eTURNRESULT result)
        {
            if (result == eTURNRESULT.CORRECT)
            {
                SetMessageBox("Player is <#1A742DFF>correct!</color>");
            }
            else if (result == eTURNRESULT.WRONG)
            {
                SetMessageBox("Player is <#E51919FF>wrong!</color>");
            }
        }

        public void SetMessageBox(string text)
        {
            Message_Text.text = text;
            StartCoroutine(MoveMessageBoxIE());
        }

        IEnumerator MoveMessageBoxIE()
        {
            Transform messagebox = MessageBox_gmobj.transform;
            messagebox.DOMoveY(MessageBox_initialpos.y, 0.0f);
            yield return new WaitForEndOfFrame();
            messagebox.DOMoveY(MessageBox_targetpos.position.y, 1.0f);
            yield return new WaitForSeconds(2.0f);
            messagebox.DOMoveY(MessageBox_initialpos.y, 1.0f);
        }

        public void OpenResetBox()
        {
            ResetBox_gmobj.transform.DOLocalMoveY(0, 1.0f);
        }

        public void HideResetBox()
        {
            ResetBox_gmobj.transform.DOMoveY(ResetBox_initialpos.y, 0.0f);
        }

        public Image GetPlayerGlow(int value)
        {
            return PlayerGlow[value];
        }

        public void TogglePlayerGlow(int value, bool unhide, float duration)
        {
            if(unhide == true)
            {
                PlayerGlow[value].DOFade(1.0f, duration);
            }
            else
            {
                PlayerGlow[value].DOFade(0.0f, duration);
            }
            TogglePlayerChatBubble(value, unhide, duration);
        }

        void TogglePlayerChatBubble(int value, bool unhide, float duration)
        {
            if (unhide == true)
            {
                PlayerChatBubble[value].DOFillAmount(1.0f, duration);
            }
            else
            {
                PlayerChatBubble[value].DOFillAmount(0.0f, duration);
            }
        }

        public void SetPlayerTurn(int value, eTURNRESULT result)
        {
            switch (result)
            {
                case eTURNRESULT.CORRECT:
                    PlayerGlow[value].DOColor(Color.green, 0.5f);
                    break;
                case eTURNRESULT.WRONG:
                    PlayerGlow[value].DOColor(Color.red, 0.5f);
                    break;
                case eTURNRESULT.TURN:
                    PlayerGlow[value].DOColor(Color.white, 0.0f);
                    break;
            }
        }

        public void GetPlayerAnswer(int value, int AiID)
        {
            PlayerAnswer[AiID].SetNumber(value);
            PlayerAnswer[AiID].ToggleFade(false, 0.0f);
            PlayerAnswer[AiID].ToggleFade(true, 1.0f);
        }

        public void RemovePlayerAnswer(int AiID)
        {
            PlayerAnswer[AiID].ToggleFade(false, 1.0f);
        }

        void ClearChildren(Transform[] List)
        {
            for (int i = 0; i < List.Length; i++)
            {
                for (int j = 0; j < List[i].childCount; j++)
                {
                    Destroy(List[i].GetChild(j).gameObject);
                }
            }
        }

        void ClearChildren(Transform List)
        {
            for (int i = 0; i < List.childCount; i++)
            {
                Destroy(List.GetChild(i).gameObject);
            }
        }

        public void ShowTiles(int value)
        {
            TileController.instance.InstantiateTiles(value, -1, false);
        }

        public Transform GetTileTargetPos(int value)
        {
            return GetTileHolder(value - 1, 0).transform;
        }

        public void AddTileToPool(GameObject tile)
        {
            allTiles.Add(tile.GetComponent<Image>());
            tile.GetComponent<Image>().DOFade(0.0f, 0.0f);
            tile.GetComponentInChildren<TextMeshProUGUI>().DOFade(0.0f, 0);
        }

        public void StartDistribution()
        {
            ToggleFrontPanel(false);
            StartCoroutine(DistributionIE());
        }

        IEnumerator DistributionIE()
        {
            yield return new WaitForEndOfFrame();
            for (int i = 0; i < allTiles.Count; i++)
            {
                tileToShuffle[i].gameObject.SetActive(true);
                tileToShuffle[i].DOFade(1.0f, 0.0f);
                tileToShuffle[i].GetComponentInChildren<TextMeshProUGUI>().DOFade(1.0f, 0);
            }
            yield return new WaitForEndOfFrame();
            for (int i = 0; i < tileToShuffle.Length / 2; i++)
            {
                tileToShuffle[i].transform.DOMove(allTiles[i].transform.position, 0.5f);
                tileToShuffle[tileToShuffle.Length - 1 - i].transform.DOMove(allTiles[tileToShuffle.Length - 1 - i].transform.position, 0.5f);
                yield return new WaitForSeconds(0.25f);
            }
            //for (int i = 0; i < tileToShuffle.Length; i++)
            //{
            //    tileToShuffle[i].transform.DOMove(allTiles[i].transform.position, 0.5f);
            //    yield return new WaitForSeconds(0.25f);
            //}
            yield return new WaitForEndOfFrame();
            for (int i = 0; i < allTiles.Count; i++)
            {
                allTiles[i].DOFade(1.0f, 0.0f);
                allTiles[i].GetComponentInChildren<TextMeshProUGUI>().DOFade(1.0f, 0);
                tileToShuffle[i].DOFade(0.0f, 1.0f);
                tileToShuffle[i].GetComponentInChildren<TextMeshProUGUI>().DOFade(0.0f, 1.0f);
            }
            yield return new WaitForSeconds(1.0f);
            yield return new WaitForEndOfFrame();
            for (int i = 0; i < allTiles.Count; i++)
            {
                tileToShuffle[i].gameObject.SetActive(false);
            }
            ResetTileToShufflePos();
            ToggleFrontPanel(true);
        }

        public void QuitGame()
        {
            Application.Quit();
        }

        public void ToggleTitlePanel(bool unhide)
        {
            TitlePanel.SetActive(unhide);
        }

        public void ToggleFrontPanel(bool unhide)
        {
            FrontPanel.SetActive(unhide);
        }
    }
}
