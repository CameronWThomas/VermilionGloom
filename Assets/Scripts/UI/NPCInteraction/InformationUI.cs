using System;
using TMPro;
using UnityEngine;

public class InformationUI : MonoBehaviour
{
    public const int HEIGHT_INTERVAL = 70;

    private Information _information = null;

    public void Initialize(Information information, int yPos)
    {
        _information = information;
        GetComponentInChildren<TMP_Text>().text = information.ToString();

        var rectTransform = GetComponent<RectTransform>();
        rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, yPos);
    }
}
