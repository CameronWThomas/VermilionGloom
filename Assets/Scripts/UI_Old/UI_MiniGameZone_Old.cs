using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UI_MiniGameZone_Old : MonoBehaviour
{
    [SerializeField]
    List<SecretLevelZoneColor> zoneColors = new()
    {
        new SecretLevelZoneColor(SecretLevel.Public, new Color(1f, 0.04746876f, 0f, 0.1764706f)),
        new SecretLevelZoneColor(SecretLevel.Private, new Color(1f, 0.6135118f, 0f, 0.1764706f)),
        new SecretLevelZoneColor(SecretLevel.Confidential, new Color(0.9917831f, 1f, 0f, 0.1764706f)),
        new SecretLevelZoneColor(SecretLevel.Vampiric, new Color(0f, 1f, 0f, 0.1764706f)),
    };

    [SerializeField] private SecretLevel _level;
    [SerializeField] private bool _isEmpty = false;

    public SecretLevel Level => _level;
    public bool IsEmpty => _isEmpty;

    public void SetLevel(SecretLevel level)
    {
        _isEmpty = false;
        _level = level;

        var color = zoneColors.First(x => x.Level == level).Color;
        SetColor(color);
    }

    public void SetEmpty()
    {
        _isEmpty = true;

        var color = new Color(0f, 0f, 0f, 0f);
        SetColor(color);
    }

    public bool IsPointInZone(float x)
    {
        var rectTransform = GetComponent<RectTransform>();
        var rect = rectTransform.rect;

        return Mathf.Abs(rectTransform.anchoredPosition.x - x) <= rect.width / 2f;
    }

    private void SetColor(Color color)
    {
        GetComponentInChildren<Image>(true).color = color;
    }
    

    [Serializable]
    private class SecretLevelZoneColor
    {
        [SerializeField] SecretLevel _level;
        [SerializeField] Color _color;

        public SecretLevelZoneColor(SecretLevel level, Color color)
        {
            _level = level;
            _color = color;
        }

        public SecretLevel Level => _level;
        public Color Color => _color;
    }
}