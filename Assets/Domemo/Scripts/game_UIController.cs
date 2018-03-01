using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;

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
        private GameObject[] PlayerTurn;

        [Header("RESULTS")]
        [SerializeField]
        private GameObject MessageBox_gmobj;
        [SerializeField]
        private TMPro.TextMeshProUGUI Message_Text;

        [Header("Button")]
        public Button pick_btn;

        public static game_UIController instance = null;
        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
            }

            instance = this;
        }

        public void Reset()
        {
            ClearChildren(ShownTileHolder);
            ClearChildren(PlayerTileHolder);
            ClearChildren(HiddenTileHolder);
        }

        public Transform GetTileHolder(int id = 0, int type = 0)
        {
            switch (type)
            {
                case 0:
                    //id = (id > 6) ? id : 0;
                    return ShownTileHolder[id];
                case 1:
                    //id = (id > 3) ? id : 0;
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
            MessageBox_gmobj.transform.DOLocalMoveY(350f, 0.0f);
            yield return new WaitForEndOfFrame();
            MessageBox_gmobj.transform.DOLocalMoveY(284f, 1.0f);
            yield return new WaitForSeconds(2.0f);
            MessageBox_gmobj.transform.DOLocalMoveY(350f, 1.0f);
        }

        public GameObject GetPlayerTurn(int value)
        {
            return PlayerTurn[value];
        }

        public void SetPlayerTurn(int value, eTURNRESULT result)
        {
            Image target = PlayerTurn[value].GetComponent<Image>();
            switch (result)
            {
                case eTURNRESULT.CORRECT:
                    target.DOColor(Color.green, 0.5f);
                    break;
                case eTURNRESULT.WRONG:
                    target.DOColor(Color.red, 0.5f);
                    break;
                case eTURNRESULT.TURN:
                    target.DOColor(Color.white, 0.0f);
                    break;
            }
        }

        public void GetPlayerAnswer(int value, int AiID)
        {
            PlayerAnswer[AiID].gameObject.SetActive(true);
            PlayerAnswer[AiID].SetNumber(value);
        }

        public void RemovePlayerAnswer(int AiID)
        {
            PlayerAnswer[AiID].gameObject.SetActive(false);
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
            TileController.instance.InstantiateTiles(value, -1);
        }

        public Transform GetTileTargetPos(int value)
        {
            return GetTileHolder(value - 1, 0).transform;
        }
    }
}
