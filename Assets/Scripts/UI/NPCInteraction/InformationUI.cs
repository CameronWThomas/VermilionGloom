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
        
        var tMPText = GetComponentInChildren<TMP_Text>();
        tMPText.text = information.ToString();
        tMPText.color = information.PrivacyLevel switch
        {
            PrivacyLevel.Private => Color.yellow,
            PrivacyLevel.Secret => Color.red,
            _ => Color.white,
        };

        var rectTransform = GetComponent<RectTransform>();
        rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, yPos);
    }
}
