using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KnowledgeBaseUI : MonoBehaviour
{
    private const int STARTING_Y_POS = -35;

    [SerializeField] GameObject _informationUIPrefab;

    private int _yPos = STARTING_Y_POS;
    private List<InformationUI> _knowledge = new();


    public void ResetKnowledge()
    {
        _yPos = STARTING_Y_POS;
        foreach (var information in _knowledge)
        {
            Destroy(information.gameObject);
        }

        _knowledge.Clear();
    }

    public void AddInformation(Information information)
    {
        var scrollContent = GetComponent<ScrollRect>().content;
        var informationUI = Instantiate(_informationUIPrefab, scrollContent.transform).GetComponent<InformationUI>();

        informationUI.Initialize(information, _yPos);
        _yPos -= InformationUI.HEIGHT_INTERVAL;
        
        scrollContent.sizeDelta = scrollContent.sizeDelta + new Vector2(0, InformationUI.HEIGHT_INTERVAL);
    }
}
