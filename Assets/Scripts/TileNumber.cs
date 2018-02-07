using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TileNumber : MonoBehaviour {

    [SerializeField]
    private TextMeshProUGUI Number;

    public void SetNumber(int number)
    {
        Number.text = number.ToString();
        SetColor(number);
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
}
