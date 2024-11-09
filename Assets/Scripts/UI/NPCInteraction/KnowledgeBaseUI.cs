using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KnowledgeBaseUI : MonoBehaviour
{
    [SerializeField] GameObject _informationUIPrefab;

    private int _yPos = -35;
    private List<InformationUI> _informations = new();

    public void AddInformation(Information information)
    {
        var scrollContent = GetComponent<ScrollRect>().content;
        var informationUI = Instantiate(_informationUIPrefab, scrollContent.transform).GetComponent<InformationUI>();

        informationUI.Initialize(information, _yPos);
        _yPos -= InformationUI.HEIGHT_INTERVAL;
        
        scrollContent.sizeDelta = scrollContent.sizeDelta + new Vector2(0, InformationUI.HEIGHT_INTERVAL);
    }
}
