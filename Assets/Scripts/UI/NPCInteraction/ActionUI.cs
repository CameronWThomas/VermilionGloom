using BehaviorDesigner.Runtime.Tasks.Unity.UnityPlayerPrefs;
using System;
using UnityEngine;
using UnityEngine.UI;

public class ActionUI : MonoBehaviour
{
    public const int WIDTH_INTERVAL = 110;

    public event Action<ActionType> OnAction;

    private ActionType _actionType;

    public ActionType ActionType => _actionType;

    public void Initialize(int xPos, ActionType actionType, Texture2D texture)
    {
        _actionType = actionType;
        GetComponentInChildren<RawImage>().texture = texture;

        var rectTransform = GetComponent<RectTransform>();
        rectTransform.anchoredPosition = new Vector2(xPos, rectTransform.anchoredPosition.y);
        UpdateInteractable(false);
    }

    public void Activate() => UpdateInteractable(true);
    public void Deactivate() => UpdateInteractable(false);

    public void PerformAction()
    {
        OnAction?.Invoke(ActionType);
    }

    private void UpdateInteractable(bool interactable) =>
        GetComponent<Button>().interactable = interactable;
}
