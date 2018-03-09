using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BBSL_DOMEMO
{
    public class TileNumber : MonoBehaviour
    {

        [SerializeField]
        private TextMeshProUGUI Number;

        void OnEnable()
        {
            if(Number.color.a <= 0)
            {
                ToggleTile(true);
            }
        }

        public void SetNumber(int number, bool hide = false)
        {
            Number.text = number.ToString();
            SetColor(number);
            if(hide)
            {
                ToggleTile(false);
            }
        }

        public void ToggleTile(bool unhide)
        {
            if(unhide)
            {
                Number.DOFade(1, 0);
                GetComponent<Image>().DOFade(1, 0);
            }
            else
            {
                Number.DOFade(0, 0);
                GetComponent<Image>().DOFade(0, 0);
            }
        }

        void SetColor(int number)
        {
            switch (number)
            {
                case 1:
                    Number.color = GetColor("5284D6FF");
                    break;
                case 2:
                    Number.color = GetColor("CE2985FF");
                    break;
                case 3:
                    Number.color = GetColor("29BACEFF");
                    break;
                case 4:
                    Number.color = GetColor("CE8C29FF");
                    break;
                case 5:
                    Number.color = GetColor("A7ABFFFF");
                    break;
                case 6:
                    Number.color = GetColor("FFA7A7FF");
                    break;
                case 7:
                    Number.color = GetColor("A7FFDEFF");
                    break;
            }
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

        public void ToggleFade( bool unhide, float duration)
        {
            if (unhide == true)
            {
                Number.DOFade(1.0f, duration);
            }
            else
            {
                Number.DOFade(0.0f, duration);
            }
        }
    }
}
