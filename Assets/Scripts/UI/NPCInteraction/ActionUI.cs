using System;
using UnityEngine;
using UnityEngine.UI;

public class ActionUI : MonoBehaviour
{
    public const int WIDTH_INTERVAL = 110;

    private ActionType _actionType;

    public ActionType ActionType => _actionType;

    public void Initialize(int xPos, ActionType actionType, Texture2D texture)
    {
        _actionType = actionType;
        GetComponentInChildren<RawImage>().texture = texture;

        var rectTransform = GetComponent<RectTransform>();
        rectTransform.anchoredPosition = new Vector2(xPos, rectTransform.anchoredPosition.y);
    }
}
