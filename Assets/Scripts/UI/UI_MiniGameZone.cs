using System;
using UnityEngine;
using UnityEngine.UI;

public class UI_MiniGameZone : MonoBehaviour
{
    public const int MAX_LEVEL = 4;

    private static Color Green = new(0f, 1f, 0f, 0.1764706f);
    private static Color Clear = new(0f, 0f, 0f, 0f);

    [SerializeField, Range(0, MAX_LEVEL)] private int _level;

    public int Level => _level;
    public bool IsEmpty => _level <= 0;

    public void SetLevel(int level)
    {
        _level = level;

        var color = level <= 0 ? Clear : Green;
        SetColor(color);
    }

    public void SetEmpty() => SetLevel(0);

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
        [SerializeField] int _level;
        [SerializeField] Color _color;

        public SecretLevelZoneColor(int level, Color color)
        {
            _level = level;
            _color = color;
        }

        public int Level => _level;
        public Color Color => _color;
    }
}